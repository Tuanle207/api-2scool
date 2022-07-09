using Microsoft.EntityFrameworkCore;
using Scool.AppConsts;
using Scool.Common;
using Scool.Dtos;
using Scool.IApplicationServices;
using Scool.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Scool.ApplicationServices
{
    public class AppSettingsAppService : ScoolAppService, IAppSettingsAppService
    {
        private readonly List<string> reportSettingCodes = new()
        {
            AppSettingTypeCode.LrPointRatio,
            AppSettingTypeCode.DcpPointRatio,
            AppSettingTypeCode.StartDcpPoint,
        };
        private readonly IRepository<AppSetting, Guid> _appSettingRepository;

        public AppSettingsAppService(IRepository<AppSetting, Guid> appSettingRepository)
        {
            _appSettingRepository = appSettingRepository;
        }

        public async Task<PagingModel<AppSettingDto>> GetAllAppSetting()
        {
            var items = await _appSettingRepository
                .AsNoTracking()
                .Select(x => ObjectMapper.Map<AppSetting, AppSettingDto>(x))
                .ToListAsync();

            return new PagingModel<AppSettingDto>(items);
        }

        public async Task<PagingModel<AppSettingDto>> GetReportAppSetting()
        {
            var items = await _appSettingRepository
                .AsNoTracking()
                .Where(x => reportSettingCodes.Contains(x.TypeCode))
                .Select(x => ObjectMapper.Map<AppSetting, AppSettingDto>(x))
                .ToListAsync();

            return new PagingModel<AppSettingDto>(items);
        }

        public async Task UpdateReportAppSetting(List<CreateUpdateAppSettingDto> settings)
        {
            if (!CurrentTenant.Id.HasValue)
            {
                return;
            }

            var allSettings = await _appSettingRepository.ToListAsync();

            foreach (var settingCode in reportSettingCodes)
            {
                var setting = allSettings.FirstOrDefault(x => x.TypeCode == settingCode);
                var dto = settings.FirstOrDefault(x => x.TypeCode == settingCode);
                if (setting == null)
                {
                    setting = new AppSetting
                    {
                        TypeCode = settingCode,
                        TenantId = CurrentTenant.Id,
                        Value = dto?.Value ?? string.Empty
                    };
                    await _appSettingRepository.InsertAsync(setting);
                }
                else
                {
                    setting.Value = dto?.Value;
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }
    }
}
