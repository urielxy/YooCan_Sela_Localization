using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Yooocan.Models
{
    public class VendorUploadProductModel
    {
        public int ProductId { get; set; }
        public int? VendorId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Specifications { get; set; }
        public string About { get; set; }
        [Required]
        public decimal? Price { get; set; }

        public float? Commission { get; set; }
        //public decimal? ListPrice { get; set; }

        public string Url { get; set; }
        public string Upc { get; set; }
        public string Brand { get; set; }

        public float? Weight { get; set; }
        public float? Width { get; set; }
        public float? Depth { get; set; }
        public float? Height { get; set; }
        public string WarrentyUrl { get; set; }
        public string BrandLogoUrl { get; set; }

        public string YouTubeId { get; set; }
        public string Colors { get; set; }
        public string AmazonId { get; set; }
        public List<int> Categories { get; set; }
        public List<int> Limitations { get; set; }
        public List<string> Images { get; set; }
        public List<string> CertificationImages { get; set; }
        public Dictionary<int, string> LimitationsOptions { get; set; }
        
        public Dictionary<string, Dictionary<int, string>> CategoriesOptions { get; set; }

        public VendorUploadProductModel()
        {
            Categories = new List<int>();
            Limitations = new List<int>();
            Images = new List<string>();
            CertificationImages = new List<string>();
        }
    }
}