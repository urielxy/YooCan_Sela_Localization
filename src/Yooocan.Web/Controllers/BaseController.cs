using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic.Extensions;

namespace Yooocan.Web.Controllers
{
    [RequireHttps(Permanent = true)]
    public class BaseController : Controller
    {
        protected readonly UserManager<ApplicationUser> UserManager;
        public IMapper Mapper { get; }
        protected ApplicationDbContext Context { get; }
        protected ILogger<Controller> Logger { get; }

        public BaseController(ApplicationDbContext context, ILogger<Controller> logger, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            Mapper = mapper;
            Context = context;
            Logger = logger;
        }

        protected string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        protected Task<ApplicationUser> GetCurrentUserAsync()
        {
            return UserManager.GetUserAsync(HttpContext.User);
        }

        protected bool IsMobileDevice()
        {
            Response.Headers.Add("Vary", "User-Agent");
            var mobileDevice = Request.Headers.ContainsKey("User-Agent") &&
                               Request.Headers["User-Agent"].ToString().ToLower().Contains("mobi");
            return mobileDevice;
        }

        protected void LogModelStateErrors()
        {
            var state = ModelState.Where(x => x.Value.Errors.Any())
                    .Select(x => new KeyValuePair<string, object>(string.IsNullOrWhiteSpace(x.Key) ? "General" : x.Key,
                    string.Join(",", x.Value.Errors.Select(e => e.ErrorMessage))));
            Logger.Log(LogLevel.Warning, 123, state, null, null);
        }

        public BadRequestObjectResult ReturnAjaxErrors()
        {
            return BadRequest(ModelState
                .Where(x => x.Value.Errors.Any())
                .Select(x => new
                             {
                                 field = x.Key.CamelCase(),
                                 error = string.Join(", ", x.Value.Errors.Select(error => error.ErrorMessage))
                             }).ToList());
        }

        protected IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl) || (!string.IsNullOrEmpty(returnUrl) &&
                                              (returnUrl.StartsWith("https://yoocanfind.com") ||
                                               returnUrl.StartsWith("https://localhost"))))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        #region Helpers For Old Views

        protected ActionResult OldIframeContainer()
        {
            var controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            var actionName = ControllerContext.RouteData.Values["action"] + "Old";
            var id = ControllerContext.RouteData.Values["id"]?.ToString();
            var queryString = ControllerContext.HttpContext.Request.QueryString;

            ViewBag.Url = $"/{controllerName}/{actionName}/{id}{queryString}";
            return View("/Views/IframeView.cshtml");
        }

        protected ActionResult OldView(object model = null)
        {
            var controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            var actionName = ControllerContext.RouteData.Values["action"].ToString();
            var suffixLocation = actionName.LastIndexOf("Old", StringComparison.Ordinal);
            if (suffixLocation > -1)
            {
                actionName = actionName.Substring(0, suffixLocation);
            }

            return View($"/Views/Old/{controllerName}/{actionName}.cshtml", model);
        }

        #endregion
    }
}