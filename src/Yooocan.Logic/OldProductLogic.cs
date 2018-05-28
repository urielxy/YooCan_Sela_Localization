using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Models;
using StackExchange.Redis;
using Newtonsoft.Json;
using Yooocan.Enums.Vendors;
using Yooocan.Models.Products;
using Yooocan.Models.Vendors;
using Yooocan.Enums.Products;

namespace Yooocan.Logic
{
    public class OldProductLogic : IOldProductLogic
    {
        private ApplicationDbContext _context;
        private ILogger<OldProductLogic> _logger;
        private IMapper _mapper;
        private SearchServiceClient _searchClient;
        private readonly IBlobUploader _blobUploader;
        private readonly IDatabase _redisDatabase;
        private readonly RedisWrapper _redisWrapper;

        public OldProductLogic(ApplicationDbContext context, ILogger<OldProductLogic> logger, IMapper mapper, SearchServiceClient searchClient, IBlobUploader blobUploader,
                            IDatabase redisDatabase, RedisWrapper redisWrapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _searchClient = searchClient;
            _blobUploader = blobUploader;
            _redisDatabase = redisDatabase;
            _redisWrapper = redisWrapper;
        }

        public async Task<Models.ProductModel> GetProductModelAsync(int productId)
        {
            var cacheKey = GetProductCacheKey(productId);
            var cacheValue = _redisDatabase.StringGet(cacheKey);
            if (cacheValue.HasValue)
                return JsonConvert.DeserializeObject<Models.ProductModel>(cacheValue);

            var product = await _context.Products
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .Include(x => x.Vendor)
                .Include(x => x.Images)
                .SingleOrDefaultAsync(x => x.Id == productId && !x.IsDeleted);

            if (product == null)
                return null;

            var headerUrl = product.ProductCategories.Select(x => x.Category.HeaderPictureUrl).FirstOrDefault();
            var model = _mapper.Map<Models.ProductModel>(product);
            model.HeaderImageUrl = headerUrl;
            var relatedProducts = await GetRelatedProductsByProductIdAsync(productId);
            var relatedProductsModel = _mapper.Map<List<ProductCardModel>>(relatedProducts);
            for (int i = 0; i < relatedProductsModel.Count; i++)
            {
                relatedProductsModel[i].IsNew = i % 3 == 0;
            }
            model.RelatedProducts = relatedProductsModel;

            var storiesFeaturing = _context.StoryProducts
                              .Where(x => x.ProductId == productId && x.IsUsedInStory)
                              .Include(x => x.Story)
                                .ThenInclude(x => x.Images)
                              .Include(x => x.Story)
                                .ThenInclude(x => x.User)
                              .Include(x => x.Story)
                                .ThenInclude(x => x.StoryCategories)
                              .Include(x=> x.Story)
                                .ThenInclude(x => x.Paragraphs)  
                              .OrderByDescending(x => x.StoryId)
                              .Take(4)
                              .ToList()
                              .Select(x => x.Story)
                              .ToList();
            var storiesFeaturingModel = _mapper.Map<List<StoryCardModel>>(storiesFeaturing);
            model.StoriesFeaturing = storiesFeaturingModel;

            cacheValue = JsonConvert.SerializeObject(model);
            _redisDatabase.StringSet(cacheKey, cacheValue, TimeSpan.FromHours(24), flags: CommandFlags.FireAndForget);

            return model;
        }

        public async Task<ProductCardModel> GetProductOfTheDay()
        {
            return await _redisWrapper.GetModelAsync(RedisKeys.ProductOfTheDay, async () =>
            {
                var promotedProductOfTheDay = _context.PromotedProducts.OrderBy(x => x.Order)
                                                                       .FirstOrDefault(x => x.PromotionType == PromotionType.ProductOfTheDay);

                var productOfTheDay = await _context.Products
                                                        .Where(x => !x.IsDeleted && x.IsPublished)   
                                                        .OrderByDescending(x => promotedProductOfTheDay == null || promotedProductOfTheDay.ProductId == x.Id)
                                                        .ThenBy(x => Guid.NewGuid())
                                                        .Include(x => x.Company)
                                                        .AsNoTracking()
                                                        .FirstOrDefaultAsync();

                //for some reason include didn't work - maybe because of the random ordering
                var productImages = await _context.ProductImages.Where(x => x.ProductId == productOfTheDay.Id).ToListAsync();
                productOfTheDay.Images = productImages;

                var model = _mapper.Map<ProductCardModel>(productOfTheDay);
                return model;
            }, TimeSpan.FromDays(1));
        }

        private string GetProductCacheKey(int productId)
        {
            var key = string.Format(RedisKeys.ProductModel, productId);
            return key;
        }

        private void RemoveProductFromCache(int productId)
        {
            _redisDatabase.KeyDelete(GetProductCacheKey(productId), CommandFlags.FireAndForget);
        }

