using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class Criteria : Entity<Guid>, IHaveCreationInfo, ISoftDelete, IMultiTenant
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public ICollection<Regulation> Regulations { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Account CreatorAccount { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? TenantId { get; set; }

        public Criteria()
        {
            Regulations = new List<Regulation>();
        }
    }
}