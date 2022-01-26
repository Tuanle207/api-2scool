using Microsoft.EntityFrameworkCore;
using Scool.Application.Contracts.Dtos;
using Scool.Application.Dtos;
using Scool.Application.IApplicationServices;
using Scool.Domain.Common;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IRepository<Grade, Guid> _gradeRepo;

        public GradesAppService(IRepository<Grade, Guid> gradeRepo) : base(gradeRepo)
        {
            _gradeRepo = gradeRepo;
        }

        public async Task<PagingModel<GradeForSimpleListDto>> GetSimpleListAsync()
        {
            var items = await _gradeRepo
                .Select(x => ObjectMapper.Map<Grade, GradeForSimpleListDto>(x))
                .ToListAsync();

            var result = new PagingModel<GradeForSimpleListDto>
            (
                items: items,
                totalCount: items.Count
            );

            return result;
        }
    }
}
