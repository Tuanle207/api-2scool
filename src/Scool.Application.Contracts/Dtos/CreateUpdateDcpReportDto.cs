using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Application.Dtos
{
    public class CreateUpdateDcpReportDto
    {
        public ICollection<CreateUpdateDcpClassReportDto> DcpClassReports { get; set; }
    }
}
