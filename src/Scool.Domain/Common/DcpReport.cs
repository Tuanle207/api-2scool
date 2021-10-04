using Scool.AppConsts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class DcpReport : Entity<Guid>, ICreationAuditedObject
    {
        public string Status { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public ICollection<DcpClassReport> DcpClassReports { get; set; }

        public DcpReport()
        {
            DcpClassReports = new List<DcpClassReport>();
            Status = DcpReportStatus.Created;
        }

        public DcpReport(Guid id)
        {
            Id = id;
            DcpClassReports = new List<DcpClassReport>();
            Status = DcpReportStatus.Created;
        }
    }
}
