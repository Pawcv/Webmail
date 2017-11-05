using System.Collections.Generic;

using MailKit.Net.Imap;
using MailKit;
using System;

namespace Core.Models
{
    public class ImapClientModel : IDisposable
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
        private Dictionary<Tuple<string, UniqueId>, MimeKit.MimeMessage> _messages;
        private bool _connected;

        public ImapClientModel(string login, string password, string host, int port, bool useSsl)
        {
            _login = login;
            _password = password;
            _host = host;
            _port = port;
            _useSsl = useSsl;
            _connected = false;
            _messages = new Dictionary<Tuple<string, UniqueId>, MimeKit.MimeMessage>();
            _client = new ImapClient();
        }

        public void Connect()
        {
            _client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            _client.Connect(_host, _port, _useSsl);
            _client.AuthenticationMechanisms.Remove("XOAUTH2");
            _client.Authenticate(_login, _password);
            _connected = true;
        }

        public void Disconnect()
        {
            _client.Disconnect(true);
            _connected = false;
        }

        public MimeKit.MimeMessage GetMessage(string folderName, uint uid)
        {
            return GetMessage(folderName, new UniqueId(uid));
        }

        public MimeKit.MimeMessage GetMessage(string folderName, UniqueId uid)
        {
            var key = Tuple.Create(folderName, uid);
            if (!_messages.ContainsKey(key))
            {
                _downloadMessage(folderName, uid);
            }
            return _messages[key];
        }

        private void _downloadMessage(string folderName, UniqueId uid)
        {
            var folder = _client.GetFolder(folderName);
            folder.Open(FolderAccess.ReadOnly);
            _messages[Tuple.Create(folderName, uid)] = folder.GetMessage(uid);
            folder.Close();
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

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
