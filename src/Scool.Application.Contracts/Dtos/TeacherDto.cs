using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class TeacherDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public DateTime Dob { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
