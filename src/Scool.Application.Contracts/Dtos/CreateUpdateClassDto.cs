using System;

namespace Scool.Application.Dtos
{
    public class CreateUpdateClassDto
    {
        public string Name { get; set; }
        public Guid CourseId { get; set; }
        public Guid GradeId { get; set; }
        public Guid FormTeacherId { get; set; }
    }
}
