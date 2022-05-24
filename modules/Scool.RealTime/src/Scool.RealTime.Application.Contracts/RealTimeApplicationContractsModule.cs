using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Scool.RealTime
{
    [DependsOn(
        typeof(RealTimeDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class RealTimeApplicationContractsModule : AbpModule
    {

    }
}
