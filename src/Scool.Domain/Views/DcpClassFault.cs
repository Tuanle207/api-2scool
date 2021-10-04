using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Domain.Views
{
    public class DcpClassFault
    {
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public string FormTeacherName { get; set; }
        public int Faults { get; set; }
        public int PenaltyPoints { get; set; }
    }
}
