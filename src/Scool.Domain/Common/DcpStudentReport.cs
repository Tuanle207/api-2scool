using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class DcpStudentReport : Entity<Guid>
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public Guid DcpClassReportItemId { get; set; }
    }
}
