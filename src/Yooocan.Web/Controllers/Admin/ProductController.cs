using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Yooocan.Logic.Products;
using Yooocan.Models.Products;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using Yooocan.Logic.Extensions;
using Yooocan.Entities.Products;
using StackExchange.Redis;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ProductController : BaseController
    {
        private readonly IProductLogic _productLogic;
        private readonly ICategoriesLogic _categoryLogic;
        private readonly IDatabase _redisClient;

        public ProductController(ApplicationDbContext context, IMapper mapperConfiguration,
            UserManager<ApplicationUser> userManager, ILogger<BaseController> logger,
            IProductLogic productLogic, ICategoriesLogic categoryLogic, IDatabase redisClient) : base(context, logger, mapperConfiguration, userManager)
        {
            _productLogic = productLogic;
            _categoryLogic = categoryLogic;
            _redisClient = redisClient;
        }

        public async Task<IActionResult> All(int? id, string orderBy = null)
        {
            var productsQuery = Context.Products
                .Include(x => x.ProductCategories)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Company)
                .Where(x => x.CompanyId != null && (x.CompanyId == id || id == null));

            if (orderBy == "price")
            {
                productsQuery = productsQuery.OrderByDescending(x => x.Price);
            }
            else
            {
                productsQuery = productsQuery.OrderBy(x => x.CompanyId)
                                                .ThenBy(x => x.Name);
            }
            var products = await productsQuery.ToListAsync();

            var model = Mapper.Map<List<ProductAllModel>>(products);
            return View(model);
        }

        public async Task<IActionResult> Create(int id)
        {
            if (id == 0 || await Context.Companies.FindAsync(id) == null)
            {
                return BadRequest("Company #id is required (/Create/#id)");
            }

            var model = new CreateProductModel
            {
                CategoriesOptions = await _categoryLogic.GetCategoriesForProductAsync(),
                CompanyId = id
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductModel model)
        {
            //model.VariationRows = JsonConvert.DeserializeObject<List<JsonVariationRowModel>>(
            //    Request.Form["VariationRows"],
            //    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var product = await _productLogic.CreateAsync(model);
            return RedirectToRoute("Product", new
            {
                id = product.Id,
                name = product.Name.ToCanonical()
            });
        }

        [Route("Product/Edit/{id:int}/{name?}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await Context.Products
                .AsNoTracking()
                .Include(x => x.ProductCategories)
                .Include(x => x.ProductLimitations)
                //.Include(x => x.VariationValues)
                //.ThenInclude(x => x.Variation)
                //.Include(x => x.VariationCombination)
                .Include(x => x.Company)
                .Include(x => x.Reviews)
                .Include(x => x.Images)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return NotFound();

            var model = Mapper.Map<CreateProductModel>(product);
            //if (product.VariationValues.Any())
            //{
            //    var variations = product.VariationValues
            //        .Select(x => new KeyValuePair<int, string>(x.VariationId, x.Variation.Name))
            //        .Distinct()
            //        .ToDictionary(x => x.Key, x => x.Value);
            //    if (product.VariationCombination?.Combinations != null)
            //    {
            //        var jsonVariationRows = JsonConvert.DeserializeObject<List<JsonVariationRow>>(product.VariationCombination.Combinations);
            //        model.VariationRows = jsonVariationRows.Select(x => new JsonVariationRowModel
            //        {
            //            Price = x.Price,
            //            Sku = x.Sku,
            //            Upc = x.Upc,
            //            Combinations = x.Combinations.ToDictionary(y => variations[y.Key],
            //                                                                    y =>
            //                                                                        product.VariationValues.Single(vv => vv.VariationId == y.Key && vv.Id == y.Value)
            //                                                                            .Value)
            //        }).ToList();
            //    }
            //}
            model.CategoriesOptions = await _categoryLogic.GetCategoriesForProductAsync();

            return View(model);
        }

        [HttpPost]
        [Route("Product/Edit/{id:int}/{name?}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateProductModel model)
        {
            //model.VariationRows = JsonConvert.DeserializeObject<List<JsonVariationRowModel>>(
            //    Request.Form["VariationRows"],
            //    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var product = await _productLogic.EditAsync(model);
            return RedirectToRoute("Product", new
            {
                id = product.Id,
                name = product.Name.ToCanonical()
            });
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<IActionResult> Delete(int id)
        {
            await _productLogic.DeleteAsync(id);

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Promoted()
        {
            var data = await Context.PromotedProducts
                .Include(x => x.Product)
                .OrderBy(x => x.Order)
                .Select(x => new PromotedProductModel
                {
                    ProductId = x.ProductId,
                    //PromotionType = x.PromotionType,
                    Name = x.Product.Name
                }).ToListAsync();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promoted([FromBody] List<PromotedProductModel> data)
        {
            if (data.Count == 0)
            {
                return BadRequest("Empty list");
            }

            Context.PromotedProducts.RemoveRange(Context.PromotedProducts.ToList());
            var entities = data.Select((x, index) => new PromotedProduct
            {
                ProductId = x.ProductId,
                //PromotionType = x.PromotionType,
                Order = index
            });
            Context.PromotedProducts.AddRange(entities);
            await Context.SaveChangesAsync();
            await _redisClient.KeyDeleteAsync(RedisKeys.ProductOfTheDay);

            return Ok();
        }

        public async Task<IActionResult> Publish(int id, bool publish)
        {
            var product = await Context.Products.FindAsync(id);

            if (publish)
            {
                product.IsPublished = true;
                product.IsOutOfStock = false;
                product.IsDeleted = false;
                product.LastUpdateDate = DateTime.UtcNow;
            }
            else
            {
                product.IsPublished = false;
                product.LastUpdateDate = DateTime.UtcNow;
            }

            await Context.SaveChangesAsync();
            return Ok();
        }
    }
}