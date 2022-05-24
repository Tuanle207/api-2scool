using System;
using Volo.Abp.Domain.Entities;

namespace Scool.Dtos
{
    public class CourseForSimpleListDto : Entity<Guid>
    {
        public string Name { get; set; }
    }
}
