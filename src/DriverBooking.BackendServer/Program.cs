
using DriverBooking.API;
using DriverBooking.API.Services;
using DriverBooking.Core.ConfigOptions;
using DriverBooking.Core.Domain.Identity;
using DriverBooking.Core.Models.Content;
using DriverBooking.Core.Repositories;
using DriverBooking.Core.SeedWorks;
using DriverBooking.Data;
using DriverBooking.Data.SeedWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DriverBooking.BackendServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            // Add DBContext and ASP.NET Identity to services
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");  

            builder.Services.AddDbContext<DriverBookingContext>(options =>
                     options.UseNpgsql(connectionString,o => o.UseNetTopologySuite())
                    );

            builder.Services.AddIdentity<AppUser, AppRole>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddEntityFrameworkStores<DriverBookingContext>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            // Register the UnitOfWork and Repositories
            builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ITripRepository, TripRepository>();

            // Register automapper
            builder.Services.AddAutoMapper(typeof(DriverInListDTO).Assembly);
            // Authentication and Authorization
            builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection("JwtTokenSettings"));
            builder.Services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
            builder.Services.AddScoped<SignInManager<AppUser>, SignInManager<AppUser>>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();

            // Default configure services for ASP.NET Core applications 
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            //Seeding data
            app.MigrateDatabase();

            app.Run();
        }
    }
}
