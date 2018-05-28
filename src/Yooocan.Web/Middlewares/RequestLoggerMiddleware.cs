using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog;

namespace Yooocan.Web.Middlewares
{
    public class RequestLoggerMiddleware
    {
        private static readonly List<string> _excludedProperties = new List<string>{"password", "confirmpassword"};
        readonly RequestDelegate _next;

        public RequestLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Log.Information("Received headers {@Headers} {@Body}", context.Request.Headers.Select(x => FormatHeader(x.Key, x.Value.ToString(), context)),
                context.Request.ContentLength > 0 && context.Request.HasFormContentType
                    ? context.Request.Form.Where(x =>
                        {
                            var key = x.Key.ToLower();
                            return !_excludedProperties.Contains(key);
                        })
                        .Select(x => $"{x.Key}:{x.Value.ToString()}")
                    : null);
            await _next.Invoke(context);
        }

        private string FormatHeader(string key, string value, HttpContext context)
        {
            if (string.Compare(key, "cookie", true) == 0)
                value = FilterCookies(context);
            return $"{key}:{value}";
        }

        private string FilterCookies(HttpContext context)
        {
            return string.Join(" ", context.Request.Cookies.Where(x => !x.Key.Contains("AspNetCore") || x.Key.Contains("Correlation"))
                                                           .Select(x => $"{x.Key}={x.Value};"));
        }
    }
}