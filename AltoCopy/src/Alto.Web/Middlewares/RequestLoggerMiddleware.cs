using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Alto.Web.Middlewares
{
    public class RequestLoggerMiddleware
    {
        private static readonly List<string> ExcludedProperties = new List<string> { "password", "confirmpassword" };
        private readonly RequestDelegate _next;

        public RequestLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Log.Information("Received headers {@Headers} {@Body}", context.Request.Headers.Select(x => $"{x.Key}:{x.Value.ToString()}"),
                context.Request.ContentLength > 0 && context.Request.HasFormContentType
                    ? context.Request.Form.Where(x =>
                    {
                        var key = x.Key.ToLower();
                        return !ExcludedProperties.Contains(key);
                    })
                        .Select(x => $"{x.Key}:{x.Value.ToString()}")
                    : null);
            await _next.Invoke(context);
        }
    }
}