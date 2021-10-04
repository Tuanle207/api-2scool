using Scool.Application.Dtos;
using Scool.Application.IApplicationServices;
using Scool.Domain.Common;
using Scool.Infrastructure.ApplicationServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Scool.Application.ApplicationServices
{
    public class GradesAppService : BasicCrudAppService<
        Grade,
        Guid,
        GradeDto,
        GradeDto,
        CreateUpdateGradeDto,
        CreateUpdateGradeDto
    >, IGradesAppService
    {
        public GradesAppService(IRepository<Grade, Guid> gradeRepo) : base(gradeRepo)
        {

        }
    }
}
