using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Core.Data
{
    public class DatabaseInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseInitializer(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
        }

        public void Initialize()
        {
            _dbContext.Database.Migrate();

            this.AddTestAdmin().Wait();
        }

        private async Task AddTestAdmin()
        {
            try
            {
                var testAdminEmail = "admin@admin.com";
                var testAdmin = await this._userManager.FindByNameAsync(testAdminEmail);

                if (testAdmin == null)
                {
                    var user = new ApplicationUser { UserName = testAdminEmail, Email = testAdminEmail, EmailConfirmed = true };

                    var userResult = await this._userManager.CreateAsync(user, "Admin1!");

                    if (!userResult.Succeeded)
                    {
                        throw new InvalidOperationException("Failed to add first admin!");
                    }
                }

                Console.WriteLine($"Test admin succesffully added!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Adding test admin failed! Reason: {e}");
            }
        }
    }
}
