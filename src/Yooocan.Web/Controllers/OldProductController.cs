using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Enums.Vendors;
using Yooocan.Logic;
using Yooocan.Logic.Extensions;
using Yooocan.Models;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers
{
    public class OldProductController : BaseController
    {
        private readonly SearchLogic _searchLogic;
        private readonly IOldProductLogic _productLogic;
        private readonly IBlobUploader _blobUploader;
        private readonly ICategoriesLogic _categoriesLogic;
        private readonly ILimitationLogic _limitationLogic;

        public OldProductController(ApplicationDbContext context, ILogger<OldProductController> logger, IMapper mapper, UserManager<ApplicationUser> userManager,
            SearchLogic searchLogic, IOldProductLogic productLogic, IBlobUploader blobUploader, ICategoriesLogic categoriesLogic, ILimitationLogic limitationLogic)
            : base(context, logger, mapper, userManager)
        {
            _searchLogic = searchLogic;
            _productLogic = productLogic;
            _blobUploader = blobUploader;
            _categoriesLogic = categoriesLogic;
            _limitationLogic = limitationLogic;
        }

        [Route("/Product/Index/{id}")]
        public async Task<ActionResult> Index(int id)
        {
            var model = await _productLogic.GetProductModelAsync(id);

            if (model == null)
                return NotFound();

            ViewBag.ShowingAmazonProducts = true;
            return View(model);
        }

        public ActionResult ReallyCreate()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var vendorsList = Context.Vendors.OrderBy(x => x.Name).Select(x => new SelectListItem
                                                                               {
                                                                                   Text = x.Name,
                                                                                   Value = x.Id.ToString()
                                                                               }).ToList();
            vendorsList.Insert(0, new SelectListItem {Text = "Select a vendor", Disabled = true});
            ViewBag.VendorsList = vendorsList;


            var categoriesList = Context.Categories
                .Where(x => x.ParentCategoryId != null && x.IsChoosableForProduct)
                .OrderBy(x => x.ParentCategory.Name)
                .ThenBy(x => x.Name)
                .Select(x => new SelectListItem
                             {
                                 Text = x.Name,
                                 Value = x.Id.ToString(),
                                 Group = new SelectListGroup {Name = x.ParentCategory.Name}
                             }).ToList();

            foreach (var categories in categoriesList.GroupBy(x => x.Group.Name))
            {
                foreach (var selectListItem in categories)
                {
                    selectListItem.Group = categories.First().Group;
                }
            }

            categoriesList.Insert(0, new SelectListItem {Text = "Select category/ies", Disabled = true});
            ViewBag.CategoriesList = categoriesList;

            var limitationsList = Context.Limitations
                .Where(x => x.ParentLimitationId == null)
                .OrderBy(x => x.ParentLimitation.Name)
                .ThenBy(x => x.Name)
                .Select(x => new SelectListItem
                             {
                                 Text = x.Name,
                                 Value = x.Id.ToString()
                             }).ToList();


            limitationsList.Insert(0, new SelectListItem {Text = "Select limitation/s"});
            ViewBag.LimitationsList = limitationsList;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateProductModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var product = await _productLogic.UploadProductAsync(model);
            return RedirectToAction("Index", "Product", new {id = product.Id});
        }

        #region Upload

        public ActionResult Upload(int? id)
        {
            return OldIframeContainer();
        }

        public async Task<ActionResult> UploadOld(int? id)
        {
            if (id == null)
            {
                var claim = User.FindFirst("vendor");
                if (claim == null)
                    return Unauthorized();

                id = int.Parse(User.FindFirst("vendor").Value);
            }
            else if (!User.IsInRole("Admin"))
            {
                return Unauthorized();
            }
            var model = new VendorUploadProductModel
            {
                CategoriesOptions = await _categoriesLogic.GetCategoriesForProductAsync(),
                LimitationsOptions = await _limitationLogic.GetLimitationsAsync(),
                VendorId = User.IsInRole("Admin") ? id : null
            };

            var vendor = await Context.Vendors.SingleOrDefaultAsync(x => x.Id == id);

            if (vendor == null)
                return NotFound();

            if (vendor.CommercialTerms == VendorCommercialTerms.Commission || vendor.CommercialTerms == VendorCommercialTerms.Affiliate)
                model.Commission = (float?) vendor.CommercialTermsRate;

            return OldView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(VendorUploadProductModel model)
        {
            if (model.VendorId == null)
            {
                var claim = User.FindFirst("vendor");
                if (claim == null)
                    return Unauthorized();

                model.VendorId = int.Parse(User.FindFirst("vendor").Value);
            }
            else if (!User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            var vendor = await Context.Vendors.SingleOrDefaultAsync(x => x.Id == model.VendorId);

            if (vendor == null)
                return NotFound();

            await _productLogic.UploadProductAsync(model);
            return Redirect($"/Vendor/MyProducts/{model.VendorId}");
        }

        #endregion

        #region List

        [Authorize(Roles = "Admin")]
        public ActionResult List()
        {
            return OldIframeContainer();
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ListOld()
        {
            var products = await Context.Products
                .Where(x => !x.IsDeleted)
                .Include(x => x.Images)
                .Include(x => x.Vendor)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            var model = Mapper.Map<IEnumerable<ProductListModel>>(products);
            return OldView(model);
        }

        #endregion

        #region Edit

        public ActionResult Edit(int id)
        {
            return OldIframeContainer();
        }

        public async Task<ActionResult> EditOld(int id)
        {
            var product = Context.Products
                .Include(x => x.Images)
                .Include(x => x.ProductCategories)
                .Include(x => x.ProductLimitations)
                .SingleOrDefault(x => x.Id == id);

            if (product == null)
                return NotFound();

            var vendorIdString = product.VendorId.ToString();
            if (!User.Claims.Any(x => x.Type == "vendor" && x.Value == vendorIdString) && !User.IsInRole("Admin"))
                return Unauthorized();

            var model = Mapper.Map<VendorUploadProductModel>(product);
            model.CategoriesOptions = await _categoriesLogic.GetCategoriesForProductAsync();
            model.LimitationsOptions = await _limitationLogic.GetLimitationsAsync();
            
            return OldView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(VendorUploadProductModel model)
        {
            if (ModelState.IsValid)
            {
                var product = Context.Products.Single(x => x.Id == model.ProductId);
                var vendorIdString = product.VendorId.ToString();

                if (!User.Claims.Any(x => x.Type == "vendor" && x.Value == vendorIdString) && !User.IsInRole("Admin"))
                    return Unauthorized();

                product = await _productLogic.EditProductAsync(model);
                return RedirectToAction("Index", new {id = product.Id});
            }

            return BadRequest(ModelState);
        }

        #endregion

        [HttpPost]
        [HttpDelete]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public ActionResult Delete(int productId)
        {
            var product = Context.Products.Single(x => x.Id == productId);

            if (product == null)
                return NotFound();

            var vendorIdString = product.VendorId.ToString();
            if (!User.Claims.Any(x => x.Type == "vendor" && x.Value == vendorIdString) && !User.IsInRole("Admin"))
                return Unauthorized();

            product.IsDeleted = true;
            product.IsPublished = false;
            product.LastUpdateDate = DateTime.UtcNow;
            Context.SaveChanges();

            if (Request.IsAjaxRequest())
                return NoContent();

            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> Publish(int productId, bool publish)
        {
            var product = await Context.Products.SingleOrDefaultAsync(x => x.Id == productId);
            if (product == null)
                return NotFound();

            var vendorIdString = product.VendorId.ToString();
            if (!User.Claims.Any(x => x.Type == "vendor" && x.Value == vendorIdString) && !User.IsInRole("Admin"))
                return Unauthorized();

            product.IsPublished = publish;
            if(product.IsPublished)
                product.IsOutOfStock = false;
            product.LastUpdateDate = DateTime.UtcNow;

            await Context.SaveChangesAsync();

            return NoContent();
        }
    }
}