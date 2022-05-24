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
using ITenantAppService = Volo.Abp.TenantManagement.ITenantAppService;
using TenantAppService = Volo.Abp.TenantManagement.TenantAppService;
using ITenantRepository = Volo.Abp.TenantManagement.ITenantRepository;
using ITenantManager = Volo.Abp.TenantManagement.ITenantManager;
using Scool.Dtos;
using Scool.Domain.Shared.AppConsts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Scool.FileHandler;

namespace Scool.ApplicationServices
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(ITenantAppService), typeof(TenantAppService), typeof(MultitenancyAppService))]
    public class MultitenancyAppService : TenantAppService, IMultitenancyAppService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IFileHandler _fileHandler;

        public MultitenancyAppService(
            ITenantRepository tenantRepository,
            ITenantManager tenantManager,
            IDataSeeder dataSeeder, IFileHandler fileHandler)
        : base(tenantRepository, tenantManager, dataSeeder)
        {
            _tenantRepository = tenantRepository;
            _fileHandler = fileHandler;
        }

        public override Task<Volo.Abp.TenantManagement.TenantDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }

        [HttpPost("api/app/multitenancy/create-with-image")]
        public async Task<Volo.Abp.TenantManagement.TenantDto> CreateAsync(Volo.Abp.TenantManagement.TenantCreateDto input, [FromBody] IFormFile image)
        {
            if (image != null)
            {
                var photoUrl = await _fileHandler.SaveFileAsync(image);
                input.SetProperty(TenantSettingType.PhotoUrl, photoUrl);
            }
            return await base.CreateAsync(input);
        }

        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }

        public override Task<Volo.Abp.TenantManagement.TenantDto> UpdateAsync(Guid id, Volo.Abp.TenantManagement.TenantUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }

        public async Task<PagingModel<TenantDto>> PostPaging(PageInfoRequestDto input)
        {
            int pageSize = input.PageSize > 0 ? input.PageSize : 10;
            int pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;
            string name = input.Filter.FirstOrDefault(x => x.Key == "Name")?.Value.ToLower() ?? string.Empty;

            var rawItems = await _tenantRepository.ToEfCoreRepository()
                .AsNoTracking()
                .ToListAsync();

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
    }
}
