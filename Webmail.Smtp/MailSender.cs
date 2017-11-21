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

        public MailSender(SmtpConfiguration configuration)
        {
            // TODO check params
            this.Credentials = new NetworkCredential(configuration.Username, configuration.Password);
            this.Host = configuration.Host;
            this.Port = configuration.Port;
            this.SecurityOptions = configuration.SecureSocketOptions;
        }

        public async Task SendMailAsync(MimeMessage message, ITransferProgress progressHandler = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var client = await GetAuthenticatedClientAsync(cancellationToken))
            {
                // TODO wziąć to skądś (może dodatkowe opcje ustawień dla usera?)
                var formatOptions = new FormatOptions
                {
                    NewLineFormat = NewLineFormat.Dos
                };

                try
                {
                    await client.SendAsync(formatOptions, message, cancellationToken, progressHandler);
                }
                catch(Exception e)
                {
                    var errorMessage = "Exception occured while sending email!";
                    logger.Warn(e, errorMessage);
                    throw new Exception(errorMessage, e);
                }
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
            }
            catch (Exception e)
            {
                var errorMessage = "Exception occured while trying to connect!";
                logger.Warn(e, errorMessage);
                throw new Exception(errorMessage, e);
            }

            try
            {
                logger.Info($"Authenticating with given credentials...");
                await client.AuthenticateAsync(this.Credentials, cancellationToken);
                logger.Info($"Authenticated!");
            }
            catch (Exception e)
            {
                var errorMessage = "Exception occured while trying to authenticate!";
                logger.Warn(e, errorMessage);
                throw new Exception(errorMessage, e);
            }

            return client;
        }
    }
}
