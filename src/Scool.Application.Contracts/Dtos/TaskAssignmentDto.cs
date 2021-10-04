using Scool.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class TaskAssignmentDto : EntityDto<Guid>
    {
        public UserProfleForTaskAssignmentDto Assignee { get; set; }
        public ClassForSimpleListDto ClassAssigned { get; set; }
        public string TaskType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime  { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId{ get; set; }
        public UserForSimpleListDto Creator{ get; set; }
    }
}
