using System;

namespace Yooocan.Entities
{
    public class PrivateMessage
    {
        public int Id { get; set; }
        public ApplicationUser FromUser { get; set; }
        public string FromUserId { get; set; }
        public ApplicationUser ToUser { get; set; }
        public string ToUserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsDeletedBySender { get; set; }
        public bool IsDeletedByRecipient { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? ReadDate { get; set; }
    }
}
