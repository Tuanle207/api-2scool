using Scool.Dtos;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using System;
using System.Threading.Tasks;

namespace Scool.IApplicationServices
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
