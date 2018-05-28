using System;

namespace Yooocan.Models.New.Messages
{
    public class ConversationMessageModel
    {
        public int Id { get; set; }
        public DateTime InsertDate { get; set; }
        public string FromUserName { get; set; }
        public string FromUserAvatar { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
    }
}