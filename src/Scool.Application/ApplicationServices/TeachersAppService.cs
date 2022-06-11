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
    public class TeachersAppService : BasicCrudAppService<
        Teacher,
        Guid,
        TeacherDto,
        TeacherDto,
        CreateUpdateTeacherDto,
        CreateUpdateTeacherDto
    >, ITeachersAppService
    {
        private readonly IRepository<Teacher, Guid> _teachersRepo;
        private readonly IRepository<Class, Guid> _classesRepo;

        public TeachersAppService(
            IRepository<Teacher, Guid> teachersRepo,
            IRepository<Class, Guid> classesRepo
        ) : base(teachersRepo)
        {
            _teachersRepo = teachersRepo;
            _classesRepo = classesRepo;
        }

        public override async Task<TeacherDto> CreateAsync(CreateUpdateTeacherDto input)
        {
            var newTeacher = ObjectMapper.Map<CreateUpdateTeacherDto, Teacher>(input);
            newTeacher.CourseId = ActiveCourse.Id.Value;
            newTeacher.TenantId = CurrentTenant.Id;

            await Repository.InsertAsync(newTeacher);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<Teacher, TeacherDto>(newTeacher);
        }

        public override async Task<PagingModel<TeacherDto>> PostPagingAsync(PageInfoRequestDto input)
        {
            if(!ActiveCourse.IsAvailable)
            {
                return new PagingModel<TeacherDto>();
            }
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;
            var query = Repository.AsNoTracking()
                .Filter(input.Filter)
                .Where(x => x.CourseId == ActiveCourse.Id.Value);
            var totalCount = await query.CountAsync();

            query = string.IsNullOrEmpty(input.SortName) ? query.OrderBy(x => x.Id) : query.OrderBy(input.SortName, input.Ascend);
            query = query.Page(pageIndex, pageSize);

            var items = await query.Select(x => ObjectMapper.Map<Teacher, TeacherDto>(x))
                .ToListAsync();
            return new PagingModel<TeacherDto>(items, totalCount, pageIndex, pageSize);
        }

        public async Task<PagingModel<TeacherForSimpleListDto>> GetSimpleListAsync()
        {
            if (!ActiveCourse.IsAvailable)
            {
                return new PagingModel<TeacherForSimpleListDto>();
            }

            var items = await _teachersRepo
                .Include(x => x.FormClass)
                .Where(x => x.CourseId == ActiveCourse.Id.Value)
                .Select(x => ObjectMapper.Map<Teacher, TeacherForSimpleListDto>(x))
                .ToListAsync();
            var result = new PagingModel<TeacherForSimpleListDto>
            (
                items: items,
                totalCount: items.Count
            );

            return result;
        }

        [HttpGet("api/app/teachers/is-already-form-teacher")]
        public async Task<bool> IsAlreadyFormTeacher([FromQuery] Guid teacherId, [FromQuery] Guid? classId)
        {
            return await _classesRepo.Where(x => x.FormTeacherId == teacherId)
                .Where(x => x.Id != classId)
                .AnyAsync();
        }

        [HttpPost("api/app/teachers/formable-teachers")]
        public async Task<PagingModel<TeacherForSimpleListDto>> GetFormableTeachers([FromQuery(Name = "classId")] Guid? classId)
        {
            if (!ActiveCourse.IsAvailable)
            {
                return new PagingModel<TeacherForSimpleListDto>();
            }

            var items = await _teachersRepo
                .Include(x => x.FormClass)
                .Where(x => x.CourseId == ActiveCourse.Id.Value && (x.FormClass == null || x.FormClass.Id == classId))
                .Select(x => ObjectMapper.Map<Teacher, TeacherForSimpleListDto>(x))
                .ToListAsync();

            var result = new PagingModel<TeacherForSimpleListDto>
            (
                items: items,
                totalCount: items.Count
            );

            return result;
        }
    }
}
