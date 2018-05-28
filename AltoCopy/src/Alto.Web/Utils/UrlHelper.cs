using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Alto.Web.Utils
{
    public static class UrlHelper
    {
        public static string GetUrlWithRemovedQueryParams(HttpRequest request, params string[] queryParams)
        {
            var path = request.Path.Value;
            var queryString = request.QueryString.ToUriComponent().Replace("?", "");
            foreach (var param in queryParams)
            {
                queryString = Regex.Replace(queryString, $"&{param}(=[^&]*)?|^{param}(=[^&]*)?&?", "");
            }
            return string.IsNullOrWhiteSpace(queryString) ? path : $"{path}?{queryString}";
        }
    }
}
