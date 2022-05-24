using Scool.Dtos;
using Scool.Infrastructure.Common;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Scool.IApplicationServices
{
    public interface INotificationsAppService : IApplicationService
    {
        Task<PagingModel<NotificationDto>> GetAppNotifications(PageInfoRequestDto input);

        Task MarkAppNotificationAsSeen(Guid id);

        Task DeleteAppNotification(Guid id);

        Task<int> GetNotificationsCount();

        Task CleanNotificationsCount();
    }
}
