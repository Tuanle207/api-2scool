using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scool.Common;
using Scool.Dtos;
using Scool.IApplicationServices;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using Scool.Infrastructure.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Scool.ApplicationServices
{
    public class ClassesAppService : BasicCrudAppService<
        Class,
        Guid,
        ClassDto,
        ClassForListDto,
        CreateUpdateClassDto,
        CreateUpdateClassDto
    >, IClassesAppService
    {
        private readonly IRepository<Class, Guid> _classRepo;

        public ClassesAppService(IRepository<Class, Guid> classRepo) : base(classRepo)
        {
            _classRepo = classRepo;
        }

        public override async Task<ClassDto> CreateAsync(CreateUpdateClassDto input)
        {
            var newClass = ObjectMapper.Map<CreateUpdateClassDto, Class>(input);
            newClass.CourseId = ActiveCourse.Id.Value;
            newClass.TenantId = CurrentTenant.Id;
            await Repository.InsertAsync(newClass);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<Class, ClassDto>(newClass);
        }

        public override async Task<PagingModel<ClassForListDto>> PostPagingAsync(PageInfoRequestDto input)
        {
            if (!ActiveCourse.IsAvailable)
            {
                return new PagingModel<ClassForListDto>();
            }

            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;
            var query = Repository.AsNoTracking()
                .Filter(input.Filter)
                .Where(x => x.CourseId == ActiveCourse.Id.Value);

            var totalCount = await query.CountAsync();

            query = query.OrderBy(x => x.Name);
            query = query.Page(pageIndex, pageSize);
            query = query.Include(e => e.Course)
                    .Include(e => e.FormTeacher)
                    .Include(e => e.Grade);
            var items = await query.Select(x => ObjectMapper.Map<Class, ClassForListDto>(x))
                .ToListAsync();
            return new PagingModel<ClassForListDto>(items, totalCount, pageIndex, pageSize);
        }

        public override async Task<ClassDto> GetAsync(Guid id)
        {
            var entity = await _classRepo.Where(e => e.Id == id)
                    .AsNoTracking()
                    .Include(e => e.Course)
                    .Include(e => e.FormTeacher)
                    .Include(e => e.Grade)
                    .Include(e => e.Students)
                    .Select(x => ObjectMapper.Map<Class, ClassDto>(x))
                    .FirstOrDefaultAsync();
            return entity;
        }

        public async Task<PagingModel<ClassForSimpleListDto>> GetSimpleListAsync()
        {
            if (!ActiveCourse.IsAvailable)
            {
                return new PagingModel<ClassForSimpleListDto>();
            }

            var items = await _classRepo
                .AsNoTracking()
                .Where(x => x.CourseId == ActiveCourse.Id.Value)
                .Include(x => x.Grade)
                .OrderBy(x => x.Name)
                .Select(x => ObjectMapper.Map<Class, ClassForSimpleListDto>(x))
                .ToListAsync();

            var result = new PagingModel<ClassForSimpleListDto>
            (
                items: items,
                totalCount: items.Count
            );

            return result;
        }

        [HttpGet("api/app/classes/is-name-already-used")]
        public async Task<bool> IsNameAlreadyUsedAsync([FromQuery] Guid? id, [FromQuery] string name)
        {
            var lowercaseName = string.IsNullOrEmpty(name) ? string.Empty : name.ToLower();
            return await _classRepo.AsNoTracking()
                .Where(x => x.CourseId == ActiveCourse.Id)
                .Where(x => x.Id != id && x.Name.ToLower() == lowercaseName)
                .AnyAsync();
        }
    }
}
