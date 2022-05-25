using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Common
{
    public interface IHaveCourse
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}
