using DriverBooking.Core.Domain.Entities;

namespace DriverBooking.Core.Models.Booking
{
    public class CustomerBookingRequest : InitBookingRequest
    {
        public VehicleType RequestVehicleType { get; set; }
        public int Price { get; set; }
        public float Distance { get; set; }
        public string PaymentMethod { get; set; }   
    }
}
