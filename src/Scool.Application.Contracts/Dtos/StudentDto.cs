using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
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
