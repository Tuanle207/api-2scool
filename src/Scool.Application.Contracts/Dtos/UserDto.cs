using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class UserDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public List<RoleForSimpleListDto> Roles { get; set; }
        public List<Guid> ListRoleId { get; set; }
    }
}
