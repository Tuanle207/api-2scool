using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
namespace Scool.Common
{
    public class Activity : Entity<Guid>, IHaveUpdationInfo, IHaveCourse, IMultiTenant, ISoftDelete
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public string Status { get; set; }
        public ICollection<ActivityParticipant> Participants { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Account CreatorAccount { get; set; }
        public Guid? LastUpdatorId { get; set; }
        public Account LastUpdatorAccount { get; set; }
        public DateTime? LastUpdationTime { get; set; }
        public Guid? TenantId { get; set; }
        public bool IsDeleted { get; set; }

        public Activity()
        {
            Participants = new List<ActivityParticipant>();
        }
    }
}