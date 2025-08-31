using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DriverBooking.API.Services.TokenServices.Interface;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Domain.Identity;
using DriverBooking.Core.Models.Auth;
using DriverBooking.Core.Models.Common;
using DriverBooking.Core.Models.Customer;
using DriverBooking.Core.Repositories;
using DriverBooking.Core.SeedWorks;
using DriverBooking.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Identity;

namespace DriverBooking.API.Services.CustomerServices
{
    public class CustomerServices : ICustomerServices
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerServices(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
                                IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<AuthenticatedResult>> RegisterCustomer(CustomerDTO customer)
        {
            // Create user
            // Create customer profile
            // Create role for user
            var appUser = new AppUser
            {
                UserName = customer.UserName,
                PhoneNumber = customer.PhoneNumber,
                IsActive = true
            };

            var createAccountResult = await _userManager.CreateAsync(appUser, customer.PassWord);

            if (!createAccountResult.Succeeded)
            {
                return ApiResponse<AuthenticatedResult>.CreateFailureResponseWithoutError("Something went wrong when create customer account");
            }

            var addRoleResult = await _userManager.AddToRoleAsync(appUser, "Customer");

            if (!addRoleResult.Succeeded)
            {
                return ApiResponse<AuthenticatedResult>.CreateFailureResponseWithoutError("Can not add role to account for customer");
            }

            var customerEntity = new Customer
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber,
                ProfileAvatarUrl = customer.ProfileAvatarUrl,
                CustomerAccount = appUser,
                CustomerAccountId = appUser.Id,
            };

            _unitOfWork._customerRepository.Add(customerEntity);
            await _unitOfWork.CompleteAsync();

            // Generate token 
            // Authorization
            var roles = await _userManager.GetRolesAsync(appUser);
            // claim of principal
            var claims = new List<Claim>
            {
                new Claim("accountId", appUser.Id.ToString()),
                new Claim("profileId", customerEntity.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, appUser.UserName),
                new Claim(ClaimTypes.Name, appUser.UserName),
                new Claim(UserClaims.Roles, string.Join(";", roles)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            appUser.RefreshToken = refreshToken;
            appUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30); // Set refresh token expiry time  
            await _userManager.UpdateAsync(appUser);

            return ApiResponse<AuthenticatedResult>.CreateSuccessResponse(new AuthenticatedResult
            {
                Token = accessToken,
                RefreshToken = refreshToken
            }, "Create User Successfully");
        }
    }
}
