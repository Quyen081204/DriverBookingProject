
namespace DriverBooking.Core.Models.Auth
{
    public class AuthenticatedResult
    {
        public Guid AccountId { get; set; } 
        public required string Token { get; set; }

        public required string RefreshToken { get; set; }   
    }
}
