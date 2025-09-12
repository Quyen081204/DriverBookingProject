using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverBooking.Core.Models.Customer
{
    public class CustomerDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9_]*$",
        ErrorMessage = "Username must start with a letter and contain only letters, numbers, or underscores")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(3, ErrorMessage = "Password must be at least 3 characters long")]
        public string PassWord { get; set; }
        [EmailAddress()]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string? ProfileAvatarUrl { get; set; }
    }
}
