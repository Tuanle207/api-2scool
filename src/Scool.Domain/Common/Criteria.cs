using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class Criteria : Entity<Guid>, ICreationAuditedObject, ISoftDelete
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public ICollection<Regulation> Regulations { get; set; }
        public DateTime CreationTime { get; set;}
        public Guid? CreatorId { get; set;}
        public bool IsDeleted { get; set; }

        public Criteria()
        {
            Regulations = new List<Regulation>();
            CreationTime = DateTime.UtcNow;
        }
    }
}