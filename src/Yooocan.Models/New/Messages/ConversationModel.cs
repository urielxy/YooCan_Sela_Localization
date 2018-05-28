using System.Collections.Generic;

namespace Yooocan.Models.New.Messages
{
    public class ConversationModel
    {
        public List<ConversationMessageModel> Read { get; set; }
        public List<ConversationMessageModel> NotRead { get; set; }

        public ConversationModel()
        {
            Read = new List<ConversationMessageModel>();
            NotRead = new List<ConversationMessageModel>();
        }
    }
}
