using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Scool.Application.ObjectExtentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Repositories;
using Scool.Domain.Common;
using Scool.AppConsts;
using Scool.Application.IApplicationServices;
using Scool.Infrastructure.Common;
using Scool.Application.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Scool.Users;
using Scool.Infrastructure.Linq;
using IdentityRole = Volo.Abp.Identity.IdentityRole;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using Volo.Abp.SettingManagement;
using Volo.Abp.Identity.Settings;

namespace Scool.ApplicationServices
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IIdentityUserAppService), typeof(IdentityUserAppService), typeof(AppIdentityUserAppService))]
    public class AppIdentityUserAppService : IdentityUserAppService, IAppIdentityUserAppService
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
        private readonly IRepository<UserProfile, Guid> _userProfilesRepository;
        private readonly IRepository<AppUser, Guid> _userRepository;

        public AppIdentityUserAppService(
            IdentityUserManager identityUserManager, 
            IIdentityUserRepository identityUserRepository,
            IIdentityRoleRepository identityRoleRepository, 
            IOptions<IdentityOptions> identityOptions,
            IRepository<UserProfile, Guid> userProfilesRepository,
            IRepository<AppUser, Guid> userRepository,
            ISettingManager settingManager)
        : base(identityUserManager, identityUserRepository, identityRoleRepository, identityOptions)
        {
            _identityUserRepository = identityUserRepository;
            _identityRoleRepository = identityRoleRepository;
            _userProfilesRepository = userProfilesRepository;
            _userRepository = userRepository;
            _settingManager = settingManager;
        }

        public async override Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
        {
            var classIdRaw = input.ExtraProperties.GetOrDefault(IdentityUserCreateDtoExt.ClassId);

            if (classIdRaw != null)
            {
                string[] roles = { AppRole.DcpReporterStudent, AppRole.LessonsRegisterReporter };
                input.RoleNames = roles;
            }

            await SetAccountOptionsAsync();
            var result = await base.CreateAsync(input);
            await CurrentUnitOfWork.SaveChangesAsync();

            var userProfile = new UserProfile
            {
                UserId = result.Id,
                DisplayName = input.Name,
                PhoneNo = input.PhoneNumber,
                ClassId = classIdRaw != null ? new Guid(classIdRaw.ToString()) : null,
                Dob = input.ExtraProperties.GetOrDefault(IdentityUserCreateDtoExt.Dob) as DateTime?
            };

            await _userProfilesRepository.InsertAsync(userProfile);

            return result;
        }

        public override async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto input)
        {
            await SetAccountOptionsAsync();
            return await base.UpdateAsync(id, input);
        }

        [HttpGet("/api/app/app-identity-user/user-for-task-assignment")]
        public async Task<PagingModel<UserForTaskAssignmentDto>> GetUserForTaskAssignment([FromQuery(Name = "classId")]Guid? classId)
        {
            var items = await _userProfilesRepository.AsQueryable()
                .WhereIf(classId is not null, x => x.ClassId == classId)
                .WhereIf(classId is null, x => x.ClassId != null)
                .Include(x => x.Class)
                .Select(x => ObjectMapper.Map<UserProfile, UserForTaskAssignmentDto>(x)).ToListAsync();

            return new PagingModel<UserForTaskAssignmentDto>(items, items.Count);
        }

        public async Task<PagingModel<UserDto>> PostPaging(PageInfoRequestDto input)
        {
            int pageSize = input.PageSize > 0 ? input.PageSize : 10;
            int pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;

            IQueryable<AppUser> query = _userRepository.AsNoTracking()
                .Filter(input.Filter);

            int totalCount = await query.CountAsync();

            // Users
            IList<UserDto> users = await _identityUserRepository.ToEfCoreRepository()
                .AsNoTracking()
                .OrderByDescending(x => x.CreationTime)
                .Page(pageIndex, pageSize)
                .Include(x => x.Roles)
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
    }
}
