using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scool.Notification
{
    public interface INotificationService
    {
        Task CreateNotificationForUserAsync(string messageType, Guid targetUserId, Guid? fromAccountId);

        Task CreateNotificationForUsersAsync(string messageType, List<Guid> targetUserIds, Guid? fromAccountId);

        Task CreateNotificationForRoleAsync(string messageType, string targetRole, Guid? fromAccountId);

        Task CreateNotificationForRolesAsync(string messageType, List<string> targetRoles, Guid? fromAccountId);

        Task CreateNotificationForClassAsync(string messageType, Guid targetClassId, Guid? fromAccountId);

        Task CreateNotificationForClassesAsync(string messageType, List<Guid> targetClassIds, Guid? fromAccountId);

        Task NotifyToUserAsync(Guid userId, string message);

        Task NotifyToUsersAsync(List<Guid> userIds, string message);

        Task NotifyToRoleAsync(string role, string message);

        Task NotifyToRolesAsync(List<string> roles, string message);

        Task NotifyToClassAsync(Guid classId, string message);

        Task NotifyToClassesAsync(List<Guid> classIds, string message);
    }
}
