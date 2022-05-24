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

        public async Task<PagingModel<TeacherForSimpleListDto>> GetSimpleListAsync()
        {
            var items = await _teachersRepo
                .Include(x => x.FormClass)
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
    }
}
