using Scool.Application.Dtos;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;
using System;
using System.Threading.Tasks;

namespace Scool.Application.IApplicationServices
{
    public interface IStudentsAppService : IBasicCrudAppService<
        Guid,
        StudentDto,
        StudentDto,
        CreateUpdateStudentDto,
        CreateUpdateStudentDto
    >
    {
        Task<PagingModel<StudentForSimpleListDto>> GetSimpleListAsync(Guid? classId);
    }
}
