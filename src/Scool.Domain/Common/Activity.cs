using System;
using System.Collections.Generic;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class Activity : Entity<Guid>, IAuditedObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public string Status { get; set; }
        public ICollection<ActivityParticipant> Participants { get; set; }

        // audit property
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }

        public Activity()
        {
            Participants = new List<ActivityParticipant>();
        }
  }
}