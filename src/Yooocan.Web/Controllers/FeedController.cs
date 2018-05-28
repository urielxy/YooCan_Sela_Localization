using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using Yooocan.Logic.Extensions;
using Yooocan.Models;
using Yooocan.Models.Feeds;

namespace Yooocan.Web.Controllers
{
    public class FeedController : BaseController
    {
        private readonly SearchLogic _searchLogic;

        public FeedController(ApplicationDbContext context, ILogger<FeedController> logger, IMapper mapper, UserManager<ApplicationUser> userManager,
            SearchLogic searchLogic) : base(context, logger, mapper, userManager)
        {
            _searchLogic = searchLogic;
        }

        [Route("Feed/{id:int}/{categoryName?}", Name = "Feed")]
        public async Task<IActionResult> Category(int id, string categoryName, int? card)
        {
            var category = await Context.Categories.Include(x => x.RedirectCategory)
                                                   .SingleOrDefaultAsync(x => x.Id == id && (x.IsChoosableForStory || x.RedirectCategoryId != null || x.Id == 132 || x.Id == 161 /*film my story competition or miss wheelchair usa*/));
            if (category == null)
                return NotFound();

            if (category.RedirectCategoryId != null)
                return RedirectToRoute("Feed", new { id = category.RedirectCategoryId, categoryName = category.RedirectCategory.Name.ToCanonical() });

            if (category.Name.ToCanonical() != categoryName)
                return RedirectToRoutePermanent("Feed", new { id, categoryName = category.Name.ToCanonical() });

            const int defaultCount = 19;
            var count = defaultCount;
            if (card != null && card + 1 >= defaultCount)
            {
                count = ((card.Value + 1) / 10 + 1) * 10;
            }

            var model = await _searchLogic.GetFeedAsync(id, GetCurrentUserId(), count, defaultCount);
            if (model.Stories.Count == 0)
                return NotFound();

            model.SelectedStoryId = card ?? 0;
            SetDifferentCardStyles(model.Stories);

            return IsMobileDevice() ? View("CategoryMobile", model) : View(model);
        }

        public async Task<ActionResult> CategoryMore(int categoryId, int offset, int count)
        {
            var stories = await _searchLogic.GetStoriesFromDbAsync(categoryId, count, offset);
            SetDifferentCardStyles(stories);

            return stories.Any() ?
                    IsMobileDevice() ?
                         PartialView("_CategoryMobileCards", new FeedCategoryModel {Stories = stories}) :
                         (ActionResult)PartialView("/Views/Home/MoreContentImFollowing.cshtml", stories) :
                    NotFound();
        }

        private void SetDifferentCardStyles(List<StoryCardModel> stories)
        {
            stories.Index().ForEach(kv =>
            {
                kv.Value.IsImageCard = kv.Key % 2 == 0 || kv.Value.IsProductsReviewed;
                kv.Value.IsDarkTheme = kv.Key % 3 == 0;
                kv.Value.ShouldShowLimitation = true;
            });
        }
    }
}