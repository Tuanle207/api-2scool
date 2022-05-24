using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class ActivityParticipant : Entity<Guid>, IMultiTenant
    {
        public Guid ClassId { get; set; }
        public Guid ActivityId { get; set; }
        public Activity Activity { get; set; }
        public int? Place { get; set; }
        public Guid? TenantId { get; set; }
    }
}