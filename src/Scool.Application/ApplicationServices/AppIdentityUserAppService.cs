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
using Scool.Dtos;
using IdentityRole = Volo.Abp.Identity.IdentityRole;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Scool.ApplicationServices
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IIdentityUserAppService), typeof(IdentityUserAppService), typeof(AppIdentityUserAppService))]
    public class AppIdentityUserAppService : IdentityUserAppService, IAppIdentityUserAppService
    {
        private readonly IdentityUserManager _identityUserManager;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IIdentityRoleRepository _identityRoleRepository;
        private readonly IRepository<UserProfile, Guid> _userProfilesRepository;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IOptions<IdentityOptions> _identityOptions;

        public AppIdentityUserAppService(IdentityUserManager identityUserManager, IIdentityUserRepository identityUserRepository,
            IIdentityRoleRepository identityRoleRepository, IOptions<IdentityOptions> identityOptions,
                IRepository<UserProfile, Guid> userProfilesRepository, IRepository<AppUser, Guid> userRepository)
                : base(identityUserManager, identityUserRepository, identityRoleRepository, identityOptions)
        {
            _identityUserManager = identityUserManager;
            _identityUserRepository = identityUserRepository;
            _identityRoleRepository = identityRoleRepository;
            _identityOptions = identityOptions;
            _userProfilesRepository = userProfilesRepository;
            _userRepository = userRepository;
        }

        public async override Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
        {
            var classIdRaw = input.ExtraProperties.GetOrDefault(IdentityUserCreateDtoExt.ClassId);

            if (classIdRaw != null)
            {
                string[] roles = { AppRole.DcpReporterStudent, AppRole.LessonsRegisterReporter };
                input.RoleNames = roles;
            }

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

            var listRoleId = users.Where(x => x.RoleId.HasValue)
                .Select(x => x.RoleId.Value)
                .ToList();

            // User roles
            IList<RoleForSimpleListDto> roles = await _identityRoleRepository.ToEfCoreRepository()
                .AsNoTracking()
                .Where(x => listRoleId.Contains(x.Id))
                .Select(x => ObjectMapper.Map<IdentityRole, RoleForSimpleListDto>(x))
                .ToListAsync();

            foreach (UserDto user in users)
            {
                RoleForSimpleListDto role = user.RoleId.HasValue 
                    ? roles.FirstOrDefault(x => x.Id == user.RoleId)
                    : null;

                user.Role = role;
            }

            return new PagingModel<UserDto>(users, totalCount, pageIndex, pageSize);
        }
    }
}
