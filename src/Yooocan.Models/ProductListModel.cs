namespace Yooocan.Models
{
    public class ProductListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specifications { get; set; }
        public string About { get; set; }
        public decimal Price { get; set; }
        public decimal? ListPrice { get; set; }
        public bool IsPublished { get; set; }
        public string VendorName { get; set; }
        public string Url { get; set; }
        public string PrimaryImageUrl { get; set; }
        public string Images { get; set; }
        public string VideoUrl { get; set; }
    }
}