using Scool.AppConsts;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class DcpReport : Entity<Guid>, IHaveCreationInfo, IMultiTenant
    {
        public string Status { get; set; }
        public DateTime CreationTime { get; set; }
        public ICollection<DcpClassReport> DcpClassReports { get; set; }
        public Guid? CreatorId { get; set; }
        public Account CreatorAccount { get; set; }
        public Guid? TenantId { get; set; }

        public DcpReport()
        {
            DcpClassReports = new List<DcpClassReport>();
            Status = DcpReportStatus.Created;
        }

        public DcpReport(Guid reportId) : this()
        {
            Id = reportId;
        }
    }
}
