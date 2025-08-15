using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;

namespace DriverBooking.Data
{
    //  This class use for seeding initial data into the database.
    public class DataSeeder
    {
        private readonly DriverBookingContext _context;
        public DataSeeder(DriverBookingContext context)
        {
            _context = context;
        }
        public async Task SeedAsync()
        {
            // Check if the database is already seeded

            var passwordHasher = new PasswordHasher<AppUser>();

            var adminRoleGuid = Guid.NewGuid();
            var customerRoleGuid = Guid.NewGuid();
            var driverRoleGuid = Guid.NewGuid();
            var staffRoleGuid = Guid.NewGuid();

            // Create roles and account users if they do not exist
            if (!_context.Roles.Any())
            {
                var adminRole = new AppRole
                {
                    Id = adminRoleGuid,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    DisplayName = "Administrator"
                };

                var customerRole = new AppRole
                {
                    Id = customerRoleGuid,
                    Name = "Customer",
                    NormalizedName = "Customer",
                    DisplayName = "Customer"
                };

                var driverRole = new AppRole
                {
                    Id = driverRoleGuid,
                    Name = "Driver",
                    NormalizedName = "DRIVER",
                    DisplayName = "Driver"
                };

                var staffRole = new AppRole
                {
                    Id = staffRoleGuid,
                    Name = "Staff",
                    NormalizedName = "STAFF",
                    DisplayName = "Staff"
                };

                await _context.Roles.AddRangeAsync(adminRole, customerRole, driverRole, staffRole);

                await _context.SaveChangesAsync();
            }

            if (!_context.Users.Any())
            {
                var adminId = Guid.NewGuid();
                var customerId = Guid.NewGuid();
                var driver1Id = Guid.NewGuid();
                var driver2Id = Guid.NewGuid();
                var driver3Id = Guid.NewGuid();


                var adminUser = new AppUser
                {
                    Id = adminId,
                    Email = "admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = false,
                };

                var customerUser = new AppUser
                {
                    Id = customerId,
                    Email = "quyen@gmail.com",
                    NormalizedEmail = "QUYEN@GMAIL.COM",
                    UserName = "quyen",
                    NormalizedUserName = "QUYEN",
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = false,
                };

                var driver1User = new AppUser
                {
                    Id = driver1Id,
                    Email = "driver1@gmail.com",
                    NormalizedEmail = "DRIVER1@GMAIL.COM",
                    UserName = "driver1",
                    NormalizedUserName = "DRIVER1",
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = false,
                };

                var driver2User = new AppUser
                {
                    Id = driver2Id,
                    Email = "driver2@gmail.com",
                    NormalizedEmail = "DRIVER2@GMAIL.COM",
                    UserName = "driver2",
                    NormalizedUserName = "DRIVER2",
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = false,
                };

                var driver3User = new AppUser
                {
                    Id = driver3Id,
                    Email = "driver3@gmail.com",
                    NormalizedEmail = "DRIVER3@GMAIL.COM",
                    UserName = "driver3",
                    NormalizedUserName = "DRIVER3",
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = false,
                };

                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "admin@123");
                customerUser.PasswordHash = passwordHasher.HashPassword(customerUser, "quyen@123");
                driver1User.PasswordHash = passwordHasher.HashPassword(driver1User, "driver1@123");
                driver2User.PasswordHash = passwordHasher.HashPassword(driver2User, "driver2@123");
                driver3User.PasswordHash = passwordHasher.HashPassword(driver3User, "driver3@123");
                await _context.Users.AddRangeAsync(adminUser, customerUser, driver1User, driver2User, driver3User);

                // Assign roles to users
                await _context.UserRoles.AddRangeAsync(
                    new IdentityUserRole<Guid> { UserId = adminId, RoleId = adminRoleGuid },
                    new IdentityUserRole<Guid> { UserId = customerId, RoleId = customerRoleGuid },
                    new IdentityUserRole<Guid> { UserId = driver1Id, RoleId = driverRoleGuid },
                    new IdentityUserRole<Guid> { UserId = driver2Id, RoleId = driverRoleGuid },
                    new IdentityUserRole<Guid> { UserId = driver3Id, RoleId = driverRoleGuid }
                );

                await _context.SaveChangesAsync();
            }


