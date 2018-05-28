using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using Serilog;
using Yooocan.Entities;
using Yooocan.Enums;
using Yooocan.Logic.Extensions;
using Yooocan.Models.Products;

namespace Yooocan.Logic.AutoMapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductModel>()
                .BeforeMap((src, dst) =>
                {
                    var cardModel = InitDiscounts(src);
                    dst.PriceAfterDiscount = cardModel.PriceAfterDiscount;
                    dst.DiscountAbsolute = cardModel.DiscountAbsolute;
                    dst.DiscountPercentage = cardModel.DiscountPercentage;
                })
                .ForMember(x => x.MainImageUrl, o => o.MapFrom(s => s.Images
                    .Where(i => i.Type == ImageType.Primary)
                    .Select(i => i.CdnUrl)
                    .SingleOrDefault()))
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images
                    .Where(i => i.Type == ImageType.Normal)
                    .OrderBy(i => i.Order)
                    .Select(i => i.CdnUrl)
                    .ToList()))
                .ForMember(x => x.LogoUrl, o => o.MapFrom(s => s.Company.Images
                    .Where(i => i.Type == AltoImageType.Logo)
                    .Select(i => i.CdnUrl)
                    .FirstOrDefault()))
                .ForMember(x => x.Category,
                    o => o.ResolveUsing(s =>
                    {
                        var mainCategory = s.ProductCategories.OrderByDescending(x => x.IsMain).FirstOrDefault()?.Category;
                        return mainCategory == null
                            ? (KeyValuePair<int, string>?)null
                            : new KeyValuePair<int, string>(mainCategory.Id, mainCategory.Name);
                    }))
                .ForMember(x => x.ParentCategory,
                    o => o.ResolveUsing(s =>
                    {
                        var mainCategory = s.ProductCategories.OrderByDescending(x => x.IsMain).FirstOrDefault()?.Category?.ParentCategory;
                        return mainCategory == null
                            ? (KeyValuePair<int, string>?)null
                            : new KeyValuePair<int, string>(mainCategory.Id, mainCategory.Name);
                    }))
                .ForMember(x => x.YouTubeId, o => o.MapFrom(s => s.YouTubeId))
                .ForMember(x => x.CompanyName, o => o.MapFrom(s => s.Company.Name))
                .ForMember(x => x.Discount, o => o.MapFrom(s => s.DiscountRate ?? s.Company.MembersDiscountRate ?? 0))
                .ForMember(x => x.DiscountType, o => o.MapFrom(s => s.DiscountRate != null ? s.DiscountType ?? RateType.Percentage :
                                                                                         s.Company.DiscountRateType ?? RateType.Percentage))
                .ForMember(x => x.Url, o => o.MapFrom(s => s.Company.ReferrerFormat != null
                    ? string.Format(s.Company.ReferrerFormat, WebUtility.UrlEncode(s.Url))
                    : s.Url))
                .ForMember(x => x.ShippingPrice, o => o.MapFrom(s => 
                    s.Company.ShippingRules.FirstOrDefault(r => r.MaxProductPrice >= s.Price && r.MinProductPrice <= s.Price).ShippingPrice))
                //.ForMember(x => x.Variations, o => o.ResolveUsing(s =>
                //{
                //    return s.VariationValues
                //        .Select(x => x.Variation)
                //        .Distinct()
                //        .Select(x => new VariationModel
                //        {
                //            Id = x.Id,
                //            Name = x.Name,
                //            Variations = s.VariationValues.Where(vv => vv.VariationId == x.Id)
                //                             .OrderBy(vv => vv.Value, new StringNumericComparer())
                //                             .ToDictionary(vv => vv.Id, vv => vv.Value)
                //        });
                //}))
                ;

            CreateMap<Product, ProductCardModel>()
                .BeforeMap((src, dst) =>
                {
                    var cardModel = InitDiscounts(src);
                    dst.PriceAfterDiscount = cardModel.PriceAfterDiscount;
                    dst.DiscountAbsolute = cardModel.DiscountAbsolute;
                    dst.DiscountPercentage = cardModel.DiscountPercentage;
                })
                .ForMember(x => x.CategoryColor, o => o.MapFrom(s => s.ProductCategories.Count == 0 ? null : s.ProductCategories.First().Category.ParentCategory.ShopBackgroundColor))
                .ForMember(x => x.CategoryName, o => o.MapFrom(s => s.ProductCategories.Count == 0 ? null : s.ProductCategories.First().Category.ParentCategory.Name))
                .ForMember(x => x.SubCategoryName, o => o.MapFrom(s => s.ProductCategories.Count == 0 ? "" : s.ProductCategories.First().Category.Name))
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images
                        .Where(x => x.Type == ImageType.Primary && !x.IsDeleted)
                        .Select(x => x.CdnUrl ?? x.Url)
                        .FirstOrDefault()))
                .ForMember(x => x.ProductPageUrl, o => o.MapFrom(s => !string.IsNullOrEmpty(s.AmazonId) ?
                    $"https://www.amazon.com/exec/obidos/ASIN/{s.AmazonId}/yoocan-20" : $"/Product/{s.Id}/{s.Name.ToCanonical()}"));

            CreateMap<Product, ProductAllModel>()
                .ForMember(x => x.CompanyName, o => o.MapFrom(s => s.Company.Name))
                .ForMember(x => x.Categories, o => o.MapFrom(s => string.Join(", ", s.ProductCategories.Select(x => x.Category.Name).ToList())))
                .ForMember(x => x.CompanyName, o => o.MapFrom(s => s.Company.Name));

            CreateMap<Product, CreateProductModel>()
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images
                    .Where(i => i.Type == ImageType.Normal || i.Type == ImageType.Primary)
                    .OrderBy(i => i.Order)
                    .Select(i => i.CdnUrl ?? i.Url).ToList()))
                .ForMember(x => x.MainCategoryId, o => o.MapFrom(s => s.ProductCategories.Where(x => x.IsMain).Select(x => x.CategoryId).FirstOrDefault()))
                .ForMember(x => x.Categories, o => o.MapFrom(s => s.ProductCategories.Select(x => x.CategoryId)))
                .ForMember(x => x.Limitations, o => o.MapFrom(s => s.ProductLimitations.Select(x => x.LimitationId)));

            CreateMap<CreateProductModel, Product>()
                .ForMember(x => x.Images, o => o.ResolveUsing(s =>
                {
                    var images = s.Images
                        .Where(image => !string.IsNullOrWhiteSpace(image))
                        .Select((image, index) => new ProductImage
                        {
                            Order = index,
                            Type = index == 0 ? ImageType.Primary : ImageType.Normal,
                            Url = image,
                            CdnUrl = image
                        }).ToList();
                    if (!string.IsNullOrWhiteSpace(s.BrandLogoUrl))
                    {
                        images.Add(new ProductImage
                        {
                            Url = s.BrandLogoUrl,
                            CdnUrl = s.BrandLogoUrl,
                            Order = s.Images.Count,
                            Type = ImageType.Brand
                        });
                    }
                    return images;
                }))
                .ForMember(x => x.ProductCategories, o => o.MapFrom(s => s.Categories.Select(x => new ProductCategory
                {
                    CategoryId = x,
                    IsMain = s.MainCategoryId == x
                })))
                .ForMember(x => x.ProductLimitations, o => o.MapFrom(s => s.Limitations.Select(x => new ProductLimitation { LimitationId = x })));
        }

        private static ProductCardModel InitDiscounts(Product product)
        {
            var result = new ProductCardModel
            {
                PriceAfterDiscount = product.Price,
                DiscountAbsolute = 0,
                DiscountPercentage = 0
            };
            if (product.Company == null)
            {
                Log.Logger.Error("Product: {productId} was queried while not having a company", product.Id);
                return result;
            }

            var discount = product.DiscountRate ?? product.Company.MembersDiscountRate ?? 0;
            var discountType = product.DiscountType ?? product.Company.DiscountRateType;            

            if (discount > 0)
            {
                if (discountType == RateType.Absolute)
                {
                    result.DiscountAbsolute = discount;
                    if (product.Price > 0)
                    {
                        result.PriceAfterDiscount = product.Price - discount;
                        result.DiscountPercentage = discount / product.Price * 100;
                    }
                }
                else
                {
                    result.PriceAfterDiscount = product.Price * (1 - discount / 100);
                    result.DiscountPercentage = discount;
                    result.DiscountAbsolute = product.Price - result.PriceAfterDiscount;
                }
            }

            return result;
        }
    }
}