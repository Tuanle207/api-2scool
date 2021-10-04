using Scool.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Scool.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(ScoolEntityFrameworkCoreDbMigrationsModule),
        typeof(ScoolApplicationContractsModule)
        )]
    public class ScoolDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
