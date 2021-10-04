using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Dtos
{
    public class CreateUpdateUserProfileDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNo { get; set; }
        public DateTime Dob { get; set; }
        public string Photo { get; set; }
        public Guid? ClassId { get; set; }
    }
}
