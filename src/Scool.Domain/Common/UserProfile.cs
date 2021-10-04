using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class UserProfile : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNo { get; set; }
        public DateTime? Dob { get; set; }
        public string Photo { get; set; }
        public Guid? ClassId { get; set; }
        public Class Class { get; set; }
    }
}
