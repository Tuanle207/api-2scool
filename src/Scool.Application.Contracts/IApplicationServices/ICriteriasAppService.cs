using Scool.Dtos;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scool.IApplicationServices
{
    public interface ICriteriasAppService : IBasicCrudAppService<
        Guid,
        CriteriaDto,
        CriteriaDto,
        CreateUpdateCriteriaDto,
        CreateUpdateCriteriaDto
    >
    {
        Task<PagingModel<CriteriaForSimpleListDto>> GetSimpleListAsync();
    }
}
