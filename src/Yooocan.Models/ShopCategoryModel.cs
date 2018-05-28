using System.Collections.Generic;

namespace Yooocan.Models
{
    public class ShopCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public List<CategoryShopSubCategoryModel> SubCategories { get; set; }
        public string ShopBackgroundColor { get; set; }
    }

    public class CategoryShopSubCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}