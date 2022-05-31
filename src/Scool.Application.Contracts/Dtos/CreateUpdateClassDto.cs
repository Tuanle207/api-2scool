using System;

namespace Scool.Dtos
{
    public class CreateUpdateClassDto
    {
        public string Name { get; set; }
        public Guid GradeId { get; set; }
        public Guid? FormTeacherId { get; set; }
    }
}
