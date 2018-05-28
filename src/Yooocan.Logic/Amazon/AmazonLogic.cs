using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nager.AmazonProductAdvertising;
using Nager.AmazonProductAdvertising.Model;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Enums;
using System.Threading;
using Polly;
using Yooocan.Logic.Messaging;
using System.Threading.Tasks;
using System.Text;

namespace Yooocan.Logic.Amazon
{
    public class AmazonLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AmazonLogic> _logger;
        private readonly IEmailSender _emailSender;
        private readonly AmazonOptions _amazonOptions;

        public AmazonLogic(ApplicationDbContext context, IMemoryCache memoryCache, IOptions<AmazonOptions> amazonOptionsWrapper, ILogger<AmazonLogic> logger, IEmailSender emailSender)
        {
            _context = context;
            _memoryCache = memoryCache;
            _logger = logger;
            _emailSender = emailSender;
            _amazonOptions = amazonOptionsWrapper.Value;
        }

        private Item[] GetProductsByIds(params string[] asins)
        {
            var authentication = new AmazonAuthentication
            {
                AccessKey = _amazonOptions.AccessKey,
                SecretKey = _amazonOptions.SecretKey
            };
            var wrapper = new AmazonWrapper(authentication, AmazonEndpoint.US, "yoocan-20");
            AmazonErrorResponse error = null;
            wrapper.ErrorReceived += e => error = e;
            var result = wrapper.Lookup(asins, AmazonResponseGroup.Large | AmazonResponseGroup.OfferFull | AmazonResponseGroup.OfferSummary | AmazonResponseGroup.VariationSummary);
            if (error != null)
            {
                throw new AmazonApiException(error);
            }
            return result.Items.Item;
        }

