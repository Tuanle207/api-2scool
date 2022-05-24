using Localization.Resources.AbpUi;
using Scool.RealTime.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Scool.RealTime
{
    [DependsOn(
        typeof(RealTimeApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule))]
    public class RealTimeHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(RealTimeHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<RealTimeResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });
        }
    }
}
