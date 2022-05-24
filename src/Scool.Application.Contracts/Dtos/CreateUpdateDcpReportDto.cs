using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Dtos
{
    public class CreateUpdateDcpReportDto
    {
        public IList<CreateUpdateDcpClassReportDto> DcpClassReports { get; set; }
    }
}
