using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace Core.Data
{
    public class DatabaseInitializer
    {
        private UserManager<ApplicationUser> userManager;

        public DatabaseInitializer(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task Initialize()
        {
            var testAdmin = await this.userManager.FindByNameAsync("admin@admin.com");

            if (testAdmin == null)
            {
                var user = new ApplicationUser { UserName = "admin@admin.com", Email = "admin@admin.com", EmailConfirmed = true };

                var userResult = await this.userManager.CreateAsync(user, "Admin1!");

                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to build user and roles");
                }
            }
        }
    }
}
