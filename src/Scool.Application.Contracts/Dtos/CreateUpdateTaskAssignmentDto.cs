using System;
using System.Collections.Generic;

namespace Scool.Application.Dtos
{
    public class CreateUpdateTaskAssignmentDto
    {
        public List<ClassAssignedItem> Items { get; set; }
        public string TaskType { get; set; }
    }

    public class ClassAssignedItem
    {
        public Guid AssigneeId { get; set; }
        public Guid ClassId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
