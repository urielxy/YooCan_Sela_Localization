using System.Collections.Generic;

namespace Yooocan.Models
{
    public class ServiceProviderShopViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ShopBackgroundColor { get; set; }
        public string HeaderPictureUrl { get; set; }
        public IEnumerable<ShopServiceProviderModel> ServiceProviders { get; set; }
        public List<CategoryShopSubCategoryModel> SubCategories { get; set; }
    }
}