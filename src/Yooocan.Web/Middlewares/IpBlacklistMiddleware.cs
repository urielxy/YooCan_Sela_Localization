using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Yooocan.Web.Utils;

namespace Yooocan.Web.Middlewares
{
    public class IpBlacklistMiddleware
    {
        readonly RequestDelegate _next;
        private static List<string> _blacklist = new List<string> { "54.159.246.36" };

        public IpBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var clientIp = NetworkHelper.GetIpAddress(context.Request);
            if(_blacklist.Contains(clientIp))
            {
                Log.Warning("request from {ip} was blocked because it's in the blacklist", clientIp);
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }            
            await _next.Invoke(context);
        }
    }
}