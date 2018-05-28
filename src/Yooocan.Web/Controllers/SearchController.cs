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
using Yooocan.Enums;
using Yooocan.Logic;
using Yooocan.Models;
using Yooocan.Models.UploadStoryModels;

namespace Yooocan.Web.Controllers
{
    public class SearchController : BaseController
    {
        private readonly SearchLogic _searchLogic;
        private readonly ILimitationLogic _limitationLogic;
        private readonly ICategoriesLogic _categoriesLogic;

        public SearchController(ApplicationDbContext context, 
            ILogger<SearchController> logger, 
            IMapper mapper, 
            UserManager<ApplicationUser> userManager, 
            SearchLogic searchLogic, 
            ILimitationLogic limitationLogic,
            ICategoriesLogic categoriesLogic) : base(context, logger, mapper, userManager)
        {
            _searchLogic = searchLogic;
            _limitationLogic = limitationLogic;
            _categoriesLogic = categoriesLogic;
        }

        [Route("Search")]
        public async Task<ActionResult> Search(int? categoryId, string query, List<int> limitationIds)
        {
            SearchResultModel model;

            if (string.IsNullOrWhiteSpace(query) && (limitationIds == null || limitationIds.Count == 0) && categoryId == null)
            {

                model = new SearchResultModel();
            }
            else
            {
                var results = await _searchLogic.SearchAsync(query, categoryId, limitationIds);
                model = Mapper.Map<SearchResultModel>(results);
            }

            model.Query = query;
            model.LimitationIds = limitationIds;
            model.CategoryId = categoryId;

            ViewBag.Limitations = (await _limitationLogic.GetLimitationsAsync()).Select(x => new SelectListItem {Value = x.Key.ToString(), Text = x.Value});
            ViewBag.Categories = (await _categoriesLogic.GetMainCategoriesForSearchAsync()).Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value });

            if(model?.Products.Any(x => x.AmazonId != null) == true)
            {
                ViewBag.ShowingAmazonProducts = true;
            }

            return View(model);
        }

        #region Separate Search By Entity

        public async Task<ActionResult> SearchStories(string query, int? categoryId, List<int> limitationIds, int page)
        {
            var model = await _searchLogic.SearchStories(query, categoryId, limitationIds, page);

            return PartialView("../Home/_MoreContentWide", model);
        }

        public async Task<ActionResult> SearchProducts(string query, int? categoryId, List<int> limitationIds, int page)
        {
            var model = await _searchLogic.SearchProducts(query, categoryId, limitationIds, page);

            return PartialView("../Shop/_MoreProducts", model);
        }

        public async Task<ActionResult> SearchBenefits(string query, int page)
        {
            var model = await _searchLogic.SearchBenefits(query, page);

            return PartialView("../Benefit/_MoreBenefits", model);
        }

        public async Task<ActionResult> SearchServiceProviders(string query, int? categoryId, List<int> limitationIds, int page)
        {
            var model = await _searchLogic.SearchServiceProviders(query, categoryId, limitationIds, page);

            return PartialView("../ServiceProvider/_CategoryCards", model);
        }

        #endregion

        public async Task<ActionResult> Embed(string query, SearchType type = SearchType.ServiceProviders)
        {
            var referrer = Request.Headers["referer"].ToString();
            var allowedHosts = new List<string>
            {
                "https://localhost:44333/",
                "https://altodev.azurewebsites.net/",
                "https://altolife-integration.azurewebsites.net/",
                "https://altolife.com/"
            };
            if (string.IsNullOrEmpty(referrer) || allowedHosts.All(host => !referrer.StartsWith(host)))
            {
                return BadRequest();
            }

            var results = await _searchLogic.SearchAsync(query, null, null, type);
            var model = Mapper.Map<SearchResultModel>(results);

            return type == SearchType.ServiceProviders
                ? View("ServiceProvidersSearchResults", model)
                : View("StoriesSearchResults", model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SearchRelatedProduct(string productSearchTerm)
        {
            if (string.IsNullOrWhiteSpace(productSearchTerm))
                return BadRequest();

            productSearchTerm = productSearchTerm.Trim();
            int numericSearchTerm;
            IQueryable<Product> query;
            if (int.TryParse(productSearchTerm, out numericSearchTerm))
            {
                query = Context.Products.Where(x => x.IsPublished && x.Id == numericSearchTerm);
            }
            else
            {
                query = Context.Products.Where(x=> x.IsPublished && x.Name.Contains(productSearchTerm));
            }

            var products = await query
                .Select(x => new
                                 SetRelatedProductModel
                                 {
                                     Id = x.Id,
                                     Name = x.Name,
                                     VendorName = x.Vendor.Name,
                                     PrimaryImageUrl = x.Images
                                         .Where(image => image.Type == ImageType.Primary && !image.IsDeleted)
                                         .Select(image => image.CdnUrl ?? image.Url)
                                         .FirstOrDefault()
                                 }).ToListAsync();

            return PartialView(products);
        }
    }
}