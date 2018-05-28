using Alto.Domain.Companies;

namespace Alto.Domain.Products
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }

        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public bool IsMain { get; set; }
    }
}