using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CloudinaryDotNet.Actions;
using DriverBooking.API.Services.TokenServices.Interface;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Domain.Identity;
using DriverBooking.Core.Models.Auth;
using DriverBooking.Core.Models.Common;
using DriverBooking.Core.Models.Customer;
using DriverBooking.Core.Models.Driver;
using DriverBooking.Core.SeedWorks;
using DriverBooking.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Identity;

namespace DriverBooking.API.Services.DriverServices
{
    public class DriverServices : IDriverServices
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public DriverServices(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
                                IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<AuthenticatedResult>> RegisterDriver(DriverDTO driver)
        {
            // Create user
            // Create driver profile
            // Create role for user
            var appUser = new AppUser
            {
                UserName = driver.UserName,
                PhoneNumber = driver.PhoneNumber,
                Email = driver.Email,
                IsActive = true
            };

            var createAccountResult = await _userManager.CreateAsync(appUser, driver.PassWord);

            if (!createAccountResult.Succeeded)
            {
                return ApiResponse<AuthenticatedResult>.CreateFailureResponseWithoutError("Something went wrong when create driver account");
            }

            var addRoleResult = await _userManager.AddToRoleAsync(appUser, "Driver");

            if (!addRoleResult.Succeeded)
            {
                return ApiResponse<AuthenticatedResult>.CreateFailureResponseWithoutError("Can not add role to account for driver");
            }

            // Create vehicle associated with driver
            var driverVehicle = CreateVehicle(driver.Vehicle);
            
            if (driverVehicle == null)
            {
                return ApiResponse<AuthenticatedResult>.CreateFailureResponseWithoutError("Error with vehicle of driver");
            }

            string[] parts = driver.FullName.Trim().Split(' ');

            string lastName = parts[parts.Length - 1];
            string firstName = string.Join(" ", parts.Take(parts.Length - 1));

            var driverEntity = new Driver
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = driver.PhoneNumber,
                ProfileAvatarUrl = driver.ProfileAvatarUrl,
                DriverAccount = appUser,
                DriverAccountId = appUser.Id,
                Vehicle = driverVehicle
            };

            _unitOfWork._driverRepository.Add(driverEntity);
            await _unitOfWork.CompleteAsync();

            // Generate token 
            // Authorization
            var roles = await _userManager.GetRolesAsync(appUser);
            // claim of principal
            var claims = new List<Claim>
            {
                new Claim("accountId", appUser.Id.ToString()),
                new Claim("profileId", driverEntity.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
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
                AccountId = appUser.Id,
                Token = accessToken,
                RefreshToken = refreshToken
            }, "Create Driver Successfully");
        }

        // Create vehicle for driver
        public Vehicle? CreateVehicle(VehicleDTO vehicle)
        {
            // decide type of vehicle
            if (vehicle.VehicleCapacity == 4 || vehicle.VehicleCapacity == 7)
            {
                var luxuryCars = new List<string> { "Mercedes-Benz", "BMW", "Audi", "Lexus", "Land Rover", "Porsche" };
                if (luxuryCars.Contains(vehicle.Model))
                {
                    vehicle.VehicleType = VehicleType.LUXURY;
                }
                else
                    vehicle.VehicleType = VehicleType.NORMAL;
            } else if (vehicle.VehicleCapacity == 2)
            {
                vehicle.VehicleType = VehicleType.SAME;
            }
            else
            {
                return null;
            }

             // get opening fee and stages fee for vehicle
             var openingFee = _unitOfWork._openingFeeRepository.Find(o => o.VehicleCapacity == vehicle.VehicleCapacity
                                                                           && o.VehicleType == vehicle.VehicleType).Single();

             var listStageFee = _unitOfWork._stageFeeRepository.Find(s => s.VehicleCapacity == vehicle.VehicleCapacity
                                                                          && s.VehicleType == vehicle.VehicleType).ToList();

            return new Vehicle
            {
                VehicleCapacity = vehicle.VehicleCapacity,
                LicensePlate = vehicle.LicensePlate,
                VehicleType = (VehicleType)vehicle.VehicleType,
                Model = vehicle.Model,
                OpeningFee = openingFee,
                StageFees = listStageFee
            };
        }
    }
}
