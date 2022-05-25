using Microsoft.Extensions.Logging;
using Scool.AppConsts;
using Scool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Scool.DataSeeds
{
    public class GradeDataSeedContributer : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Grade, Guid> _gradesRepo;
        private readonly ILogger<GradeDataSeedContributer> _logger;

        public GradeDataSeedContributer(
            IRepository<Grade, Guid> gradesRepo,
            ILogger<GradeDataSeedContributer> logger
        )
        {
            _gradesRepo = gradesRepo;
            _logger = logger;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (!context.TenantId.HasValue)
            {
                return;
            }
            _logger.LogInformation("Started to seed grade");

            try
            {
                var grades = new List<Grade>
                {
                    new Grade
                    {
                        TenantId = context.TenantId,
                        DisplayName = "Khối 10",
                        GradeCode = GradeCode.Ten,
                        Description = "Khối dành cho học sinh lớp 10"
                    },
                    new Grade
                    {
                        TenantId = context.TenantId,
                        DisplayName = "Khối 11",
                        GradeCode = GradeCode.Eleven,
                        Description = "Khối dành cho học sinh lớp 10"
                    },
                    new Grade
                    {
                        TenantId = context.TenantId,
                        DisplayName = "Khối 12",
                        GradeCode = GradeCode.Twelve,
                        Description = "Khối dành cho học sinh lớp 12"
                    },
                };
                var currentGrades = await _gradesRepo.ToListAsync();
                foreach (var grade in grades)
                {
                    if (currentGrades.All(x => x.GradeCode != grade.GradeCode))
                    {
                        await _gradesRepo.InsertAsync(grade);
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to seed grade", ex.Message);
            }

        }
    }
}
