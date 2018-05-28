namespace Yooocan.Logic
{
    public class HtmlSanitizer
    {
        public string SanitizeStory(string text)
        {
            var storyHtmlSantizer = new Ganss.XSS.HtmlSanitizer();
            storyHtmlSantizer.AllowedTags.Clear();
            storyHtmlSantizer.AllowedTags.Add("br");
            storyHtmlSantizer.AllowedTags.Add("em");
            storyHtmlSantizer.AllowedTags.Add("strong");

            storyHtmlSantizer.AllowedCssProperties.Clear();
            storyHtmlSantizer.AllowedCssProperties.Add("text-decoration");
            //_storyHtmlSantizer.AllowedTags.Add("a");
            // _storyHtmlSantizer.AllowedSchemes.Add("mailto");

            storyHtmlSantizer.RemovingTag += (sender, args) =>
            {
                args.Tag.OuterHtml = storyHtmlSantizer.Sanitize(args.Tag.InnerHtml);
                args.Cancel = true;
            };

            text = storyHtmlSantizer.Sanitize(text).Trim();
            return text;
        }

        public string SanitizeHtml(string text)
        {
            var sanitizer = new Ganss.XSS.HtmlSanitizer();
            text = sanitizer.Sanitize(text);
            return text;
        }
    }
}