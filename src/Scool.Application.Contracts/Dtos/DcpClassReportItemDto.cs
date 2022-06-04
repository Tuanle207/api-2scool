using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class DcpClassReportItemDto : EntityDto<Guid>
    {
        public Guid DcpClassReportId { get; set; }
        public Guid RegulationId { get; set; }
        public int PenaltyPoint { get; set; }
        public RegulationForSimpleListDto Regulation { get; set; }
        public ICollection<DcpStudentReportDto> RelatedStudents { get; set; }
    }
}
