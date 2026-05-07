using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
namespace Graduation_Project.BLL.Seeders
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var adminEmail = "Admin@localbrand.com";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin != null) return; 

            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Super Admin",
                UserType = UserType.Admin,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsBlocked = false
            };

            await userManager.CreateAsync(admin, "Admin@12345");
        }
    }
}