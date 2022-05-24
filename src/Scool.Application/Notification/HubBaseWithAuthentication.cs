using Microsoft.AspNetCore.SignalR;
using Scool.Users;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;

namespace Scool.Notification
{
    public abstract class HubBaseWithAuthentication<T> : AbpHub<T> where T : class
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext.Request.Query.ContainsKey("group_names"))
            {
                var groups = httpContext.Request.Query["group_names"].ToList();
                var connectionId = Context.ConnectionId;
                foreach (var group in groups)
                {
                    await Groups.AddToGroupAsync(connectionId, group);
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
