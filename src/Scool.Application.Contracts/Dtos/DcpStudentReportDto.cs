using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Scool.Application.Dtos
{
    public class DcpStudentReportDto : Entity<Guid>
    {
        public Guid StudentId { get; set; }
        public StudentDto Student { get; set; }
        public Guid DcpClassReportItemId { get; set; }
    }
}
