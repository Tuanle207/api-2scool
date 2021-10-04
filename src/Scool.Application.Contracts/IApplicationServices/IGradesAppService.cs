using Scool.Application.Dtos;
using Scool.Infrastructure.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
