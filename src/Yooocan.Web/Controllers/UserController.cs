using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using System.Security;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Yooocan.Models;
using Microsoft.AspNetCore.JsonPatch;
using Yooocan.Enums.Notifications;
using Yooocan.Models.Users;
using Yooocan.Web.Utils;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserLogic _userLogic;
        private readonly IStoryLogic _storyLogic;
        private readonly IGoogleAnalyticsLogic _googleAnalyticsLogic;
        private readonly INotificationLogic _notificationLogic;
        private readonly ILimitationLogic _limitationLogic;

        public UserController(ApplicationDbContext context, ILogger<UserController> logger, IMapper mapper, UserManager<ApplicationUser> userManager,
            IUserLogic userLogic, IStoryLogic storyLogic, IGoogleAnalyticsLogic googleAnalyticsLogic, INotificationLogic notificationLogic, ILimitationLogic limitationLogic) : base(context, logger, mapper, userManager)
        {
            _userLogic = userLogic;
            _storyLogic = storyLogic;
            _googleAnalyticsLogic = googleAnalyticsLogic;
            _notificationLogic = notificationLogic;
            _limitationLogic = limitationLogic;
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public ActionResult ChangeFollowState(string userId)
        {
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var firstName = User.FindFirst(ClaimTypes.GivenName).Value;
            var lastName = User.FindFirst(ClaimTypes.Surname).Value;
            var imageUrl = User.FindFirst("picture")?.Value;
            var alreadyFollow = Context.Followers.Any(x => x.FollowerUserId == loggedInUserId && x.FollowedUserId == userId && !x.IsDeleted);

            if (!alreadyFollow)
            {
                Context.Followers.Add(new FollowerFollowed { FollowedUserId = userId, FollowerUserId = loggedInUserId });
                _googleAnalyticsLogic.TrackEvent(loggedInUserId, "User", "Follow", userId);

                var notification = _notificationLogic.CreateNotification(User, NotificationType.FollowerNew);
                notification.AddRecipient(userId);
                //currently not sending notification because there's nothing the receiver can do with the info - may need a sync api
                //_notificationLogic.SendNotificationAsync(notification);
            }
            else
            {
                _googleAnalyticsLogic.TrackEvent(loggedInUserId, "User", "Unfollow", userId);
                var followerFolloweds = Context.Followers.Where(x => x.FollowerUserId == loggedInUserId && x.FollowedUserId == userId && !x.IsDeleted).ToList();
                foreach (var followerFollowed in followerFolloweds)
                {
                    followerFollowed.IsDeleted = true;
                }
            }

            Context.SaveChanges();
            return NoContent();
        }

        public async Task<ActionResult> Edit(string id, string returnUrl)
        {
            if (id == null)
            {
                id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            if (User.FindFirst(ClaimTypes.NameIdentifier).Value != id && !User.IsInRole("Admin"))
            {
                throw new SecurityException($"{User.FindFirst(ClaimTypes.NameIdentifier).Value} is trying to edit other user({id}) bio");
            }
            var edittedUser = await Context.Users.SingleOrDefaultAsync(x => x.Id == id);
            if (edittedUser == null)
                return NotFound();

            var model = Mapper.Map<UserBioModel>(edittedUser);
            var stories = await Context.Stories
                .Where(x => x.UserId == id && !x.IsDeleted)
                .Include(x => x.Paragraphs)
                .Include(x => x.User)
                .Include(x => x.Images)
                .Include(x => x.StoryCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .ToListAsync();
            model.Stories = Mapper.Map<List<StoryCardModel>>(stories);
            if (!string.IsNullOrEmpty(returnUrl))
            {
                ModelState.AddModelError(string.Empty, "Please complete your profile first");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> Edit(UserBioModel userBio, string returnUrl = "/")
        {
            if (!ModelState.IsValid)
                return View(userBio);

            if (User.FindFirst(ClaimTypes.NameIdentifier).Value != userBio.Id && !User.IsInRole("Admin"))
            {
                throw new SecurityException($"{User.Identity.Name} is trying to edit other user({userBio.Id}) bio");
            }

            await _userLogic.EditBioAsync(userBio, User.FindFirst(ClaimTypes.NameIdentifier).Value != userBio.Id);
            return LocalRedirect(returnUrl);
        }

        [HttpPatch]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        [Route("/User/{id?}")]
        public async Task<IActionResult> Patch(string id, [FromBody]JsonPatchDocument<UserBioModel> patch)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            if (id == null)
                id = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (User.FindFirst(ClaimTypes.NameIdentifier).Value != id && !User.IsInRole("Admin"))
            {
                throw new SecurityException($"{User.FindFirst(ClaimTypes.NameIdentifier).Value} is trying to edit other user({id}) bio");
            }
            var edittedUser = await UserManager.FindByIdAsync(id);
            if (edittedUser == null)
                return NotFound();
            var originalModel = Mapper.Map<UserBioModel>(edittedUser);
            var patchedModel = originalModel.ShallowCopy();
            patch.ApplyTo(patchedModel);

            if (patchedModel.AboutMe != originalModel.AboutMe)
            {
                originalModel.AboutMe = patchedModel.AboutMe?.Trim();
            }
            if (patchedModel.FirstName != originalModel.FirstName)
            {
                originalModel.FirstName = patchedModel.FirstName?.Trim();
            }
            if (patchedModel.LastName != originalModel.LastName)
            {
                originalModel.LastName = patchedModel.LastName?.Trim();
            }
            if (patchedModel.PictureDataUri != null)
            {
                originalModel.PictureDataUri = patchedModel.PictureDataUri?.Trim();
            }
            if (patchedModel.HeaderImageDataUri != null)
            {
                originalModel.HeaderImageDataUri = patchedModel.HeaderImageDataUri?.Trim();
            }

            try
            {
                await _userLogic.EditBioAsync(originalModel, User.FindFirst(ClaimTypes.NameIdentifier).Value != originalModel.Id);
            }
            catch (DBConcurrencyException)
            {
                return StatusCode((int)HttpStatusCode.Conflict);
            }

            return NoContent();
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> SubscribeToNewsletter(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest();

            var alreadySubscribed = await _userLogic.SubscribeToNewsletterAsync(email, NetworkHelper.GetIpAddress(Request));
            if (alreadySubscribed)
                return NoContent();

            return Ok();
        }

        [Authorize]
        public ActionResult GetAvatar()
        {
            return Content(User.FindFirstValue("picture"));
        }

        public async Task<ActionResult> MyStories()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var stories = await _storyLogic.GetUserStoriesAsync(userId);

            var model = Mapper.Map<List<UserStoryModel>>(stories);
            return View(model);
        }

        public async Task<ActionResult> Following()
        {
            var model = await _userLogic.GetFollowingAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return View(model);
        }

        public async Task<ActionResult> Followers()
        {
            var model = await _userLogic.GetFollowersAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return View(model);
        }

        [Authorize]
        public async Task<ActionResult> CustomizeYourFeed(string returnUrl = null)
        {
            var model = new CustomizeFeedModel
                        {
                            Categories = await Context.Categories.Where(x => x.IsActiveForFeed && x.ParentCategoryId == null)
                                .OrderBy(x => x.Name)
                                .Select(x => new CustomizeFeedModel.CategoryModel
                                             {
                                                 Id = x.Id,
                                                 Name = x.Name,
                                                 RoundIcon = x.RoundIcon,
                                             })
                                .ToListAsync()
                        };


            var colors = await Context.Categories.Where(x => x.IsActiveForFeed && !string.IsNullOrEmpty(x.ShopBackgroundColor)).Select(x => x.ShopBackgroundColor).Distinct().ToListAsync();
            var limitations = await _limitationLogic.GetLimitationsAsync();
            // It's reference type and stored in memory, so need to clone the data.
            limitations = limitations.ToDictionary(x => x.Key, x => x.Value);

            if (limitations.ContainsValue("Other"))
                limitations.Remove(limitations.Single(x => x.Value == "Other").Key);

            model.Limitations = limitations.Select((x, index) => new CustomizeFeedModel.LimitationModel
                                                                 {
                                                                     Id = x.Key,
                                                                     Name = x.Value,
                                                                     Color = colors[index%colors.Count]
                                                                 }).ToList();
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<IActionResult> CustomizeYourFeed(List<int> categories, List<int> limitations)
        {
            var userId = GetCurrentUserId();
            var user = await Context.Users.Include(x => x.Categories)
                .Include(x => x.Limitations)
                .SingleAsync(x => x.Id == userId);

            foreach (var category in categories.Where(x => !user.Categories.Any(c => c.CategoryId == x && c.DeleteDate == null)))
            {
                user.Categories.Add(new CategoryFollower
                                    {
                                        CategoryId = category
                                    });
            }

            foreach (var limitation in limitations.Where(x => !user.Limitations.Any(l => l.LimitationId == x && l.DeleteDate == null)))
            {
                user.Limitations.Add(new LimitationFollower
                                     {
                                         LimitationId = limitation
                                     });
            }
            user.CustomizedFeedDone = true;
            await Context.SaveChangesAsync();
            return RedirectToLocal("/");
        }
    }
}