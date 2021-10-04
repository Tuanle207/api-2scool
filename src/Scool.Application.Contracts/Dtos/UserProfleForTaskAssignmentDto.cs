using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class UserProfleForTaskAssignmentDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public ClassForSimpleListDto Class { get; set; }
        public string PhoneNumber { get; set; }
    }
}
