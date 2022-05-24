using Scool.RealTime.Common;
using System.Threading.Tasks;

namespace Scool.RealTime.Notification
{
    public interface INotificationAppService
    {
        Task<PagingModel<AppNotificationDto>> GetAppNotifications(NotificationFilterDto filter);
    }
}
