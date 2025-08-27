using DriverBooking.Core.Models.Booking;
using DriverBooking.Core.Models.Common;

namespace DriverBooking.API.Services.BookingServices.Interface
{
    public interface IBookingService
    {
        Task<ApiResponse<InitBookingResponse>> InitBookingTrip(InitBookingRequest initBookingRequest);

        // Calculate the distance between origin and destination in km
        Task<double> CalculateDistanceInKm(PointDTO origin, PointDTO dest);
    }
}
