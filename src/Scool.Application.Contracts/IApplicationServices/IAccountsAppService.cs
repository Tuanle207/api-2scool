using Scool.Dtos;
using Scool.Infrastructure.Common;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Scool.IApplicationServices
{
    public interface IAccountsAppService : IApplicationService
    {
        Task<PagingModel<SimpleAccountDto>> GetTaskAssignmentAccounts(Guid? classId);

        Task<CurrentAccountDto> GetCurrentAccount();

        Task AddAccount();

        Task DeleteAccount();

        Task GetMyAccount();

        Task GetAccount(Guid authUserId);

        Task UpdateMyAccount();

        Task UpdateAccount();
    }
}
