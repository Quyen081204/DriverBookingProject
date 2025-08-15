using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DriverBooking.Core.Domain.Identity
{
    [Table("AppUsers")] 
    public class AppUser :IdentityUser<Guid>
    {
        public bool IsActive { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
