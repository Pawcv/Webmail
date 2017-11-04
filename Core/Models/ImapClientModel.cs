using System.Collections.Generic;

using MailKit.Net.Imap;
using MailKit;

namespace Core.Models
{
    public class ImapClientModel
    {
        public List<string> FolderList
        {
            get
            {
                if (_folderList == null)
                {
                    _folderList = new List<string>();
                    _getFolderNames();
                }
                return _folderList;
            }
        }

        private readonly string _login;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _useSsl;

        private ImapClient _client;
        private List<string> _folderList;
        private bool _connected;

        public ImapClientModel(string login, string password, string host, int port, bool useSsl)
        {
            _login = login;
            _password = password;
            _host = host;
            _port = port;
            _useSsl = useSsl;
            _connected = false;
        }

        public void Connect()
        {
            _client = new ImapClient();
            _client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            _client.Connect(_host, _port, _useSsl);
            _client.AuthenticationMechanisms.Remove("XOAUTH2");
            _client.Authenticate(_login, _password);
            _connected = true;
        }

        public void Disconnect()
        {
            _client.Disconnect(true);
            _client.Dispose();
            _connected = false;
        }

        public void DownloadHeaders()
        {

        }

        public void DownloadFullMessage()
        {

        }


        private void _getFolderNames()
        {
            var root = _client.GetFolder(_client.PersonalNamespaces[0]);
            _getFolderNamesRecursively(root);
        }

        private void _getFolderNamesRecursively(IMailFolder folder)
        {
            _folderList.Add(folder.FullName);
            foreach (var subfolder in folder.GetSubfolders(false))
            {
                _getFolderNamesRecursively(subfolder);
            }
        }
    }
}
