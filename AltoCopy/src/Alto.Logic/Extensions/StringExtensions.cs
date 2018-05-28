using System.Text.RegularExpressions;

namespace Alto.Logic.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex NonWordOrSpaceRegex = new Regex(@"[^\w ]", RegexOptions.Compiled);
        private static readonly Regex SpaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex StripHtmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        public static string ToCanonical(this string str)
        {
            var result = NonWordOrSpaceRegex.Replace(str.Trim().ToLower(), "");
            result = SpaceRegex.Replace(result, "-");
            return result;
        }

        public static string CamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str))
                str = str.Substring(0, 1).ToLower() + str.Substring(1);

            return str;
        }

        public static string FirstLetterToUpper(this string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public static string StripHtml(this string str)
        {
            return StripHtmlRegex.Replace(str, string.Empty);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
    }
}