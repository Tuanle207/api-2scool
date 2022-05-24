using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Scool.RealTime.EntityFrameworkCore
{
    [ConnectionStringName(RealTimeDbProperties.ConnectionStringName)]
    public interface IRealTimeDbContext : IEfCoreDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * DbSet<Question> Questions { get; }
         */
    }
}