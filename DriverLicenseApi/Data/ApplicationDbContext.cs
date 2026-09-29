using DriverLicenseApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DriverLicenseApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }

        public DbSet<LicenseApplication> LicenseApplications => Set<LicenseApplication>();
        public DbSet<ApplicationStatusHistory> ApplicationStatusHistories => Set<ApplicationStatusHistory>();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LicenseApplication>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationStatusHistory>()
                .HasOne(h => h.LicenseApplication)
                .WithMany(a => a.StatusHistories)
                .HasForeignKey(h => h.LicenseApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
