using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Yooocan.Enums;

namespace Yooocan.Models.Products
{
    public class CreateProductModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }

        [DisplayName("Vendor UPC")]
        public string Upc { get; set; }

        [DisplayName("Vendor Item #")]
        public string Sku { get; set; }

        [Required]
        [DisplayName("Product name")]
        public string Name { get; set; }
        [Required]
        public string About { get; set; }
        public decimal? Price { get; set; }

        public decimal? DiscountRate { get; set; }
        public RateType? DiscountType { get; set; }

        [DisplayName("Sold on site?")]
        public bool IsSoldOnSite { get; set; }

        [Required]
        public string Url { get; set; }
        [DisplayName("Product brand")]
        public string Brand { get; set; }
        public string BrandLogoUrl { get; set; }
        public bool VariationsChanged { get; set; }

        [DisplayName("CouponCode")]
        public string CouponCode { get; set; }

        [DisplayName("Coupon not needed")]
        public bool CouponNotNeeded { get; set; }
        public string YouTubeId { get; set; }
        public List<int> Categories { get; set; }
        public int MainCategoryId { get; set; }

        [DisplayName("Redirect page comment")]
        public string RedirectPageComment { get; set; }

        public List<int> Limitations { get; set; }
        public List<string> Images { get; set; }
        public Dictionary<int, string> LimitationsOptions { get; set; }

        public Dictionary<string, Dictionary<int, string>> CategoriesOptions { get; set; }
        public CreateProductModel()
        {
            CategoriesOptions = new Dictionary<string, Dictionary<int, string>>();
            LimitationsOptions = new Dictionary<int, string>();
            Categories = new List<int>();
            Limitations = new List<int>();
            Images = new List<string>();
        }
    }
}