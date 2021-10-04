using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Scool.Data
{
    /* This is used if database provider does't define
     * IScoolDbSchemaMigrator implementation.
     */
    public class NullScoolDbSchemaMigrator : IScoolDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}