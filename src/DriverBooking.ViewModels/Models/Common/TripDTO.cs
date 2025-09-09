using System.Text.Json.Serialization;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Models.Driver;
using DriverBooking.Core.SeedWorks.Constants;

namespace DriverBooking.Core.Models.Common
{
    public class TripDTO
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public TripStatus? Status { get; set; }

        [JsonPropertyName("tripStatus")]
        public string TripStatusValue => Status.GetDescription();        
        public double Price { get; set; }
        public required string PaymentMethod { get; set; }
        public PointDTO CurrentLocation { get; set; }
        public PointDTO Dest { get; set; }
        public PointDTO Depart { get; set; }
        public string DestAddress { get; set; }
        public string DepartAddress { get; set; }
        public string? CustomerNote { get; set; }
        public VehicleType RequestVehicleType { get; set; }
        public int RequestVehicleCapacity { get; set; }
        public float Distance { get; set; }
        [JsonIgnore]
        public DistanceUnit DistanceUnit { get; set; }
        [JsonPropertyName("DistanceUnit")]
        public string DistanceUnitValue => DistanceUnit.GetDescription();
        public int? DriverId { get; set; }
        public DriverDTO? Driver { get; set; }
    }
}
