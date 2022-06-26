using Scool.Dtos;
using Scool.Infrastructure.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Scool.IApplicationServices
{
    public interface IAppSettingsAppService : IApplicationService
    {
        Task<PagingModel<AppSettingDto>> GetAllAppSetting();

        Task<PagingModel<AppSettingDto>> GetReportsAppSetting();

        Task UpdateAppSetting(List<AppSettingDto> settings);
    }
}
