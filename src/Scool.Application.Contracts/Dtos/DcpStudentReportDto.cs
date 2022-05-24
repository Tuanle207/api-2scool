using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Scool.Dtos
{
    public class DcpStudentReportDto : Entity<Guid>
    {
        public Guid StudentId { get; set; }
        public StudentForSimpleListDto Student { get; set; }
        public Guid DcpClassReportItemId { get; set; }
    }
}
