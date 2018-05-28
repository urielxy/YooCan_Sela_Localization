using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Yooocan.Entities;
using Yooocan.Enums.Notifications;
using Yooocan.Models.New;

namespace Yooocan.Logic
{
    public interface INotificationLogic
    {
        Task<int> GetUnreadCountAsync(string userId);
        Task<List<NotificationModel>> GetUserNotificationsAsync(string userId, bool markAsRead = false);
        Task MarkAsReadAsync(string targetUserId, NotificationType notificationType, string sourceUserId = null, int? objectId = null);
        Task SendNotificationAsync(Notification notification);
        Notification CreateNotification(ClaimsPrincipal sourceUser, NotificationType notificationType);
    }
}