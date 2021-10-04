using System;

namespace Scool.Application.Dtos
{
    public class CreateUpdateCourseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
    }
}