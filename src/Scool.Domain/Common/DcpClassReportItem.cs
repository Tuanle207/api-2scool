using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class DcpClassReportItem : Entity<Guid>, IMultiTenant
    {
        public DcpClassReportItem()
        {
            RelatedStudents = new List<DcpStudentReport>();
        }

        public DcpClassReportItem(Guid id) : this()
        {
            Id = id;
        }

        public Guid DcpClassReportId { get; set; }
        public Guid RegulationId { get; set; }
        public Regulation Regulation { get; set; }
        public int PenaltyPoint { get; set; }
        public ICollection<DcpStudentReport> RelatedStudents { get; set; }
        public Guid? TenantId { get; set; }
    }
}
