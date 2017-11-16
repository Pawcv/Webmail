using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimeKit;
using System.Net;
using System.Threading.Tasks;
using Webmail.Smtp;

namespace Smpt.Tests
{
    [TestClass]
    public class MailSenderTests
    {
        [TestMethod]
        public async Task SendMailTest()
        {
            var sender = this.GetSender();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Ja", "asdf-67@wp.pl"));
            message.To.Add(new MailboxAddress("Ja", "asdf-67@wp.pl"));
            message.Subject = "test";
            message.Body = new TextPart("plain")
            {
                Text = "Blabla"
            };

            await sender.SendMailAsync(message);
        }

        private MailSender GetSender()
        {
            return new MailSender(new NetworkCredential("asdf-67@wp.pl", "asdF123$"), "smtp.wp.pl", 465, MailKit.Security.SecureSocketOptions.Auto);
        }
    }
}