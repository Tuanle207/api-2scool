using System;
using Volo.Abp.Domain.Repositories;
using System.Threading.Tasks;
using Scool.Infrastructure.Linq;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Scool.Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scool.Dtos;
using Scool.Common;
using Scool.IApplicationServices;
using Scool.Permissions;
using Scool.Infrastructure.AppService;

namespace Scool.ApplicationServices
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

            var items = await query.Select(x => ObjectMapper.Map<Course, CourseForSimpleListDto>(x))
                .ToListAsync();
            var totalCount = await query.CountAsync();

            return new PagingModel<CourseForSimpleListDto>(items, totalCount, pageIndex, pageSize);
        }

        [Authorize(CoursesPermissions.GetAll)]
        [HttpGet("api/app/courses/is-name-already-used")]
        public async Task<bool> IsNameAlreadyUsedAsync([FromQuery] Guid? id, [FromQuery] string name)
        {
            var lowercaseName = string.IsNullOrEmpty(name) ? string.Empty : name.ToLower();
            return await _courseRepo.AsNoTracking()
                .Where(x => x.Id != id && x.Name.ToLower() == lowercaseName)
                .AnyAsync();
        }
    }
}