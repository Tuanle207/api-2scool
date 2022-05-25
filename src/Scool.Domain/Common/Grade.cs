using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class Grade : Entity<Guid>, IMultiTenant
    {
        public string DisplayName { get; set; }
        public int GradeCode { get; set; }
        public string Description { get; set; }
        public Guid? TenantId { get; set; }
    }
}