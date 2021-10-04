using System;
using System.Collections.Generic;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class TaskAssignment : Entity<Guid>, ICreationAuditedObject
    {
        public Guid AssigneeId { get; set; }
        public UserProfile AssigneeProfile { get; set; }
        public Guid ClassAssignedId { get; set; }
        public Class ClassAssigned { get; set; }
        public string TaskType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime  { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
    }
}