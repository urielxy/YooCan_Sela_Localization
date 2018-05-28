using System;
using Yooocan.Enums.Products;

namespace Yooocan.Entities.Products
{
    public class PromotedProduct
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Order { get; set; }
        public Product Product { get; set; }
        public PromotionType PromotionType { get; set; }
        public DateTime InsertDate { get; set; }
    }
}