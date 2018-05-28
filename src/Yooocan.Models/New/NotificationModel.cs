using System;
using Yooocan.Enums.Notifications;

namespace Yooocan.Models.New
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public NotificationType NotificationType { get; set; }
        public string NotificationImageUrl { get; set; }
        public string NotificationText { get; set; }
        public string NotificationLink { get; set; }
        public string NotificationSourceUserId { get; set; }
        public int? NotificationObjectId { get; set; }
        public DateTime? ReadDate { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
