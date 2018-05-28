using System.Collections.Generic;
using Yooocan.Models.Products;

namespace Yooocan.Models
{
    public class ProductModel
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
        public float? Width { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public float? Depth { get; set; }
        public string WarrentyUrl { get; set; }
        public string Brand { get; set; }
        public string BrandLogUrl { get; set; }
        public string Upc { get; set; }
        public string HeaderImageUrl { get; set; }
        public string YouTubeId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int? AltoId { get; set; }
        public List<string> Colors { get; set; }
        public List<string> Images { get; set; }
        public List<string> CertificationImages { get; set; }
        public List<ProductCardModel> RelatedProducts { get; set; }
        public List<StoryCardModel> StoriesFeaturing { get; set; }
    }
}