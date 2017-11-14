using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class ImapProviderModel
    {
        [Key]
        public Guid _id { get; set; }

        public string login { get; set; }

        public string password { get; set; }

        public string host { get; set; }

        public int port { get; set; }

        public bool useSsl { get; set; }

        public ImapProviderModel()
        {
            login = null;
            password = null;
            host = null;
            port = 993;
            useSsl = true;
        }

        public ImapProviderModel(string login, string password, string host, int port, bool useSsl)
        {
            this.login = login;
            this.password = password;
            this.host = host;
            this.port = port;
            this.useSsl = useSsl;
        }
    }
}
