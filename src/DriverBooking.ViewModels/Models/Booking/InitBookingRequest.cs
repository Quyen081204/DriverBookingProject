using System.ComponentModel.DataAnnotations;
using DriverBooking.Core.Models.Common;
using NetTopologySuite.Geometries;

namespace DriverBooking.Core.Models.Booking
{
    public class InitBookingRequest
    {
        public PointDTO Depart { get; set; }
        public PointDTO Dest { get; set; } 
        public string DepartAddress { get; set; } = default!;
        public string DestAddress { get; set; } = default!;
        [Range(2,7)]
        public int RequestVehicleCapacity { get; set; }
        public string? CustomerNote { get; set; }   
    }
}
