using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Yooocan.Entities;
using Yooocan.Entities.Referrals;
using Yooocan.Entities.ServiceProviders;
using Yooocan.Enums;
using Yooocan.Logic.Extensions;
using Yooocan.Models;
using Yooocan.Models.Feeds;
using Yooocan.Models.New;
using Yooocan.Models.New.Home;
using Yooocan.Models.New.Messages;
using Yooocan.Models.New.Stories;
using Yooocan.Models.SearchIndexes;
using Yooocan.Models.ServiceProviders;
using Yooocan.Models.UploadStoryModels;
using Yooocan.Models.Vendors;
using ServiceProviderIndexModel = Yooocan.Models.ServiceProviders.ServiceProviderIndexModel;
using Yooocan.Entities.Blog;
using Yooocan.Logic.AutoMapper;
using System.Reflection;
using Yooocan.Logic.Utils;

namespace Yooocan.Logic
{
    public class AutoMapperInitializer
    {
        private readonly string _imagesCdnPath;
        private readonly string _storagePath;

        public AutoMapperInitializer(string storagePath, string imagesCdnPath)
        {
            _imagesCdnPath = imagesCdnPath;
            _storagePath = storagePath;
        }

        public IMapper InitAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
                         {
                             cfg.AddProfiles(typeof(BenefitProfile).GetTypeInfo().Assembly);

                             ConfigCategory(cfg);
                             ConfigSearch(cfg);
                             ConfigFeed(cfg);
                             ConfigFileUpload(cfg);
                             ConfigProduct(cfg);
                             ConfigShop(cfg);
                             ConfigStory(cfg);
                             ConfigVendor(cfg);
                             ConfigServiceProvider(cfg);
                             ConfigMessage(cfg);
                             ConfigBlog(cfg);

                             cfg.CreateMap<Category, CategoryModel>();
                             cfg.CreateMap<Product, Models.ProductModel>()
                                 .ForMember(x => x.VendorName, o => o.MapFrom(s => s.Vendor.Name))
                                 .ForMember(x => x.Colors, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Colors) ? new List<string>() : s.Colors.Split(new[] { ",", ".", ";" }, StringSplitOptions.RemoveEmptyEntries).ToList()))
                                 .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images
                                     .Where(pi => pi.Type == ImageType.Primary && !pi.IsDeleted)
                                     .Select(pi => pi.CdnUrl ?? pi.Url)
                                     .FirstOrDefault()))
                                 .ForMember(x => x.Images, o => o.MapFrom(s => s.Images
                                     .Where(x => !x.IsDeleted && (x.Type == ImageType.Normal || x.Type == ImageType.Primary))
                                     .OrderBy(x => x.Order)
                                     .Select(pi => pi.CdnUrl ?? pi.Url)
                                     .ToList()))
                                .ForMember(x => x.CertificationImages, o => o.MapFrom(s => s.Images
                                     .Where(x => !x.IsDeleted && x.Type == ImageType.Certification)
                                     .OrderBy(x => x.Order)
                                     .Select(pi => pi.CdnUrl ?? pi.Url)
                                     .ToList()))
                                .ForMember(x => x.BrandLogUrl, o => o.MapFrom(s => s.Images
                                     .Where(x => !x.IsDeleted && x.Type == ImageType.Brand)
                                     .Select(pi => pi.CdnUrl ?? pi.Url)
                                     .FirstOrDefault()))
                                .ForMember(x => x.CategoryId, o => o.MapFrom(s => s.ProductCategories.FirstOrDefault().Category.ParentCategoryId))
                                .ForMember(x => x.CategoryName, o => o.MapFrom(s => s.ProductCategories.FirstOrDefault().Category.ParentCategory.Name))
                                .ForMember(x => x.SubCategoryId, o => o.MapFrom(s => s.ProductCategories.FirstOrDefault().CategoryId))
                                .ForMember(x => x.SubCategoryName, o => o.MapFrom(s => s.ProductCategories.FirstOrDefault().Category.Name));

                             cfg.CreateMap<Product, ProductListModel>()
                                 .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s =>
                                     s.Images.Where(pi => pi.Type == ImageType.Primary && !pi.IsDeleted)
                                         .Select(pi => pi.Url)
                                         .SingleOrDefault()))
                                 .ForMember(x => x.Images, o => o.MapFrom(s => string.Join(",", s.Images
                                     .Where(pi => pi.Type == ImageType.Normal && !pi.IsDeleted)
                                     .OrderBy(x => x.Order)
                                     .Select(pi => pi.Url)
                                     .ToList())))
                                 .ForMember(x => x.VendorName, o => o.MapFrom(s => s.Vendor.Name));


                             cfg.CreateMap<StoryComment, StoryCommentModel>()
                                 .ForMember(x => x.AuthorName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName))
                                 .ForMember(x => x.AuthorId, o => o.MapFrom(s => s.User.Id))
                                 .ForMember(x => x.AuthorPictureUrl, o => o.MapFrom(s => s.User.PictureUrl));


                             cfg.CreateMap<Story, PublishStoryModel>()
                                 .ForMember(x => x.AuthorName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName));



                             cfg.CreateMap<Story, UserStoryModel>()
                                 .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images
                                     .Where(x => !x.IsDeleted)
                                     .Select(x => x.CdnUrl ?? x.Url)
                                     .FirstOrDefault()))
                                 .ForMember(x => x.PublishDate, o => o.MapFrom(s => s.InsertDate.ToString("MM.dd.yyyy")))
                                 .ForMember(x => x.Text, o => o.ResolveUsing(s =>
                                 {
                                     var paragraph = s.Paragraphs.FirstOrDefault();
                                     if (paragraph != null)
                                     {
                                         return $"{paragraph.Title} - {paragraph.Text}";
                                     }
                                     return "";
                                 }));

                             cfg.CreateMap<Story, StoryCardModel>()
                                 .ForMember(x => x.CategoryColor, o => o.MapFrom(s => s.StoryCategories.FirstOrDefault(sc => sc.IsPrimary).Category.ParentCategory.ShopBackgroundColor))
                                 .ForMember(x => x.CategoryName, o => o.MapFrom(s => s.StoryCategories.FirstOrDefault(sc => sc.IsPrimary).Category.ParentCategory.Name))
                                 .ForMember(x => x.CategoryId, o => o.MapFrom(s => s.StoryCategories.FirstOrDefault(sc => sc.IsPrimary).Category.ParentCategoryId))
                                 .ForMember(x => x.LimitationName, o => o.MapFrom(s => s.StoryLimitations.OrderByDescending(sc => sc.IsPrimary).FirstOrDefault().Limitation.Name))
                                 .ForMember(x => x.AuthorName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName))
                                 .ForMember(x => x.Content, o => o.MapFrom(s => s.Paragraphs == null || s.Paragraphs.All(p => p.IsDeleted) ? null : s.Paragraphs.Where(p => !p.IsDeleted).OrderBy(p => p.Order).FirstOrDefault().Title +" " + s.Paragraphs.Where(p => !p.IsDeleted).OrderBy(p => p.Order).FirstOrDefault().Text.StripHtml().Truncate(150)))
                                 .ForMember(x => x.AuthorLocation, o => o.MapFrom(s => !string.IsNullOrEmpty(s.User.City) && !string.IsNullOrEmpty(s.User.Country)
                                     ? s.User.Country == "United States"
                                         ? $"{s.User.City} {s.User.State}, USA"
                                         : $"{s.User.City}, {s.User.Country}"
                                     : s.User.Location))
                                 .ForMember(x => x.AuthorPictureUrl, o => o.MapFrom(s => s.User.PictureUrl))
                                 .ForMember(x => x.CountryCode, o => o.MapFrom(s => CountryHelper.ConvertToCountryCode(s.Country)))
                                 .ForMember(x => x.CommentsCount, o => o.MapFrom(s => s.Comments.Count))
                                 .ForMember(x => x.ImageUrl, o => o.MapFrom(s => s.Images
                                     .Where(x => !x.IsDeleted)
                                     .OrderBy(x => x.Type == ImageType.Header)
                                     .ThenBy(x => x.Order)
                                     .Select(x => x.CdnUrl ?? x.Url)
                                     .FirstOrDefault()))
                                 .ForMember(x => x.HotAreaLeft, o => o.ResolveUsing(s => s.Images
                                     .Where(x => !x.IsDeleted)
                                     .OrderBy(x => x.Type == ImageType.Header)
                                     .ThenBy(x => x.Order)
                                     .Select(x => x.HotAreaLeft)
                                     .FirstOrDefault()))
                                 .ForMember(x => x.HotAreaTop, o => o.ResolveUsing(s => s.Images
                                     .Where(x => !x.IsDeleted)
                                     .OrderBy(x => x.Type == ImageType.Header)
                                     .ThenBy(x => x.Order)
                                     .Select(x => x.HotAreaTop)
                                     .FirstOrDefault()));

                             cfg.CreateMap<Story, FeaturedStoryHeader>()
                                 .ForMember(x => x.CategoryColor,
                                     o => o.MapFrom(s => s.StoryCategories.First(sc => sc.IsPrimary).Category.ParentCategory.ShopBackgroundColor))
                                 .ForMember(x => x.Category, o => o.MapFrom(s => s.StoryCategories.First(sc => sc.IsPrimary).Category.ParentCategory.Name))
                                 .ForMember(x => x.CategoryId, o => o.MapFrom(s => s.StoryCategories.First(sc => sc.IsPrimary).Category.ParentCategoryId))
                                 .ForMember(x => x.Author, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName))
                                 .ForMember(x => x.CountryCode, o => o.MapFrom(s => CountryHelper.ConvertToCountryCode(s.Country)))
                                 .ForMember(x => x.ImageUrl, o => o.ResolveUsing(s => s.Images
                                     .Where(x => !x.IsDeleted)
                                     .OrderBy(x => x.Type == ImageType.Header)
                                     .ThenBy(x => x.Order)
                                     .Select(x => x.CdnUrl ?? x.Url)
                                     .FirstOrDefault()))
                                .ForMember(x => x.HotAreaLeft, o => o.ResolveUsing(s => s.Images
                                     .Where(x => !x.IsDeleted)
                                     .OrderBy(x => x.Type == ImageType.Header)
                                     .ThenBy(x => x.Order)
                                     .Select(x => x.HotAreaLeft)
                                     .FirstOrDefault()))
                                .ForMember(x => x.HotAreaTop, o => o.ResolveUsing(s => s.Images
                                     .Where(x => !x.IsDeleted)
                                     .OrderBy(x => x.Type == ImageType.Header)
                                     .ThenBy(x => x.Order)
                                     .Select(x => x.HotAreaTop)
                                     .FirstOrDefault()));

                             cfg.CreateMap<Vendor, VendorListModel>()
                                .ForMember(x => x.HasSignedUp, o => o.MapFrom(x => x.OnBoardingDate != null))
                                .ForMember(x => x.HasUploadedProducts, o => o.MapFrom(x => x.Products.Any()));

                             cfg.CreateMap<CreateVendorModel, Vendor>();


                             cfg.CreateMap<Limitation, LimitationListModel>()
                                 .ForMember(x => x.ParentLimitationName, o => o.MapFrom(s => s.ParentLimitation.Name));

                             cfg.CreateMap<Category, CategoryListModel>()
                                 .ForMember(x => x.ParentCategoryName, o => o.MapFrom(s => s.ParentCategory.Name));

                             cfg.CreateMap<CreateCategoryModel, Category>();
                             cfg.CreateMap<ApplicationUser, UserBioModel>();

                             cfg.CreateMap<PrivateMessage, ConversationMessageModel>()
                                .ForMember(x => x.FromUserName, o => o.MapFrom(x => $"{x.FromUser.FirstName} {x.FromUser.LastName}"))
                                .ForMember(x => x.FromUserAvatar, o => o.MapFrom(x => x.FromUser.PictureUrl));

                             cfg.CreateMap<NotificationRecipient, NotificationModel>();
                             //allow list "deep" copy
                             cfg.CreateMap<NotificationModel, NotificationModel>();

                             cfg.CreateMap<ReferralClientData, ProductReferral>();
                             cfg.CreateMap<ReferralClientData, ServiceProviderReferral>();
                             cfg.CreateMap<ReferralClientData, BenefitReferral>();
                         });

            return config.CreateMapper();
        }

        private void ConfigMessage(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PrivateMessageModel, PrivateMessage>().ReverseMap();
        }

        private void ConfigCategory(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Category, FeaturedCategory>()
                .ForMember(x => x.TagColor, o => o.MapFrom(x => x.ShopBackgroundColor));
        }

        private void ConfigVendor(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Product, MyProductsProductModel>()
                .ForMember(x => x.Categories, o => o.MapFrom(s => string.Join("<br>", s.ProductCategories.Select(pc => pc.Category.Name))))
                .ForMember(x => x.Limitations, o => o.MapFrom(s => string.Join("<br>", s.ProductLimitations.Select(pc => pc.Limitation.Name))))
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images
                                                                           .Where(i => i.Type == ImageType.Primary && !i.IsDeleted)
                                                                           .Select(i => i.CdnUrl ?? i.Url)
                                                                           .FirstOrDefault() ?? ""));

            cfg.CreateMap<RegisterVendorModel, Vendor>();
            cfg.CreateMap<RegisterVendorModel, VendorRegistration>();
            cfg.CreateMap<VendorRegistration, RegisterVendorModel>();

            cfg.CreateMap<VendorRegistration, Vendor>()
                .ForMember(x => x.ContactPersonEmail, o => o.MapFrom(x => x.Email))
                .ForMember(x => x.ContactPersonPosition, o => o.MapFrom(x => x.ContactPresonRole))
                .ForMember(x => x.ContactPersonName, o => o.MapFrom(x => x.ContactPresonName))
                .ForMember(x => x.TelephoneNumber, o => o.MapFrom(x => x.PhoneNumber))
                .ForMember(x => x.Id, o => o.Ignore());

            cfg.CreateMap<VendorModel, Vendor>();
            cfg.CreateMap<Vendor, VendorModel>();

            cfg.CreateMap<VendorModel, Vendor>();
            cfg.CreateMap<Vendor, VendorModel>();

        }

        private void ConfigShop(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Product, ShopProductModel>()
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images
                                                                           .Where(i => i.Type == ImageType.Primary && !i.IsDeleted)
                                                                           .Select(i => i.CdnUrl ?? i.Url)
                                                                           .FirstOrDefault() ?? ""));

            cfg.CreateMap<ServiceProvider, ShopServiceProviderModel>()
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images
                                                                        .Where(i => i.IsPrimaryImage && !i.IsDeleted)
                                                                        .Select(i => i.CdnUrl ?? i.Url)
                                                                        .FirstOrDefault() ?? ""));
        }

        private void ConfigFileUpload(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<IFormFile, UploadFileModel>()
                .ForMember(x => x.ContentType, o => o.MapFrom(s => s.ContentType))
                .ForMember(x => x.FileName, o => o.MapFrom(s => s.FileName))
                .ForMember(x => x.Stream, o => o.MapFrom(s => s.OpenReadStream()));
        }

        private void ConfigProduct(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Models.CreateProductModel, Product>()
                .ForMember(x => x.ProductLimitations, o => o.MapFrom(s => s.Limitations.Select(l => new ProductLimitation { LimitationId = l }).ToList()))
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select((url, index) => new ProductImage
                    {
                        Url = url,
                        CdnUrl = url.Replace(_storagePath, _imagesCdnPath),
                        Order = index
                    })))
                .ForMember(x => x.ProductCategories, o => o.MapFrom(s => s.Categories.Select(c => new ProductCategory { CategoryId = c }).ToList()));

            cfg.CreateMap<Product, EditProductModel>()
                .ForMember(x => x.Images, o => o.MapFrom(s =>
                    s.Images.Where(x => !x.IsDeleted && x.Type == ImageType.Normal)
                        .Select(i => i.OriginalUrl ?? i.Url)
                        .ToList()))
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s =>
                    s.Images.Where(x => !x.IsDeleted && x.Type == ImageType.Primary)
                        .Select(i => i.OriginalUrl ?? i.Url)
                        .SingleOrDefault()));

            cfg.CreateMap<VendorUploadProductModel, Product>()
                .ForMember(x => x.ProductLimitations, o => o.MapFrom(s => s.Limitations.Select(l => new ProductLimitation { LimitationId = l }).ToList()))
                .ForMember(x => x.ProductCategories, o => o.MapFrom(s => s.Categories.Select(c => new ProductCategory { CategoryId = c }).ToList()))
                .ForMember(x => x.Images, o => o.ResolveUsing(s =>
                {
                    var images = new List<ProductImage>();
                    images.AddRange(s.Images
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Select((x, index) => new ProductImage
                        {
                            Url = x,
                            CdnUrl = x.Replace(_storagePath, _imagesCdnPath),
                            Order = index,
                            Type = index == 0 ? ImageType.Primary : ImageType.Normal,
                        }));

                    images.AddRange(s.CertificationImages
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Select((x, index) => new ProductImage
                        {
                            Url = x,
                            CdnUrl = x.Replace(_storagePath, _imagesCdnPath),
                            Order = index,
                            Type = ImageType.Certification
                        }));
                    if (!string.IsNullOrWhiteSpace(s.BrandLogoUrl))
                    {
                        images.Add(new ProductImage
                        {
                            Type = ImageType.Brand,
                            Url = s.BrandLogoUrl,
                            CdnUrl = s.BrandLogoUrl.Replace(_storagePath, _imagesCdnPath),
                        });
                    }

                    return images;
                })).ReverseMap()
                .ForMember(x => x.ProductId, o => o.MapFrom(s => s.Id))
                .ForMember(x => x.Limitations, o => o.MapFrom(s => s.ProductLimitations.Select(l => l.LimitationId).ToList()))
                .ForMember(x => x.Categories, o => o.MapFrom(s => s.ProductCategories.Select(c => c.CategoryId).ToList()))
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images.Where(x => !x.IsDeleted && (x.Type == ImageType.Normal || x.Type == ImageType.Primary)).OrderBy(x => x.Order).Select(x => x.Url).ToList()))
                .ForMember(x => x.CertificationImages, o => o.MapFrom(s => s.Images.Where(x => !x.IsDeleted && (x.Type == ImageType.Certification)).OrderBy(x => x.Order).Select(x => x.Url).ToList()))
                .ForMember(x => x.BrandLogoUrl, o => o.MapFrom(s => s.Images.Where(x => !x.IsDeleted && x.Type == ImageType.Brand).Select(x => x.Url).SingleOrDefault()));

            cfg.CreateMap<EditProductModel, Product>()
                .ForMember(x => x.ProductLimitations, o => o.MapFrom(
                            s => s.Limitations.Select(l => new ProductLimitation { LimitationId = l }).ToList()))
                .ForMember(x => x.Images, o => o.ResolveUsing(s =>
                {
                    var images = new List<ProductImage>();
                    if (s.PrimaryImageUrl != null &&
                        s.PrimaryImageUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                    {
                        images.Add(new ProductImage
                        {
                            Order = 0,
                            OriginalUrl = s.PrimaryImageUrl
                        });
                    }
                    if (s.Images != null)
                        images.AddRange(
                            s.Images.Where(x => x != null &&
                                                x.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                                .Select((p, index) => new ProductImage
                                {
                                    OriginalUrl = p,
                                    Order = index + 1
                                })
                                .ToList());
                    return images;
                }))
                .ForMember(x => x.ProductCategories,
                    o => o.MapFrom(s => s.Categories.Select(c => new ProductCategory { CategoryId = c }).ToList()));
        }

        private void ConfigStory(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<StoryParagraph, StoryParagraphModel>().ReverseMap();

            cfg.CreateMap<UploadStoryModel, Story>()
                .ForMember(x => x.Images, o => o.ResolveUsing(s =>
                {
                    var images = s.Images.Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select((image, index) => new StoryImage
                        {
                            Url = image,
                            CdnUrl = image.Replace(_storagePath, _imagesCdnPath),
                            Order = index,
                            Type = index == 0 ? ImageType.Primary : ImageType.Normal,
                        })
                                                  .ToList();
                    if (!string.IsNullOrEmpty(s.HeaderImageUrl))
                    {
                        images.Add(new StoryImage
                        {
                            Url = s.HeaderImageUrl,
                            CdnUrl = s.HeaderImageUrl.Replace(_storagePath, _imagesCdnPath),
                            Order = 0,
                            Type = ImageType.Header
                        });
                    }
                    return images;
                }))
                .ForMember(x => x.Paragraphs, o => o.MapFrom(s => s.Paragraphs.Select((x, index) =>
                    new StoryParagraph
                    {
                        Text = x.Text,
                        Title = x.Title,
                        Order = index,
                    }).ToList()))
                .ForMember(x => x.StoryCategories, o => o.ResolveUsing(s => s.Categories.Select(categoryId => new StoryCategory { CategoryId = categoryId, IsPrimary = s.PrimaryCategoryId == categoryId })))
                .ForMember(x => x.StoryLimitations, o => o.ResolveUsing(s => s.Limitations?.Select(limitationId => new StoryLimitation { LimitationId = limitationId, IsPrimary = s.PrimaryLimitationId == limitationId })));

            cfg.CreateMap<Story, UploadStoryModel>()
                .ForMember(x => x.Images,
                    o => o.MapFrom(s => s.Images
                        .Where(x => !x.IsDeleted && (x.Type == ImageType.Normal || x.Type == ImageType.Primary))
                        .OrderBy(x => x.Order)
                        .Select(x => x.Url).ToList()))
                .ForMember(x => x.HeaderImageUrl, o => o.MapFrom(s => s.Images
                    .Where(x => !x.IsDeleted && x.Type == ImageType.Header)
                    .Select(x => x.Url)
                    .FirstOrDefault()))
                .ForMember(x => x.Paragraphs, o => o.MapFrom(s => s.Paragraphs.Where(x => !x.IsDeleted).OrderBy(x=> x.Order)))
                .ForMember(x => x.Categories, o => o.MapFrom(s => s.StoryCategories.Select(x => x.CategoryId).ToList()))
                .ForMember(x => x.PrimaryCategoryId, o => o.MapFrom(s => s.StoryCategories.Where(x => x.IsPrimary).Select(x => x.CategoryId).FirstOrDefault()))
                .ForMember(x => x.PrimaryLimitationId, o => o.MapFrom(s => s.StoryLimitations.Where(x => x.IsPrimary).Select(x => x.LimitationId).FirstOrDefault()))
                .ForMember(x => x.Limitations, o => o.MapFrom(s => s.StoryLimitations.Select(x => x.LimitationId).ToList()));

            cfg.CreateMap<InternalCreateStoryModel, UploadStoryModel>();
            cfg.CreateMap<Story, StoryIndexModel>()
                .ForMember(x => x.AuthorName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName));

            cfg.CreateMap<Story, StoryModel>()
                .ForMember(x => x.CategoryColor, o => o.MapFrom(s => s.StoryCategories.FirstOrDefault(sc => sc.IsPrimary).Category.ParentCategory.ShopBackgroundColor))
                .ForMember(x => x.CategoryName, o => o.MapFrom(s => s.StoryCategories.FirstOrDefault(sc => sc.IsPrimary).Category.ParentCategory.Name))
                .ForMember(x => x.LimitationName, o => o.MapFrom(s => s.StoryLimitations.OrderBy(sc => !sc.IsPrimary).FirstOrDefault().Limitation.Name))
                .ForMember(x => x.HeaderImageUrl, o => o.MapFrom(s => s.Images
                                                                          .Where(x => x.Type == ImageType.Header && !x.IsDeleted)
                                                                          .Select(x => x.CdnUrl)
                                                                          .FirstOrDefault() ??
                                                                      s.StoryCategories.FirstOrDefault(sc => sc.IsPrimary).Category.ParentCategory.HeaderPictureUrl))
                .ForMember(x => x.MobileHeaderImageUrl, o => o.MapFrom(s => s.Images
                                                                          .Where(x => x.Type == ImageType.Header && !x.IsDeleted)
                                                                          .Select(x => x.CdnUrl)
                                                                          .FirstOrDefault() ??
                                                                      s.StoryCategories.FirstOrDefault(sc => sc.IsPrimary).Category.ParentCategory.MobileHeaderPictureUrl ??
                                                                      s.StoryCategories.FirstOrDefault(sc => sc.IsPrimary).Category.ParentCategory.HeaderPictureUrl))
                .ForMember(x => x.AuthorName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName))
                .ForMember(x => x.AuthorId, o => o.MapFrom(s => s.UserId))
                .ForMember(x => x.SubCategories, o => o.MapFrom(s => s.StoryCategories.Select(sc => new Tuple<int, string>(sc.CategoryId, sc.Category.Name)).ToList()))
                .ForMember(x => x.AuthorLocation, o => o.MapFrom(s => !string.IsNullOrEmpty(s.User.City) && !string.IsNullOrEmpty(s.User.Country)
                    ? s.User.Country == "United States"
                        ? $"{s.User.City} {s.User.State}, USA"
                        : $"{s.User.City}, {s.User.Country}"
                    : s.User.Location))
                .ForMember(x => x.AuthorProfileUrl, o => o.MapFrom(s => s.User.PictureUrl))
                .ForMember(x => x.Comments,
                    o => o.MapFrom(s => s.Comments.Where(x => !x.IsDeleted).OrderByDescending(x => x.InsertDate)))
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Order)
                    .Select(x => x.CdnUrl ?? x.Url)
                    .FirstOrDefault()))
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images.Where(x => !x.IsDeleted && (x.Type == ImageType.Normal || x.Type == ImageType.Primary)).OrderBy(x => x.Order).Select(x => x.CdnUrl ?? x.Url)))
                .ForMember(x => x.AuthorAboutMe, o => o.MapFrom(s => s.User.AboutMe))
                .ForMember(x => x.Paragraphs, o => o.MapFrom(s => s.Paragraphs.Where(x => !x.IsDeleted).OrderBy(x => x.Order)));

            cfg.CreateMap<UploadStoryModel, PreviewStoryModel>()
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images.FirstOrDefault()))
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images.Where(image => !string.IsNullOrWhiteSpace(image))));
        }

        private void ConfigBlog(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Post, StoryModel>()
                .ForMember(x => x.IsBlogPost, o => o.UseValue(true))
                .ForMember(x => x.CategoryColor, o => o.UseValue("d94840"))
                .ForMember(x => x.CategoryName, o => o.UseValue("yoocan blog"))
                .ForMember(x => x.HeaderImageUrl, o => o.MapFrom(s => s.Images
                                                                          .Where(x => x.Type == ImageType.Header)
                                                                          .Select(x => x.Url)
                                                                          .FirstOrDefault()))
                .ForMember(x => x.AuthorName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName))
                .ForMember(x => x.AuthorId, o => o.MapFrom(s => s.UserId))
                //.ForMember(x => x.AuthorLocation, o => o.MapFrom(s => !string.IsNullOrEmpty(s.User.City) && !string.IsNullOrEmpty(s.User.Country)
                //    ? s.User.Country == "United States"
                //        ? $"{s.User.City} {s.User.State}, USA"
                //        : $"{s.User.City}, {s.User.Country}"
                //    : s.User.Location))
                .ForMember(x => x.AuthorProfileUrl, o => o.MapFrom(s => s.User.PictureUrl))
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images
                    .OrderBy(x => x.Order)
                    .Select(x => x.Url)
                    .FirstOrDefault()))
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images.OrderBy(x => x.Order).Select(x => x.Url)))
                .ForMember(x => x.AuthorAboutMe, o => o.MapFrom(s => s.User.AboutMe))
                .ForMember(x => x.Paragraphs, o => o.MapFrom(s => new List<StoryParagraphModel> { new StoryParagraphModel { Text = s.Content } }));

            cfg.CreateMap<Post, StoryCardModel>()
                .ForMember(x => x.CategoryColor, o => o.UseValue("d94840"))
                .ForMember(x => x.CategoryName, o => o.UseValue("yoocan blog"))
                .ForMember(x => x.IsBlogPost, o => o.UseValue(true))
                .ForMember(x => x.Content, o => o.MapFrom(s => s.Content.StripHtml().Truncate(150)))
                .ForMember(x => x.ImageUrl, o => o.MapFrom(s => s.Images
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Type == ImageType.Header)
                    .ThenBy(x => x.Order)
                    .Select(x => x.Url)
                    .FirstOrDefault()));
        }

        private void ConfigFeed(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Feed, StoryCardModel>()
                .ForMember(x => x.AuthorName, o => o.MapFrom(s => s.FirstName + " " + s.LastName))
                .ForMember(x => x.AuthorLocation, o => o.MapFrom(s => s.Location))
                .ForMember(x => x.AuthorPictureUrl, o => o.MapFrom(s => s.UserImageUrl))
                .ForMember(x => x.ImageUrl, o => o.MapFrom(s => s.PrimaryImageUrl));

            cfg.CreateMap<Category, FeedCategoriesModel>();
        }

        private void ConfigSearch(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<SearchCategoryModel, Category>();

            cfg.CreateMap<Product, SearchProductModel>()
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images.Where(x => x.Type == ImageType.Primary && !x.IsDeleted).Select(x => x.CdnUrl ?? x.Url).FirstOrDefault()))
                .ForMember(x => x.VendorName, o => o.MapFrom(s => s.Vendor.Name))
                .ForMember(x => x.About,
                    o => o.MapFrom(s => s.About.Length <= 100 ? s.About : s.About.Substring(0, 100) + "..."));

            cfg.CreateMap<ServiceProvider, SearchServiceProviderModel>()
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images.Where(x => !x.IsDeleted).Select(x => x.CdnUrl ?? x.Url).FirstOrDefault()))
                .ForMember(x => x.AboutTheCompany, o => o.MapFrom(s => s.AboutTheCompany.Length <= 100 ? s.AboutTheCompany : s.AboutTheCompany.Substring(0, 100) + "..."));

            cfg.CreateMap<Story, SearchStoryModel>()
                .ForMember(x => x.AuthorName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName))
                .ForMember(x => x.AuthorLocation, o => o.MapFrom(s => !string.IsNullOrEmpty(s.User.City) && !string.IsNullOrEmpty(s.User.Country)
                    ? s.User.Country == "United States"
                        ? $"{s.User.City} {s.User.State}, USA"
                        : $"{s.User.City}, {s.User.Country}"
                    : s.User.Location))
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images
                        .Where(x => !x.IsDeleted)
                        .OrderBy(x => x.Order)
                        .Select(x => x.CdnUrl ?? x.Url)
                        .FirstOrDefault()))
                .ForMember(x => x.PublishDate, o => o.MapFrom(s => s.InsertDate.ToString("MM.dd.yyyy")));
            cfg.CreateMap<SearchResult, SearchResultModel>();
        }

        private void ConfigServiceProvider(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ContactServiceProviderModel, ServiceProviderContactRequest>().ReverseMap();

            cfg.CreateMap<ServiceProvider, RelatedServiceProviderModel>()
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images
                    .Where(image => !image.IsDeleted && image.IsPrimaryImage)
                    .Select(image => image.CdnUrl ?? image.Url)
                    .SingleOrDefault()));

            cfg.CreateMap<ServiceProvider, ServiceProviderAllModel>();

            cfg.CreateMap<CreateServiceProviderActivityModel, ServiceProviderActivity>().ReverseMap();

            cfg.CreateMap<ServiceProvider, CreateServiceProviderModel>()
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images
                    .Where(image => !image.IsDeleted)
                    .OrderBy(x => x.Order)
                    .Select(image => image.CdnUrl ?? image.Url)
                    .ToList()))
                .ForMember(x => x.YouTubeIds, o => o.MapFrom(s => s.Videos
                    .Where(video => !video.IsDeleted)
                    .OrderBy(x => x.Order)
                    .Select(video => video.YouTubeId)
                    .ToList()))
                .ForMember(x => x.Categories, o => o.MapFrom(s => s.ServiceProviderCategories
                    .Where(category => !category.IsDeleted)
                    .Select(category => category.CategoryId)
                    .ToList()))
                .ForMember(x => x.MainCategoryNamesToIds, o => o.MapFrom(s => s.ServiceProviderCategories
                    .Where(category => !category.IsDeleted && category.Category.ParentCategoryId == null)
                    .ToDictionary(category => category.Category.Name, category => category.CategoryId)))
                .ForMember(x => x.Limitations, o => o.MapFrom(s => s.ServiceProviderLimitations
                    .Where(limitation => !limitation.IsDeleted)
                    .Select(limitation => limitation.LimitationId)
                    .ToList()));

            cfg.CreateMap<CreateServiceProviderModel, ServiceProvider>()
                .ForMember(x => x.ServiceProviderLimitations, o => o.MapFrom(s => s.Limitations.Select(x => new ServiceProviderLimitation { LimitationId = x })))
                .ForMember(x => x.ServiceProviderCategories, o => o.MapFrom(s => s.Categories.Select(x => new ServiceProviderCategory { CategoryId = x })))
                .ForMember(x => x.Activities, o => o.MapFrom(s => s.Activities.Select((a, index) =>
                    new ServiceProviderActivity
                    {
                        Name = a.Name,
                        Description = a.Description,
                        OpenDays = a.OpenDays,
                        Units = a.Units,
                        Price = a.Price ?? 0,
                        Order = index
                    })))
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images
                    .Where(image => !string.IsNullOrEmpty(image))
                    .Select((image, index) => new ServiceProviderImage
                    {
                        Url = image,
                        CdnUrl = image.Replace(_storagePath, _imagesCdnPath),
                        IsPrimaryImage = index == 0,
                        Order = index
                    })))
                .ForMember(x => x.Videos, o => o.MapFrom(s => s.YouTubeIds
                    .Where(youtubeId => !string.IsNullOrEmpty(youtubeId))
                    .Select((youtubeId, index) => new ServiceProviderVideo
                    {
                        YouTubeId = youtubeId,
                        IsPrimaryVideo = index == 0,
                        Order = index
                    })));

            cfg.CreateMap<ServiceProvider, ServiceProviderIndexModel>()
                .ForMember(x => x.YouTubeIds, o => o.MapFrom(s => s.Videos
                    .Where(video => !video.IsDeleted)
                    .OrderBy(video => video.Order)
                    .Select(video => video.YouTubeId)))
                .ForMember(x => x.PrimaryYouTubeId, o => o.MapFrom(s => s.Videos
                    .Where(video => !video.IsDeleted && video.IsPrimaryVideo)
                    .Select(image => image.YouTubeId)
                    .SingleOrDefault()))
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images
                    .Where(image => !image.IsDeleted)
                    .OrderBy(image => image.Order)
                    .Select(image => image.CdnUrl ?? image.Url)))
                    .ForMember(x => x.Activities, o => o.MapFrom(s => s.Activities.OrderBy(a => a.Order).ToList()))
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images
                    .Where(image => !image.IsDeleted && image.IsPrimaryImage)
                    .Select(image => image.CdnUrl ?? image.Url)
                    .SingleOrDefault()))
                .ForMember(x => x.HeaderImageUrl, o => o.MapFrom(s => s.HeaderImageUrl ?? s.ServiceProviderCategories.FirstOrDefault().Category.HeaderPictureUrl))
                .ForMember(x => x.MobileHeaderImageUrl, o => o.MapFrom(s => s.HeaderImageUrl ??
                                    s.ServiceProviderCategories.FirstOrDefault().Category.MobileHeaderPictureUrl ??
                                    s.ServiceProviderCategories.FirstOrDefault().Category.HeaderPictureUrl));

            cfg.CreateMap<CreateServiceProviderModel, ServiceProviderIndexModel>()
                .ForMember(x => x.IsPreview, o => o.UseValue(true))
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images.Where(image => !string.IsNullOrWhiteSpace(image))))
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(s => s.Images.FirstOrDefault(image => !string.IsNullOrWhiteSpace(image))))
                .ForMember(x => x.YouTubeIds, o => o.MapFrom(s => s.YouTubeIds.Where(id => !string.IsNullOrWhiteSpace(id))));
        }
    }
}