using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Dtos
{
    public class CreateUpdateDcpClassReportItemDto
    {
        public Guid RegulationId { get; set; }
        public IList<Guid> RelatedStudentIds { get; set; }
    }
}
