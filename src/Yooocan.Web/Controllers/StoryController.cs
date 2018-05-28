using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
using Yooocan.Logic.Extensions;
using Yooocan.Web.Utils;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers
{
    public class StoryController :BaseController
    {
        private readonly IStoryLogic _storyLogic;
        private readonly ICategoriesLogic _categoriesLogic;
        private readonly ILimitationLogic _limitationLogic;
        private readonly IGoogleAnalyticsLogic _googleAnalyticsLogic;
        private readonly HtmlSanitizer _htmlSanitizer;

        public StoryController(ApplicationDbContext context, ILogger<StoryController> logger, IMapper mapper, UserManager<ApplicationUser> userManager, 
            IStoryLogic storyLogic, ICategoriesLogic categoriesLogic, ILimitationLogic limitationLogic, 
            IGoogleAnalyticsLogic googleAnalyticsLogic, HtmlSanitizer htmlSanitizer) : base(context, logger, mapper, userManager)
        {
            _storyLogic = storyLogic;
            _categoriesLogic = categoriesLogic;
            _limitationLogic = limitationLogic;
            _googleAnalyticsLogic = googleAnalyticsLogic;
            _htmlSanitizer = htmlSanitizer;
        }

        [HttpGet]
        [Route("Story/{id:int}/{title?}", Name = "Story")]
        public async Task<ActionResult> Index(int id, string title)
        {
            var model = await _storyLogic.GetStoryModelForPage(id);
            if (model == null)
                return NotFound();

            if (title != model.Title.ToCanonical())
                return RedirectToRoutePermanent("story", new {id, Title = model.Title.ToCanonical()});

            //todo: refactor all thisnon efficient stuff.
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var userId = user.Id;
                model.IsLoggedIn = true;
                user = await Context.Users
                    .Include(x => x.Follows)
                    .Where(x => x.Id == userId).SingleAsync();

                model.IsFollowed = user.Follows.Any(x => x.FollowedUserId == model.AuthorId && !x.IsDeleted);
                model.IsLiked = Context.StoryLikes.Any(x => x.StoryId == id && x.UserId == userId && !x.IsDeleted);
                Context.ReadHistories.Add(new ReadHistory
                                          {
                                              UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                                              StoryId = id
                                          });
                await Context.SaveChangesAsync();
            }

            if (Request.Cookies.ContainsKey($"promtShareStory-{model.Id}") && model.AuthorId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                ViewBag.ShowShareAfterPublish = true;
                Response.Cookies.Delete($"promtShareStory-{model.Id}");
            }
            if (model.RelatedProducts.Any(x => !string.IsNullOrEmpty(x.AmazonId)))
            {
                ViewBag.ShowingAmazonProducts = true;
            }

            return View(model);
        }

        public async Task<ActionResult> LoadStories(int subCategoryId, int skip)
        {
            var stories = await Context.Stories
                .Where(x => x.IsPublished && x.StoryCategories.Any(sc => sc.CategoryId == subCategoryId && sc.IsPrimary && !sc.IsDeleted))
                .OrderByDescending(x => x.InsertDate)
                .Skip(skip)
                .Take(5)
                .Include(x => x.User)
                .Include(x=> x.Images)
                .ToListAsync();
            var model = Mapper.Map<List<StoryCardModel>>(stories);
            return Json(model);
        }

        [Authorize]
        public async Task<ActionResult> Create(bool competition = false)
        {
            var user = await GetCurrentUserAsync();
            //if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
            //{
            //    return RedirectToAction("EditBio", "User", new {returnUrl = Request.ToUri().PathAndQuery});
            //}
            ViewBag.AboutMe = user.AboutMe;
            var model = new UploadStoryModel
                        {
                            CategoriesOptions = await _categoriesLogic.GetCategoriesForStoryAsync(),
                            LimitationsOptions = await _limitationLogic.GetLimitationsAsync(),
                            UserLongitude = user.Longitude,
                            UserLatitude = user.Latitude,
                            UserInstagramUserName = user.InstagramUserName,
                            UserFacebookPageUrl = user.FacebookPageUrl,
                            IsInCompetition = competition
                        };

            return View(model);
        }

        [Authorize]
        [Route("Story/Edit/{id:int}/{title?}")]
        public async Task<ActionResult> Edit(int id)
        {
            var story = Context.Stories
                .Include(x => x.User)
                .Include(x => x.Images)
                .Include(x => x.StoryCategories)
                .Include(x => x.StoryLimitations)
                .Include(x => x.Paragraphs)
                .AsNoTracking()
                .Single(x => x.Id == id);

            if (story == null)
                return NotFound();

            if (!AuthorizeEdit(story))
                return Unauthorized();

            var model = Mapper.Map<UploadStoryModel>(story);

            model.CategoriesOptions = await _categoriesLogic.GetCategoriesForStoryAsync();
            model.LimitationsOptions = await _limitationLogic.GetLimitationsAsync();            

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("Story/Edit/{id:int}/{title?}")]
        public async Task<ActionResult> Edit(UploadStoryModel model)
        {
            if (ModelState.IsValid)
            {
                var storyFromDb = Context.Stories.Single(x => x.Id == model.Id);
                if (!AuthorizeEdit(storyFromDb))
                    return Unauthorized();

                var story = await _storyLogic.EditStoryAsync(model);
                return RedirectToRoute("Story", new {id = story.Id, title = model.Title.ToCanonical()});
            }

            return BadRequest(ModelState);
        }

        private bool AuthorizeEdit(Story story)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("ContentAdmin"))
            {
                if (story.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    Logger.LogError($"User {User.Identity.Name} tried to edit story {story.Id} {story.Title}");
                    return false;
                }
            }
            return true;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UploadStoryModel model)
        {            
            if (ModelState.IsValid)
            {
                var story = await _storyLogic.UploadStoryAsync(model, User.FindFirstValue(ClaimTypes.NameIdentifier));
                _googleAnalyticsLogic.TrackEvent(User.FindFirst(ClaimTypes.NameIdentifier).Value, "Create story", "Create story", "User story");
                Response.Cookies.Append($"promtShareStory-{story.Id}", story.Id.ToString());
                var redirectPromoPage = model.Template == "tips" ? "tipsthankyou" : "storysubmitted";
                return Redirect($"http://promo.yoocanfind.com/{redirectPromoPage}?userId={User.FindFirstValue(ClaimTypes.NameIdentifier)}" +
                                $"&returnUrl={WebUtility.UrlEncode(Url.Action("Index", new { id = story.Id }))}");          
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpGet]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Preview(UploadStoryModel model)
        {
            var user = await GetCurrentUserAsync();
            
            var viewModel = Mapper.Map<PreviewStoryModel>(model);
            if (string.IsNullOrEmpty(model.HeaderImageUrl))
                viewModel.HeaderImageUrl =  (await Context.Categories.FirstOrDefaultAsync(x => model.Categories.Contains(x.Id)))?.HeaderPictureUrl;
            foreach (var paragraph in model.Paragraphs)
            {
                paragraph.Text = _htmlSanitizer.SanitizeStory(paragraph.Text);
            }

            viewModel.AuthorName = $"{user.FirstName} {user.LastName}";
            viewModel.AuthorAboutMe = user.AboutMe;
            viewModel.AuthorProfileUrl = user.PictureUrl;
            
            return PartialView("Preview", viewModel);
        }

        [NonAction]
        public ActionResult InternalCreate()
        {
            var categoriesList = Context.Categories
                .Where(x => x.ParentCategoryId != null)
                .OrderBy(x => x.ParentCategory.Name)
                .ThenBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Group = new SelectListGroup { Name = x.ParentCategory.Name }
                }).ToList();

            foreach (var categories in categoriesList.GroupBy(x => x.Group.Name))
            {
                foreach (var selectListItem in categories)
                {
                    selectListItem.Group = categories.First().Group;
                }
            }

            categoriesList.Insert(0, new SelectListItem { Text = "Select category/ies", Disabled = true });
            ViewBag.CategoriesList = categoriesList;

            var limitationsList = Context.Limitations
                .Where(x => x.ParentLimitationId == null)
                .OrderBy(x => x.ParentLimitation.Name)
                .ThenBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Group = new SelectListGroup { Name = x.ParentLimitation.Name }
                }).ToList();


            foreach (var limitations in limitationsList.GroupBy(x => x.Group.Name))
            {
                foreach (var selectListItem in limitations)
                {
                    selectListItem.Group = limitations.First().Group;
                }
            }

            limitationsList.Insert(0, new SelectListItem { Text = "Select limitation/s" });
            ViewBag.LimitationsList = limitationsList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonAction]
        public async Task<ActionResult> InternalCreate(InternalCreateStoryModel model)
        {
            var uppercaseEmail = model.Email.ToUpper().Trim();
            ApplicationUser user = Context.Users.FirstOrDefault(x => x.NormalizedEmail == uppercaseEmail);
            if (user == null)
            {
                user = new ApplicationUser
                           {
                               AboutMe = model.AboutMe.Trim(),
                               Email = model.Email.Trim(),
                               EmailConfirmed = false,
                               FirstName = model.FirstName.Trim(),
                               LastName = model.LastName.Trim(),
                               Location = model.Location.Trim(),
                               PictureUrl = model.PictureUrl.Trim(),
                               UserName = model.Email.Trim()
                           };
                var result = await UserManager.CreateAsync(user);
                if (!result.Succeeded)
                    throw new InvalidOperationException(string.Join(",", result.Errors.Select(x => x.Description)));
            }

            var uploadModel = Mapper.Map<UploadStoryModel>(model);

            var story = await _storyLogic.UploadStoryAsync(uploadModel, user.Id);
            _googleAnalyticsLogic.TrackEvent(null, "Create story", "Create story", "Internal create story");

            return RedirectToAction("Index", "Story", new {id = story.Id});
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PublishStory(bool showDeleted = false, bool showCompetitionEntries = false)
        {
            var model = await Context.Stories
                .Where(x => (((!x.IsPublished && !showDeleted && !x.IsDeleted) || (showDeleted && x.IsDeleted)) && !showCompetitionEntries) 
                            || (showCompetitionEntries && x.IsInCompetition && !x.IsDeleted))
                .OrderByDescending(x => x.Id)
                .Select(x => new PublishStoryModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    AuthorName = x.User.FirstName + " " + x.User.LastName
                }).ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PublishStory(int storyId)
        {
            var title = Context.Stories.Where(x => x.Id == storyId).Select(x => x.Title).Single();
            var storyUrl = Url.RouteUrl("Story", new { id = storyId, title = title.ToCanonical() }, HttpContext.Request.Scheme);
            //todo: make this job run on a web job, instead of web app
            await _storyLogic.ApproveStoryAsync(storyId, storyUrl);
            return RedirectToAction(nameof(PublishStory));
        }

        [HttpPost]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> Delete(int storyId)
        {
            await _storyLogic.ToggleDeleteStoryAsync(storyId);
            return RedirectToAction(nameof(PublishStory));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetStoryProducts(int storyId)
        {
            ViewBag.StoryId = storyId;

            var products = await Context.StoryProducts
                .Where(x => x.StoryId == storyId)
                .Select(x =>
                    new SetStoryProductModel
                    {
                        Id = x.ProductId,
                        Name = x.Product.Name,
                        VendorName = x.Product.Name,
                        PrimaryImageUrl = x.Product.Images.Where(pi => pi.Type == ImageType.Primary && !pi.IsDeleted)
                            .Select(pi => pi.CdnUrl ?? pi.Url)
                            .FirstOrDefault(),
                        IsUsedInStory = x.IsUsedInStory
                    }).ToListAsync();

            return PartialView("_SetStoryProducts", products);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<IActionResult> SetStoryProducts(int storyId, IEnumerable<SetStoryProductModel> products)
        {
            await _storyLogic.SetRelatedProductsAsync(storyId, products);
            return RedirectToRoute("Story", new
                                            {
                                                id = storyId
                                            });
        }

        [Authorize]
        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> Like(int id)
        {
            await _storyLogic.LikeAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));

            return NoContent();
        }

        [Authorize]
        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> UnLike(int id)
        {
            await _storyLogic.UnLikeAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> SetHotArea(int storyId, string cdnUrl, float left, float top)
        {
            var parsedUrl = new Uri(cdnUrl);
            var fileName = parsedUrl.AbsolutePath.Substring(parsedUrl.AbsolutePath.LastIndexOf('/') + 1);
            var image = await Context.StoryImages.Where(x => x.StoryId == storyId && x.CdnUrl.Contains(fileName) && !x.IsDeleted).SingleAsync();
            image.HotAreaLeft = left;
            image.HotAreaTop = top;

            await Context.SaveChangesAsync();
            return NoContent();
        }
    }
}