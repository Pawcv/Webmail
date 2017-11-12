using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Threading.Tasks;

namespace Core.Models
{
    public class MailMessageModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Recipent { get; set; }

        private SmtpClient _client;

        public void Connect()
        {
            var serverName = "smtp.";
            serverName += Login.Split('@')[1];

            _client = new SmtpClient();
            _client.Connect(serverName, 465, true);
            _client.Authenticate(Login, Password);
        }

        public void SendMessage()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Testowy nadawca", Login));
            message.To.Add(new MailboxAddress("Testowy odbiorca", Recipent));
            message.Subject = Title;

            message.Body = new TextPart("html")
            {
                Text = Content
            };

            _client.Send(message);
        }

        public void Disconnect()
        {
            _client.Disconnect(true);
            _client.Dispose();
        }
    }
}
