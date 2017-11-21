using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class ProviderModel
    {
        [Key]
        public Guid _id { get; set; }

        public string login { get; set; }

        public string password { get; set; }

        public string ImapHost { get; set; }

        public int ImapPort { get; set; }

        public string SmtpHost { get; set; }

        public int SmtpPort { get; set; }

        public bool useSsl { get; set; }

        public ProviderModel()
        {
            login = null;
            password = null;
            ImapHost = null;
            ImapPort = 993;
            SmtpHost = null;
            SmtpPort = 465;
            useSsl = true;
        }

        public ProviderModel(string login, string password, string imaphost, int imapport, string smtphost, int smtpport, bool useSsl)
        {
            this.login = login;
            this.password = password;
            this.ImapHost = imaphost;
            this.ImapPort = imapport;
            this.SmtpHost = smtphost;
            this.SmtpPort = smtpport;
            this.useSsl = useSsl;
        }
    }
}
