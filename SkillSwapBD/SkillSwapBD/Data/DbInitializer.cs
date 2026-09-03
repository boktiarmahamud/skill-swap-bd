using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillSwapBD.Models;

namespace SkillSwapBD.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            foreach (var role in new[] { "Admin", "User" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var admin = await userManager.FindByEmailAsync("admin@skillswapbd.com");
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin@skillswapbd.com",
                    Email = "admin@skillswapbd.com",
                    EmailConfirmed = true,
                    FullName = "Admin",
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            var rahim = await userManager.FindByEmailAsync("rahim@example.com");
            if (rahim == null)
            {
                rahim = new ApplicationUser
                {
                    UserName = "rahim@example.com",
                    Email = "rahim@example.com",
                    EmailConfirmed = true,
                    FullName = "Rahim Ahmed",
                    Location = "Dhaka",
                    CreatedAt = DateTime.UtcNow
                };
                await userManager.CreateAsync(rahim, "Passw0rd!123");
                await userManager.AddToRoleAsync(rahim, "User");
            }
        }
    }
}
