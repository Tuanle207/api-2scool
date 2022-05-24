﻿using Scool.Dtos;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using System;
using System.Threading.Tasks;

namespace Scool.IApplicationServices
{
    public interface IRegulationsAppService : IBasicCrudAppService<
        Guid,
        RegulationDto,
        RegulationDto,
        CreateUpdateRegulationDto,
        CreateUpdateRegulationDto
    >
    {
        Task<PagingModel<RegulationForSimpleListDto>> GetSimpleListAsync();
    }
}
