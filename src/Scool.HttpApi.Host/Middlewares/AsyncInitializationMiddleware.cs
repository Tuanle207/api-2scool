using Microsoft.AspNetCore.Http;
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

        public async Task Invoke(HttpContext context, ICurrentUser currentUser, IRepository<Account, Guid> accountRepo)
        {
            await AttachCurrentAccountToContext(context, currentUser, accountRepo);
            await _next(context);
        }

        private async Task AttachCurrentAccountToContext(HttpContext context, ICurrentUser currentUser, IRepository<Account, Guid> accountRepo)
        {
            if (currentUser.Id.HasValue)
            {
                var currentAccount = await accountRepo.FirstOrDefaultAsync(x => x.UserId == currentUser.Id);
                context.Items[HttpContextConstants.CurrentAccountProperty] = currentAccount;
            }
            else
            {
                context.Items[HttpContextConstants.CurrentAccountProperty] = null;
            }    
        }
    }
}
