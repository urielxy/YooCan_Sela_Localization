using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Yooocan.Models;
using Yooocan.Models.New.Messages;

namespace Yooocan.Logic
{
    public interface IPrivateMessageLogic
    {
        Task<ConversationModel> GetConversationAsync(string currentUserId, string otherUserId);
        Task<IList<PreviewModel>> GetConversationsAsync(string currentUserId, string userId);
        Task SendMessageAsync(ClaimsPrincipal currentUser, PrivateMessageModel messageModel);
        Task SendNotificationsOnIncomingMessageAsync();
    }
}