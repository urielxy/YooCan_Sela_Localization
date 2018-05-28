using System.Collections.Generic;
using System.Threading.Tasks;
using Yooocan.Models;

namespace Yooocan.Logic
{
    public interface ICategoriesLogic
    {
        Task<Dictionary<int, string>> GetMainCategoriesForSearchAsync();
        Task<Dictionary<string, Dictionary<int, string>>> GetCategoriesForStoryAsync();
        Task<Dictionary<string, Dictionary<int, string>>> GetCategoriesForProductAsync();
        Task FollowCategoryAsync(int id, string userId);
        Task UnfollowCategoryAsync(int id, string userId);
        Task<List<CategoryModel>> GetMenuShopAndServiceProvidersCategories();
        Task<List<CategoryModel>> GetMenuFeedCategories();
    }
}