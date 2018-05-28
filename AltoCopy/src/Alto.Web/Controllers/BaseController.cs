using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain;
using Alto.Logic.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Alto.Web.Controllers
{
    [RequireHttps(Permanent = true)]
    public class BaseController : Controller
    {
        protected AltoDbContext Context { get; }
        protected UserManager<AltoUser> UserManager { get; }
        protected ILogger<BaseController> Logger { get; }
        protected IMapper Mapper { get; set; }

        public BaseController(AltoDbContext context, MapperConfiguration mapperConfiguration, UserManager<AltoUser> userManager, ILogger<BaseController> logger)
        {
            Context = context;
            UserManager = userManager;
            Logger = logger;
            Mapper = mapperConfiguration.CreateMapper();
        }

        protected int? GetCurrentUserId()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        protected Task<AltoUser> GetCurrentUserAsync()
        {
            return UserManager.GetUserAsync(HttpContext.User);
        }

        protected bool IsMobileDevice()
        {
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

        protected BadRequestObjectResult ReturnAjaxErrors()
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
            if (Url.IsLocalUrl(returnUrl) || !string.IsNullOrEmpty(returnUrl) &&
                                              (returnUrl.StartsWith("https://yoocanfind.com") ||
                                               returnUrl.StartsWith("https://localhost")))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
