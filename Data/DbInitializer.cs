using Microsoft.AspNetCore.Identity;
using MiniStationery.Mvc.Models;

namespace MiniStationery.Mvc.Data;

public static class DbInitializer
{
    public static async Task SeedIdentityAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Admin", "Staff", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await CreateUser(userManager, "admin@ministationery.test", "Admin@123", "Admin");
        await CreateUser(userManager, "staff@ministationery.test", "Staff@123", "Staff");
        await CreateUser(userManager, "user@ministationery.test", "User@123", "User");
    }

    private static async Task CreateUser(UserManager<ApplicationUser> userManager, string email, string password, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, FullName = role + " Demo" };
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);
        }
    }
}