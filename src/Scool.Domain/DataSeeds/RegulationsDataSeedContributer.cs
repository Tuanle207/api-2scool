﻿using Microsoft.Extensions.Logging;
using Scool.Common;
using Scool.DataSeeds.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Scool.DataSeeds
{
    public class RegulationsDataSeedContributer : DataSeedBase, IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Criteria, Guid> _criteriasRepo;
        private readonly IRepository<Regulation, Guid> _regulationsRep;
        private readonly ILogger<RegulationsDataSeedContributer> _logger;

        public RegulationsDataSeedContributer(
            IRepository<Criteria, Guid> criteriasRepo, 
            IRepository<Regulation, Guid> regulationsRep,
            ILogger<RegulationsDataSeedContributer> logger
        )
        {
            _criteriasRepo = criteriasRepo;
            _regulationsRep = regulationsRep;
            _logger = logger;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            //try
            //{
            //    var path = GetJsonDataFilePath("regulations-data.json");
            //    var criterias = ParseDataFromJsonFile<List<CriteriaDto>>(path);

            //    if (await _criteriasRepo.AnyAsync(x => true) && await _regulationsRep.AnyAsync(x => true))
            //    {
            //        return;
            //    }

            //    _logger.LogInformation("Seeding criterias and regulations");

            //    foreach (var item in criterias)
            //    {
            //        var criteria = await _criteriasRepo.InsertAsync(new Criteria
            //        {
            //            DisplayName = item.CriteriaName
            //        });

            //        foreach (var reg in item.Items)
            //        {
            //            await _regulationsRep.InsertAsync(new Regulation
            //            {
            //                DisplayName = reg.RegulationName,
            //                Point = reg.Point,
            //                CriteriaId = criteria.Id,
            //                Type = reg.Type
            //            });
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError("Failed to seed criterias and regulations", ex.Message);
            //}

        }
    }
}
