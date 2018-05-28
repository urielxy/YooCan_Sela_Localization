using System;
using System.Collections.Generic;
using Yooocan.Enums.Notifications;

namespace Yooocan.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public List<NotificationRecipient> Recipients { get; set; }
        public string SourceUserId { get; set; }
        public ApplicationUser SourceUser { get; set; }
        public NotificationType Type { get; set; }
        public string ImageUrl { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public int? ObjectId { get; set; }
        public int? ParentId { get; set; }
        public ParentType? ParentType { get; set; }
        public DateTime InsertDate { get; set; }
        public bool IsDeleted { get; set; }

        public Notification()
        {
            Recipients = new List<NotificationRecipient>();
        }

        public void AddRecipient(string userId)
        {
            Recipients.Add(new NotificationRecipient
            {
                Notification = this,
                UserId = userId
            });
        }
    }
}
