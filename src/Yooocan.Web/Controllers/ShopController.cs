using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using Yooocan.Logic.Extensions;
using System;

namespace Yooocan.Web.Controllers
{
    public class ShopController : BaseController
    {
        private readonly IShopLogic _shopLogic;

        public ShopController(ApplicationDbContext context, 
            ILogger<ShopController> logger, 
            IMapper mapper, 
            UserManager<ApplicationUser> userManager, 
            IShopLogic shopLogic)
            : base(context, logger, mapper, userManager)
        {
            _shopLogic = shopLogic;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _shopLogic.GetShopHomeAsync();
            return View(model);
        }

        [Route("Shop/CategoryShop/{id:int}")] // Old route, remove later when Search engine forget about it.
        [Route("Shop/Category/{id:int}/{categoryName?}", Name = "CategoryShop")]
        public async Task<ActionResult> Category(int id, string categoryName)
        {
            const int initialCount = 24;
            var model = await _shopLogic.GetCategoryShopAsync(id, initialCount);
            if (model == null)
                return NotFound();

            if (model.CategoryId != id)
                return RedirectToRoute("CategoryShop", new { id = model.CategoryId, categoryName = model.CategoryName.ToCanonical() });

            if (Request.GetDisplayUrl().Contains("CategoryShop") || categoryName != model.CategoryName.ToCanonical())
                return RedirectToRoute("CategoryShop", new {id, categoryName = model.CategoryName.ToCanonical() });

            ViewBag.ShowingAmazonProducts = true;
            return View(model);
        }

        public async Task<ActionResult> MoreCategoryProducts(int id, DateTime maxDate, int count, int lastId)
        {
            var model = await _shopLogic.GetCategoryShopFromDbAsync(id, count, maxDate, lastId);
            return model.Products.Any() ? (ActionResult)PartialView("_MoreProducts", model.Products) : NotFound();
        }

        [Route("Shop/ServiceProviders/{id:int}")]
        public async Task<ActionResult> OldServiceProvidersRoute(int id)
        {
            var category = await Context.Categories.Where(x => x.IsActiveForShop && x.Id == id).SingleOrDefaultAsync();
            if (category == null)
                return NotFound();

            return RedirectToRoutePermanent("CategoryServiceProvider", new
                                                                       {
                                                                           id,
                                                                           categoryName = category.Name.ToCanonical()
                                                                       });
        }

        [Route("Shop/SubCategoryShop/{id:int}")]
        public async Task<ActionResult> OldSubCategoryRoute(int id)
        {
            var category = await Context.Categories.Include(x => x.ParentCategory)
                .Where(x => x.IsActiveForFeed && x.Id == id).SingleOrDefaultAsync();

            if (category?.ParentCategoryId == null)
                return NotFound();

            return RedirectToRoutePermanent("CategoryShop", new
                                                            {
                                                                id = category.ParentCategoryId,
                                                                categoryName = category.ParentCategory.Name.ToCanonical()
                                                            });
        }
    }
}