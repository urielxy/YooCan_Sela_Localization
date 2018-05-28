using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;

namespace Yooocan.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly SearchLogic _searchLogic;
        private readonly IHomeLogic _homeLogic;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, IMapper mapper,
            UserManager<ApplicationUser> userManager,
            SearchLogic searchLogic, IHomeLogic homeLogic) : base(context, logger, mapper,
            userManager)
        {
            _searchLogic = searchLogic;
            _homeLogic = homeLogic;
        }

        [ResponseCache(Duration = 20, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> Index()
        {
            if (IsMobileDevice())
            {
                var mobileModel = await _homeLogic.GetMobileModelAsync();

                return View("SimpleHomeMobile", mobileModel);
            }

            var model = await _homeLogic.GetModelAsync();
            return View(model);
        }

        [ResponseCache(Duration = 20, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> Stories()
        {
            if (!IsMobileDevice())
                return RedirectToAction(nameof(Index));
            var model = await _homeLogic.GetMobileModelAsync();
            return View("HomeMobile", model);
        }

        [ResponseCache(Duration = 20, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult> ContentImFollowing()
        {
            var userId = GetCurrentUserId();
            var model = await _homeLogic.GetContentImFollowingAsync(userId, 23);

            return PartialView(model);
        }

        public async Task<ActionResult> MoreContentImFollowing(DateTime maxDate, int lastId, int count)
        {
            var userId = GetCurrentUserId();
            var model = await _homeLogic.GetContentImFollowingAsync(userId, count, maxDate, lastId);

            return model.Stories.Any() ? (ActionResult)PartialView("_StoriesBackgroundImageCards", model.Stories) : NotFound();
        }

        public async Task<ActionResult> MoreStories(DateTime maxDate, int lastId, int count)
        {
            var stories = await _homeLogic.GetStoriesFromDb(count, maxDate, lastId);

            return stories.Any() ? (ActionResult)PartialView(stories) : NotFound();
        }

        [Authorize]
        public async Task<IActionResult> GetMainFeedHistory()
        {
            var userId = GetCurrentUserId();
            var results = await _searchLogic.GetReadHistoryForMainFeedAsync(userId);

            return Json(results);
        }

        [Route("About", Name = "About")]
        public IActionResult About()
        {
            return View();
        }

        [Route("TermsOfUse", Name = "TermsOfUse")]
        [Route("Home/TermsOfUse", Name = "HomeTermsOfUse")]
        public ActionResult TermsOfUse()
        {
            if (Request.GetDisplayUrl().Contains("/Home"))
                return RedirectToRoutePermanent("TermsOfUse");

            return View();
        }

        [Route("WhyAlto")]
        public IActionResult WhyAlto()
        {
            return RedirectToAction("About");
        }

        [Route("Privacy", Name = "Privacy")]
        [Route("Home/Privacy", Name = "HomePrivacy")]
        [Route("PrivacyPolicy")] //from alto
        public ActionResult Privacy()
        {
            if (Request.GetEncodedPathAndQuery() != "/Privacy")
                return RedirectToRoutePermanent("Privacy");

            return View();
        }

        [Route("FAQ", Name = "FAQ")]
        public ActionResult Faq()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult PageNotFound()
        {
            return View();
        }

        public IActionResult Error404()
        {
            return View("PageNotFound");
        }

        [Route("ReelAbilities", Name = "ReelAbilities")]
        public IActionResult ReelAbilities()
        {
            return View();
        }

        [Route("Technologies")]
        public IActionResult Technologies()
        {
            return View();
        }

        public ActionResult ExcludeFromTracking()
        {
            if (!Request.Cookies.ContainsKey("removeTracking"))
                Response.Cookies.Append("removeTracking", "true",
                    new CookieOptions { Expires = DateTimeOffset.MaxValue });

            return Content("<html><body><h1>You will not be tracked from now on...</h1></body></html>", "text/html");
        }
    }
}