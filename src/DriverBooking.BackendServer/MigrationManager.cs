using DriverBooking.Data;
using Microsoft.EntityFrameworkCore;

namespace DriverBooking.API
{
    public static class MigrationManager
    {
        public static WebApplication MigrateDatabase(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                // A scoped service should always be used from within a scope
                // A service just a object provide functionality to other objects
                var dbContext = scope.ServiceProvider.GetRequiredService<DriverBookingContext>();
                if (dbContext.Database.IsRelational())
                {
                    dbContext.Database.Migrate();
                }
                new DataSeeder(dbContext).SeedAsync().Wait();
            }

            return app;
        }
    }
}

// 
