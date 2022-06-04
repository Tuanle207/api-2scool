using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class DcpStudentReport : Entity<Guid>, IMultiTenant
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public Guid DcpClassReportItemId { get; set; }
        public DcpClassReportItem DcpClassReportItem { get; set; }
        public Guid? TenantId { get; set; }
    }
}
