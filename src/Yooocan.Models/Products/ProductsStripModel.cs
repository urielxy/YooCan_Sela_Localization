using System.Collections.Generic;

namespace Yooocan.Models.Products
{
    public class ProductsStripModel
    {
        public string Title { get; set; }
        public List<ProductCardModel> Products { get; set; }
        public bool IsSlim { get; set; }
    }
}