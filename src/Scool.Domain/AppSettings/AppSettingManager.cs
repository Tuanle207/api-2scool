using Scool.AppConsts;
using Scool.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Scool.AppSettings
{
    public class AppSettingManager : IAppSettingManager, IScopedDependency
    {
        private readonly List<string> reportSettingCodes = new()
        {
            AppSettingTypeCode.LrPointRatio,
            AppSettingTypeCode.DcpPointRatio,
            AppSettingTypeCode.StartDcpPoint,
        };
        private readonly IRepository<AppSetting> _appSettingsRepository;

        public AppSettingManager(IRepository<AppSetting> appSettingsRepository)
        {
            _appSettingsRepository = appSettingsRepository;
        }

        public async Task<Dictionary<string, string>> GetReportSettingValuesAsync()
        {
            var allSettings = await _appSettingsRepository.GetListAsync();
            return allSettings.Where(x => reportSettingCodes.Contains(x.TypeCode))
                .ToDictionary(x => x.TypeCode, x => x.Value);
        }

        public async Task<string> GetValueAsync(string typeCode)
        {
            var allSettings = await _appSettingsRepository.GetListAsync();
            return allSettings.FirstOrDefault(x => x.TypeCode == typeCode).Value;
        }
    }
}
