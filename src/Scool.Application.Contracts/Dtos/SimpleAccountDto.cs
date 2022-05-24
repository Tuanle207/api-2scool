using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class SimpleAccountDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string ClassDisplayName { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsStudent { get; set; }
        public bool IsTeacher { get; set; }
    }
}
