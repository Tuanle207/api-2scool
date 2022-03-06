using Microsoft.AspNetCore.Mvc;
using Scool.Application.IApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.ApplicationServices
{
    public class UserProfilesAppService : IUserProfilesAppService
    {
        [HttpPost("/api/app/user-profiles/add")]
        public Task AddUserProfile()
        {
            throw new NotImplementedException();
        }

        [HttpDelete("/api/app/user-profiles/delete")]
        public Task DeleteUserProfile()
        {
            throw new NotImplementedException();
        }

        [HttpGet("/api/app/user-profiles/get-my-profile")]
        public Task GetMyUserProfile()
        {
            throw new NotImplementedException();
        }

        [HttpGet("/api/app/user-profiles/{userId}")]
        public Task GetUserProfile([FromRoute] Guid authUserId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("/api/app/user-profiles/update-my-profile")]
        public Task UpdateMyProfile()
        {
            throw new NotImplementedException();
        }

        [HttpPut("/api/app/user-profiles/{userId}")]
        public Task UpdateUserProfile()
        {
            throw new NotImplementedException();
        }
    }
}
