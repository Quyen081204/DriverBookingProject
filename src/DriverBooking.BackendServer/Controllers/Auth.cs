using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DriverBooking.API.Services.TokenServices.Interface;
using DriverBooking.Core.Domain.Identity;
using DriverBooking.Core.Models.Auth;
using DriverBooking.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace DriverBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;   
        public AuthController(UserManager<AppUser> userManager, 
                              SignInManager<AppUser> signInManager,
                              ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }
        // Add your authentication methods here, e.g., Register, Login, etc.

        // Example: [HttpPost("register")]
        // public async Task<IActionResult> Register(RegisterModel model) { ... }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticatedResult>> Login([FromBody] DriverBooking.Core.Models.Auth.LoginRequest request)
        {
            // Authentication
            if (request == null)
                return BadRequest("Invalid login request");

            var user = await _userManager.FindByNameAsync(request.Username);

            if (user == null || user.IsActive == false || user.LockoutEnabled)
                return Unauthorized("Invalid username or password");    

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid username or password");
            }
           
            // Authorization
            var roles = await _userManager.GetRolesAsync(user);
            // claim of principal
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(UserClaims.Id, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(UserClaims.Roles, string.Join(";", roles)),
                //new Claim(UserClaims.Permissions, JsonSerializer.Serialize(permissions)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();
            
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30); // Set refresh token expiry time  
            await _userManager.UpdateAsync(user);   

            return Ok(new AuthenticatedResult {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}
