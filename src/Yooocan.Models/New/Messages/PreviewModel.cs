using System;

namespace Yooocan.Models.New.Messages
{
    public class PreviewModel
    {
        public string OtherUserId { get; set; }
        public string OtherName { get; set; }
        public string OtherAvatar { get; set; }
        public string Content { get; set; }
        public DateTime? LastMessageDate { get; set; }
    }
}