using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yooocan.Models;
using Yooocan.Models.New.Home;

namespace Yooocan.Logic
{
    public interface IHomeLogic
    {
        Task<HomeModel> GetModelAsync();
        Task<MobileHomeModel> GetMobileModelAsync();
        Task<List<FeaturedStoryHeader>> GetLatestStoriesPerCategoryAsync();

        Task<NewUserHomeModel> GetNewUserModelAsync();
        Task<ContentImFollowingModel> GetContentImFollowingAsync(string userId, int count, DateTime? maxDate = null, int? lastId = null);
        Task<List<StoryCardModel>> GetStoriesFromDb(int count, DateTime? maxDate = null, int? maxId = null, List<int> excludeIds = null);
    }
}