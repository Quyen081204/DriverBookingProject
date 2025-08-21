using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverBooking.Core.Models
{
    public class PageResult<T> : PageResultBase where T : class
    {
        public List<T> Results { get; set; } = new List<T>();
    }
}
