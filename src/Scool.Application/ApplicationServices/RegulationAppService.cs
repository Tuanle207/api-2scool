using Microsoft.EntityFrameworkCore;
using Scool.Application.Dtos;
using Scool.Domain.Common;
using Scool.IApplicationServices;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Scool.ApplicationServices
{
    public class RegulationAppService : BasicCrudAppService<
        Regulation,
        Guid,
        RegulationDto,
        RegulationDto,
        CreateUpdateRegulationDto,
        CreateUpdateRegulationDto
    >, IRegulationsAppService
    {
        private readonly IRepository<Regulation, Guid> _regulationsRepo;

        public RegulationAppService(IRepository<Regulation, Guid> regulationsRepo) : base(regulationsRepo)
        {
            _regulationsRepo = regulationsRepo;
        }

        public async Task<PagingModel<RegulationForSimpleListDto>> GetSimpleListAsync()
        {
            var regulations = await _regulationsRepo
                .Include(x => x.Criteria)
                .OrderBy(x => x.DisplayName)
                .Select(x => ObjectMapper.Map<Regulation, RegulationForSimpleListDto>(x))
                .ToListAsync();

            var result = new PagingModel<RegulationForSimpleListDto>
            (
                items: regulations,
                totalCount: regulations.Count
            );

            return result;
        }
    }
}
