using EfficiencyHub.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace EfficiencyHub.Web.Infrastructure.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAndAdminAsync(
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            string[] roleNames = { "Administrator", "User" };

            // Seed roles
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
                    if (result.Succeeded)
                    {
                        Console.WriteLine($"Role '{roleName}' created successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Error creating role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // Seed Admin User
            string adminEmail = configuration["AdminUser:Email"] ?? throw new InvalidOperationException("Admin email is not configured properly.");
            string adminPassword = configuration["AdminUser:Password"] ?? throw new InvalidOperationException("Admin password is not configured properly.");

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
                else
                {
                    Console.WriteLine($"Error creating admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
