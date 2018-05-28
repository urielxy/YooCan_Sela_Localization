using System.Collections.Generic;
using Alto.Models.Products;

namespace Alto.Models.Home
{
    public class HomeModel
    {
        public List<BenefitsStripModel> BenefitsStrips { get; set; }
        public List<ProductsStripModel> ProductsStrips { get; set; }
    }
}