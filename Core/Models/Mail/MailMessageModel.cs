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
        public string Title { get; set; }
        public string Content { get; set; }
        public string Recipent { get; set; }

        private SmtpClient _client;

        public void Connect(ApplicationUser user)
        {
            var serverName = "smtp.";
            serverName += user.ImapModel.login.Split('@')[1];

            _client = new SmtpClient();
            _client.Connect(serverName, 465, true);
            _client.Authenticate(user.ImapModel.login, user.ImapModel.password);
        }

        public void SendMessage(ApplicationUser user)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(user.UserName, user.ImapModel.login));
            message.To.Add(new MailboxAddress(Recipent, Recipent));
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
