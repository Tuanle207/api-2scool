using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class AppSetting : Entity<Guid>, IMultiTenant
    {
        public string TypeCode { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public Guid? UserId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
