using Scool.Dtos;
using Scool.Infrastructure.Common;
using System;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace Scool.IApplicationServices
{
    public interface IIdentityAppService : IIdentityUserAppService
    {
        Task<PagingModel<UserDto>> PostPaging(PageInfoRequestDto input);

        Task<bool> IsEmailAlreadyUsed(Guid? userId, string email);

        Task<string> DoesStudentHaveAccountAlready(Guid studentId);

        Task<string> DoesTeacherHaveAccountAlready(Guid studentId);

        Task<bool> IsRoleNameAlreadyUsed(Guid? roleId, string name);

        Task<string> ResetUserPassWord(Guid userId);
    }
}
