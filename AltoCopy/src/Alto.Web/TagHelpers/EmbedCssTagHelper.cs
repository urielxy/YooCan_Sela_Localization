using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;

namespace Alto.Web.TagHelpers
{
    [HtmlTargetElement("embedCss")]
    public class EmbedCssTagHelper : TagHelper
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHostingEnvironment _hostingEnvironment;
        private const string CacheKeyPrefix = "EmbedCss_";
        public string Href { get; set; }

        public EmbedCssTagHelper(IMemoryCache memoryCache, IHostingEnvironment hostingEnvironment)
        {
            _memoryCache = memoryCache;
            _hostingEnvironment = hostingEnvironment;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var cacheKey = CacheKeyPrefix + Href;
            output.TagName = "style";
            var content = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                if (_hostingEnvironment.IsStaging() || _hostingEnvironment.IsProduction())
                {
                    Href += ".min";
                }
                Href += ".css";

                Href = Href.Replace('/', '\\');

                var path = _hostingEnvironment.WebRootPath + Href;
                var result = File.ReadAllText(path);
                entry.SetAbsoluteExpiration(_hostingEnvironment.IsStaging() || _hostingEnvironment.IsProduction() ? TimeSpan.FromHours(1) : TimeSpan.FromMilliseconds(1));
                entry.SetValue(result);
                return result;
            });

            output.Content.SetHtmlContent(content);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}