using System.Collections.Generic;
using Yooocan.Models.Products;

namespace Yooocan.Models
{
    public class CategoryShopModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ShopBackgroundColor { get; set; }
        public string HeaderPictureUrl { get; set; }
        public IEnumerable<ProductCardModel> Products { get; set; }
    }
}