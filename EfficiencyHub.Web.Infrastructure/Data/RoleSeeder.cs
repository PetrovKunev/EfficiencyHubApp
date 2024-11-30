using EfficiencyHub.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace EfficiencyHub.Web.Infrastructure.Data
{
    public static class RoleSeeder
    {
        public static async Task EnsureRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roleNames = { "User", "Administrator" };
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
        }


        public static async Task EnsureAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            string adminEmail = "admin@gmail.com";
            string adminPassword = "Admin@123";

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
            }
        }


    }
}
