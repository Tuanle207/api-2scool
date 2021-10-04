using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Application.Dtos
{
    public class CreateUpdateDcpClassReportItemDto
    {
        public Guid RegulationId { get; set; }
        public ICollection<Guid> RelatedStudentIds { get; set; }
    }
}
