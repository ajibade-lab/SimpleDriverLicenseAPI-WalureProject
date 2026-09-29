using DriverLicenseApi.Models;
using Microsoft.AspNetCore.Identity;

namespace DriverLicenseApi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Applicant", "Staff", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed default Staff User
            var staffEmail = "staff@license.gov";
            var staffUser = await userManager.FindByEmailAsync(staffEmail);
            if (staffUser == null)
            {
                staffUser = new ApplicationUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    FullName = "Officer John Doe",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(staffUser, "StaffPass123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(staffUser, "Staff");
                }
            }
        }
    }
}
