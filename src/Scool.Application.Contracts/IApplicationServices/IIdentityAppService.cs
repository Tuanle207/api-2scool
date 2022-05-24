using Scool.Dtos;
using Scool.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace Scool.IApplicationServices
{
    public interface IIdentityAppService : IIdentityUserAppService
    {
        Task<PagingModel<UserDto>> PostPaging(PageInfoRequestDto input);

        Task<bool> IsEmailAlreadyUsed(Guid? userId, string email);

        Task<string> DoesStudentHaveAccountAlready(Guid studentId);

        Task<bool> IsRoleNameAlreadyUsed(Guid? roleId, string name);
    }
}
