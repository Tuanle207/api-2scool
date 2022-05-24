using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class TaskAssignmentForUpdateDto : EntityDto<Guid>
    {
        public Guid AssigneeId { get; set; }
        public Guid ClassAssignedId { get; set; }
        public string TaskType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
