using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Dtos
{
    public class StudentDataImportDto
    {
        public int Index { get; set; }
        public string FullName { get; set; }
        public string ClassName { get; set; }
        public string ParentPhoneNumber { get; set; }
        public DateTime Dob { get; set; }
    }
}
