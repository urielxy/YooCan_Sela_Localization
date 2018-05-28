using System;

namespace Yooocan.Models
{
    public class PrivateMessageModel
    {
        public int Id { get; set; }
        public DateTime InsertDate { get; set; }
        public string FromUserId { get; set; }
        public string FromUserName { get; set; }
        public string FromUserAvatar { get; set; }
        public string ToUserId { get; set; }
        public string ToUserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
    }
}