            // Create profile for customer 
            if (!_context.Customers.Any())
            {
                var customer = new Customer
                {
                    CustomerAccountId = _context.Users.Local.FirstOrDefault(u => u.UserName == "quyen")?.Id ?? Guid.Empty,
                    FirstName = "Nhu",
                    LastName = "Quyen",
                    PhoneNumber = "0972286956",
                    DateOfBirth = DateTime.UtcNow.AddYears(-21),
                };
                await _context.Customers.AddAsync(customer);
            }

            //Create profile for drivers if they do not exist
            if (!_context.Drivers.Any())
            {
                var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

                var driver1 = new Driver
                {
                    DriverAccountId = _context.Users.Local.FirstOrDefault(u => u.UserName == "driver1")?.Id ?? Guid.Empty,
                    FirstName = "Nguyen",
                    LastName = "Van A",
                    PhoneNumber = "0987654321",
                    DateOfBirth = DateTime.UtcNow.AddYears(-25)
                };
                var driver2 = new Driver
                {
                    DriverAccountId = _context.Users.Local.FirstOrDefault(u => u.UserName == "driver2")?.Id ?? Guid.Empty,
                    FirstName = "Tran",
                    LastName = "Thi B",
                    PhoneNumber = "0912345678",
                    DateOfBirth = DateTime.UtcNow.AddYears(-30),
                    CurrentLocation = geometryFactory.CreatePoint(new Coordinate(106.6297, 10.8231)), // Example coordinates  
                };
                var driver3 = new Driver
                {
                    DriverAccountId = _context.Users.Local.FirstOrDefault(u => u.UserName == "driver3")?.Id ?? Guid.Empty,
                    FirstName = "Le",
                    LastName = "Van C",
                    PhoneNumber = "0901234567",
                    DateOfBirth = DateTime.UtcNow.AddYears(-28),
                };
                await _context.Drivers.AddRangeAsync(driver1, driver2, driver3);
            }

            // Opening fees 
            if (!_context.OpeningFees.Any())
            {
                // Create opening fees if they do not exist
                var openingFeeForMotor = new OpeningFee
                {
                    VehicleCapacity = 2,
                    VehicleType = VehicleType.SAME,
                    Price = 0
                };

                var openingFeeForCar4Lux = new OpeningFee
                {
                    VehicleCapacity = 4,
                    VehicleType = VehicleType.LUXURY,
                    Price = 12000
                };

                var openingFeeForCar4Normal = new OpeningFee
                {
                    VehicleCapacity = 4,
                    VehicleType = VehicleType.NORMAL,
                    Price = 10000
                };

                var openingFeesForCar7Lux = new OpeningFee
                {
                    VehicleCapacity = 7,
                    VehicleType = VehicleType.LUXURY,
                    Price = 15000
                };

                var openingFeesForCar7Normal = new OpeningFee
                {
                    VehicleCapacity = 7,
                    VehicleType = VehicleType.NORMAL,
                    Price = 13000
                };

                await _context.OpeningFees.AddRangeAsync(
                    openingFeeForMotor,
                    openingFeeForCar4Lux,
                    openingFeeForCar4Normal,
                    openingFeesForCar7Lux,
                    openingFeesForCar7Normal
                );
            }

