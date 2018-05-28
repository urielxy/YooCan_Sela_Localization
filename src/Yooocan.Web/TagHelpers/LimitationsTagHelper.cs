using System;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Yooocan.Dal;

namespace Yooocan.Web.TagHelpers
{
    public class LimitationsTagHelper : TagHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "LimitationTagHelperResult";
        public LimitationsTagHelper(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";
            output.Attributes.Add("class", "dropdown-menu");
            var content = _memoryCache.GetOrCreate(CacheKey, entry =>
            {
                var classes = new[] {"default", "primary", "success", "info", "warning", "danger"};
                var element = @"<li>
    <div class=""checkbox checkbox-{0}"">
        <input type=""checkbox"" id=""limitation-checkbox-{1}"" value=""{2}"" name=""limitations"" data-name=""{3}"">
        <label for=""limitation-checkbox-{1}"" style=""color:black"">{3}</label>
    </div>
</li>";
                var limitationOptions = _context.Limitations
                    .Where(x => x.ParentLimitationId == null)
                    .OrderBy(x => x.Name == "Other")
                    .ThenBy(x => x.Name)
                    .ToList()
                    //.Select(x => $"<option value=\"{x.Id}\">{x.Name}</option>").ToList();
                    .Select((x, index) => string.Format(element,
                        classes[index%classes.Length],
                        index,
                        x.Id,
                        x.Name)).ToList();

                var result = string.Join(Environment.NewLine, limitationOptions);
                entry.SetAbsoluteExpiration(TimeSpan.FromDays(1));
                entry.SetValue(result);
                return result;
            });

            output.Content.SetHtmlContent(content);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
