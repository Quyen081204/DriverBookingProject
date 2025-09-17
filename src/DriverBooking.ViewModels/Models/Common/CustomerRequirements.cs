using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverBooking.Core.Domain.Entities;

namespace DriverBooking.Core.Models.Common
{
    /// <summary>
    /// Đây là class dùng để thể hiện những yêu cầu đặt xe của người dùng (loại xe, số ghế, khoảng cách)
    /// </summary>
    public class CustomerRequirements
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public float withinM { get; set; }

        public int vehicleCapacity { get; set; }

        public VehicleType vehicleType { get; set; }
    }
}
