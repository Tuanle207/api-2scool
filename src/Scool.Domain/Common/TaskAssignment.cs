using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class TaskAssignment : Entity<Guid>, IHaveCreationInfo, IHaveCourse, IMultiTenant
    {
        public Guid AssigneeId { get; set; }
        public Account Assignee { get; set; }
        public Guid ClassAssignedId { get; set; }
        public Class ClassAssigned { get; set; }
        public string TaskType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Account CreatorAccount { get; set; }
        public Guid? TenantId { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}