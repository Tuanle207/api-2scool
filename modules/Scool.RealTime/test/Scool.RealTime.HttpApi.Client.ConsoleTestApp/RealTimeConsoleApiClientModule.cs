using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Scool.RealTime
{
    [DependsOn(
        typeof(RealTimeHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class RealTimeConsoleApiClientModule : AbpModule
    {
        
    }
}
