using System;
using System.IO;
using Volo.Abp.BackgroundJobs;

namespace Scool.Email
{
    [BackgroundJobName("report_emails")]
    public class ReportEmailSendingArgs : SimpleEmailSendingArgs
    {
        public string ReportName { get; set; }
        public string ReportType { get; set; }
        public Guid? EntityId { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }

    public class EmailAttachment
    {
        public string Filename { get; set; }
        public byte[] Content{ get; set; }
    }
}
