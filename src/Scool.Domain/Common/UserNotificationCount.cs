using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class UserNotificationCount : Entity<Guid>, IMultiTenant
    {
        public Guid UserId { get; set; }
        public int Value { get; set; } = 0;
        public Guid? TenantId { get; set; }
    }
}
