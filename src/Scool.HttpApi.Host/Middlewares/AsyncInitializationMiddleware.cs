using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Scool.AppConsts;
using Scool.Common;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Scool.Middlewares
{
    public class AsyncInitializationMiddleware
    {
        private readonly RequestDelegate _next;

        public AsyncInitializationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ICurrentUser currentUser, IRepository<Account, Guid> accountRepo, IRepository<Course, Guid> courseRepo)
        {
            await AttachCurrentAccountToContext(context, currentUser, accountRepo);
            await AttachActiveCourseToContext(context, courseRepo);
            await _next(context);
        }

        private async Task AttachCurrentAccountToContext(HttpContext context, ICurrentUser currentUser, IRepository<Account, Guid> accountRepo)
        {
            if (context.Request.Headers[HttpHeaderConstants.QueryCurrentAccountHeader].Count == 0)
            {
                return;
            }
            if (int.TryParse(context.Request.Headers[HttpHeaderConstants.QueryCurrentAccountHeader][0], out int attach) && attach != 1)
            {
                return;
            }
            if (currentUser.Id.HasValue)
            {
                var currentAccount = await accountRepo.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == currentUser.Id);
                context.Items[HttpContextConstants.CurrentAccountProperty] = currentAccount;
            }
            else
            {
                context.Items[HttpContextConstants.CurrentAccountProperty] = null;
            }    
        }

        private async Task AttachActiveCourseToContext(HttpContext context, IRepository<Course, Guid> courseRepo)
        {
            if (context.Request.Headers[HttpHeaderConstants.QueryActiveCourseHeader].Count == 0)
            {
                return;
            }
            if (int.TryParse(context.Request.Headers[HttpHeaderConstants.QueryActiveCourseHeader][0], out int attach) && attach > 0)
            {
                var activeCourse = await courseRepo.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IsActive);
                context.Items[HttpContextConstants.ActiveCourseProperty] = activeCourse;
            }

        }
    }
}
