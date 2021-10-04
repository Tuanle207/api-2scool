using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class DcpClassReport : Entity<Guid>
    {
        public Guid DcpReportId { get; set; }
        public Guid ClassId { get; set; }
        public Class Class { get; set; }
        public int PenaltyTotal { get; set; }
        public ICollection<DcpClassReportItem> Faults { get; set; }

        public DcpClassReport()
        {
            Faults = new List<DcpClassReportItem>();
            PenaltyTotal = 0;
        }
        public DcpClassReport(Guid id)
        {
            Id = id;
            Faults = new List<DcpClassReportItem>();
            PenaltyTotal = 0;
        }
    }
}
