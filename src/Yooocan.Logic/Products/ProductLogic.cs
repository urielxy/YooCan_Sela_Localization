using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using Yooocan.Logic.Images;
using Yooocan.Models.Products;

namespace Yooocan.Logic.Products
{
    public class ProductLogic : IProductLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDatabase _redisDatabase;
        private readonly ILogger<ProductLogic> _logger;
        private readonly IBlobUploader _blobUploader;
        private readonly AzureImageResizer _imageResizer;

        public ProductLogic(ApplicationDbContext context, IMapper mapper, IDatabase redisDatabase, 
            ILogger<ProductLogic> logger, IBlobUploader blobUploader, AzureImageResizer imageResizer)
        {
            _context = context;
            _mapper = mapper;
            _redisDatabase = redisDatabase;
            _logger = logger;
            _blobUploader = blobUploader;
            _imageResizer = imageResizer;
        }

        public async Task<ProductModel> GetModelAsync(int id)
        {
            var cacheKey = string.Format(RedisKeys.NewProductModel, id);
            ProductModel model;
            RedisValue redisValue;
            try
            {
                redisValue = await _redisDatabase.StringGetAsync(cacheKey);
            }
            catch (Exception e)
            {
                _logger.LogError(321322, e, "Error when trying to get cache from Redis for {resource}", cacheKey);
                model = await GetModelFromDbAsync(id);
                return model;
            }

            if (redisValue.HasValue)
                return JsonConvert.DeserializeObject<ProductModel>(redisValue);

            model = await GetModelFromDbAsync(id);
            if (model == null)
                return null;

            redisValue = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            try
            {
                _redisDatabase.StringSet(cacheKey, redisValue, TimeSpan.FromHours(24), flags: CommandFlags.FireAndForget);
            }
            catch (Exception e)
            {
                _logger.LogError(112233, e, "Writing to Redis of {product} failed", id);
            }
            return model;
        }

        private async Task<ProductModel> GetModelFromDbAsync(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .Include(x => x.Images)
                .Include(x => x.Company)
                .ThenInclude(company => company.Images)
                .Include(x => x.Company)
                .ThenInclude(x => x.ShippingRules)
                //.Include(x => x.VariationValues)
                //.ThenInclude(x => x.Variation)
                .SingleAsync(x => x.Id == id);

            //if (!product.IsSoldOnSite)
            //{
            //    product.VariationCombination = null;
            //    product.VariationValues.Clear();
            //}

            var model = _mapper.Map<ProductModel>(product);

            if (model.ParentCategory?.Value == "Travel")
                model.IsTravel = true;
            else
            {
                const int itemsInStrip = 10;
                var relatedProducts = await _context.Products
                    .Include(x => x.Images)
                    .Include(x => x.Company)
                    .Where(x => x.CompanyId == product.CompanyId && x.IsPublished && x.Id != id)
                    .OrderByDescending(x => x.Id)
                    .Take(itemsInStrip)
                    .ToListAsync();

                var alreadyfetchedIds = relatedProducts.Select(x => x.Id).ToList();
                if (alreadyfetchedIds.Count < itemsInStrip)
                {
                    var subCategoriesIds = product.ProductCategories.Select(x => x.CategoryId).ToList();
                    var categoryProducts = await _context.Products
                        .Include(x => x.Images)
                        .Include(x => x.Company)
                        .Where(x => x.IsPublished &&
                                    !alreadyfetchedIds.Contains(x.Id) && x.Id != id &&
                                    x.ProductCategories
                                        //.Where(pc => pc.IsMain)
                                        .Select(pc => pc.CategoryId)
                                        .Any(categoryId => subCategoriesIds.Contains(categoryId)))
                        .OrderByDescending(x => x.Id)
                        .Take(itemsInStrip - alreadyfetchedIds.Count)
                        .AsNoTracking()
                        .ToListAsync();
                    relatedProducts.AddRange(categoryProducts);
                    alreadyfetchedIds = relatedProducts.Select(x => x.Id).ToList();
                }

                if (alreadyfetchedIds.Count != itemsInStrip)
                {
                    var randomProducts = await _context.Products
                        .Include(x => x.Images)
                        .Include(x => x.Company)
                        .Where(x => x.IsPublished && x.Id != id &&
                                    !alreadyfetchedIds.Contains(x.Id))
                        .OrderByDescending(x => x.Id)
                        .Take(itemsInStrip - alreadyfetchedIds.Count)
                        .AsNoTracking()
                        .ToListAsync();
                    relatedProducts.AddRange(randomProducts);
                }
                var relatedProductsCards = _mapper.Map<List<ProductCardModel>>(relatedProducts);

                model.RelatedProducts = new ProductsStripModel
                                        {
                                            Products = relatedProductsCards,
                                            Title = "Members Who Bought This Item Also Bought",
                                            IsSlim = true
                                        };
            }
            return model;
        }

