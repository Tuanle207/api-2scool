using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class ClassForStudentDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public Guid CourseId { get; set; }
        public Guid FormTeacherId { get; set; }
    }
}
