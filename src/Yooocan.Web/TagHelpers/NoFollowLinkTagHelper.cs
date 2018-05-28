using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Yooocan.Web.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "no-follow-external", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class NoFollowLinkTagHelper : TagHelper
    {
        public string Href { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.SetAttribute("href", Href);
            if (Href.StartsWith("http"))
            {
                output.Attributes.SetAttribute("rel", "nofollow noopener");
                output.Attributes.SetAttribute("target", "_blank");
            }
            output.Attributes.RemoveAt(output.Attributes.IndexOfName("no-follow-external"));

            base.Process(context, output);
        }
    }
}
