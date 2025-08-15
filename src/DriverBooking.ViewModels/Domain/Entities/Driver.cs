using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DriverBooking.Core.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace DriverBooking.Core.Domain.Entities
{
    public enum DriverStatus
    {
        OFF, ON, ONTRIP
    }
    [Table("Drivers")]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    public class Driver
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        public required string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [Required]
        [MaxLength(50)]
        public required string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public required string LastName { get; set; }
        [MaxLength(500)]
        public string? ProfileAvatarUrl { get; set; }
        public DriverStatus DriverStatus { get; set; }
        public int TripCount { get; set; }
        public int RatingCount { get; set; }
        public Point? CurrentLocation { get; set; }

        public Guid DriverAccountId { get; set; } 
        public AppUser DriverAccount { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}