        private Item[] GetProductsByIdsWithRetries(params string[] asins)
        {
            var retryPolicy = Policy.Handle<AmazonApiException>().WaitAndRetry(5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) * 5),
                (exception, timeSpan, retryAttempt, context) =>
                    _logger.LogWarning($"Amazon request failed, retry number: {retryAttempt}, message: {exception.Message}"));

            return retryPolicy.Execute(() => GetProductsByIds(asins));
        }

        public (int ChangedImages, int OutOfStockProducts) RefreshData()
        {
            var productsWithoutImages = new List<Product>();
            var missingProducts = new List<Product>();
            var outOfStockProducts = new List<Product>();
            var backInStockProducts = new List<Product>();

            const int maxBatchSize = 10;
            var changedProductsImagesCount = 0;
            var existingAmazonProducts = _context.Products.Where(x => x.AmazonId != null && !x.IsDeleted && (x.IsPublished || x.IsOutOfStock)).Include(x => x.Images).ToList();
            var productsCount = existingAmazonProducts.Count;
            while (existingAmazonProducts.Any())
            {
                var batch = existingAmazonProducts.Take(maxBatchSize).ToList();
                var batchAsins = batch.Select(x => x.AmazonId).ToArray();
                var batchRefresh = GetProductsByIdsWithRetries(batchAsins);
                var returnedAsins = batchRefresh.Select(x => x.ASIN).ToList();
                var missingProductsAsins = batchAsins.Except(returnedAsins).ToList();
                if (missingProductsAsins.Any())
                {
                    var missingPublishedProducts = batch.Where(x => missingProductsAsins.Contains(x.AmazonId) && x.IsPublished).ToList();
                    if (missingPublishedProducts.Any())
                    {
                        var errorMessage = $"Found mismatch between requested and returned product ids, unpublishing missing products: {string.Join(",", missingPublishedProducts.Select(x => x.AmazonId))}";
                        _logger.LogWarning(errorMessage);
                        missingProducts.AddRange(missingPublishedProducts);
                    }
                }

                for (var i = 0; i < batch.Count; i++)
                {
                    if (missingProductsAsins.Contains(batch[i].AmazonId))
                        continue;
                    var productFromApi = batchRefresh.Single(x => x.ASIN == batch[i].AmazonId);
                    var productImage = batch[i].Images.Single(x => !x.IsDeleted && x.Type == ImageType.Primary);
                    var previousImageUrl = productImage.CdnUrl;
                    if (UpdateProductImage(productFromApi, productImage) == null && batch[i].IsPublished)
                    {
                        _logger.LogWarning("Unable to find images for product {productId}, old url: {imageUrl}, unpublishing it", batch[i].Id, productImage.CdnUrl);
                        productsWithoutImages.Add(batch[i]);
                        changedProductsImagesCount++;
                    }
                    else if (productImage.CdnUrl != previousImageUrl && batch[i].IsPublished)
                    {
                        _logger.LogInformation("Found different image for product {productId}, old url: {oldImageUrl}, new url: {newImageUrl}",
                            batch[i].Id, productImage.CdnUrl, productFromApi.MediumImage?.URL);
                        changedProductsImagesCount++;
                    }
                    if (!productFromApi.ItemAttributes.ProductTypeName.ToLower().Contains("book") &&
                        (productFromApi.Offers == null || int.Parse(productFromApi.Offers.TotalOffers) == 0) &&
                        (productFromApi.OfferSummary == null || (int.Parse(productFromApi.OfferSummary.TotalCollectible) == 0 &&
                                                                 int.Parse(productFromApi.OfferSummary.TotalNew) == 0 &&
                                                                 int.Parse(productFromApi.OfferSummary.TotalRefurbished) == 0 &&
                                                                 int.Parse(productFromApi.OfferSummary.TotalUsed) == 0)) &&
                        string.IsNullOrEmpty(productFromApi.VariationSummary?.HighestPrice?.Amount))
                    {
                        if (batch[i].IsPublished)
                        {
                            _logger.LogInformation("Found an out of stock product: {productId}, {amazonProductId}", batch[i].Id, productFromApi.ASIN);
                            outOfStockProducts.Add(batch[i]);
                        }
                    }
                    else if (!batch[i].IsPublished && !string.IsNullOrEmpty(productImage?.CdnUrl))
                    {
                        backInStockProducts.Add(batch[i]);
                    }
                }
                existingAmazonProducts = existingAmazonProducts.Skip(maxBatchSize).ToList();

                if (existingAmazonProducts.Any())
                {
                    //amazon throttles at 1 request per second per IP
                    Thread.Sleep(1100);
                }
            }
            DoIsPublishedChanges(productsWithoutImages, missingProducts, outOfStockProducts, backInStockProducts, productsCount);
            _context.SaveChanges();
            return (changedProductsImagesCount, outOfStockProducts.Count);
        }

        private void DoIsPublishedChanges(List<Product> productsWithoutImages, List<Product> missingProducts, List<Product> outOfStockProducts, List<Product> backInStockProducts, int productsCount)
        {
            var allProductsToUnpublish = productsWithoutImages.Union(missingProducts).Union(outOfStockProducts).Distinct().ToList();
            if (allProductsToUnpublish.Any() || backInStockProducts.Any())
            {
                if (allProductsToUnpublish.Count >= productsCount / 2)
                {
                    _logger.LogError("Not unpublishing more than 50% of products, as it could have been caused by a problem with the API");
                }
                else
                {
                    allProductsToUnpublish.ForEach(p =>
                    {
                        p.IsPublished = false;
                        p.IsOutOfStock = true;
                        p.LastUpdateDate = DateTime.UtcNow;
                    });
                    backInStockProducts.ForEach(p =>
                    {
                        p.IsPublished = true;
                        p.IsOutOfStock = false;
                        p.LastUpdateDate = DateTime.UtcNow;
                    });
                    var message = $@"The following products got unpublished because they had no up-to-date image:<br />{GetProductsHtml(productsWithoutImages)}
The following products got unpublished because they were not returned by Amazon (possibly got deleted):<br />{GetProductsHtml(missingProducts)}
The following products got unpublished because they were out of stock:<br />{GetProductsHtml(outOfStockProducts)}
The following products got published because they are back in stock:<br />{GetProductsHtml(backInStockProducts)}";
                    _logger.LogInformation(message);
                    Task.Run(() => _emailSender.SendEmailAsync(null, "evgeny@yoocantech.com;jessica@yoocantech.com;moshe@yoocantech.com", "yoocan Amazon Job summary - Some products got automatically unpublished/published",
                             message, "Jobs-amazon job", null)).GetAwaiter().GetResult();
                }
            }
        }

        private string GetProductsHtml(List<Product> products)
        {
            var result = new StringBuilder();
            foreach (var product in products)
            {
                result.AppendLine($"Id: {product.Id}, ASIN: {product.AmazonId}, Name: {product.Name}<br />");
            }
            result.AppendLine("<br /><br />");

            return result.ToString();
        }

        public (int ProductId, int AmazonVendorId) AddAmazonProduct(string asin)
        {
            var existingProduct = _context.Products.SingleOrDefault(x => x.AmazonId == asin && !x.IsDeleted);
            if (existingProduct != null)
                return (existingProduct.Id, GetAmazonVendorId().VendorId);

            var amazonProduct = GetProductsByIdsWithRetries(asin)[0];
            var product = new Product
            {
                Name = amazonProduct.ItemAttributes.Title,
                Images = new List<ProductImage> { UpdateProductImage(amazonProduct) },
                AmazonId = asin,
                VendorId = GetAmazonVendorId().VendorId,
                CompanyId = GetAmazonVendorId().CompanyId
            };
            _context.Products.Add(product);
            _context.SaveChanges();

            return (product.Id, GetAmazonVendorId().VendorId);
        }

        private ProductImage UpdateProductImage(Item amazonProduct, ProductImage image = null)
        {
            if (image == null)
                image = new ProductImage();

            var mediumImage = amazonProduct.MediumImage?.URL;
            var largeImage = amazonProduct.LargeImage?.URL;

            if (amazonProduct.MediumImage == null)
            {
                mediumImage = amazonProduct.ImageSets?[0]?.MediumImage?.URL;
                largeImage = amazonProduct.ImageSets?[0]?.LargeImage?.URL;
            }

            if (mediumImage == null)
                return null;

            if (image.CdnUrl == mediumImage)
                return image;

            image.CdnUrl = mediumImage;
            image.Url = mediumImage;
            image.OriginalUrl = largeImage;
            image.Type = ImageType.Primary;
            return image;
        }

        private (int VendorId, int CompanyId) GetAmazonVendorId()
        {
            return _memoryCache.GetOrCreate("AmazonVendorId", entry =>
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1);
                var vendorId = _context.Vendors.Where(x => x.Name == "Amazon").Select(x => x.Id).Single();
                var companyId = _context.Companies.Where(x => x.Name == "Amazon").Select(x => x.Id).Single();
                return (vendorId, companyId);
            });
        }
    }
}

