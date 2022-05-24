using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Scool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Scool.Notification
{
    public class NotificationService : INotificationService, IScopedDependency
    {
        private readonly IHubContext<NotificationHub, INoticationHubClient> _notificationHubContext;
        private readonly ICurrentTenant _currentTenant;
        private readonly IRepository<AppNotification, Guid> _appNotificationsRepo;
        private readonly IRepository<UserNotificationCount, Guid> _notificationsCountRepo;
        private readonly IRepository<Account, Guid> _accountsRepo;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IIdentityRoleRepository _identityRoleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(
            IHubContext<NotificationHub, INoticationHubClient> notificationHubContext,
            IIdentityUserRepository identityUserRepository,
            IIdentityRoleRepository identityRoleRepository,
            IRepository<AppNotification, Guid> appNotificationsRepo,
            ICurrentTenant currentTenant, IRepository<Account, Guid> accountsRepo,
            IRepository<UserNotificationCount, Guid> notificationsCountRepo, IUnitOfWork unitOfWork)
        {
            _notificationHubContext = notificationHubContext;
            _currentTenant = currentTenant;
            _identityUserRepository = identityUserRepository;
            _appNotificationsRepo = appNotificationsRepo;
            _accountsRepo = accountsRepo;
            _identityRoleRepository = identityRoleRepository;
            _notificationsCountRepo = notificationsCountRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateNotificationForUserAsync(string messageType, Guid targetUserId, Guid? fromAccountId)
        {
            await CreateNotificationForUsersAsync(messageType, new List<Guid> { targetUserId }, fromAccountId);
        }

        public async Task CreateNotificationForUsersAsync(string messageType, List<Guid> targetUserIds, Guid? fromAccountId)
        {
            var notifications = targetUserIds.Select(userId => new AppNotification
            {
                Title = messageType,
                Content = messageType ,
                FromAccountId = fromAccountId,
                Seen = false,
                TenantId = _currentTenant.Id,
                CreationTime = DateTime.UtcNow,
                UserId = userId,
            });
            await _appNotificationsRepo.InsertManyAsync(notifications);

            var counts = await _notificationsCountRepo.Where(x => targetUserIds.Contains(x.UserId))
                .ToListAsync();
            foreach (var count in counts)
            {
                count.Value++;
            }
            var newCounts = targetUserIds.Where(x => counts.All(c => c.UserId != x))
                .Select(x => new UserNotificationCount
                {
                    TenantId = _currentTenant.Id,
                    UserId = x,
                    Value = 1
                })
                .ToList();
            await _notificationsCountRepo.InsertManyAsync(newCounts);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreateNotificationForRoleAsync(string messageType, string targetRole, Guid? fromAccountId)
        {
            await CreateNotificationForRolesAsync(messageType, new List<string> { targetRole }, fromAccountId);
        }

        public async Task CreateNotificationForRolesAsync(string messageType, List<string> targetRoles, Guid? fromAccountId)
        {
            var roleIds = await _identityRoleRepository.ToEfCoreRepository()
                .AsNoTracking()
                .Where(x => targetRoles.Contains(x.Name))
                .Select(x => x.Id)
                .ToListAsync();
            var targetUserIds = await _identityUserRepository.ToEfCoreRepository()
                .Include(x => x.Roles)
                .Where(x => x.Roles.Any(c => roleIds.Contains(c.RoleId)))
                .Select(x => x.Id)
                .ToListAsync();

            await CreateNotificationForUsersAsync(messageType, targetUserIds, fromAccountId);
        }

        public async Task CreateNotificationForClassAsync(string messageType, Guid targetClassId, Guid? fromAccountId)
        {
            await CreateNotificationForClassesAsync(messageType, new List<Guid> { targetClassId }, fromAccountId);
        }

        public async Task CreateNotificationForClassesAsync(string messageType, List<Guid> targetClassIds, Guid? fromAccountId)
        {
            var targetUserIds = await _accountsRepo.AsNoTracking()
                .Where(x => x.ClassId.HasValue & targetClassIds.Contains(x.ClassId.Value))
                .Select(x => x.UserId)
                .ToListAsync();

            await CreateNotificationForUsersAsync(messageType, targetUserIds, fromAccountId);
        }

        public async Task NotifyToClassAsync(Guid classId, string message)
        {
            await _notificationHubContext.Clients.Group(classId.ToString()).ReceiveNotification(message);
        }

        public async Task NotifyToClassesAsync(List<Guid> classIds, string message)
        {
            await _notificationHubContext.Clients.Groups(classIds.Select(x => x.ToString()).ToList())
                .ReceiveNotification(message);
        }

        public async Task NotifyToRoleAsync(string groupName, string message)
        {
            await _notificationHubContext.Clients.Group(groupName).ReceiveNotification(message);
        }

        public async Task NotifyToRolesAsync(List<string> groupNames, string message)
        {
            await _notificationHubContext.Clients.Groups(groupNames).ReceiveNotification(message);
        }

        public async Task NotifyToUserAsync(Guid userId, string message)
        {
            await _notificationHubContext.Clients.User(userId.ToString()).ReceiveNotification(message);
        }

        public async Task NotifyToUsersAsync(List<Guid> userIds, string message)
        {
            await _notificationHubContext.Clients.Users(userIds.Select(x => x.ToString()).ToList())
                .ReceiveNotification(message);
        }
    }
}
