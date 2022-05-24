using Volo.Abp.Modularity;

namespace Scool.RealTime
{
    [DependsOn(
        typeof(RealTimeApplicationModule),
        typeof(RealTimeDomainTestModule)
        )]
    public class RealTimeApplicationTestModule : AbpModule
    {

    }
}
