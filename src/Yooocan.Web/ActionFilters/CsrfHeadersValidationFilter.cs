using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Yooocan.Web.ActionFilters
{
    public class CsrfHeadersValidationFilter : IActionFilter
    {
        public CsrfHeadersValidationFilter(ILogger<CsrfHeadersValidationFilter> logger, IHostingEnvironment environment)
        {
            Logger = logger;
            Environment = environment;
        }

        public ILogger<CsrfHeadersValidationFilter> Logger { get; }
        public IHostingEnvironment Environment { get; }

        public void OnActionExecuted(ActionExecutedContext context)
        {            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Request.Headers;
            const string domainName = "https://yoocanfind.com";
            if (Environment.IsProduction() && headers["Origin"].ToString() != domainName && !headers["Referer"].ToString().StartsWith($"{domainName}/"))
            {
                Logger.LogWarning("Potential CSRF blocked from origin: {origin}, referer: {referrer}", headers["Origin"].ToString(), headers["Referer"].ToString());
                context.Result = new BadRequestResult();
            }
        }
    }
}
