using Scool.Application.Dtos;
using Scool.Infrastructure.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Text;
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
        Task<IEnumerable<TeacherForSimpleListDto>> GetSimpleListAsync();
    }
}
