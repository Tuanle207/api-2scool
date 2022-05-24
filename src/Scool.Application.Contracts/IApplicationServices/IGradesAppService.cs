using Scool.Dtos;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scool.IApplicationServices
{
    public interface IGradesAppService : IBasicCrudAppService<
        Guid,
        GradeDto,
        GradeDto,
        CreateUpdateGradeDto,
        CreateUpdateGradeDto
    >
    {
        Task<PagingModel<GradeForSimpleListDto>> GetSimpleListAsync();

        Task<bool> IsNameAlreadyUsedAsync(Guid? id, string name);
    }
}
