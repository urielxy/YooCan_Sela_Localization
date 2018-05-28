using System.Collections.Generic;

namespace Yooocan.Models
{
    public class ShopModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<ShopSubCategoryModel> SubCategories { get; set; }
    }

    public class ShopProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? ListPrice { get; set; }
        public string PrimaryImageUrl { get; set; }
    }

    public class ShopSubCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParentCategoryName { get; set; }
        public IEnumerable<ShopProductModel> Products { get; set; }
        public IEnumerable<ShopServiceProviderModel> ServiceProviders { get; set; }
    }

    public class ShopServiceProviderModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PrimaryImageUrl { get; set; }
    }
}