using Microsoft.EntityFrameworkCore;
using Webmail.Data.Entities;

namespace Webmail.Data
{
    public class WebmailContext : DbContext
    {
        public WebmailContext(DbContextOptions<WebmailContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