            // Stage fees
            if (!_context.StageFees.Any())
            {
                //Create stage fees if they do not exist

                var stageFeeForMotor = new StageFee
                {
                    VehicleCapacity = 2,
                    VehicleType = VehicleType.SAME,
                    FromKm = 0,
                    PricePerKm = 10000
                };

                var stage0FeeForCar4Normal = new StageFee
                {
                    VehicleCapacity = 4,
                    VehicleType = VehicleType.NORMAL,
                    FromKm = 0,
                    PricePerKm = 13000
                };

                var stage1FeeForCar4Normal = new StageFee
                {
                    VehicleCapacity = 4,
                    VehicleType = VehicleType.NORMAL,
                    FromKm = 31,
                    PricePerKm = 10000
                };

                var stage0FeeForCar4Lux = new StageFee
                {
                    VehicleCapacity = 4,
                    VehicleType = VehicleType.LUXURY,
                    FromKm = 0,
                    PricePerKm = 15000
                };

                var stage2FeeForCar4Lux = new StageFee
                {
                    VehicleCapacity = 4,
                    VehicleType = VehicleType.LUXURY,
                    FromKm = 31,
                    PricePerKm = 13000
                };

                var stage0FeeForCar7Normal = new StageFee
                {
                    VehicleCapacity = 7,
                    VehicleType = VehicleType.NORMAL,
                    FromKm = 0,
                    PricePerKm = 13000
                };

                var stage1FeeForCar7Normal = new StageFee
                {
                    VehicleCapacity = 7,
                    VehicleType = VehicleType.NORMAL,
                    FromKm = 31,
                    PricePerKm = 11000
                };

                var stage0FeeForCar7Luxury = new StageFee
                {
                    VehicleCapacity = 7,
                    VehicleType = VehicleType.LUXURY,
                    FromKm = 0,
                    PricePerKm = 15000
                };

                var stage1FeeForCar7Luxury = new StageFee
                {
                    VehicleCapacity = 7,
                    VehicleType = VehicleType.LUXURY,
                    FromKm = 31,
                    PricePerKm = 13000
                };

                await _context.StageFees.AddRangeAsync(
                    stageFeeForMotor,
                    stage0FeeForCar4Normal,
                    stage1FeeForCar4Normal,
                    stage0FeeForCar4Lux,
                    stage2FeeForCar4Lux,
                    stage0FeeForCar7Normal,
                    stage1FeeForCar7Normal,
                    stage0FeeForCar7Luxury,
                    stage1FeeForCar7Luxury
                );
            }

            await _context.SaveChangesAsync();

            // Vehicles
            if (!_context.Vehicles.Any())
            {
                var vehicle1 = new Vehicle
                {
                    DriverId = _context.Drivers.Local.FirstOrDefault(d => d.FirstName == "Nguyen" && d.LastName == "Van A")?.Id ?? 0,
                    VehicleType = VehicleType.SAME,
                    VehicleCapacity = 2,
                    OpeningFeeId = _context.OpeningFees.Local.FirstOrDefault(of => of.VehicleType == VehicleType.SAME && of.VehicleCapacity == 2)?.Id ?? 0,
                    StageFees = _context.StageFees.Local.Where(sf => sf.VehicleType == VehicleType.SAME && sf.VehicleCapacity == 2).ToList(),
                    LicensePlate = "59A-12345",
                    Model = "Honda Wave"
                };

                var vehicle2 = new Vehicle
                {
                    DriverId = _context.Drivers.Local.FirstOrDefault(d => d.FirstName == "Tran" && d.LastName == "Thi B")?.Id ?? 0,
                    VehicleType = VehicleType.NORMAL,
                    VehicleCapacity = 4,
                    OpeningFeeId = _context.OpeningFees.Local
                                           .FirstOrDefault(of => of.VehicleType == VehicleType.NORMAL && of.VehicleCapacity == 4)?.Id ?? 0,
                    StageFees = _context.StageFees.Local
                                                  .Where(sf => sf.VehicleType == VehicleType.NORMAL && sf.VehicleCapacity == 4).ToList(),
                    LicensePlate = "59A-67890",
                    Model = "VinFast"
                };

                var vehicle3 = new Vehicle
                {
                    DriverId = _context.Drivers.Local.FirstOrDefault(d => d.FirstName == "Le" && d.LastName == "Van C")?.Id ?? 0,
                    VehicleType = VehicleType.LUXURY,
                    VehicleCapacity = 7,
                    OpeningFeeId = _context.OpeningFees.Local.FirstOrDefault(of => of.VehicleType == VehicleType.LUXURY && of.VehicleCapacity == 7)?.Id ?? 0,
                    StageFees = _context.StageFees.Local.Where(sf => sf.VehicleType == VehicleType.LUXURY && sf.VehicleCapacity == 7).ToList(),
                    LicensePlate = "59A-54321",
                    Model = "Audi"
                };
                await _context.Vehicles.AddRangeAsync(vehicle1, vehicle2, vehicle3);
            }

            await _context.SaveChangesAsync();
        }
    }
}
// Next migrate to database with this seeder
