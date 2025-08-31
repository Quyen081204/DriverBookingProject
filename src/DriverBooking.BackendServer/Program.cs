
using System.Text;
using DriverBooking.API;
using DriverBooking.API.Services.BookingServices;
using DriverBooking.API.Services.BookingServices.Interface;
using DriverBooking.API.Services.CustomerServices;
using DriverBooking.API.Services.TokenServices;
using DriverBooking.API.Services.TokenServices.Interface;
using DriverBooking.API.Services.UploadServices;
using DriverBooking.Core.ConfigOptions;
using DriverBooking.Core.Domain.Identity;
using DriverBooking.Core.Models.Content;
using DriverBooking.Core.Repositories;
using DriverBooking.Core.SeedWorks;
using DriverBooking.Data;
using DriverBooking.Data.Repositories;
using DriverBooking.Data.SeedWorks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
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
                options.User.RequireUniqueEmail = false;
            });

            // Register CloudinarySettings config
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

            // Register HttpClient in DI
            builder.Services.AddHttpClient("GoongClient", client =>
            {
                var host = builder.Configuration.GetValue<string>("GoongAPI:host");

                client.BaseAddress = new Uri(host);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Register the UnitOfWork and Repositories
            builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ITripRepository, TripRepository>();
            builder.Services.AddScoped<IDriverRepository, DriverRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

            // Register other services
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IUploadService, CloudinaryUploadService>();
            builder.Services.AddScoped<ICustomerServices, CustomerServices>();

            // Register automapper
            builder.Services.AddAutoMapper(typeof(DriverInListDTO).Assembly);
            // Authentication and Authorization
            builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection("JwtTokenSettings"));
            builder.Services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
            builder.Services.AddScoped<SignInManager<AppUser>, SignInManager<AppUser>>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();

            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                // config to validate the token
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["JwtTokenSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtTokenSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtTokenSettings:Key"]))
                };
            });

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
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            //Seeding data
            app.MigrateDatabase();

            app.Run();
        }
    }
}
