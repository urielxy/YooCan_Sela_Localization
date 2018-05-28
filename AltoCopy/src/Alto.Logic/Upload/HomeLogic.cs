using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain.Benefits;
using Alto.Domain.Products;
using Alto.Enums;
using Alto.Models.Cards;
using Alto.Models.Home;
using Alto.Models.Products;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Alto.Logic.Upload
{
    public class HomeLogic : IHomeLogic
    {
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<HomeLogic> _logger;
        private readonly IDatabase _redisDatabase;
        private readonly AltoDbContext _context;

        private const int ItemsInStrip = 10;

        public HomeLogic(IMapper mapper, IServiceProvider serviceProvider, ILogger<HomeLogic> logger, IDatabase redisDatabase, AltoDbContext context)
        {
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _redisDatabase = redisDatabase;
            _context = context;
        }

        public async Task<HomeModel> GetModelAsync()
        {
            var cacheKey = RedisKeys.HomeModel;
            return await GetModelFromCacheAsync(GetModelFromDbAsync, cacheKey);           
        }

        public async Task<HomeModel> GetRandomModelAsync()
        {
            return await GetModelFromCacheAsync(GetRandomModelFromDbAsync, RedisKeys.HomeRandomModel);
        }

        private async Task<HomeModel> GetModelFromCacheAsync(Func<Task<HomeModel>> getFromDb, string cacheKey)
        {
            HomeModel model;
            RedisValue redisValue;
            try
            {
                redisValue = await _redisDatabase.StringGetAsync(cacheKey);
            }
            catch (Exception e)
            {
                _logger.LogError(321322, e, "Error when trying to get cache from Redis for {resource}", cacheKey);
                model = await getFromDb();
                return model;
            }

            if (redisValue.HasValue)
                return JsonConvert.DeserializeObject<HomeModel>(redisValue);

            model = await getFromDb();

            redisValue = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            try
            {
                _redisDatabase.StringSet(cacheKey, redisValue, TimeSpan.FromHours(24), flags: CommandFlags.FireAndForget);
            }
            catch (Exception e)
            {
                _logger.LogError(112233, e, "Writing to Redis of HomeModel failed");
            }

            return model;
        }

        #region Db Access

        private async Task<HomeModel> GetRandomModelFromDbAsync()
        {            
            var randomProducts = await GetRandomProducts(ItemsInStrip * 2);
            var randomBenefits = await GetRandomBenefits(ItemsInStrip * 2);

            var productCards = _mapper.Map<List<Product>, List<ProductCardModel>>(randomProducts);
            var benefitCards = _mapper.Map<List<Benefit>, List<BenefitCardModel>>(randomBenefits);

            var homeModel = CreateHomeModel(productCards.Skip(ItemsInStrip).Take(ItemsInStrip).ToList(),
                productCards.Take(ItemsInStrip).ToList(),
                benefitCards.Take(ItemsInStrip).ToList(),
                benefitCards.Skip(ItemsInStrip).Take(ItemsInStrip).ToList());

            return homeModel;
        }

        private async Task<List<Product>> GetRandomProducts(int count)
        {
            var randomProductIds = await _context.Products.Where(x => x.DeleteDate == null && x.IsPublished)                                                    
                                                        .AsNoTracking()
                                                        .Select(x => x.Id)
                                                        .OrderBy(x => Guid.NewGuid())
                                                        .Take(count * 2)                                                        
                                                        .ToListAsync();           

            var randomProducts = await _context.Products.Where(x => randomProductIds.Contains(x.Id))
                                                        .AsNoTracking() 
                                                        .Include(x => x.Images)
                                                        .Include(x => x.Company)
                                                        .ToListAsync();

            randomProducts = randomProducts.OrderBy(x => Guid.NewGuid()).ToList();
            var randomProductsWithCompanyLimit = randomProducts.GroupBy(x => x.CompanyId).SelectMany(x => x.Take(2)).OrderBy(x => Guid.NewGuid()).ToList();
            if (randomProductsWithCompanyLimit.Count < count)
            {
                var randomProductsWithCompanyLimitIds = randomProductsWithCompanyLimit.Select(x => x.Id).ToList();
                randomProductsWithCompanyLimit = randomProductsWithCompanyLimit.Union(randomProducts.Where(x => !randomProductsWithCompanyLimitIds.Contains(x.Id))
                                                                                                    .Take(count - randomProductsWithCompanyLimit.Count))                    
                                                                               .ToList();
            }

            return randomProductsWithCompanyLimit.OrderByDescending(x => x.Price).ToList();
        }

        private async Task<List<Benefit>> GetRandomBenefits(int count)
        {
            var randomIds = await _context.Benefits.Where(x => x.DeleteDate == null && x.IsPublished)
                                                        .AsNoTracking()
                                                        .Select(x => x.Id)
                                                        .OrderBy(x => Guid.NewGuid())
                                                        .Take(count)
                                                        .ToListAsync();

            var randomBenefits= await _context.Benefits.Where(x => randomIds.Contains(x.Id))
                                                        .AsNoTracking()
                                                        .Include(x => x.Images)
                                                        .Include(x => x.Company)
                                                        .Include(x => x.Categories)
                                                            .ThenInclude(x => x.Category)
                                                        .ToListAsync();

            return randomBenefits;
        }

        private async Task<HomeModel> GetModelFromDbAsync()
        {
            List<ProductCardModel> popularProductsCards = null;
            List<ProductCardModel> newProductsCards = null;

            List<BenefitCardModel> popularBenefitsCards = null;
            List<BenefitCardModel> newBenefitsCards = null;
            var productsTask = Task.Run(async () =>
            {
                using (var productsContext = _serviceProvider.GetService<AltoDbContext>())
                {
                    var popularProducts = await (from promotedProduct in productsContext.PromotedProducts
                                                 join product in productsContext.Products on promotedProduct.ProductId equals product.Id
                                                 where promotedProduct.PromotionType == PromotionType.Popular
                                                 orderby promotedProduct.Order
                                                 select product)
                        .Include(x => x.Images)
                        .Include(x => x.Company)
                        .AsNoTracking()
                        .Take(ItemsInStrip)
                        .ToListAsync();

                    var alreadyfetchedIds = popularProducts.Select(x => x.Id).ToList();
                    if (alreadyfetchedIds.Count < ItemsInStrip)
                    {
                        var randomProducts = await productsContext.Products
                            .Where(x => x.DeleteDate == null && x.IsPublished &&
                                        !alreadyfetchedIds.Contains(x.Id))
                            .Include(x => x.Images)
                            .Include(x => x.Company)
                            .AsNoTracking()
                            .Take(ItemsInStrip - alreadyfetchedIds.Count)
                            .ToListAsync();
                        popularProducts.AddRange(randomProducts);
                    }

                    popularProductsCards = _mapper.Map<List<ProductCardModel>>(popularProducts);
                    alreadyfetchedIds = popularProductsCards.Select(x => x.Id).ToList();

                    var newProducts = await (from promotedProduct in productsContext.PromotedProducts
                                             join product in productsContext.Products on promotedProduct.ProductId equals product.Id
                                             where promotedProduct.PromotionType == PromotionType.New &&
                                                   !alreadyfetchedIds.Contains(product.Id)
                                             orderby promotedProduct.Order
                                             select product)
                        .Include(x => x.Images)
                        .Include(x => x.Company)
                        .AsNoTracking()
                        .Take(ItemsInStrip)
                        .ToListAsync();

                    if (newProducts.Count < ItemsInStrip)
                    {
                        alreadyfetchedIds.AddRange(newProducts.Select(x => x.Id));

                        var randomProducts = await productsContext.Products
                            .Where(x => x.DeleteDate == null && x.IsPublished &&
                                        !alreadyfetchedIds.Contains(x.Id))
                            .Include(x => x.Images)
                            .Include(x => x.Company)
                            .AsNoTracking()
                            .OrderByDescending(x => x.InsertDate)
                            .Take(ItemsInStrip - newProducts.Count)
                            .ToListAsync();
                        newProducts.AddRange(randomProducts);
                    }

                    newProductsCards = _mapper.Map<List<ProductCardModel>>(newProducts);
                }
            });

            var benefitsTask = Task.Run(async () =>
            {
                using (var benefitsContext = _serviceProvider.GetService<AltoDbContext>())
                {
                    var recommendeBenefits = await (from promotedBenefit in benefitsContext.PromotedBenefits
                                                    join benefit in benefitsContext.Benefits on promotedBenefit.BenefitId equals benefit.Id
                                                    where promotedBenefit.PromotionType == PromotionType.Popular
                                                    orderby promotedBenefit.Order
                                                    select benefit)
                        .Include(x => x.Images)
                        .Include(x => x.Company)
                        .Include(x => x.Categories)
                            .ThenInclude(x => x.Category)
                        .AsNoTracking()
                        .Take(ItemsInStrip)
                        .ToListAsync();
                    var alreadyfetchedIds = recommendeBenefits.Select(x => x.Id).ToList();
                    if (alreadyfetchedIds.Count < ItemsInStrip)
                    {
                        var randomBenefits = await benefitsContext.Benefits
                            .Where(x => x.DeleteDate == null && x.IsPublished && !alreadyfetchedIds.Contains(x.Id))
                            .Include(x => x.Company)
                            .Include(x => x.Images)
                            .Include(x => x.Categories)
                                .ThenInclude(x => x.Category)
                            .AsNoTracking()
                            .Take(ItemsInStrip - alreadyfetchedIds.Count)
                            .ToListAsync();
                        recommendeBenefits.AddRange(randomBenefits);
                    }

                    popularBenefitsCards = _mapper.Map<List<BenefitCardModel>>(recommendeBenefits);
                    alreadyfetchedIds = popularBenefitsCards.Select(x => x.Id).ToList();

                    var newBenefits = await (from promotedBenefit in benefitsContext.PromotedBenefits
                                             join benefit in benefitsContext.Benefits on promotedBenefit.BenefitId equals benefit.Id
                                             where promotedBenefit.PromotionType == PromotionType.New &&
                                                   !alreadyfetchedIds.Contains(benefit.Id)
                                             orderby promotedBenefit.Order
                                             select benefit)
                        .Include(x => x.Company)
                        .Include(x => x.Images)
                        .Include(x => x.Categories)
                            .ThenInclude(x => x.Category)
                        .AsNoTracking()
                        .Take(ItemsInStrip)
                        .ToListAsync();

                    if (newBenefits.Count < ItemsInStrip)
                    {
                        alreadyfetchedIds.AddRange(newBenefits.Select(x => x.Id));

                        var randomBenefits = await benefitsContext.Benefits
                            .Where(x => x.DeleteDate == null && x.IsPublished && !alreadyfetchedIds.Contains(x.Id))
                            .Include(x => x.Company)
                            .Include(x => x.Images)
                            .Include(x => x.Categories)
                                .ThenInclude(x => x.Category)
                            .AsNoTracking()
                            .OrderByDescending(x => x.InsertDate)
                            .Take(ItemsInStrip - newBenefits.Count)
                            .ToListAsync();
                        newBenefits.AddRange(randomBenefits);
                    }

                    newBenefitsCards = _mapper.Map<List<BenefitCardModel>>(newBenefits);
                }
            });

            await Task.WhenAll(productsTask, benefitsTask);
            
            return CreateHomeModel(newProductsCards, popularProductsCards, newBenefitsCards, popularBenefitsCards);
        }

        #endregion

        private HomeModel CreateHomeModel(List<ProductCardModel> newProducts,
            List<ProductCardModel> popularProducts, 
            List<BenefitCardModel> newBenefits, 
            List<BenefitCardModel> popularBenefits)
        {
            var model = new HomeModel
            {
                BenefitsStrips = new List<BenefitsStripModel>
                                             {
                                                 new BenefitsStripModel
                                                 {
                                                     //Title = "NEWEST BENEFITS",
                                                     Benefits = newBenefits
                                                 },
                                                 new BenefitsStripModel
                                                 {
                                                     //Title = "MOST POPULAR BENEFITS",
                                                     Benefits = popularBenefits
                                                 }

                                             },
                ProductsStrips = new List<ProductsStripModel>
                                             {
                                                 new ProductsStripModel
                                                 {
                                                     Title = "NEWEST DISCOUNTS",
                                                     Products = newProducts,
                                                     IsSlim = true
                                                 },
                                                 new ProductsStripModel
                                                 {
                                                     Title = "MOST POPULAR DISCOUNTS",
                                                     Products = popularProducts,
                                                     IsSlim = true
                                                 }
                                             }
            };
            return model;
        }
    }
}