using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Application.Dtos
{
    public class TaskAssignmentFilterDto
    {
        public string ClassName { get; set; }
        public string AssigneeName { get; set; }
        public string TaskType { get; set; }
    }
}
