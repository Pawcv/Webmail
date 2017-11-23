using MailKit.Security;
using System.ComponentModel.DataAnnotations;

namespace Webmail.Smtp
{
    public class SmtpConfiguration
    {
        [Key]
        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; } = 465;
        public string Username { get; set; }
        public string Password { get; set; }
        public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.Auto;

        public void OverrideWith(SmtpConfiguration other)
        {
            this.Host = other.Host;
            this.Port = other.Port;
            this.Username = other.Username;
            this.Password = other.Password;
            this.SecureSocketOptions = other.SecureSocketOptions;
        }
    }
}
