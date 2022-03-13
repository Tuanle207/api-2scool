using Scool.Application.Dtos;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;
using System;
using System.Threading.Tasks;

namespace Scool.Application.IApplicationServices
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
    }
}
