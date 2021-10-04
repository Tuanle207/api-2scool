using System;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class ActivityParticipant : Entity<Guid>
    {
        public Guid ClassId { get; set; }
        public Guid ActivityId { get; set; }
        public Activity Activity { get; set; }
        public int? Place { get; set; }
    }
}