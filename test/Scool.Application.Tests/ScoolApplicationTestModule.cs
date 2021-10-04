using Volo.Abp.Modularity;

namespace Scool
{
    [DependsOn(
        typeof(ScoolApplicationModule),
        typeof(ScoolDomainTestModule)
        )]
    public class ScoolApplicationTestModule : AbpModule
    {

    }
}