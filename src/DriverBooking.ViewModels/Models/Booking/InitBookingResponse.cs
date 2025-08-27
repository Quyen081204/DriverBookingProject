using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Models.Common;
using NetTopologySuite.Geometries;

namespace DriverBooking.Core.Models.Booking
{
    public class InitBookingResponse
    {
        public List<VehicleRequestInfo> VehicleOptions { get; set; } = new List<VehicleRequestInfo>();

        public List<AvailableDriverLocation> FreeDrivers { get; set; } = new List<AvailableDriverLocation>();

        public string? CustomerNote { get; set; }
    }

    public class VehicleRequestInfo  
    {
        public string VehicleType { get; set; }
        public int VehicleCapacity { get; set; }
        public double Price { get;set; }
    }

    public class AvailableDriverLocation
    {
        public int DriverId { get; set; }
        public PointDTO CurrentLocation { get; set; }
        public double Distance { get; set; } // in km
    }
}

// continue contruct the API