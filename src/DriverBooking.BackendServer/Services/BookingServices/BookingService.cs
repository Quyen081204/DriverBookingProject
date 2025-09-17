using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DriverBooking.API.Services.BookingServices.Interface;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.HubConfigs;
using DriverBooking.Core.Models.Booking;
using DriverBooking.Core.Models.Common;
using DriverBooking.Core.Models.Driver;
using DriverBooking.Core.SeedWorks;
using DriverBooking.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.SignalR;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace DriverBooking.API.Services.BookingServices
{
    public class BookingService : IBookingService
    {
        // Get info price from DB foreach request vehicle capacity
        private readonly IUnitOfWork _unitOfWork;
        private HttpClient goongClient;
        private IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<BookingHub> _hubContext;
        private readonly ConnectionMapping<string> _connections;
        private readonly ManageCancellationToken _manageCancellationToken;
        public BookingService(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory
                            , IConfiguration configuration, IHttpContextAccessor httpContextAccessor
                            , IHubContext<BookingHub> hubContext, ConnectionMapping<string> connections,
                              ManageCancellationToken manageCancellationToken)
        {
            _unitOfWork = unitOfWork;
            goongClient = httpClientFactory.CreateClient("GoongClient");
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
            _connections = connections;
            _manageCancellationToken = manageCancellationToken;
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

            // customer requirement without vehicle type
            var customerRequirements = new CustomerRequirements
            {
                lat = initBookingRequest.Depart.Lat,
                lon = initBookingRequest.Depart.Lon,
                withinM = 3000,
                vehicleCapacity = initBookingRequest.RequestVehicleCapacity,
                vehicleType = initBookingRequest.RequestVehicleCapacity == 2 ? VehicleType.SAME :VehicleType.NORMAL
            };

            var freeDrivers = await _unitOfWork._driverRepository.GetDriversWithinMetersAsync(customerRequirements);

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

        public async Task<ApiResponse<TripDTO>> ProcessBooking(CustomerBookingRequest request)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            // create trip for customer
            var newTrip = new Trip
            {

                Price = request.Price,
                PaymentMethod = request.PaymentMethod,
                Dest = geometryFactory.CreatePoint(new Coordinate(request.Dest.Lon, request.Dest.Lat)),
                Depart = geometryFactory.CreatePoint(new Coordinate(request.Depart.Lon, request.Depart.Lat)),
                // this is text version
                DestAddress = request.DestAddress,
                DepartAddress = request.DepartAddress,
                CustomerNote = request.CustomerNote,
                RequestVehicleType = request.RequestVehicleType,

                RequestVehicleCapacity = request.RequestVehicleCapacity,

                Distance = request.Distance,
                DistanceUnit = DistanceUnit.KM,

                CustomerId = GetProfileId()
            };

            _unitOfWork._tripRepository.Add(newTrip);
            await _unitOfWork.CompleteAsync();

            List<int> driversRejectTrip = new List<int>();
            int findWithinRadius = 2000;
            int findWithinTime = 120;
            var customerRequirements = new CustomerRequirements
            {
                lat = request.Depart.Lat,
                lon = request.Depart.Lon,
                withinM = findWithinRadius,
                vehicleCapacity = request.RequestVehicleCapacity,
                vehicleType = request.RequestVehicleType
            };
            // Token source dùng để điều khiển hủy book chuyến
            var tripTokenSource = new CancellationTokenSource();
            // Key Token để hủy trip
            var keyTripToken = newTrip.Id;
            _manageCancellationToken.Add(keyTripToken, tripTokenSource);
            // Gửi token trip cho khách
            await _hubContext.Clients.Client(_connections.GetConnection(this.GetUserName())).SendAsync("ReceiveTripToken", keyTripToken);
            
            // Khi customer chưa hủy chuyến
            while (!tripTokenSource.Token.IsCancellationRequested)
            {
                var ctsNoti = new CancellationTokenSource();
                var keepSendNotifycations = ctsNoti.Token;
                ctsNoti.CancelAfter(TimeSpan.FromSeconds(findWithinTime));

                // Bat dau gui thong bao tai xe trong vòng 2 phút
                while (!keepSendNotifycations.IsCancellationRequested)
                {
                    IEnumerable<AvailableDriverLocation> freeDrivers;
                    // Lặp tìm nếu chưa thấy tài xế trong 2 phút
                    do
                    {
                        // find driver
                        freeDrivers = await _unitOfWork._driverRepository.GetDriversWithinMetersAsync(customerRequirements);
                    } while (freeDrivers.Count() == 0 && !ctsNoti.IsCancellationRequested);

                    // Push noti for each driver
                    foreach (var driver in freeDrivers)
                    {
                        // Gửi thông báo tài xế
                        if (!driversRejectTrip.Contains(driver.DriverId))
                        {
                            var driverUserName = await GetDriverUserName(driver.DriverId);
                            if (driverUserName != null)
                            {
                                // Trong quá trình gửi thông báo mà khách hủy chuyến thì trả về kết quả
                                try
                                {
                                    bool driverAnswer = await _hubContext.Clients.Client(_connections.GetConnection(driverUserName))
                                                                     .InvokeAsync<bool>("DriverResponse", newTrip, tripTokenSource.Token);
                                    if (driverAnswer)
                                    {
                                        newTrip.RequestStatus = TripRequestStatus.CONFIRMED;
                                        newTrip.DriverId = driver.DriverId;
                                        newTrip.CurrentLocation = geometryFactory.CreatePoint(new Coordinate(driver.CurrentLocation.Lon, driver.CurrentLocation.Lat));
                                        newTrip.StartTime = DateTime.Now;
                                        await _unitOfWork._tripRepository.UpdateTrip(newTrip.Id, newTrip);
                                        var tripDTO = new TripDTO
                                        {
                                            Id = newTrip.Id,
                                            StartTime = (DateTime)newTrip.StartTime,  
                                            Status = newTrip.Status,
                                            Price = newTrip.Price,
                                            PaymentMethod = newTrip.PaymentMethod,
                                            CurrentLocation = new PointDTO { Lat = driver.CurrentLocation.Lat, Lon = driver.CurrentLocation.Lon },
                                            DepartAddress = request.DepartAddress,
                                            DestAddress = request.DestAddress,
                                            Distance = request.Distance,
                                            Driver = await this.GetDriverDTO(driver.DriverId)
                                        };

                                        return ApiResponse<TripDTO>.CreateSuccessResponse(tripDTO);  
                                    }
                                    else
                                    {
                                        // Driver reject trip
                                        driversRejectTrip.Add(driver.DriverId);
                                    }
                                } catch(TaskCanceledException)
                                {
                                    // Khachs hang huy chuyen di
                                    await _hubContext.Clients.Client(driverUserName).SendAsync("CustomerCancelTrip");
                                    return ApiResponse<TripDTO>.CreateFailureResponseWithoutError("You have canceled the trip");
                                }
                            } 
                        }
                    }
                }


                // Hết thời gian và vẫn chưa tìm thấy
                if (findWithinRadius == 4000)
                {
                    // Đã qua 6p mà vẫn không tìm thấy => tự động hủy
                    _manageCancellationToken.CancelToken(keyTripToken);
                }
                else
                {
                    // Hỏi khách có muốn tiếp tục tìm

                    await _hubContext.Clients.Client(_connections.GetConnection(this.GetUserName()))
                                                            .SendAsync("NotifyNotFoudDriver");
                    bool continueSearch = await _hubContext.Clients.Client(_connections.GetConnection(this.GetUserName()))
                                                                            .InvokeAsync<bool>("ContinueSearch", CancellationToken.None);

                    if (!continueSearch)
                    {
                        _manageCancellationToken.CancelToken(keyTripToken);
                    }
                    else
                    {
                        // Tiếp tục tìm tăng bán kính lên 1km
                        findWithinRadius += 1000;
                    }
                }
                
            }

            // Hủy chuyến
            newTrip.CancelReason = "Exceed time!!!";
            newTrip.RequestStatus = TripRequestStatus.CANCELED;
            await _unitOfWork._tripRepository.UpdateTrip(newTrip.Id, newTrip);
            return ApiResponse<TripDTO>.CreateFailureResponseWithoutError("Not Found Any Drivers");
        }

        // Còn thiếu 1 phương thức nếu chuyến xe đã vào trạng thái đang di chuyển nghĩa là khách đã lên xe thì đi keyTripToken
        // Phương thức này dành cho tài xế gọi trong hub

        private async Task<DriverDTO?> GetDriverDTO(int driverId)
        {
            var driver = await _unitOfWork._driverRepository.GetDriverById(driverId);
            if (driver != null)
            {
                return new DriverDTO
                {
                    FullName = driver.FirstName + " " + driver.LastName,
                    Id = driver.Id,
                    PhoneNumber = driver.PhoneNumber,
                    ProfileAvatarUrl = driver.ProfileAvatarUrl,
                    Vehicle = new VehicleDTO
                    {
                        LicensePlate = driver.Vehicle.LicensePlate,
                        Model = driver.Vehicle.Model,
                        VehicleCapacity = driver.Vehicle.VehicleCapacity,
                        VehicleType = driver.Vehicle.VehicleType
                    }
                };
            }
            return null;
        }
        private async Task<string> GetDriverUserName(int id)
        {
            return await _unitOfWork._driverRepository.GetDriverUserNameById(id);
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


        private int GetProfileId()
        {
            return int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst("profileId")?.Value);
        }

        private string GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User.Identity.Name;
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
    }
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