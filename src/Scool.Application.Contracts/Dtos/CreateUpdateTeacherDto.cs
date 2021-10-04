using System;

namespace Scool.Application.Dtos
{
    public class CreateUpdateTeacherDto
    {
        public string Name { get; set; }
        public DateTime Dob { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
