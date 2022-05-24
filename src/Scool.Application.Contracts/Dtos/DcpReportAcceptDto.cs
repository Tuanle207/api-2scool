using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Dtos
{
    public class DcpReportAcceptDto
    {
        public ICollection<Guid> ReportIds { get; set; }
    }
}
