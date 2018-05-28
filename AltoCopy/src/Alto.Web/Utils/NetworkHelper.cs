using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Alto.Web.Utils
{
    public class NetworkHelper
    {
        public static string GetIpAddress(HttpRequest request)
        {
            var forwarded = request.Headers["X-Forwarded-For"].FirstOrDefault();
            string ip;
            if (string.IsNullOrEmpty(forwarded))
            {
                ip = request.HttpContext.Connection.RemoteIpAddress.ToString() == "::1"
                    ? "localhost"
                    : request.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            else
            {
                ip = forwarded.Split(':').First();
            }
            return ip;
        }
    }
}
