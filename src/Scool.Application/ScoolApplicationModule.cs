using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.MailKit;
using Volo.Abp.BackgroundJobs;

namespace Scool
{
    [DependsOn(
        typeof(ScoolDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(ScoolApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(AbpSettingManagementApplicationModule),
        typeof(AbpAspNetCoreSignalRModule),
        typeof(AbpMailKitModule),
        typeof(AbpBackgroundJobsModule)
    )]
    public class ScoolApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<ScoolApplicationModule>();
            });
        }
    }
}
