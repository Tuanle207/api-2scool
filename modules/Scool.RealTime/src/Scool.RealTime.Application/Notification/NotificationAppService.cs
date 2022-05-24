using Scool.RealTime.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scool.RealTime.Notification
{
    public class NotificationAppService : RealTimeAppService, INotificationAppService
    {
        public async Task<PagingModel<AppNotificationDto>> GetAppNotifications(NotificationFilterDto filter)
        {
            return new PagingModel<AppNotificationDto>(new List<AppNotificationDto>(), 0);
        }
    }
}
