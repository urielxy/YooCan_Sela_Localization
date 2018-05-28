using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Alto.Dal;
using Microsoft.AspNetCore.Http;

namespace Alto.Web.Middlewares
{
    public class EnrichSerilogUserMiddleware
    {
        private readonly RequestDelegate _next;

        public EnrichSerilogUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, AltoDbContext context)
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