using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class Regulation : Entity<Guid>, IHaveCreationInfo, IHaveCourse, ISoftDelete, IMultiTenant
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int Point { get; set; }
        public string Type { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid CriteriaId { get; set; }
        public Criteria Criteria { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Account CreatorAccount { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? TenantId { get; set; }

        public Regulation()
        {
            IsActive = true;
        }
    }
}