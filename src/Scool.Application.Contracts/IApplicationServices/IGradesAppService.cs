using Scool.Application.Dtos;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Application.IApplicationServices
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
    }
}
