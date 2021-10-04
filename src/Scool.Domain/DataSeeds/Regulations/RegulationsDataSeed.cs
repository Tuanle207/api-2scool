using Microsoft.Extensions.Logging;
using Scool.Domain.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Scool.DataSeeds.Regulations
{
    public class RegulationsDataSeed : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Criteria, Guid> _criteriasRepo;
        private readonly IRepository<Regulation, Guid> _regulationsRep;
        private readonly ILogger<RegulationsDataSeed> _logger;

        public RegulationsDataSeed(
            IRepository<Criteria, Guid> criteriasRepo, 
            IRepository<Regulation, Guid> regulationsRep,
            ILogger<RegulationsDataSeed> logger
        )
        {
            _criteriasRepo = criteriasRepo;
            _regulationsRep = regulationsRep;
            _logger = logger;
        }
        async Task IDataSeedContributor.SeedAsync(DataSeedContext context)
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "regulations-data.json");
            var jsonText = File.ReadAllText(path);
            var criterias = JsonSerializer.Deserialize<List<CriteriaDto>>(jsonText);
            foreach (var item in criterias)
            {
                var criteria = await _criteriasRepo.InsertAsync(new Criteria
                {
                    DisplayName = item.criteriaName
                });

                foreach (var reg in item.items)
                {
                    await _regulationsRep.InsertAsync(new Regulation
                    {
                        DisplayName = reg.regulationName,
                        Point = reg.point,
                        CriteriaId = criteria.Id
                    });
                }
            }

        }
    }

    internal class RegulationDto
    {
        public string regulationName { get; set; }
        public string description { get; set; }
        public string note { get; set; }
        public int point { get; set; }
    }

    internal class CriteriaDto
    {
        public string criteriaName { get; set; }
        public IEnumerable<RegulationDto> items { get; set; }
    }


}
