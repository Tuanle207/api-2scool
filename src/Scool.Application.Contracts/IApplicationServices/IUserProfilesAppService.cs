using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Scool.Application.IApplicationServices
{
    public interface IUserProfilesAppService : IApplicationService
    {
        Task AddUserProfile();

        Task DeleteUserProfile();
        
        Task GetMyUserProfile();
        
        Task GetUserProfile(Guid authUserId);
        
        Task UpdateMyProfile();
        
        Task UpdateUserProfile();
    }
}
