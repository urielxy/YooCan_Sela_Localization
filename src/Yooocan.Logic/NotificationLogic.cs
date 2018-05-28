using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using StackExchange.Redis;
using Yooocan.Dal;
using Yooocan.Models.New;
using Microsoft.EntityFrameworkCore;
using Yooocan.Entities;
using System;
using Newtonsoft.Json;
using Yooocan.Enums.Notifications;
using System.Security.Claims;

namespace Yooocan.Logic
{
    public class NotificationLogic : INotificationLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly IDatabase _redisDatabase;
        private readonly IMapper _mapper;

        public NotificationLogic(ApplicationDbContext context, IMapper mapper, IDatabase redisDatabase)
        {
            _context = context;
            _redisDatabase = redisDatabase;
            _mapper = mapper;
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            var notifications = await GetUserNotificationsAsync(userId);
            var count = notifications.Count(x => x.ReadDate == null);

            return count;
        }

        public Notification CreateNotification(ClaimsPrincipal sourceUser, NotificationType notificationType)
        {
            var loggedInUserId = sourceUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            var firstName = sourceUser.FindFirst(ClaimTypes.GivenName).Value;
            var lastName = sourceUser.FindFirst(ClaimTypes.Surname).Value;
            var imageUrl = sourceUser.FindFirst("picture")?.Value;

            var notification = new Notification
                               {
                                   Type = notificationType,
                                   SourceUserId = loggedInUserId,
                                   Text = GetNotificationText(firstName, lastName, notificationType),
                                   ImageUrl = imageUrl
                               };
            return notification;
        }

        private string GetNotificationText(string firstName, string lastName, NotificationType notificationType)
        {
            switch(notificationType)
            {
                case NotificationType.FollowerNew:
                    return $"<strong>{firstName} {lastName}</strong> followed you.";
                case NotificationType.MessageNew:
                    return $"<strong>{firstName} {lastName}</strong> has sent you a message.";
                case NotificationType.CommentNew:
                    return $"<strong>{firstName} {lastName}</strong> commented on your story.";
                default:
                    throw new NotImplementedException($"not implemented notification type {notificationType.ToString()}");
            }
        }

        public async Task<List<NotificationModel>> GetUserNotificationsAsync(string userId, bool markAsRead = false)
        {
            var cacheValue = await _redisDatabase.StringGetAsync(GetCacheKey(userId));
            List<NotificationModel> model = null;

            if (cacheValue.HasValue)
            {
                model = JsonConvert.DeserializeObject<List<NotificationModel>>(cacheValue);
            }
            else
            {
                //TODO: check why this query joins twice on the same table on the same field
                var notifications = await _context.NotificationRecipients
                                                    .Where(x => x.UserId == userId && !x.IsDeleted && !x.Notification.IsDeleted)
                                                                                       .Include(x => x.Notification)
                                                                                       .OrderByDescending(x => x.InsertDate)
                                                                                       .ToListAsync();

                model = _mapper.Map<List<NotificationModel>>(notifications);
                SetCache(userId, model);
            }

            if (markAsRead)
            {
                var unread = model.Where(x => x.ReadDate == null).ToList();
                if (unread.Any())
                {
                    var beforeMarkedAsReadCopy = _mapper.Map<List<NotificationModel>>(model);
                    await MarkAsReadAsync(unread);
                    SetCache(userId, model);
                    return beforeMarkedAsReadCopy;
                }
            }

            return model;
        }

        public async Task MarkAsReadAsync(string targetUserId, NotificationType notificationType, string sourceUserId = null, int? objectId = null)
        {
            var allNotifications = await GetUserNotificationsAsync(targetUserId);
            var neededNotificationsFilter = allNotifications.Where(x => x.NotificationType == notificationType && x.ReadDate == null);
            if (sourceUserId != null)
            {
                neededNotificationsFilter = neededNotificationsFilter.Where(x => x.NotificationSourceUserId == sourceUserId);
            }
            if (objectId != null)
            {
                neededNotificationsFilter = neededNotificationsFilter.Where(x => x.NotificationObjectId == objectId);
            }
            var neededNotifications = neededNotificationsFilter.ToList();

            if (neededNotifications.Any())
            {
                await MarkAsReadAsync(neededNotifications);
                SetCache(targetUserId, allNotifications);
            }
        }

        private void SetCache(string userId, List<NotificationModel> userNotifications)
        {
            _redisDatabase.StringSet(GetCacheKey(userId), JsonConvert.SerializeObject(userNotifications), TimeSpan.FromHours(12), flags: CommandFlags.FireAndForget);
        }

        private async Task MarkAsReadAsync(List<NotificationModel> notificationModels)
        {
            var ids = notificationModels.Select(x => x.Id).ToList();
            var notifications = await _context.NotificationRecipients.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var notification in notifications)
            {
                if (notification.ReadDate == null)
                    notification.ReadDate = DateTime.Now;
                notificationModels.Single(x => x.Id == notification.Id).ReadDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SendNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);          
            foreach (var recipient in notification.Recipients)
            {
                _redisDatabase.KeyDelete(GetCacheKey(recipient.UserId));
            }
            await _context.SaveChangesAsync();
        }

        private string GetCacheKey(string userId)
        {
            return string.Format(RedisKeys.Notifications, userId);
        }
    }
}
