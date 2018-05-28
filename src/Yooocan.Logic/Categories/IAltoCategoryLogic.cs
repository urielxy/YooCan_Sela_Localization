using System.Collections.Generic;
using System.Threading.Tasks;
using Yooocan.Models.Categories;

namespace Yooocan.Logic.Categories
{
    public interface IAltoCategoryLogic
    {
        Task<AltoCategoryFeedModel> GetFeedModelAsync(int id);
        Task<AltoCategoryFeedModel> GetParentFeedModelAsync(int id);
        Task<Dictionary<string, Dictionary<int, string>>> GetCategoriesOptionsAsync();
        Task<List<AltoCategoryMenuModel>> GetMenuCategories();
    }
}