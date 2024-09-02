using CarpoolPlatformAPI.Data.Configurations;
using CarpoolPlatformAPI.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarpoolPlatformAPI.Data
{
    public class CarpoolPlatformDbContext : IdentityDbContext<User>
    {
        public DbSet<Ride> Rides { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Picture> Pictures { get; set; }

        public CarpoolPlatformDbContext(DbContextOptions<CarpoolPlatformDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string basicUserRoleId = "4066da82-f923-4a73-ae50-a29a5c76c5c1";
            string adminRoleId = "53fee358-1930-4d37-8c9c-4225365c33c1";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = basicUserRoleId,
                    ConcurrencyStamp = basicUserRoleId,
                    Name = "Basic_User",
                    NormalizedName = "BASIC_USER"
                },
                new IdentityRole
                {
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RideConfiguration());
            modelBuilder.ApplyConfiguration(new BookingConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new PictureConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
