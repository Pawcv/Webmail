using System.Collections.Generic;

using MailKit.Net.Imap;
using MailKit;

namespace Core.Models
{
    public class ImapClientModel
    {
        public string ActiveFolder
        {
            get { return _activeFolder; }
            set
            {
                if (value != _activeFolder)
                {
                    // active folder changed, so list of headers must be downloaded for a new folder
                    _headers = null;
                }
                _activeFolder = value;
            }
        }

        public List<IMailFolder> Folders
        {
            get
            {
                if (_folders == null)
                {
                    _folders = new List<IMailFolder>();
                    _downloadFolders();
                }
                return _folders;
            }
        }

        // list of headers in active folder
        public IList<IMessageSummary> Headers
        {
            get
            {
                if (_headers == null)
                {
                    _downloadHeaders();
                }
                return _headers;
            }
        }

        private readonly string _login;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _useSsl;

        private ImapClient _client;
        private string _activeFolder;
        private List<IMailFolder> _folders;
        private IList<IMessageSummary> _headers;
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

        public void DownloadFullMessage()
        {

        }


        private void _downloadFolders()
        {
            var root = _client.GetFolder(_client.PersonalNamespaces[0]);
            _downloadFoldersRecursively(root);
        }

        private void _downloadFoldersRecursively(IMailFolder folder)
        {
            _folders.Add(folder);
            foreach (var subfolder in folder.GetSubfolders(false))
            {
                _downloadFoldersRecursively(subfolder);
            }
        }
        private void _downloadHeaders()
        {
            var folder = _client.GetFolder(ActiveFolder);
            folder.Open(FolderAccess.ReadOnly);
            _headers = folder.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId);
            folder.Close();
        }
    }
}
