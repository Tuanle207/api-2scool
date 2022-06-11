using System.Collections.Generic;
using System.IO;
using Volo.Abp.BackgroundJobs;

namespace Scool.Email
{
    [BackgroundJobName("simple_emails")]
    public class SimpleEmailSendingArgs
    {
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
