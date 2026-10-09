using DriverLicenseApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DriverLicenseApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
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

    //public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    //{
    //    public ApplicationDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

    //        // Hardcode your development connection string here just for migrations
    //        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DriverLicenseProjectDB;Trusted_Connection=True;TrustServerCertificate=True;");

    //        return new ApplicationDbContext(optionsBuilder.Options);
    //    }
    //}
}
