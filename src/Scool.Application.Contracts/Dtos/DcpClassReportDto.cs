using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Scool.Application.Dtos
{
    public class DcpClassReportDto : Entity<Guid>
    {
        public Guid DcpReportId { get; set; }
        public int PenaltyTotal { get; set; }
        public Guid ClassId { get; set; }
        public ClassForSimpleListDto Class { get; set; }
        public ICollection<DcpClassReportItemDto> Faults { get; set; }
    }
}
