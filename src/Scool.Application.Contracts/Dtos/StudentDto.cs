using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class StudentDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public Guid ClassId { get; set; }
        public ClassForStudentDto Class { get; set; }
        public DateTime Dob { get; set; }
        public string ParentPhoneNumber { get; set; }
    }
}
