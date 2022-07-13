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
using Scool.Dtos;
using Scool.ObjectExtentions;
using Scool.Common;
using Scool.IApplicationServices;
using Scool.Email;
using Microsoft.AspNetCore.Authorization;
using Scool.Infrastructure.Helpers;
using Volo.Abp.TenantManagement;
using Scool.Domain.Shared.AppConsts;
using Volo.Abp.Application.Dtos;

namespace Scool.ApplicationServices
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IIdentityUserAppService), typeof(IdentityUserAppService), typeof(IdentityAppService), typeof(IIdentityAppService))]
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
        private readonly ITenantRepository _tenantRepository;
        private readonly IEmailSender _emailSender;

        public IdentityAppService(
            IdentityUserManager identityUserManager,
            IIdentityUserRepository identityUserRepository,
            IIdentityRoleRepository identityRoleRepository,
            IOptions<IdentityOptions> identityOptions,
            IRepository<Account, Guid> accountRepository,
            IRepository<Student, Guid> studentRepository,
            IRepository<Teacher, Guid> teacherRepository,
            ISettingManager settingManager,
            IEmailSender emailSender,
            ITenantRepository tenantRepository)
        : base(identityUserManager, identityUserRepository, identityRoleRepository, identityOptions)
        {
            _identityUserRepository = identityUserRepository;
            _identityRoleRepository = identityRoleRepository;
            _accountRepository = accountRepository;
            _settingManager = settingManager;
            _emailSender = emailSender;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _tenantRepository = tenantRepository;
        }

        [AllowAnonymous]
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
            input.Password = StringHelper.GetRandomPasswordString();

            await SetAccountOptionsAsync();
            var result = await base.CreateAsync(input);
            await CurrentUnitOfWork.SaveChangesAsync();

            account.UserId = result.Id;
            account.TenantId = CurrentTenant.Id;
            await _accountRepository.InsertAsync(account);

            var tenant = CurrentTenant.Id.HasValue ? await _tenantRepository.FindAsync(CurrentTenant.Id.Value) : null;
            var appName = tenant == null ? "2Scool" : $"2Scool - {tenant.ExtraProperties[TenantSettingType.DisplayName]}";
            var emailBody = @$"
                <p>Xin chào <strong>{account.DisplayName}</strong>,</p>
                <p>Tài khoản của bạn đã được tạo thành công trên hệ thống quản lý nề nếp {appName}!</p>
                <br>
                <p>Email đăng nhập: <strong>{input.Email}</strong></p>
                <p>Mật khẩu đăng nhập: <strong>{input.Password}</strong></p>
                <br>
                <p>Vui lòng đăng nhập và đổi mật khẩu.</p>
            ";
            await _emailSender.QueueAsync(new SimpleEmailSendingArgs
            {
                Subject = "Tài khoản 2Scool của bạn đã được tạo thành công",
                Body = emailBody,
                To = new List<string> { account.Email }
            });

            return result;
        }

        public override async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto input)
        {
            await SetAccountOptionsAsync();
            return await base.UpdateAsync(id, input);
        }

        [Authorize(Scool.Permission.IdentityPermissions.Users.Get)]
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

        [HttpGet("api/app/identity/does-teacher-have-account-already")]
        public async Task<string> DoesTeacherHaveAccountAlready([FromQuery] Guid teacherId)
        {
            var account = await _accountRepository.AsNoTracking()
                .FirstOrDefaultAsync(x => x.TeacherId == teacherId);
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

        public override async Task DeleteAsync(Guid id)
        {
            await base.DeleteAsync(id);
            var account = await _accountRepository.FirstOrDefaultAsync(x => x.UserId == id);
            if (account != null)
            {
                account.ClassId = null;
                account.Student = null;
                account.TeacherId = null;
                await CurrentUnitOfWork.SaveChangesAsync();

                await _accountRepository.DeleteAsync(account);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        [AllowAnonymous]
        public override async Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync()
        {
            var list = await RoleRepository.GetListAsync();
            return new ListResultDto<IdentityRoleDto>(
                ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(list.Where(x => x.Name != "admin").ToList()));
        }

        [HttpPost("api/app/identity/reset-password/{userId}")]
        public async Task<string> ResetUserPassWord(Guid userId)
        {
            var user = await UserManager.GetByIdAsync(userId);
            var roles = await GetAssignableRolesAsync();
            var rolenames = roles.Items.Where(x => user.Roles.Any(c => c.RoleId == x.Id))
                .Select(x => x.Name)
                .ToArray();
            var userForUpdate = ObjectMapper.Map<IdentityUser, IdentityUserUpdateDto>(user);
            userForUpdate.RoleNames = rolenames;
            userForUpdate.Password = StringHelper.GetRandomPasswordString();
            await UpdateAsync(userId, userForUpdate);

            var tenant = CurrentTenant.Id.HasValue ? await _tenantRepository.FindAsync(CurrentTenant.Id.Value) : null;
            var appName = tenant == null ? "2Scool" : $"2Scool - {tenant.ExtraProperties[TenantSettingType.DisplayName]}";
            var emailBody = @$"
                <p>Xin chào <strong>{userForUpdate.Name}</strong>,</p>
                <p>Tài khoản của bạn đã được đặt lại mật khẩu trên hệ thống quản lý nề nếp {appName}!</p>
                <br>
                <p>Email đăng nhập: <strong>{userForUpdate.Email}</strong></p>
                <p>Mật khẩu đăng nhập: <strong>{userForUpdate.Password}</strong></p>
                <br>
                <p>Vui lòng đăng nhập và đổi mật khẩu</p>
            ";
            await _emailSender.QueueAsync(new SimpleEmailSendingArgs
            {
                Subject = "Tài khoản 2Scool của bạn đã được đặt lại mật khẩu.",
                Body = emailBody,
                To = new List<string> { userForUpdate.Email }
            });

            return userForUpdate.Password;
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
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
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
