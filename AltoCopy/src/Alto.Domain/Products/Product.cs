using System;
using System.Collections.Generic;
using Alto.Domain.Companies;
using Alto.Enums;

namespace Alto.Domain.Products
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public decimal Price { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? Discount { get; set; }
        public RateType? DiscountType { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Company Company { get; set; }
        public int CompanyId { get; set; }
        public string Url { get; set; }
        public string YouTubeId { get; set; }
        public string VideoUrl { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public float? Depth { get; set; }
        public string Colors { get; set; }
        public string WarrantyUrl { get; set; }
        public string Brand { get; set; }
        public string Upc { get; set; }
        public string Sku { get; set; }
        public bool IsSoldOnSite { get; set; }
        public string CouponCode { get; set; }
        public bool CouponNotNeeded { get; set; }
        public string RedirectPageComment { get; set; }
        public ProductVariationCombination VariationCombination { get; set; }
        public ProductShipping Shipping { get; set; }
        public List<ProductVariationValue> VariationValues { get; set; }
        public List<ProductImage> Images { get; set; }
        public List<ProductReview> Reviews { get; set; }
        public List<ProductLimitation> ProductLimitations { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }

        public Product()
        {
            Images = new List<ProductImage>();
            Reviews = new List<ProductReview>();
            ProductLimitations = new List<ProductLimitation>();
            ProductCategories = new List<ProductCategory>();
            VariationValues = new List<ProductVariationValue>();
        }
    }
}