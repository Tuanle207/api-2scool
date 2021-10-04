using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class CourseListDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
    }
}