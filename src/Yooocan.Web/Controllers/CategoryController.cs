using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using Yooocan.Models;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers
{
    [Authorize]
    public class CategoryController : BaseController
    {
        private readonly SearchLogic _searchLogic;
        private readonly ICategoriesLogic _categoriesLogic;

        public CategoryController(ApplicationDbContext context, ILogger<SearchController> logger,
            IMapper mapper, UserManager<ApplicationUser> userManager, SearchLogic searchLogic, ICategoriesLogic categoriesLogic) : base(context, logger, mapper, userManager)
        {
            _searchLogic = searchLogic;
            _categoriesLogic = categoriesLogic;
        }

        [AllowAnonymous]
        public async Task<JsonResult> Get(int page = 0, int pageSize = 12)
        {
            var entities = await _searchLogic.GetMainCategoriesAsync(page, pageSize);
            var models = Mapper.Map<List<CategoryModel>>(entities);

            return Json(models);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return OldIframeContainer();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexOld()
        {
            var categories = Context.Categories.Include(x => x.ParentCategory);
            var model = Mapper.Map<IEnumerable<CategoryListModel>>(categories);

            return OldView(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return OldIframeContainer();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreateOld()
        {
            var categories = Context.Categories
                .Where(x => x.ParentCategoryId == null)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            categories.Insert(0, new SelectListItem { Text = "Select parent category" });
            ViewBag.ParentCategories = categories;

            return OldView();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateCategoryModel model)
        {
            if (Context.Limitations.Any(x => x.Name == model.Name))
            {
                ModelState.AddModelError("Name", "Name already exists");
                return OldView(model);
            }

            if (model.ParentCategoryId != null)
            {
                var parent = Context.Categories.Single(x => x.Id == model.ParentCategoryId);
                model.ShopBackgroundColor = parent.ShopBackgroundColor;
                model.HeaderPictureUrl = parent.HeaderPictureUrl;
                model.PictureUrl = parent.PictureUrl;
            }

            var category = Mapper.Map<Category>(model);

            category.ShopBackgroundColor = category.ShopBackgroundColor ?? "";
            Context.Categories.Add(category);
            Context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit()
        {
            return OldIframeContainer();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditOld(int id)
        {
            var category = Context.Categories.SingleOrDefault(x => x.Id == id);
            if (category == null)
                return NotFound();

            CreateCategoryModel model = new CreateCategoryModel
            {
                Name = category.Name,
                IsChoosableForProduct = category.IsChoosableForProduct,
                IsActiveForFeed = category.IsActiveForFeed,
                IsActiveForShop = category.IsActiveForShop,
                IsChoosableForStory = category.IsChoosableForStory,
                HeaderPictureUrl = category.HeaderPictureUrl,
                PictureUrl = category.PictureUrl,
                ParentCategoryId = category.ParentCategoryId,
                ShopBackgroundColor = category.ShopBackgroundColor
            };

            var categories = Context.Categories
                .Where(x => x.ParentCategoryId == null)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = x.Id == category.ParentCategoryId
                }).ToList();
            categories.Insert(0, new SelectListItem { Text = "Select parent category" });
            ViewBag.ParentCategories = categories;

            return OldView(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateCategoryModel model)
        {
            var category = Context.Categories.Single(x => x.Id == model.Id);
            category.Name = model.Name;
            category.IsChoosableForProduct = model.IsChoosableForProduct;
            category.IsChoosableForStory = model.IsChoosableForStory;
            category.IsActiveForFeed = model.IsActiveForFeed;
            category.IsActiveForShop = model.IsActiveForShop;
            category.ParentCategoryId = model.ParentCategoryId;
            category.ShopBackgroundColor = model.ShopBackgroundColor ?? "";

            Context.SaveChanges();
            return RedirectToAction("EditOld", new { Id = model.Id });
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> FollowCategory(int id)
        {
            var userId = GetCurrentUserId();
            await _categoriesLogic.FollowCategoryAsync(id, userId);
            return NoContent();
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> UnfollowCategory(int id)
        {
            var userId = GetCurrentUserId();
            await _categoriesLogic.UnfollowCategoryAsync(id, userId);
            return NoContent();
        }
    }
}