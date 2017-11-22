using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class ImapConfiguration
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; } = 993;
        public bool UseSsl { get; set; }

        public void OverrideWith(ImapConfiguration other)
        {
            this.Host = other.Host;
            this.Port = other.Port;
            this.Login = other.Login;
            this.Password = other.Password;
            this.UseSsl = other.UseSsl;
        }
    }
}
