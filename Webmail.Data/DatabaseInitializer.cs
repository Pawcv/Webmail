using System.Linq;
using Webmail.Data.Entities;

namespace Webmail.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(WebmailContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return; // dane już są
            }

            var users = new User[]
            {
                new User { Login = "Admin", HashedPassword = User.HashPassword("Admin") },
                new User { Login = "User1", HashedPassword = User.HashPassword("User1") },
                new User { Login = "User2", HashedPassword = User.HashPassword("User2") }
            };

            context.Users.AddRange(users);

            context.SaveChanges();
        }
    }
}
