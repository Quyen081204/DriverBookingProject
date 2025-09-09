using System.Text.Json.Serialization;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Models.Customer;
using DriverBooking.Core.Models.Driver;
using DriverBooking.Core.SeedWorks.Constants;

namespace DriverBooking.Core.Models.Common
{
    public class TripListDTO
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public TripStatus? Status { get; set; }

        [JsonPropertyName("tripStatus")]
        public string TripStatusValue => Status.GetDescription();

        [JsonIgnore]
        public TripRequestStatus RequestStatus { get; set; }

        [JsonPropertyName("tripRequestStatus")]
        public string TripRequestStatus => RequestStatus.GetDescription();
        public double Price { get; set; }
        public required string PaymentMethod { get; set; }
        public PointDTO CurrentLocation { get; set; }
        public PointDTO Dest { get; set; }
        public PointDTO Depart { get; set; }
        public string DestAddress { get; set; }
        public string DepartAddress { get; set; }
        public string? CustomerNote { get; set; }
        public string? CancelReason { get; set; }
        public VehicleType RequestVehicleType { get; set; }
        public int RequestVehicleCapacity { get; set; }
        public float Distance { get; set; }
        public DistanceUnit DistanceUnit { get; set; }
        public required int CustomerId { get; set; }
        public int? DriverId { get; set; }
        public CustomerDTO Customer { get; set; }
        public DriverDTO? Driver { get; set; }
    }
}
