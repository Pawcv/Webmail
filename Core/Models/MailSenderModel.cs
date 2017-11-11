using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace Core.Models
{
    public class MailSenderModel
    {
        private readonly string _login;

        private readonly string _password;

        private SmtpClient _client;

        public MailSenderModel(string login, string password)
        {
            _login = login;
            _password = password;
        }

        public void Connect()
        {
            var serverName = "smtp.";
            serverName += _login.Split('@')[1];

            _client = new SmtpClient();
            _client.Connect(serverName, 465, true);
            _client.Authenticate(_login, _password);
        }

        public void SendMessage(string text)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Testowy nadawca", _login));
            message.To.Add(new MailboxAddress("Testowy odbiorca", _login));
            message.Subject = "Test";

            message.Body = new TextPart("plain")
            {
                Text = text
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
