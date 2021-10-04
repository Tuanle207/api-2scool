using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Application.Dtos
{
    public class CreateUpdateDcpClassReportDto
    {
        public Guid ClassId { get; set; }
        public ICollection<CreateUpdateDcpClassReportItemDto> Faults { get; set; }
    }
}
