using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NLog;
using System;
using System.Net;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Webmail.Smtp
{
    public class MailSender
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public ICredentials Credentials { get; }
        public string Host { get; }
        public int Port { get; }
        public SecureSocketOptions SecurityOptions { get; }

        public MailSender(ICredentials credentials, string host, int port, SecureSocketOptions securityOptions)
        {
            // TODO check params
            this.Credentials = credentials;
            this.Host = host;
            this.Port = port;
            this.SecurityOptions = securityOptions;
        }

        public async Task SendMailAsync(MimeMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var client = await GetAuthenticatedClientAsync(cancellationToken))
            {
                // TODO wziąć to skądś (może dodatkowe opcje ustawień dla usera?)
                var formatOptions = new FormatOptions
                {
                    NewLineFormat = NewLineFormat.Dos
                };

                // TODO raportowanie postępów
                ITransferProgress progress = null;

                await client.SendAsync(formatOptions, message, cancellationToken, progress);
            }
        }

        private async Task<SmtpClient> GetAuthenticatedClientAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (sender, cert, chain, errors) =>
                {
                    // TODO sprawdzać certyfikat bardziej dokładnie na podstawie znanych certyfikatów
#if DEBUG
                    return true;
#else
                    return errors == SslPolicyErrors.None;
#endif
                }
            };

            try
            {
                logger.Info($"Connecting to {this.Host}:{this.Port} with security options {this.SecurityOptions}...");
                await client.ConnectAsync(this.Host, this.Port, this.SecurityOptions, cancellationToken);
                logger.Info($"Connected!");

                logger.Info($"Authenticating with given credentials...");
                await client.AuthenticateAsync(this.Credentials, cancellationToken);
                logger.Info($"Authenticated!");
            }
            catch (Exception e)
            {
                logger.Warn(e, "Exception occured when connecting or authenticating.");
                // TODO zrobić sensowne logowanie błędów
                throw;
            }

            return client;
        }
    }
}
