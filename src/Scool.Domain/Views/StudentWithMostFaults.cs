using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Domain.Views
{
    public class StudentWithMostFaults
    {
        public Guid Id { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public int Faults { get; set; }
    }
}
