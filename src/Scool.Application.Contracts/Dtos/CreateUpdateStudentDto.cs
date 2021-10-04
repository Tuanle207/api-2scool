using System;

namespace Scool.Application.Dtos
{
    public class CreateUpdateStudentDto
    {
        public string Name { get; set; }
        public Guid ClassId { get; set; }
        public DateTime Dob { get; set; }
        public string ParentPhoneNumber { get; set; }
    }
}
