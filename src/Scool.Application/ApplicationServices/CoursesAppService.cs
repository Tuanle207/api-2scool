using System;
using Scool.Domain.Common;
using Volo.Abp.Domain.Repositories;
using System.Threading.Tasks;
using Scool.Infrastructure.Linq;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Scool.Infrastructure.ApplicationServices;
using Scool.Application.IApplicationServices;
using Scool.Application.Dtos;
using Scool.Infrastructure.Common;
using Scool.Application.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace Scool.Application.ApplicationServices
{
    public class CoursesAppService :
        BasicCrudAppService<
            Course,
            Guid,
            CourseDto,
            CourseListDto,
            CreateUpdateCourseDto,
            CreateUpdateCourseDto
        >, ICoursesAppService
    {
        private readonly IRepository<Course, Guid> _courseRepo;

        public CoursesAppService(IRepository<Course, Guid> courseRepo) : base(courseRepo)
        {
            _courseRepo = courseRepo;
            GetPolicyName = CoursesPermissions.Get;
            GetListPolicyName = CoursesPermissions.GetAll;
            CreatePolicyName = CoursesPermissions.Create;
            UpdatePolicyName = CoursesPermissions.Update;
            DeletePolicyName = CoursesPermissions.Delete;

        }

        [Authorize(CoursesPermissions.GetAll)]
        public async Task<PagingModel<CourseForSimpleListDto>> GetSimpleListAsync(PageInfoRequestDto input)
        {
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;
            var query = _courseRepo.Filter(input.Filter)
                            .OrderBy(x => x.StartTime);

            var items = ObjectMapper.Map<List<Course>, List<CourseForSimpleListDto>>(await query.ToListAsync());
            var totalCount = await _courseRepo.Filter(input.Filter).CountAsync();

            return new PagingModel<CourseForSimpleListDto>(items, totalCount, pageIndex, pageSize);
        }
    }
}