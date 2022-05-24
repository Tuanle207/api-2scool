using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace Scool.RealTime
{
    [DependsOn(
        typeof(RealTimeApplicationContractsModule),
        typeof(AbpHttpClientModule))]
    public class RealTimeHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "RealTime";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(RealTimeApplicationContractsModule).Assembly,
                RemoteServiceName
            );
        }
    }
}
