using System.Collections.Generic;
using Yooocan.Enums;

namespace Yooocan.Models.Products
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public BenefitType Type { get; set; }
        public List<string> Images { get; set; }
        public string MainImageUrl { get; set; }
        public string HeaderImageUrl { get; set; }
        public string LogoUrl { get; set; }
        public string YouTubeId { get; set; }
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public bool CompanyIsForeignCurrency { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountAbsolute { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public RateType DiscountType { get; set; }
        public bool IsSoldOnSite { get; set; }
        public decimal Price { get; set; }
        public decimal ShippingPrice { get; set; }
        public string About { get; set; }
        public string VariationCombinations { get; set; }
        public bool IsTravel { get; set; }
        public bool IsFreeTrialPromo { get; set; }
        public bool CouponNotNeeded { get; set; }
        public string RedirectPageComment { get; set; }
        public KeyValuePair<int,string>? Category { get; set; }
        public KeyValuePair<int,string>? ParentCategory { get; set; }
        public List<VariationModel> Variations { get; set; }

        public ProductsStripModel RelatedProducts { get; set; }
    }
    public class VariationModel 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<int, string> Variations { get; set; }
    }
}
