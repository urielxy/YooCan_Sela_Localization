using System;
using Microsoft.AspNetCore.Builder;
using Serilog.Context;

namespace Yooocan.Web.Middlewares
{
    public static class EnrichLoggerEnricher
    {
        public static IDisposable AddIdentityContext(string username)
        {
            return LogContext.PushProperty("username", username);
        }

        public static void EnrichLogger(this IApplicationBuilder app)
        {
            app.UseMiddleware<EnrichSerilogUserMiddleware>();
            app.UseMiddleware<RequestLoggerMiddleware>();
        }
    }
}