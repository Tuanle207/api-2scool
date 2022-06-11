using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Scool.Email
{
    public class EmailSender : IEmailSender, ITransientDependency
    {
        private readonly EmailOptions _options;
        private readonly ILogger<EmailSender> _logger;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public EmailSender(IBackgroundJobManager backgroundJobManager, IOptions<EmailOptions> options, ILogger<EmailSender> logger)
        {
            _backgroundJobManager = backgroundJobManager;
            _options = options.Value;
            _logger = logger;
        }

        public async Task QueueAsync(SimpleEmailSendingArgs email)
        {
            await _backgroundJobManager.EnqueueAsync(email);
        }

        public async Task QueueAsync(ReportEmailSendingArgs email)
        {
            await _backgroundJobManager.EnqueueAsync(email);
        }

        public async Task SendAsync(string to, string subject, string body, List<EmailAttachment> attachments = null)
        {
            await SendAsync(new List<string> { to }, subject, body, attachments);
        }

        public async Task SendAsync(List<string> to, string subject, string body, List<EmailAttachment> attachments = null)
        {
            try
            {
                var fromMail = _options.SmtpUsername;
                var clientSecrets = new ClientSecrets
                {
                    ClientId = _options.GoogleClientId,
                    ClientSecret = _options.GoogleClientSecret,
                };

                var authorizationCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = clientSecrets
                });

                var tokenResponse = await authorizationCodeFlow.RefreshTokenAsync(fromMail, _options.GoogleRefreshToken, CancellationToken.None);

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(fromMail));
                email.To.AddRange(to.Select(x => MailboxAddress.Parse(x)));
                email.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = body
                };

                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        builder.Attachments.Add(attachment.Filename, attachment.Content);
                    }
                }

                email.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(_options.SmtpHost, _options.SmtpPort, SecureSocketOptions.StartTls);
                    var oauth2 = new SaslMechanismOAuth2(_options.SmtpUsername, tokenResponse.AccessToken);
                    client.Authenticate(oauth2);
                    await client.SendAsync(email);
                    client.Disconnect(true);
                }
                _logger.LogInformation($"Email sent to {to.JoinAsString(", ")}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {to.JoinAsString(", ")}", ex.Message, ex);
            }
        }
    }
}
