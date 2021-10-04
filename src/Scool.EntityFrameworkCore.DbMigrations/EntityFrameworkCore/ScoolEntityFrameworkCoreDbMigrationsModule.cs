using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Scool.EntityFrameworkCore
{
    [DependsOn(
        typeof(ScoolEntityFrameworkCoreModule)
        )]
    public class ScoolEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<ScoolMigrationsDbContext>();
        }
    }
}