        private async Task<List<Product>> GetRelatedProductsByProductIdAsync(int productId)
        {
            const int maxRelatedProducts = 4;
            var products = new List<Product>();
            var relatedCategories = await _context.ProductCategories.Where(x => x.ProductId == productId).Select(x => x.CategoryId).ToArrayAsync();
            var vendorId = await _context.Products.Where(x => x.Id == productId).Select(x => x.VendorId).SingleAsync();
            products = await _context.Products
                .Where(x => x.IsPublished && x.VendorId == vendorId)
                .Include(x => x.Company)
                .Include(x => x.Images)
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .OrderByDescending(x => x.Id)
                .Take(maxRelatedProducts)
                .ToListAsync();
            if (products.Count < maxRelatedProducts)
            {
                var vendorProductsIds = products.Select(x => x.Id).ToList();
                var categoryProducts = await _context.Products
                    .Where(x => x.IsPublished && x.ProductCategories.Any(cs => relatedCategories.Contains(cs.Category.Id)) &&
                                !vendorProductsIds.Contains(x.Id))
                    .Include(x => x.Company)
                    .Include(x => x.Images)
                    .Include(x => x.ProductCategories)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.ParentCategory)
                    .OrderByDescending(x => x.Id)
                    .Take(maxRelatedProducts - products.Count)
                    .ToListAsync();
                products.AddRange(categoryProducts);
            }
            return products;
        }

        public async Task<Product> UploadProductAsync(Models.CreateProductModel model)
        {
            var product = _mapper.Map<Product>(model);
            //todo: remove the auto approval when vendors will start uploading their products
            product.IsPublished = true;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product> EditProductAsync(VendorUploadProductModel model)
        {
            var dbProduct = _context.Products
                .Include(x => x.Images)
                .Include(x => x.ProductCategories)
                .Include(x => x.ProductLimitations)
                .Single(x => x.Id == model.ProductId);

            foreach (var productImage in dbProduct.Images)
            {
                productImage.IsDeleted = true;
            }

            var unchoosableCategoriesIds = await _context.Categories.Where(x => !x.IsChoosableForProduct).Select(x => x.Id).ToListAsync();
            _context.RemoveRange(dbProduct.ProductCategories.Where(x => !unchoosableCategoriesIds.Contains(x.CategoryId)));
            _context.RemoveRange(dbProduct.ProductLimitations);
            _context.SaveChanges();

            var product = _mapper.Map<Product>(model);

            dbProduct.LastUpdateDate = DateTime.UtcNow;
            dbProduct.Name = product.Name;
            dbProduct.About = product.About;
            dbProduct.ListPrice = product.ListPrice;
            dbProduct.Price = product.Price;
            dbProduct.Specifications = product.Specifications;
            dbProduct.Url = product.Url;
            dbProduct.YouTubeId = product.YouTubeId;
            dbProduct.VideoUrl = product.VideoUrl;
            dbProduct.Upc = product.Upc;
            dbProduct.WarrentyUrl = product.WarrentyUrl;
            dbProduct.Brand = product.Brand;
            dbProduct.Width = product.Width;
            dbProduct.Height = product.Height;
            dbProduct.Depth = product.Depth;
            dbProduct.Weight = product.Weight;
            dbProduct.Colors = product.Colors;

            dbProduct.Images.AddRange(product.Images);
            dbProduct.ProductCategories = product.ProductCategories;
            dbProduct.ProductLimitations = product.ProductLimitations;
            if (model.VendorId != null)
            {
                dbProduct.VendorId = model.VendorId.Value;
            }
            RemoveProductFromCache(dbProduct.Id);

            await _context.SaveChangesAsync();
            return dbProduct;
        }

        public async Task<MyProductsPageModel> GetMyProducts(int vendorId)
        {
            var vendor = await _context.Vendors.SingleOrDefaultAsync(x => x.Id == vendorId);
            if (vendor == null)
                return null;

            var products = await _context.Products
                .Where(x => x.VendorId == vendorId && !x.IsDeleted)
                .Include(x => x.Images)
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.ProductLimitations)
                .ThenInclude(x => x.Limitation)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            var productsModels = _mapper.Map<List<MyProductsProductModel>>(products);
            var model = new MyProductsPageModel
            {
                Products = productsModels,
                VendorName = vendor.Name,
                VendorId = vendorId,
                CompanyCode = vendor.CompanyCode,
                VendorLogoUrl = vendor.LogoUrl,
                Commission = vendor.CommercialTerms == VendorCommercialTerms.Commission || vendor.CommercialTerms == VendorCommercialTerms.Affiliate
                                ? vendor.CommercialTermsRate
                                : null
            };
            return model;
        }

        public async Task<Product> UploadProductAsync(VendorUploadProductModel model)
        {
            var product = _mapper.Map<Product>(model);
            product.IsPublished = true;
            product.LastUpdateDate = DateTime.UtcNow;

            var fileUploads = await _context.FileUploads.Where(x => model.Images.Contains(x.Url) || model.WarrentyUrl == x.Url).ToListAsync();
            foreach (var fileUpload in fileUploads)
            {
                fileUpload.IsUsed = true;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}