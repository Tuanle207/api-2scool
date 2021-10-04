using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class UserForTaskAssignmentDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public Guid UserProfileId { get; set; }
        public ClassForSimpleListDto Class { get; set; }
        public string PhoneNumber { get; set; }
    }
}
