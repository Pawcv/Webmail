using System.Collections.Generic;

using MailKit.Net.Imap;
using MailKit;
using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.Models
{
    public class ImapClientModel : IDisposable
    {
        public readonly static Dictionary<string, MailKit.Search.OrderBy> OrderTypes = new Dictionary<string, MailKit.Search.OrderBy>
        {
            {"Arrival", MailKit.Search.OrderBy.Arrival},
            {"Reverse Arrival", MailKit.Search.OrderBy.ReverseArrival },
            {"From", MailKit.Search.OrderBy.From },
            {"Reverse From", MailKit.Search.OrderBy.ReverseFrom},
            {"Subject", MailKit.Search.OrderBy.Subject },
            {"Reverse Subject", MailKit.Search.OrderBy.ReverseSubject }
        };

        public static ConcurrentDictionary<string, ImapClientModel> ImapClientModelsDictionary = new ConcurrentDictionary<string, ImapClientModel>();

        public string ActiveFolder
        {
            get => _activeFolder;
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

        public bool IsConnected => _client.IsConnected;

        private readonly string _login;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _useSsl;

        private readonly ImapClient _client;
        private string _activeFolder;
        private List<IMailFolder> _folders;
        private IList<IMessageSummary> _headers;
        private Dictionary<Tuple<string, UniqueId>, MimeKit.MimeMessage> _messages;
        private bool _disposed;

        public ImapClientModel(string login, string password, string host, int port, bool useSsl)
        {
            _login = login;
            _password = password;
            _host = host;
            _port = port;
            _useSsl = useSsl;
            _messages = new Dictionary<Tuple<string, UniqueId>, MimeKit.MimeMessage>();
            _client = new ImapClient();
            ImapClientModelsDictionary.TryAdd(_login+_password, this);
        }

        public void Connect()
        {
            _client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            _client.Connect(_host, _port, _useSsl);
            _client.AuthenticationMechanisms.Remove("XOAUTH2");
            _client.Authenticate(_login, _password);
        }

        public void Disconnect()
        {
            _client.Disconnect(true);
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

        public SelectList GetOrderTypes()
        {
            return new SelectList(OrderTypes, "Key", "Key");
        }

        public void SortHeaders(IList<MailKit.Search.OrderBy> order)
        {
            if (_headers != null)
            {
               _headers = MessageSorter.Sort(_headers, order);
            }
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
            if (!folder.FullName.Equals(""))
            {
                Folders.Add(folder);
            }
            foreach (var subfolder in folder.GetSubfolders())
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

            if (!_disposed)
            {
                if (disposing)
                {
                    _client.Dispose();
                }
                ImapClientModelsDictionary.TryRemove(_login + _password, out var _);
                _disposed = true;
            }
        }

        ~ImapClientModel()
        {
            Dispose(false);
        }
    }
}
