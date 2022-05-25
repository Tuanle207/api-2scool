using System;
using System.Collections.Generic;
using System.Text;
using Scool.Courses;
using Scool.Localization;
using Scool.Users;
using Volo.Abp.Application.Services;

namespace Scool
{
    /* Inherit your application services from this class.
     */
    public abstract class ScoolAppService : ApplicationService
    {
        protected ICurrentAccount CurrentAccount => LazyServiceProvider.LazyGetRequiredService<ICurrentAccount>();
        protected IActiveCourse ActiveCourse => LazyServiceProvider.LazyGetRequiredService<IActiveCourse>();
 
        protected ScoolAppService()
        {
            LocalizationResource = typeof(ScoolResource);
        }
    }
}
