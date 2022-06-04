using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class DcpReportDto : EntityDto<Guid>
    {
        public string Status { get; set; }
        public string ReportedClassDisplayNames { get; set; }
        public ICollection<DcpClassReportDto> DcpClassReports { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public SimpleAccountDto Creator { get; set; }
    }
}
