using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DriverBooking.Core.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace DriverBooking.Core.Domain.Entities
{
    [Table("Customers")]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    public class Customer
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

        public Guid CustomerAccountId { get; set; }
        public AppUser CustomerAccount { get; set; }

    }
}
