using Scool.Dtos;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using System;
using System.Threading.Tasks;

namespace Scool.IApplicationServices
{
    public interface ITeachersAppService : IBasicCrudAppService<
        Guid,
        TeacherDto,
        TeacherDto,
        CreateUpdateTeacherDto,
        CreateUpdateTeacherDto
    >
    {
        Task<PagingModel<TeacherForSimpleListDto>> GetSimpleListAsync();

        Task<bool> IsAlreadyFormTeacher(Guid id, Guid? classId);

        Task<PagingModel<TeacherForSimpleListDto>> GetFormableTeachers(Guid? classId);
    }
}
