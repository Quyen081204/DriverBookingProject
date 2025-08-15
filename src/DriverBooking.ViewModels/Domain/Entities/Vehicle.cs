using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DriverBooking.Core.Domain.Entities
{
    public enum VehicleType { SAME ,LUXURY, NORMAL,  }
    [Table("Vehicles")]
    [Index(nameof(LicensePlate), IsUnique = true)] 
    [Index(nameof(DriverId), IsUnique =true)]
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required VehicleType VehicleType { get; set; }
        [Required]
        [Range(2,7)]
        public int VehicleCapacity { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Model { get; set; }

        [Required]
        [MaxLength(10)]
        public required string LicensePlate { get; set; }

        // FK
        public int DriverId { get; set; }
        public int OpeningFeeId { get; set; }
        public  Driver Driver { get; set; }
        public  OpeningFee OpeningFee { get; set; }
        public  List<StageFee> StageFees { get; set; }
    }
}
