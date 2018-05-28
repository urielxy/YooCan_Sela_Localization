using System.Collections.Generic;
using System.Threading.Tasks;
using Yooocan.Models;
using Yooocan.Models.Users;

namespace Yooocan.Logic
{
    public interface IUserLogic
    {
        Task EditBioAsync(UserBioModel userBio, bool adminChange);
        Task<List<FollowingModel>> GetFollowingAsync(string userId);
        Task<List<FollowerModel>> GetFollowersAsync(string userId);
        Task<bool> SubscribeToNewsletterAsync(string email, string ipAddress);
    }
}