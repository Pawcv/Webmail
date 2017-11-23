using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Webmail.Smtp;

namespace Core.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ICollection<SmtpConfiguration> SmtpConfigurations { get; set; } = new List<SmtpConfiguration>();
        public ICollection<ImapConfiguration> ImapConfigurations { get; set; } = new List<ImapConfiguration>();
    }
}
