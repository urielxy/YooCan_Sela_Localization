namespace Alto.Models.Cards
{
    public class ProductCardModel
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal DiscountAbsolute { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string VendorName { get; set; }
        public string Description { get; set; }
        public int MainCategoryId { get; set; }
        public int MainCategoryName { get; set; }
    }
}