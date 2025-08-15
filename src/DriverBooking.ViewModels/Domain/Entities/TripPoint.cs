using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace DriverBooking.Core.Domain.Entities
{
    [Table("TripPoints")]   
    public class TripPoint
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required Guid TripId { get; set; }
        public required Point Location { get; set; }
        public DateTime TimeStamp { get; set; }

        public Trip Trip { get; set; }
    }
}
