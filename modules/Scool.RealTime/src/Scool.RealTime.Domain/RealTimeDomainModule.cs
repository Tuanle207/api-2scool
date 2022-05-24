using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Scool.RealTime
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(RealTimeDomainSharedModule)
    )]
    public class RealTimeDomainModule : AbpModule
    {

    }
}
