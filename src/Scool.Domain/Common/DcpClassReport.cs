using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class DcpClassReport : Entity<Guid>, IMultiTenant
    {
        public Guid DcpReportId { get; set; }
        public DcpReport DcpReport { get; set; }
        public Guid ClassId { get; set; }
        public Class Class { get; set; }
        public int PenaltyTotal { get; set; }
        public Guid? TenantId { get; set; }
        public ICollection<DcpClassReportItem> Faults { get; set; }

        public DcpClassReport()
        {
            Faults = new List<DcpClassReportItem>();
            PenaltyTotal = 0;
        }
        public DcpClassReport(Guid id)
        {
            Id = id;
            Faults = new List<DcpClassReportItem>();
            PenaltyTotal = 0;
        }
    }
}
