using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.DependencyInjection;

namespace Scool.Notification
{
    [Authorize]
    public class NotificationHub : HubBaseWithAuthentication<INoticationHubClient>
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task NotifyToUserAsync(Guid userId, string message)
        {
            await Clients.User(userId.ToString()).ReceiveNotification(message);
        }

        public async Task NotifyToUsersAsync(List<Guid> userIds, string message)
        {
            await Clients.Users(userIds.Select(x => x.ToString()).ToList()).ReceiveNotification(message);
        }

        public async Task NotifyToGroupAsync(string groupName, string message)
        {
            await Clients.Group(groupName).ReceiveNotification(message);
        }

        public async Task NotifyToGroupsAsync(List<string> groupNames, string message)
        {
            await Clients.Groups(groupNames).ReceiveNotification(message);
        }

        public async Task NotifyToClassAsync(Guid classId, string message)
        {
            await Clients.Group(classId.ToString()).ReceiveNotification(message);
        }

        public async Task NotifyToClassesAsync(List<Guid> classIds, string message)
        {
            await Clients.Groups(classIds.Select(x => x.ToString()).ToList())
                .ReceiveNotification(message);
        }
    }
}
