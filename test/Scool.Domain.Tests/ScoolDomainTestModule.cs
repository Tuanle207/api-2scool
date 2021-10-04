using Scool.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Scool
{
    [DependsOn(
        typeof(ScoolEntityFrameworkCoreTestModule)
        )]
    public class ScoolDomainTestModule : AbpModule
    {

    }
}