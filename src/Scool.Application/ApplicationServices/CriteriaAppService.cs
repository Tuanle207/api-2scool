using Microsoft.EntityFrameworkCore;
using Scool.Application.Dtos;
using Scool.Application.IApplicationServices;
using Scool.Domain.Common;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Scool.ApplicationServices
{
    public class CriteriaAppService : BasicCrudAppService<
        Criteria,
        Guid,
        CriteriaDto,
        CriteriaDto,
        CreateUpdateCriteriaDto,
        CreateUpdateCriteriaDto
    >, ICriteriasAppService
    {
        private readonly IRepository<Criteria> _criteriasRepo;

        public CriteriaAppService(IRepository<Criteria, Guid> criteriasRepo) : base(criteriasRepo)
        {
            _criteriasRepo = criteriasRepo;
        }

        public async Task<PagingModel<CriteriaForSimpleListDto>> GetSimpleListAsync()
        {
            var criteras = await _criteriasRepo
                .Select(x => ObjectMapper.Map<Criteria, CriteriaForSimpleListDto>(x))
                .ToListAsync();
            var result = new PagingModel<CriteriaForSimpleListDto>
            (
                items: criteras,
                totalCount: criteras.Count
            );
            return result;
        }
    }
}
