using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class AppNotification : Entity<Guid>, IMultiTenant, IHasCreationTime
    {
        public Guid? TenantId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool Seen { get; set; } = false;
        public DateTime CreationTime { get; set; }
        public Guid? FromAccountId { get; set; }
        public Account FromAccount { get; set; }
    }
}
