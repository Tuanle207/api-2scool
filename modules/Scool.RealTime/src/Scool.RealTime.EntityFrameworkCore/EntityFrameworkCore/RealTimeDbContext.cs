using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Scool.RealTime.EntityFrameworkCore
{
    [ConnectionStringName(RealTimeDbProperties.ConnectionStringName)]
    public class RealTimeDbContext : AbpDbContext<RealTimeDbContext>, IRealTimeDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * public DbSet<Question> Questions { get; set; }
         */

        public RealTimeDbContext(DbContextOptions<RealTimeDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureRealTime();
        }
    }
}