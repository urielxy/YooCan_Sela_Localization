using System;

namespace Yooocan.Entities
{
    public class NotificationLog
    {
        public int Id { get; set; }
        public string NotificationId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Method { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime InsertDate { get; set; }
    }
}