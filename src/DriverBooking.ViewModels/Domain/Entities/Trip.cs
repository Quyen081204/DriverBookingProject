using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace DriverBooking.Core.Domain.Entities
{
    public enum TripStatus { PICKINGUP, ARRIVED, ONTRIP, ENDTRIP }
    public enum TripRequestStatus { PENDING, CONFIRMED, CANCELED }
    public enum DistanceUnit { KM, M }
    [Table("Trips")]
    public class Trip
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public TripStatus? Status { get; set; }
        public required TripRequestStatus RequestStatus { get; set; }
        public double Price { get; set; }
        public required string PaymentMethod { get; set; }
        public Point? CurrentLocation { get; set; }
        [Required]
        public required Point Dest { get; set; }
        [Required]
        public required Point Depart { get; set; }

        // this is text version
        [MaxLength(500)]
        public required string DestAddress { get; set; }
        // this is text version
        [MaxLength(500)]
        public required string DepartAddress { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [MaxLength(500)]
        public string? CustomerNote { get; set; }
        [MaxLength(500)]
        public string? CancelReason { get; set; }
        public VehicleType RequestVehicleType { get; set; }
        [Range(2,7)]
        public int RequestVehicleCapacity { get; set; }
        public float Distance { get; set; }
        public DistanceUnit DistanceUnit { get; set; }
        [Range(1, 5)]
        public int TripRating { get; set; } 
        [Required]
        public required int CustomerId { get; set; }
        public int? DriverId { get; set; }

        public required Customer Customer { get; set; }

        public Driver? Driver { get; set; } 
    }
}
