namespace Yooocan.Models.Vendors
{
    public class MyProductsProductModel
    {
        public int Id { get; set; }
        public string Upc { get; set; }
        public string BrandName { get; set; }
        public string Name { get; set; }
        public bool IsPublished { get; set; }
        public string PrimaryImageUrl { get; set; }
        public decimal Price { get; set; }
        public string Categories { get; set; }
        public string Limitations { get; set; }
        public string AmazonId { get; set; }
    }
}