using System;
using System.Threading.Tasks;
using Scool.Dtos;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;

namespace Scool.IApplicationServices
{
    public interface ICoursesAppService : IBasicCrudAppService<
        Guid,
        CourseDto,
        CourseListDto,
        CreateUpdateCourseDto,
        CreateUpdateCourseDto
     >
    {
        Task<PagingModel<CourseForSimpleListDto>> GetSimpleListAsync(PageInfoRequestDto input);

        Task<bool> IsNameAlreadyUsedAsync(Guid? id, string name);

        Task<bool> MarkAsActiveCourseAsync(Guid id);

        Task<bool> HasActiveCourseAsync();

    }
}