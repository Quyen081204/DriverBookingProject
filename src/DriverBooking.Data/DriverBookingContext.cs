using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DriverBooking.Data
{
    public class DriverBookingContext : IdentityDbContext<AppUser,AppRole,Guid>
    {
        public DriverBookingContext(DbContextOptions<DriverBookingContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>().ToTable("AppUsers");
            builder.Entity<AppRole>().ToTable("AppRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims").HasKey(x => x.Id);
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims").HasKey(x => x.Id);
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(x => x.UserId);
            builder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens").HasKey(x => x.UserId);
            builder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles").HasKey(x => new { x.UserId, x.RoleId });

            builder.Entity<AppUser>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();
            builder.Entity<AppUser>().Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()");

            // Customer 
            builder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.DateOfBirth).HasDefaultValueSql("NOW()");
                // Relationship with AppUser    
                entity.HasOne(e => e.CustomerAccount)
                      .WithOne()
                      .HasForeignKey<Customer>(e => e.CustomerAccountId);
            });

            // Driver entity
            builder.Entity<Driver>(entity =>
            {
                entity.Property(e => e.DateOfBirth).HasDefaultValueSql("NOW()");    
                entity.Property(e => e.DriverStatus).HasDefaultValue(DriverStatus.OFF);
                entity.Property(e => e.CurrentLocation).HasColumnType("geography (point, 4326)");
                entity.HasIndex(e => e.CurrentLocation).HasMethod("GIST");  
                entity.Property(e => e.TripCount).HasDefaultValue(0);
                entity.Property(e => e.RatingCount).HasDefaultValue(0);
                entity.HasIndex(e => e.PhoneNumber).IsUnique(true);

                // Relationship with AppUser
                entity.HasOne(e => e.DriverAccount)
                      .WithOne()
                      .HasForeignKey<Driver>(e => e.DriverAccountId);
            });

            // Vehicle 
            builder.Entity<Vehicle>(entity =>
            {
                // Relationship with driver 
                entity.HasOne(e => e.Driver)
                      .WithOne(e => e.Vehicle)
                      .HasForeignKey<Vehicle>(e => e.DriverId);

                // Opening Fee
                entity.HasOne(e => e.OpeningFee)
                      .WithMany()
                      .HasForeignKey(e => e.OpeningFeeId);

                // Many to many with StageFees
                entity.HasMany(e => e.StageFees)
                      .WithMany();
            });

            // Trip 
            builder.Entity<Trip>(entity =>
            {
                entity.Property(e => e.CurrentLocation).HasColumnType("geography (point, 4326)");
                entity.Property(e => e.Dest).HasColumnType("geography (point, 4326)");
                entity.Property(e => e.Depart).HasColumnType("geography (point, 4326)");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Property(e => e.RequestStatus).HasDefaultValue(TripRequestStatus.PENDING);

                // Relationship Custormer
                entity.HasOne(e => e.Customer)
                      .WithMany()
                      .HasForeignKey(e => e.CustomerId);
                // Driver
                entity.HasOne(e => e.Driver)
                      .WithMany()
                      .HasForeignKey(e => e.DriverId);
            });

            // TripPoint
            builder.Entity<TripPoint>(entity =>
            {
                entity.Property(e => e.TimeStamp).HasDefaultValueSql("NOW()");
                entity.Property(e => e.Location).HasColumnType("geography (point, 4326)");

                // Relationship with Trip
                entity.HasOne(e => e.Trip)
                      .WithMany()
                      .HasForeignKey(e => e.TripId);
            });

        }

        // Add DbSet properties for your entities here
        // Create dbset for remaning table
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<OpeningFee> OpeningFees { get; set; }
        public DbSet<StageFee> StageFees { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripPoint> TripPoints { get; set; }
    }
}
