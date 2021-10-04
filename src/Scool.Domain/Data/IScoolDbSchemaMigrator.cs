using System.Threading.Tasks;

namespace Scool.Data
{
    public interface IScoolDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
