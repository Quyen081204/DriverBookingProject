using System.Runtime.CompilerServices;
using System.Text.Json;
using DriverBooking.API.Services.BookingServices.Interface;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Models.Booking;
using DriverBooking.Core.Models.Common;
using DriverBooking.Core.SeedWorks;

namespace DriverBooking.API.Services.BookingServices
{
    public class BookingService : IBookingService
    {
        // Get info price from DB foreach request vehicle capacity
        private readonly IUnitOfWork _unitOfWork;
        private HttpClient goongClient;
        private IConfiguration _configuration;
        public BookingService(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            goongClient = httpClientFactory.CreateClient("GoongClient");
            _configuration = configuration;
        }

        public async Task<ApiResponse<InitBookingResponse>> InitBookingTrip(InitBookingRequest initBookingRequest)
        {
            double distance = await CalculateDistanceInKm(initBookingRequest.Depart, initBookingRequest.Dest);

            if (distance == 0)
            {
                return ApiResponse<InitBookingResponse>.CreateFailureResponseWithoutError("Depart and Dest cannot be the same location");
            }

            // calculate the price base on the price and request vehicle capacity

            var vehicleOptions = new List<VehicleRequestInfo>() {
                CalculatePriceForRequestVehicleCapacity(2, VehicleType.SAME, distance, Get_unitOfWork()),
                CalculatePriceForRequestVehicleCapacity(4, VehicleType.NORMAL, distance, Get_unitOfWork()),
                CalculatePriceForRequestVehicleCapacity(4, VehicleType.LUXURY, distance, Get_unitOfWork()),
                CalculatePriceForRequestVehicleCapacity(7, VehicleType.NORMAL, distance, Get_unitOfWork()),
                CalculatePriceForRequestVehicleCapacity(7, VehicleType.LUXURY, distance, Get_unitOfWork())
            };

            var freeDrivers = await _unitOfWork._driverRepository.GetLocationFreeDriversWithinMetersAsync(initBookingRequest.Depart.Lat,initBookingRequest.Depart.Lon,3000);

            return ApiResponse<InitBookingResponse>.CreateSuccessResponse(
                new InitBookingResponse
                {
                    VehicleOptions = vehicleOptions,
                    FreeDrivers = freeDrivers.ToList(),
                    CustomerNote = initBookingRequest.CustomerNote
                },
                "Vehicles options and available driver info"
            );

        }
        static string VehicleTypeToString(VehicleType vehicleType)
        {
            if (vehicleType == VehicleType.SAME)
                return "Motorbike";

            if (vehicleType == VehicleType.LUXURY)
                return "Luxury";

            if (vehicleType == VehicleType.NORMAL)
                return "Nomal";

            return "Not found vehicle type";
        }

        private IUnitOfWork Get_unitOfWork()
        {
            return _unitOfWork;
        }

        private VehicleRequestInfo CalculatePriceForRequestVehicleCapacity(int requestVehicleCapacity, VehicleType requestVehicleType, double distance, IUnitOfWork _unitOfWork)
        {
            double price = 0;
            int firstStageKm = 30;
            double openingFee = _unitOfWork._openingFeeRepository.Find(of => of.VehicleCapacity == requestVehicleCapacity && of.VehicleType == requestVehicleType).Single().Price;
            if (distance > firstStageKm)
            {
                var FirstStageFee = _unitOfWork._stageFeeRepository.Find(sf => sf.VehicleCapacity == requestVehicleCapacity && sf.VehicleType == requestVehicleType && sf.FromKm == 0).Single().PricePerKm;
                var SecondStageFee = _unitOfWork._stageFeeRepository.Find(sf => sf.VehicleCapacity == requestVehicleCapacity && sf.VehicleType == requestVehicleType && sf.FromKm > firstStageKm).Single().PricePerKm;
                price = (int)(openingFee + firstStageKm * FirstStageFee + (distance - firstStageKm) * SecondStageFee);
            }
            else
            {
                var FirstStageFee = _unitOfWork._stageFeeRepository.Find(sf => sf.VehicleCapacity == requestVehicleCapacity && sf.VehicleType == requestVehicleType && sf.FromKm == 0).Single().PricePerKm;
                price = (int)(openingFee + FirstStageFee * distance);
            }

            return new VehicleRequestInfo
            {
                VehicleCapacity = requestVehicleCapacity,
                VehicleType = VehicleTypeToString(requestVehicleType),
                Price = price
            };
        }

        public async Task<double> CalculateDistanceInKm(PointDTO origin, PointDTO dest)
        {
            var apiKey = _configuration.GetValue<string>("GoongAPI:ApiKey");

            var response = await goongClient.GetAsync($"v2/distancematrix?origins={origin.Lat},{origin.Lon} &destinations={dest.Lat},{dest.Lon}&vehicle=car&api_key={apiKey}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            // Lấy distance.value (mét)
            var distanceInMeters = doc.RootElement
                .GetProperty("rows")[0]
                .GetProperty("elements")[0]
                .GetProperty("distance")
                .GetProperty("value")
                .GetInt32();

            double distanceInKm = distanceInMeters / 1000.0;
            return distanceInKm;
        }

        //static double Haversine(double lat1, double lon1, double lat2, double lon2)
        //{
        //    const double R = 6371000; // Earth radius in meters
        //    var lat1Rad = DegreesToRadians(lat1);
        //    var lat2Rad = DegreesToRadians(lat2);
        //    var dLat = DegreesToRadians(lat2 - lat1);
        //    var dLon = DegreesToRadians(lon2 - lon1);

        //    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
        //            Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
        //            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        //    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        //    return R * c;
        //}

        //static double DegreesToRadians(double deg) => deg * (Math.PI / 180);
    }
}
