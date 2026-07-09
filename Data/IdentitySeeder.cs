using DailyWorkReport.Constants;
using DailyWorkReport.Models;
using Microsoft.AspNetCore.Identity;

namespace DailyWorkReport.Data;
public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
        string[] roleNames = { Roles.Admin, Roles.User };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create an admin user if it doesn't exist
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser { UserName = "admin" };
            var result =await userManager.CreateAsync(adminUser, "Admin@123");
            if(result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            }
            else
            {
                throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}