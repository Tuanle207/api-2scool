using Microsoft.Extensions.Logging;
using Scool.AppConsts;
using Scool.Common;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Scool.DataSeeds
{
    public class AppSettingDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<AppSetting, Guid> _settingRepo;
        private readonly ILogger<GradeDataSeedContributer> _logger;

        public AppSettingDataSeedContributor(
            IRepository<AppSetting, Guid> settingRepo,
            ILogger<GradeDataSeedContributer> logger
        )
        {
            _settingRepo = settingRepo;
            _logger = logger;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (!context.TenantId.HasValue)
            {
                return;
            }
            _logger.LogInformation("Started to seed tenant app setting");

            try
            {
                if (!await _settingRepo.AnyAsync(x => x.TypeCode == AppSettingTypeCode.StartDcpPoint))
                {
                    var startDcpPointSetting = new AppSetting
                    {
                        TypeCode = AppSettingTypeCode.StartDcpPoint,
                        TenantId = context.TenantId.Value,
                        Value = "100"
                    };

                    await _settingRepo.InsertAsync(startDcpPointSetting);
                }

                if (!await _settingRepo.AnyAsync(x => x.TypeCode == AppSettingTypeCode.DcpPointRatio))
                {
                    var startDcpPointSetting = new AppSetting
                    {
                        TypeCode = AppSettingTypeCode.DcpPointRatio,
                        TenantId = context.TenantId.Value,
                        Value = "2"
                    };

                    await _settingRepo.InsertAsync(startDcpPointSetting);
                }

                if (!await _settingRepo.AnyAsync(x => x.TypeCode == AppSettingTypeCode.LrPointRatio))
                {
                    var lrPointRatioSetting = new AppSetting
                    {
                        TypeCode = AppSettingTypeCode.LrPointRatio,
                        TenantId = context.TenantId.Value,
                        Value = "1"
                    };

                    await _settingRepo.InsertAsync(lrPointRatioSetting);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to seed grade", ex.Message);
            }

        }

    }
}
