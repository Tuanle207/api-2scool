using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scool.Data;
using Volo.Abp.DependencyInjection;

namespace Scool.EntityFrameworkCore
{
    public class EntityFrameworkCoreScoolDbSchemaMigrator
        : IScoolDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreScoolDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the ScoolMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<ScoolMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}