using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scool.Common;
using Scool.Dtos;
using Scool.IApplicationServices;
using Scool.Infrastructure.Common;
using Scool.Infrastructure.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Scool.ApplicationServices
{
    public class NotificationsAppService : ScoolAppService, INotificationsAppService
    {
        private readonly IRepository<AppNotification, Guid> _appNotificationsRepository;
        private readonly IRepository<UserNotificationCount, Guid> _userNotificationsCountRepository;

        public NotificationsAppService(IRepository<AppNotification, Guid> appNotificationsRepository,
            IRepository<UserNotificationCount, Guid> userNotificationsCountRepository)
        {
            _appNotificationsRepository = appNotificationsRepository;
            _userNotificationsCountRepository = userNotificationsCountRepository;
        }

        [HttpPost("api/app/notifications/paging")]
        public async Task<PagingModel<NotificationDto>> GetAppNotifications(PageInfoRequestDto input)
        {
            if (!CurrentUser.IsAuthenticated)
            {
                return new PagingModel<NotificationDto>(new List<NotificationDto>(), 0);
            }

            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;

            var query = _appNotificationsRepository.AsNoTracking()
                .Where(x => x.UserId == CurrentUser.Id);

            query = query.Page(pageIndex, pageSize);

            var totalCount = await query.CountAsync();

            var notifications = await query                
                .Select(x => ObjectMapper.Map<AppNotification, NotificationDto>(x))
                .ToListAsync();

            return new PagingModel<NotificationDto>(notifications, totalCount, pageIndex, pageSize);
        }

        [HttpPost("api/app/notifications/{id}")]
        public async Task MarkAppNotificationAsSeen(Guid id)
        {
            var notification = await _appNotificationsRepository.FirstOrDefaultAsync(x => x.Id == id);
            notification.Seen = true;
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [HttpDelete("api/app/notifications/{id}")]
        public async Task DeleteAppNotification(Guid id)
        {
            var notification = await _appNotificationsRepository.FirstOrDefaultAsync(x => x.Id == id);
            await _appNotificationsRepository.DeleteAsync(notification);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [HttpGet("api/app/notifications-count")]
        public async Task<int> GetNotificationsCount()
        {
            if (!CurrentUser.IsAuthenticated)
            {
                return 0;
            }
            var count = await _userNotificationsCountRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);
            if (count == null)
            {
                return 0;
            }
            return count.Value;
        }

        [HttpDelete("api/app/notifications-count")]
        public async Task CleanNotificationsCount()
        {
            if (!CurrentUser.IsAuthenticated)
            {
                return;
            }
            var count = await _userNotificationsCountRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);
            if (count != null)
            {
                count.Value = 0;
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
    }
}