        public async Task<Product> CreateAsync(CreateProductModel model)
        {
            model.Id = default(int);
            var product = _mapper.Map<Product>(model);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            //await SaveVariationsAsync(model, product);
            await TakeDifferentSizes(product.Images.ToList());
            return product;
        }

        //private async Task SaveVariationsAsync(CreateProductModel model, Product product)
        //{
        //    var variations = model.VariationRows.SelectMany(x => x.Combinations);
        //    var variationsNames = variations.Select(x => x.Key).Distinct().ToList();
        //    var dbVariations = (await _context.Variations
        //            .Where(x => variationsNames.Contains(x.Name))
        //            .ToListAsync())
        //        .DistinctBy(x => x.Name)
        //        .ToDictionary(x => x.Name, x => x);

        //    var productVariations = model.VariationRows
        //        .SelectMany(x => x.Combinations)
        //        .Distinct()
        //        .Select(x =>
        //        {
        //            if (!dbVariations.ContainsKey(x.Key))
        //            {
        //                dbVariations.Add(x.Key, new Variation { Name = x.Key });
        //            }
        //            var results = new ProductVariationValue
        //            {
        //                Variation = dbVariations[x.Key],
        //                Value = x.Value
        //            };
        //            return results;
        //        }).ToList();

        //    // Need to flush the changes to the DB now since the json isn't tracked entity and we need the IDs.
        //    product.VariationValues = productVariations;
        //    await _context.SaveChangesAsync();


        //    var variationRows = new List<JsonVariationRow>();
        //    foreach (var variationRow in model.VariationRows)
        //    {
        //        variationRows.Add(new JsonVariationRow
        //        {
        //            Sku = variationRow.Sku,
        //            Upc = variationRow.Upc,
        //            Price = variationRow.Price,
        //            Combinations = variationRow.Combinations.ToDictionary(x => dbVariations[x.Key].Id,
        //                                  x => productVariations.Where(pv => pv.VariationId == dbVariations[x.Key].Id && pv.Value == x.Value).Single().Id)
        //        });
        //    }

        //    var jsonCombinations = JsonConvert.SerializeObject(variationRows, Formatting.None,
        //        new JsonSerializerSettings
        //        {
        //            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        //            DefaultValueHandling = DefaultValueHandling.Ignore
        //        });

        //    product.VariationCombination = await _context.ProductVariationCombinations.SingleOrDefaultAsync(x => x.ProductId == product.Id) ??
        //                                   new ProductVariationCombination();

        //    product.VariationCombination.Combinations = jsonCombinations;

        //    await _context.SaveChangesAsync();
        //}

        public async Task<Product> EditAsync(CreateProductModel model)
        {
            var oldCategories = await _context.ProductCategories.Where(x => x.ProductId == model.Id).ToListAsync();
            _context.RemoveRange(oldCategories);

            var oldImages = await _context.ProductImages.Where(x => x.ProductId == model.Id).ToListAsync();
            foreach (var oldImage in oldImages)
            {
                oldImage.IsDeleted = true;
            }

            var existingProduct = await _context.Products.Where(x => x.Id ==model.Id)
                                                        .AsNoTracking()
                                                        .SingleAsync();
            var product = _mapper.Map<Product>(model);
            product.IsPublished = existingProduct.IsPublished;
            product.AltoId = existingProduct.AltoId;
            product.VendorId = existingProduct.VendorId;
            product.Specifications = existingProduct.Specifications;
            product.AmazonId = existingProduct.AmazonId;
            product.LastUpdateDate = DateTime.UtcNow;

            _context.Update(product);       
             
            await _context.SaveChangesAsync();

            //if (model.VariationsChanged)
            //{
            //    // Deleting old variation values and creating new ones later
            //    _context.RemoveRange(_context.ProductVariationValues.Where(x => x.ProductId == model.Id));                
            //    await _context.SaveChangesAsync();

            //    await SaveVariationsAsync(model, product);
            //}
            _redisDatabase.KeyDelete(string.Format(RedisKeys.NewProductModel, model.Id));

            await TakeDifferentSizes(product.Images.ToList());
            return product;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.SingleAsync(x => x.Id == id);
            product.IsDeleted = true;
            product.LastUpdateDate = DateTime.UtcNow;

            _redisDatabase.KeyDelete(string.Format(RedisKeys.NewProductModel, id));
            await _context.SaveChangesAsync();
        }

        public async Task TakeDifferentSizes(List<ProductImage> images)
        {
            foreach (var productImage in images)
            {
                try
                {
                    await _imageResizer.GenerateOrGetResizedImage(productImage.Url, TransformationMode.Contain, 300, 200);
                    await _imageResizer.GenerateOrGetResizedImage(productImage.Url, TransformationMode.Contain, 50, 50);
                }
                catch (ArgumentException) { }
            }
            await _context.SaveChangesAsync();
        }
    }
}