using System;

namespace Yooocan.Entities
{
    public class NotificationRecipient
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public Notification Notification { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime? ReadDate { get; set; }
        public DateTime InsertDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
