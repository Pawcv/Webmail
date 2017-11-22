using System.Collections.Generic;

using MailKit.Net.Imap;
using MailKit;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Core.Models
{
    public class ImapClientModel : IDisposable
    {
        public static ConcurrentDictionary<string, ImapClientModel> ImapClientModelsDictionary = new ConcurrentDictionary<string, ImapClientModel>();

        public string ActiveFolder { get; set; }

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
                if (ActiveFolder == null)
                {
                    return null;
                }

                if (!_headers.ContainsKey(ActiveFolder))
                {
                    _downloadHeaders(ActiveFolder);
                }
                return _headers[ActiveFolder];
            }
        }

        public bool IsConnected => _client.IsConnected;

        private readonly string _login;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _useSsl;

        private readonly ImapClient _client;
        private List<IMailFolder> _folders;
        private ConcurrentDictionary<string, IList<IMessageSummary>> _headers; //key: folder name
        private Dictionary<Tuple<string, UniqueId>, MimeKit.MimeMessage> _messages;
        private ImapClient _idleClient;
        private Thread _idleThread;
        private CancellationTokenSource _idleThreadDone;
        private bool _disposed;

        public ImapClientModel(string login, string password, string host, int port, bool useSsl)
        {
            _login = login;
            _password = password;
            _host = host;
            _port = port;
            _useSsl = useSsl;
            _headers = new ConcurrentDictionary<string, IList<IMessageSummary>>();
            _messages = new Dictionary<Tuple<string, UniqueId>, MimeKit.MimeMessage>();
            _client = new ImapClient();
            _idleClient = new ImapClient();
            ImapClientModelsDictionary.TryAdd(_login + _password, this);
        }

        public void Connect()
        {
            _client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            _client.Connect(_host, _port, _useSsl);
            _client.AuthenticationMechanisms.Remove("XOAUTH2");
            _client.Authenticate(_login, _password);

            _idleClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
            _idleClient.Connect(_host, _port, _useSsl);
            _idleClient.AuthenticationMechanisms.Remove("XOAUTH2");
            _idleClient.Authenticate(_login, _password);

            //TODO subscribe all folders
            _subscribeFolder("INBOX");
            _startIdleThread();
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

        public void Refresh()
        {
            _downloadFolders();
            if (!Folders.Exists(f => f.FullName.Equals(ActiveFolder)))
            {
                ActiveFolder = "INBOX";
            }
            _downloadHeaders(ActiveFolder);
        }

        private void _downloadMessage(string folderName, UniqueId uid)
        {
            lock (_client.SyncRoot)
            {
                var folder = _client.GetFolder(folderName);
                folder.Open(FolderAccess.ReadOnly);
                _messages[Tuple.Create(folderName, uid)] = folder.GetMessage(uid);
                folder.Close();
            }
        }


        private void _downloadFolders()
        {
            lock (_client.SyncRoot)
            {
                _folders.Clear();
                var root = _client.GetFolder(_client.PersonalNamespaces[0]);
                _downloadFoldersRecursively(root);
            }
        }

        private void _downloadFoldersRecursively(IMailFolder folder)
        {
            if (!folder.FullName.Equals("") && folder.Exists)
            {
                _folders.Add(folder);
            }
            foreach (var subfolder in folder.GetSubfolders())
            {
                _downloadFoldersRecursively(subfolder);
            }
        }

        private void _downloadHeaders(string folderName)
        {
            lock (_client.SyncRoot)
            {
                var folder = _client.GetFolder(folderName);
                folder.Open(FolderAccess.ReadOnly);
                _headers[folderName] = folder.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId);
                folder.Close();
            }
        }

        private void _subscribeAllFolders()
        {
            foreach (var folder in Folders)
            {
                _subscribeFolder(folder.FullName);
            }
        }

        private void _subscribeFolder(string folderName)
        {
            lock (_idleClient.SyncRoot)
            {
                var folder = _idleClient.GetFolder(folderName);
                folder.Open(FolderAccess.ReadOnly);

                folder.MessageExpunged += (sender, e) =>
                {
                //TODO remove message from dictionary
                _downloadHeaders(((ImapFolder)sender).FullName);
                };
                folder.CountChanged += (sender, e) =>
                {
                    _downloadHeaders(((ImapFolder)sender).FullName);
                };
            }
        }

        private void _startIdleThread()
        {
            _idleThreadDone = new CancellationTokenSource();
            _idleThread = new Thread(_idleLoop);
            _idleThread.Start(new IdleState(_idleClient, _idleThreadDone.Token));
        }

        private void _stopIdleThread()
        {
            if (_idleThreadDone != null)
            {
                _idleThreadDone.Cancel();
                _idleThread.Join();
                _idleThreadDone.Dispose();
                _idleThreadDone = null;
            }
        }

        private static void _idleLoop(object state)
        {
            var idleState = (IdleState)state;

            lock (idleState.Client.SyncRoot)
            {
                while (!idleState.IsCancellationRequested)
                {
                    using (var timeout = new CancellationTokenSource(new TimeSpan(0, 0, 10)))
                    {
                        try
                        {
                            idleState.SetTimeoutSource(timeout);

                            if (idleState.Client.Capabilities.HasFlag(ImapCapabilities.Idle))
                            {
                                idleState.Client.Idle(timeout.Token, idleState.CancellationToken);
                            }
                            else
                            {
                                idleState.Client.NoOp(idleState.CancellationToken);

                                WaitHandle.WaitAny(new[] { timeout.Token.WaitHandle, idleState.CancellationToken.WaitHandle });
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                        catch (ImapProtocolException)
                        {
                            break;
                        }
                        catch (ImapCommandException)
                        {
                            break;
                        }
                        finally
                        {
                            idleState.SetTimeoutSource(null);
                        }
                    }
                }
            }
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
                    _idleClient.Dispose();
                    _idleThreadDone.Dispose();
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

class IdleState
{
    readonly object mutex = new object();
    CancellationTokenSource timeout;
    public CancellationToken CancellationToken { get; private set; }
    public CancellationToken DoneToken { get; private set; }
    public ImapClient Client { get; private set; }

    public bool IsCancellationRequested
    {
        get
        {
            return CancellationToken.IsCancellationRequested || DoneToken.IsCancellationRequested;
        }
    }

    public IdleState(ImapClient client, CancellationToken doneToken, CancellationToken cancellationToken = default(CancellationToken))
    {
        this.CancellationToken = cancellationToken;
        this.DoneToken = doneToken;
        this.Client = client;

        this.DoneToken.Register(CancelTimeout);
    }

    // gracefully cancel timeout
    void CancelTimeout()
    {
        lock (mutex)
        {
            if (timeout != null)
                timeout.Cancel();
        }
    }

    public void SetTimeoutSource(CancellationTokenSource source)
    {
        lock (mutex)
        {
            timeout = source;

            if (timeout != null && IsCancellationRequested)
                timeout.Cancel();
        }
    }
}

