using System;
using System.Threading.Tasks;
using Yooocan.Models;
using Yooocan.Models.Shop;

namespace Yooocan.Logic
{
    public interface IShopLogic
    {
        Task<CategoryShopModel> GetCategoryShopAsync(int subCategoryId, int count);
        Task<CategoryShopModel> GetCategoryShopFromDbAsync(int categoryId, int count, DateTime? maxDate = null, int? lastId = null);
        Task<ShopHomeModel> GetShopHomeAsync();
    }
}