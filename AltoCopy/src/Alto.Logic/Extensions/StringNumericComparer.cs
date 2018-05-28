using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Alto.Logic.Extensions
{
    public class StringNumericComparer : IComparer<string>
    {
        readonly Regex _regex = new Regex(@"^(\d+(\.\d+)?)", RegexOptions.Compiled);
        public int Compare(string s1, string s2)
        {
            var value1 = _regex.Match(s1).Groups[0].Value;
            var value2 = _regex.Match(s2).Groups[0].Value;
            if (value1.Length > 0 && value2.Length > 0)
                return decimal.Parse(value1).CompareTo(decimal.Parse(value2));

            return string.Compare(s1, s2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}