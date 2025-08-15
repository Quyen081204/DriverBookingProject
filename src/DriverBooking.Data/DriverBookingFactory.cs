using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DriverBooking.Data
{
    public class DriverBookingFactory : IDesignTimeDbContextFactory<DriverBookingContext>
    {
        public DriverBookingContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            // Create options for the DbContext using the configuration
            var builder = new DbContextOptionsBuilder<DriverBookingContext>();


            builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                o => o.UseNetTopologySuite());

            return new DriverBookingContext(builder.Options);
        }
    }
}
