using System;

namespace Yooocan.Models.Products
{
    public class ProductCardModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountAbsolute { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string PrimaryImageUrl { get; set; }
        public string ProductPageUrl { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public string SubCategoryName { get; set; }
        public string About { get; set; }
        public string AmazonId { get; set; }
        public bool IsNew { get; set; }
        public DateTime InsertDate { get; set; }
        public int MainCategoryId { get; set; }
        public int MainCategoryName { get; set; }
    }
}