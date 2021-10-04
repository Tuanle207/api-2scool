using Scool.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class TaskAssignmentForUpdateDto : EntityDto<Guid>
    {
        public Guid AssigneeId { get; set; }
        public Guid ClassAssignedId { get; set; }
        public string TaskType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime  { get; set; }
    }
}
