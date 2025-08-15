using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriverBooking.Core.Domain.Entities
{
    [Table("OpeningFees")]
    public class OpeningFee
    {
        [Key]
        public int Id { get; set; }
        public VehicleType VehicleType { get; set; }
        [Range(2,7)]
        public int VehicleCapacity { get; set; }
        public double Price { get; set; }
    }
}
