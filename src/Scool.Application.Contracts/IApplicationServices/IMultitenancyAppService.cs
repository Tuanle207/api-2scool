using Microsoft.AspNetCore.Http;
using Scool.Dtos;
using Scool.Infrastructure.Common;
using System;
using System.Threading.Tasks;

namespace Scool.IApplicationServices
{
    public interface IMultitenancyAppService : Volo.Abp.TenantManagement.ITenantAppService
    {
        Task<Volo.Abp.TenantManagement.TenantDto> CreateAsync(Volo.Abp.TenantManagement.TenantCreateDto input, IFormFile image);

        Task<bool> IsNameAlreadyUsedAsync(Guid? id, string name);

        Task<string> GetDisplayNameFromTenantNameAsync(string name);

        Task<PagingModel<TenantDto>> PostPaging(PageInfoRequestDto input);
    }
}
