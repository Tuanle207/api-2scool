using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class DcpClassReportItem : Entity<Guid>
    {
        public DcpClassReportItem()
        {
            RelatedStudents = new List<DcpStudentReport>();
        }

        public DcpClassReportItem(Guid id) : this()
        {
            Id = id;
        }

        public Guid DcpClassReportId { get; set; }
        public Guid RegulationId { get; set; }
        public Regulation Regulation { get; set; }
        public ICollection<DcpStudentReport> RelatedStudents { get; set; }
    }
}
