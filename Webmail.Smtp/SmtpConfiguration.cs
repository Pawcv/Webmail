using MailKit.Security;

namespace Webmail.Smtp
{
    public class SmtpConfiguration
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public SecureSocketOptions SecureSocketOptions { get; set; }
    }
}
