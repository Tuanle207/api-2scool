using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace Scool.Users
{
    public interface IIdentityManager
    {
        Task<IdentityUser> GetUserByUserId(Guid userId);

        Task<IdentityUser> GetUserByAccountId(Guid accountId);

        Task<bool> EmailHasBeenUsed(string email, Guid userId);

        Task<bool> IsMasterAdmin(Guid userId);

        Task<bool> IsDcpManager(Guid userId);

        Task<bool> IsLRReporter(Guid userId);

        Task<bool> IsDcpReporter(Guid userId);


    }
}
