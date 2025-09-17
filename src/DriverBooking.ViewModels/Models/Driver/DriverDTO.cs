using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Models.Customer;
using DriverBooking.Core.SeedWorks.Constants;

namespace DriverBooking.Core.Models.Driver
{
    public class DriverDTO : CustomerDTO
    {
        public int Id { get; set; }
        public VehicleDTO Vehicle { get; set; }
    }

    public class VehicleDTO
    {
        [Required]
        [Range(2, 7)]
        public int VehicleCapacity { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Model { get; set; }

        [Required]
        [MaxLength(10)]
        public required string LicensePlate { get; set; }
        [JsonIgnore]
        public VehicleType? VehicleType { get; set; }
        [JsonPropertyName("VehicleType")]
        public string VehicleTypValue => VehicleType.GetDescription();
    }
}
