namespace Yooocan.Models.Products
{
    public class ProductAllModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Categories { get; set; }
        public bool IsPublished { get; set; }
        public decimal Price { get; set; }
    }
}