using System;

namespace Alto.Domain.Products
{
    public class ProductVariationCombination
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public string Combinations { get; set; }
        public DateTime InsertDate { get; set; }
    }
}