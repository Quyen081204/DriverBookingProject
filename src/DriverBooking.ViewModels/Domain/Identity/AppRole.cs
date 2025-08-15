using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DriverBooking.Core.Domain.Identity
{
    public class AppRole : IdentityRole<Guid>
    {
        [MaxLength(200)]
        public required string DisplayName { get; set; } 
    }
}
