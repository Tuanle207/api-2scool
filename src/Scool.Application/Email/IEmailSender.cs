using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scool.Email
{
    public interface IEmailSender
    {
        Task QueueAsync(SimpleEmailSendingArgs email);
        Task QueueAsync(ReportEmailSendingArgs email);
        Task SendAsync(string to, string subject, string body, List<EmailAttachment> attachments = null);
        Task SendAsync(List<string> to, string subject, string body, List<EmailAttachment> attachments = null);
    }
}
