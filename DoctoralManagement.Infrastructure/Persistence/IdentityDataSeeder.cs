using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DoctoralManagement.Infrastructure.Persistence
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // Check if Admin Role exists
            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                adminRole = new IdentityRole<int> { Name = "Admin", NormalizedName = "ADMIN" };
                await roleManager.CreateAsync(adminRole);
            }

            // Check if Admin User exists
            var adminUser = await userManager.FindByEmailAsync("admin@dms.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin123@dms.com",
                    Email = "admin@dms.com",
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    Role = UserRole.Admin,
                    IsActive = true
                };

                await userManager.CreateAsync(adminUser, "Admin123!"); // strong password
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
