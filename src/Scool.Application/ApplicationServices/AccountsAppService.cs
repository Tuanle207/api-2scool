using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scool.Common;
using Scool.Dtos;
using Scool.IApplicationServices;
using Scool.Infrastructure.Common;
using Scool.Users;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Scool.ApplicationServices
{
    public class AccountsAppService : ScoolAppService, IAccountsAppService
    {
        private readonly IRepository<Account, Guid> _accountRepository;

        public AccountsAppService(IRepository<Account, Guid> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet("api/app/accounts/task-assignment-accounts")]
        public async Task<PagingModel<SimpleAccountDto>> GetTaskAssignmentAccounts(Guid? classId)
        {
            var items = await _accountRepository.AsNoTracking()
                .WhereIf(classId != null, x => x.ClassId == classId && x.StudentId != null)
                .WhereIf(classId == null, x => x.ClassId != null && x.TeacherId == null)
                .Include(x => x.Class)
                .Select(x => ObjectMapper.Map<Account, SimpleAccountDto>(x)).ToListAsync();

            return new PagingModel<SimpleAccountDto>(items, items.Count);
        }

        [HttpGet("api/app/accounts/current-account")]
        public async Task<CurrentAccountDto> GetCurrentAccount()
        {
            return new CurrentAccountDto
            {
                IsAuthenticated = CurrentAccount.IsAuthenticated,
                IsStudent = CurrentAccount.IsStudent,
                IsTeacher = CurrentAccount.IsTeacher,
                Id = CurrentAccount.Id,
                UserId = CurrentAccount.UserId,
                DisplayName = CurrentAccount.DisplayName,
                Email = CurrentAccount.Email,
                PhoneNumber = CurrentAccount.PhoneNumber,
                Dob = CurrentAccount.Dob,
                Avatar = CurrentAccount.Avatar,
                ClassId =  CurrentAccount.ClassId,
                StudentId = CurrentAccount.StudentId,
                TeacherId = CurrentAccount.TeacherId,
                CreationTime = CurrentAccount.CreationTime,
                CreatorId = CurrentAccount.CreatorId,
            };
        }


        [HttpPost("api/app/accounts/add")]
        public Task AddAccount()
        {
            throw new NotImplementedException();
        }

        [HttpDelete("api/app/accounts/delete")]
        public Task DeleteAccount()
        {
            throw new NotImplementedException();
        }

        [HttpGet("api/app/accounts/get-my-profile")]
        public Task GetMyAccount()
        {
            throw new NotImplementedException();
        }

        [HttpGet("api/app/accounts/{userId}")]
        public Task GetAccount([FromRoute] Guid authUserId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("api/app/accounts/update-my-account")]
        public Task UpdateMyAccount()
        {
            throw new NotImplementedException();
        }

        [HttpPut("api/app/accounts/{userId}")]
        public Task UpdateAccount()
        {
            throw new NotImplementedException();
        }
    }
}
