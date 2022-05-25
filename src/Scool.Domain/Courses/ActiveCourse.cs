using Microsoft.AspNetCore.Http;
using Scool.AppConsts;
using Scool.Common;
using System;
using Volo.Abp.DependencyInjection;

namespace Scool.Courses
{
    public class ActiveCourse : IActiveCourse, IScopedDependency
    {
        private readonly Course _activeCourse;

        public ActiveCourse(IHttpContextAccessor httpContextAccessor)
        {
            _activeCourse = httpContextAccessor.HttpContext?
                .Items[HttpContextConstants.ActiveCourseProperty] as Course;
        }

        protected Course Course => _activeCourse;

        public bool IsAvailable => Course != null;

        public Guid? Id => Course?.Id;

        public string Name => Course?.Name;

        public string Description => Course?.Description;

        public DateTime? StartTime => Course?.StartTime;

        public DateTime? FinishTime => Course?.FinishTime;

        public bool? Ended => FinishTime.HasValue ? FinishTime.Value.CompareTo(DateTime.UtcNow) < 0 : null;
    }
}
