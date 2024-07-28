using CarpoolPlatformAPI.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarpoolPlatformAPI.Data
{
    public class CarpoolPlatformDbContext : IdentityDbContext<User>
    {

        public DbSet<User> Users { get; set; }

        public CarpoolPlatformDbContext(DbContextOptions<CarpoolPlatformDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /*string basicUserRoleId = "4066da82-f923-4a73-ae50-a29a5c76c5c1";
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

            modelBuilder.Entity<IdentityRole>().HasData(roles);*/
        }
    }

}
