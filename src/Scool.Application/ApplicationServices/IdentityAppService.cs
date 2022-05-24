using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Repositories;
using Scool.AppConsts;
using Scool.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Scool.Infrastructure.Linq;
using IdentityRole = Volo.Abp.Identity.IdentityRole;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using Volo.Abp.SettingManagement;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Emailing;
using Scool.Dtos;
using Scool.ObjectExtentions;
using Scool.Common;
using Scool.IApplicationServices;

namespace Scool.ApplicationServices
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IIdentityUserAppService), typeof(IdentityUserAppService), typeof(IdentityAppService))]
    public class IdentityAppService : IdentityUserAppService, IIdentityAppService
    {
        private static readonly int PASSWORD_REQUIRED_MIN_LENGTH = 6;
        private static readonly bool PASSWORD_REQUIRED_DIGIT = false;
        private static readonly bool PASSWORD_REQUIRED_UPPERCASE = false;
        private static readonly bool PASSWORD_REQUIRED_LOWERCASE = false;
        private static readonly bool PASSWORD_REQUIRED_NON_ALPHANUMERIC = false;
        private static readonly int PASSWORD_REQUIRED_UNIQUE_CHARACTERS = 0;

        private readonly ISettingManager _settingManager;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IIdentityRoleRepository _identityRoleRepository;
        private readonly IRepository<Account, Guid> _accountRepository;
        private readonly IRepository<Student, Guid> _studentRepository;
        private readonly IRepository<Teacher, Guid> _teacherRepository;
        private readonly IRepository<Class, Guid> _classRepository;
        private readonly IEmailSender _emailSender;

        public IdentityAppService(
            IdentityUserManager identityUserManager,
            IIdentityUserRepository identityUserRepository,
            IIdentityRoleRepository identityRoleRepository,
            IOptions<IdentityOptions> identityOptions,
            IRepository<Account, Guid> accountRepository,
            IRepository<Student, Guid> studentRepository,
            IRepository<Class, Guid> classRepository,
            IRepository<Teacher, Guid> teacherRepository,
            ISettingManager settingManager,
            IEmailSender emailSender)
        : base(identityUserManager, identityUserRepository, identityRoleRepository, identityOptions)
        {
            _identityUserRepository = identityUserRepository;
            _identityRoleRepository = identityRoleRepository;
            _accountRepository = accountRepository;
            _settingManager = settingManager;
            _emailSender = emailSender;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _classRepository = classRepository;
        }

        public async override Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
        {
            var studentIdStr = (string)input.ExtraProperties.GetOrDefault(IdentityUserCreateDtoExt.StudentId);
            var teacherIdStr = (string)input.ExtraProperties.GetOrDefault(IdentityUserCreateDtoExt.TeacherId);
            var studentId = string.IsNullOrEmpty(studentIdStr) ? null : (Guid?)new Guid(studentIdStr);
            var teacherId = string.IsNullOrEmpty(teacherIdStr) ? null : (Guid?)new Guid(teacherIdStr);

            var account = await GetNewAccountAsync(
                email: input.Email, 
                name: input.Name, 
                phoneNumber: input.PhoneNumber, 
                studentId: studentId,
                teacherId: teacherId
            );

            if (studentId is not null)
            {
                input.RoleNames = new string[] { AppRole.DcpReporterStudent, AppRole.LessonsRegisterReporter };
            }
            if (teacherId is not null)
            {
                input.RoleNames = new string[] { AppRole.DcpManager };
            }
            input.UserName = ParseUserNameFromEmail(input.Email);

            await SetAccountOptionsAsync();
            var result = await base.CreateAsync(input);
            await CurrentUnitOfWork.SaveChangesAsync();

            account.UserId = result.Id;
            account.TenantId = CurrentTenant.Id;
            await _accountRepository.InsertAsync(account);

            var emailBody = @"
                <p>Hi Tuan,</p>
                <p>This email is from 2Scool,</p>
                <br>
                <p>Thanks and Best regards</p>
            ";
            await _emailSender.SendAsync(to: "18521597@gm.uit.edu.vn", subject: "Email from 2Scool", body: emailBody);

            return result;
        }

        public override async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto input)
        {
            await SetAccountOptionsAsync();
            return await base.UpdateAsync(id, input);
        }

        private async Task CreateOrUpdateAccount(Guid userId)
        {
            var account = await _accountRepository.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        //[HttpGet("/api/app/app-identity-user/user-for-task-assignment")]
        //public async Task<PagingModel<TaskAssigneeDto>> GetUserForTaskAssignment([FromQuery(Name = "classId")]Guid? classId)
        //{
        //    var items = await _accountRepository.AsQueryable()
        //        //.WhereIf(classId is not null, x => x.ClassId == classId)
        //        //.WhereIf(classId is null, x => x.ClassId != null)
        //        //.Include(x => x.Class)
        //        .Select(x => ObjectMapper.Map<Account, TaskAssigneeDto>(x)).ToListAsync();

        //    return new PagingModel<TaskAssigneeDto>(items, items.Count);
        //}

        public async Task<PagingModel<UserDto>> PostPaging(PageInfoRequestDto input)
        {
            int pageSize = input.PageSize > 0 ? input.PageSize : 10;
            int pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;
            string name = input.Filter.FirstOrDefault(x => x.Key == "Name")?.Value.ToLower() ?? string.Empty;
            string roleFilter = input.Filter.FirstOrDefault(x => x.Key == "RoleId")?.Value ?? string.Empty;

            IQueryable<IdentityUser> query = _identityUserRepository.ToEfCoreRepository()
                .AsNoTracking()
                .Where(x => x.Name.ToLower().Contains(name));

            if (!string.IsNullOrEmpty(roleFilter))
            {
                var listRoleId = roleFilter.Split(',').Select(x => new Guid(x)).ToList();
                query = query.Where(x => x.Roles.Any(c => listRoleId.Contains(c.RoleId)));
            }

            int totalCount = await query.CountAsync();

            // Users
            IList<UserDto> users = await query
                .Page(pageIndex, pageSize)
                .Include(x => x.Roles)
                .OrderByDescending(x => x.CreationTime)
                .Select(x => ObjectMapper.Map<IdentityUser, UserDto>(x))
                .ToListAsync();

            var listUniqueRoleId = users
                .SelectMany(x => x.ListRoleId)
                .Distinct()
                .ToList();

            // User roles
            IList<RoleForSimpleListDto> roles = await _identityRoleRepository.ToEfCoreRepository()
                .AsNoTracking()
                .Where(x => listUniqueRoleId.Contains(x.Id))
                .Select(x => ObjectMapper.Map<IdentityRole, RoleForSimpleListDto>(x))
                .ToListAsync();

            foreach (UserDto user in users)
            {
                foreach (Guid roleId in user.ListRoleId)
                {
                    RoleForSimpleListDto role = roles.FirstOrDefault(x => x.Id == roleId);
                    if (role != null)
                    {
                        user.Roles.Add(role);
                    }
                }
            }

            return new PagingModel<UserDto>(users, totalCount, pageIndex, pageSize);
        }

        [HttpGet("api/app/identity/is-email-already-used")]
        public Task<bool> IsEmailAlreadyUsed([FromQuery] Guid? userId, [FromQuery] string email)
        {
            var lowercaseEmail = string.IsNullOrEmpty(email) ? string.Empty : email.ToLower();
            return _identityUserRepository.ToEfCoreRepository()
                .AsNoTracking()
                .Where(x => x.Email.ToLower() == lowercaseEmail)
                .Where(x => x.Id != userId)
                .AnyAsync();
        }

        [HttpGet("api/app/identity/does-student-have-account-already")]
        public async Task<string> DoesStudentHaveAccountAlready([FromQuery] Guid studentId)
        {
            var account = await _accountRepository.AsNoTracking()
                .FirstOrDefaultAsync(x => x.StudentId == studentId);
            if (account is null)
            {
                return string.Empty;
            }
            return await _identityUserRepository.ToEfCoreRepository()
                .AsNoTracking()
                .Where(x => x.Id == account.UserId)
                .Select(x => x.Email)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        [HttpGet("api/app/identity/is-role-name-already-used")]
        public Task<bool> IsRoleNameAlreadyUsed(Guid? roleId, string name)
        {
            var lowercaseName = string.IsNullOrEmpty(name) ? string.Empty : name.ToLower();
            return _identityRoleRepository.ToEfCoreRepository()
                .Where(x => x.Name == lowercaseName)
                .Where(x => x.Id != roleId)
                .AnyAsync();
        }

        private async Task SetAccountOptionsAsync()
        {
            await _settingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequiredLength,
                PASSWORD_REQUIRED_MIN_LENGTH.ToString()
            );
            await _settingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequireDigit,
                PASSWORD_REQUIRED_DIGIT.ToString()
            );
            await _settingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequireUppercase,
                PASSWORD_REQUIRED_UPPERCASE.ToString()
            );
            await _settingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequireLowercase,
                PASSWORD_REQUIRED_LOWERCASE.ToString()
            );
            await _settingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequireNonAlphanumeric,
                PASSWORD_REQUIRED_NON_ALPHANUMERIC.ToString()
            );
            await _settingManager.SetForCurrentTenantAsync(
                IdentitySettingNames.Password.RequiredUniqueChars,
                PASSWORD_REQUIRED_UNIQUE_CHARACTERS.ToString()
            );
        }

        private string ParseUserNameFromEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || (!string.IsNullOrEmpty(email) && !email.Contains("@")))
            {
                return string.Empty;
            }
            return email.Replace('@', '.');
        }

        private async Task<Account> GetNewAccountAsync(string email, string name, string phoneNumber,
            Guid? studentId, Guid? teacherId)
        {
            var account = new Account
            {
                Email = email,
                DisplayName = name,
                PhoneNumber = phoneNumber,
            };
            if (studentId is not null)
            {
                var student = await _studentRepository
                    .Include(x => x.Class)
                    .FirstOrDefaultAsync(x => x.Id == studentId);
                account.StudentId = student.Id;
                account.ClassId = student?.ClassId;
                account.ClassDisplayName = student.Class.Name;
            }
            if (teacherId is not null)
            {
                var teacher = await _teacherRepository
                    .Include(x => x.FormClass)
                    .FirstOrDefaultAsync(x => x.Id == teacherId);
                account.TeacherId = teacherId;
                account.ClassId = teacher.FormClass?.Id;
                account.ClassDisplayName = teacher.FormClass?.Name;
            }
            return account;
        }
    }
}
