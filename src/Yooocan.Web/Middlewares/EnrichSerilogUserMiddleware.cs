using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Yooocan.Dal;

namespace Yooocan.Web.Middlewares
{
    public class EnrichSerilogUserMiddleware
    {
        private readonly RequestDelegate _next;

        public EnrichSerilogUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ApplicationDbContext context)
        {
            var username = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            IDisposable property = null;
            if (username != null)
            {
                property = EnrichLoggerEnricher.AddIdentityContext(username);
            }
            using (property)
            {
                await _next(httpContext);
            }
        }
    }
}