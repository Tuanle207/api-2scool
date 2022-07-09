using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scool.IApplicationServices;
using Scool.Infrastructure.Common;
using Scool.Infrastructure.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Tenant = Volo.Abp.TenantManagement.Tenant;
using AbpTenantDto = Volo.Abp.TenantManagement.TenantDto;
using ITenantAppService = Volo.Abp.TenantManagement.ITenantAppService;
using TenantAppService = Volo.Abp.TenantManagement.TenantAppService;
using ITenantRepository = Volo.Abp.TenantManagement.ITenantRepository;
using ITenantManager = Volo.Abp.TenantManagement.ITenantManager;
using Scool.Dtos;
using Scool.Domain.Shared.AppConsts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Scool.FileHandler;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Identity;
using Scool.Infrastructure.Helpers;
using Scool.AppConsts;
using Scool.Email;

namespace Scool.ApplicationServices
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(ITenantAppService), typeof(TenantAppService), typeof(MultitenancyAppService))]
    public class MultitenancyAppService : TenantAppService, IMultitenancyAppService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IFileHandler _fileHandler;
        private readonly IIdentityAppService _identityAppService;
        private readonly IEmailSender _emailSender;

        public MultitenancyAppService(
            ITenantRepository tenantRepository,
            ITenantManager tenantManager,
            IDataSeeder dataSeeder, IFileHandler fileHandler,
            IIdentityAppService identityAppService, 
            IEmailSender emailSender)
        : base(tenantRepository, tenantManager, dataSeeder)
        {
            _tenantRepository = tenantRepository;
            _fileHandler = fileHandler;
            _identityAppService = identityAppService;
            _emailSender = emailSender;
        }

        public override Task<AbpTenantDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }

        [HttpPost("api/app/multitenancy/create-with-image")]
        public async Task<AbpTenantDto> CreateAsync(Volo.Abp.TenantManagement.TenantCreateDto input, [FromBody] IFormFile image)
        {
            if (image != null)
            {
                var photoUrl = await _fileHandler.SaveFileAsync(image);
                input.SetProperty(TenantSettingType.PhotoUrl, photoUrl);
            }

            var tenant = await TenantManager.CreateAsync(input.Name);
            input.MapExtraPropertiesTo(tenant);

            await TenantRepository.InsertAsync(tenant);

            await CurrentUnitOfWork.SaveChangesAsync();

            using (CurrentTenant.Change(tenant.Id, tenant.Name))
            {
                await DataSeeder.SeedAsync(
                    new DataSeedContext(tenant.Id)
                        //.WithProperty("AdminEmail", input.AdminEmailAddress)
                        //.WithProperty("AdminPassword", input.AdminPassword)
                    );

                await _identityAppService.CreateAsync(new IdentityUserCreateDto
                {
                    Email = input.AdminEmailAddress,
                    Password = StringHelper.GetRandomPasswordString(),
                    Name = "Quản Trị Viên",
                    RoleNames = new string[] { AppRole.Admin }
                });
            }

            return ObjectMapper.Map<Tenant, AbpTenantDto>(tenant);
        }

        public override async Task<AbpTenantDto> CreateAsync(Volo.Abp.TenantManagement.TenantCreateDto input)
        {
            var tenant = await TenantManager.CreateAsync(input.Name);
            input.MapExtraPropertiesTo(tenant);

            await TenantRepository.InsertAsync(tenant);

            await CurrentUnitOfWork.SaveChangesAsync();

            var schoolName = input.GetProperty(TenantSettingType.DisplayName);
            var emailBody = $@"
                <p>Hệ thống quản lý nề nếp 2Scool đã được cài đặt thành công cho {schoolName}!</p>
                <br>
                <p>Bạn có thể truy cập vào hệ thống thông qua địa chỉ <strong>http://{tenant.Name}.qlnn.live</strong> hoặc thông qua ứng dụng di động (chọn tên trường <strong>{input.GetProperty(TenantSettingType.DisplayName)}</strong> khi đăng nhập)</p>  
            ";
            await _emailSender.QueueAsync(new SimpleEmailSendingArgs
            {
                Body = emailBody,
                Subject = $"Hệ thống quản lý nề nếp 2Scool đã được cài đặt thành công cho {schoolName}",
                To = new List<string> { input.AdminEmailAddress }
            });

            using (CurrentTenant.Change(tenant.Id, tenant.Name))
            {
                await DataSeeder.SeedAsync(new DataSeedContext(tenant.Id));

                await _identityAppService.CreateAsync(new IdentityUserCreateDto
                {
                    Email = input.AdminEmailAddress,
                    UserName = input.AdminEmailAddress,
                    Password = input.AdminEmailAddress,
                    Name = "Quản Trị Viên",
                    RoleNames = new string[] { AppRole.Admin }
                });
            }

            return ObjectMapper.Map<Tenant, AbpTenantDto>(tenant);
        }

        public override async Task DeleteAsync(Guid id)
        {
            await base.DeleteAsync(id);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public override async Task<AbpTenantDto> UpdateAsync(Guid id, Volo.Abp.TenantManagement.TenantUpdateDto input)
        {
            return await base.UpdateAsync(id, input);
        }

        public async Task<PagingModel<TenantDto>> PostPaging(PageInfoRequestDto input)
        {
            int pageSize = input.PageSize > 0 ? input.PageSize : 10;
            int pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;
            string name = input.Filter.FirstOrDefault(x => x.Key == "Name")?.Value.ToLower() ?? string.Empty;

            IQueryable<Tenant> query = _tenantRepository.ToEfCoreRepository()
                .AsNoTracking()
                .Where(x => x.Name.ToLower().Contains(name));

            int totalCount = await query.CountAsync();

            IList<TenantDto> tenants = await query
                .OrderByDescending(x => x.CreationTime)
                .ThenBy(x => x.Name)
                .Page(pageIndex, pageSize)
                .Select(x => ObjectMapper.Map<Tenant, TenantDto>(x))
                .ToListAsync();

            return new PagingModel<TenantDto>(tenants, totalCount, pageIndex, pageSize);
        }

        [AllowAnonymous]
        [HttpGet("api/app/multitenancy/display-name-from-tenant-name")]
        public async Task<string> GetDisplayNameFromTenantNameAsync(string name) 
        {
            var tenant = await _tenantRepository.FindByNameAsync(name);
            return tenant != null ? tenant.GetProperty(TenantSettingType.DisplayName).ToString() : string.Empty;
        }

        [HttpGet("api/app/multitenancy/is-name-already-used")]
        public Task<bool> IsNameAlreadyUsedAsync(Guid? id, string name)
        {
            var lowercaseName = string.IsNullOrEmpty(name) ? string.Empty : name.ToLower();
            
            return _tenantRepository.ToEfCoreRepository()
                .AsNoTracking()
                .Where(x => x.Id != id && x.Name.ToLower() == lowercaseName)
                .AnyAsync();
        }

        [AllowAnonymous]
        [HttpGet("api/app/multitenancy/simple-list")]
        public async Task<PagingModel<TenantDto>> GetSimpleListAsync(PageInfoRequestDto input)
        {
            return new PagingModel<TenantDto>(await _tenantRepository.ToEfCoreRepository()
                .AsNoTracking()
                .Select(x => ObjectMapper.Map<Tenant, TenantDto>(x))
                .ToListAsync());
        }
    }
}
