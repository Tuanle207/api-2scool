using Scool.Application.Dtos;
using Scool.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace Scool.Application.IApplicationServices
{
    public interface IAppIdentityUserAppService : IIdentityUserAppService
    {
        Task<PagingModel<UserForTaskAssignmentDto>> GetUserForTaskAssignment();
    }
}
