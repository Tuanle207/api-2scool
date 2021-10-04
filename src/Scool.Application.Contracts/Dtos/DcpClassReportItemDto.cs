using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Scool.Application.Dtos
{
    public class DcpClassReportItemDto : Entity<Guid>
    {
        public Guid DcpClassReportId { get; set; }
        public Guid RegulationId { get; set; }
        public RegulationForSimpleListDto Regulation { get; set; }
        public ICollection<DcpStudentReportDto> RelatedStudents { get; set; }
    }
}
