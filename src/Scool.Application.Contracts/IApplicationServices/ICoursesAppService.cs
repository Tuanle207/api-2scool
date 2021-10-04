using System;
using System.Threading.Tasks;
using Scool.Application.Dtos;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;

namespace Scool.Application.IApplicationServices
{
    public interface ICoursesAppService: IBasicCrudAppService<
        Guid,
        CourseDto,
        CourseListDto,
        CreateUpdateCourseDto,
        CreateUpdateCourseDto
     >
    {
        Task<PagingModel<CourseForSimpleListDto>> GetSimpleListAsync(PageInfoRequestDto input);
    }
}