using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scool.Common;
using Scool.Dtos;
using Scool.IApplicationServices;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Scool.ApplicationServices
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

        [HttpGet("api/app/grades/is-name-already-used")]
        public async Task<bool> IsNameAlreadyUsedAsync(Guid? id, string name)
        {
            var lowercaseName = string.IsNullOrEmpty(name) ? string.Empty : name.ToLower();
            return await _gradeRepo.AsNoTracking()
                .Where(x => x.Id != id && x.DisplayName.ToLower() == lowercaseName)
                .AnyAsync();
        }
    }
}
