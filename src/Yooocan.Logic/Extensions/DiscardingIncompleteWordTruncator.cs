using System;
using System.Linq;
using Humanizer;

namespace Yooocan.Logic.Extensions
{
    public class DiscardingIncompleteWordTruncator : ITruncator
    {
        public string Truncate(string value, int length, string truncationString, TruncateFrom truncateFrom = TruncateFrom.Right)
        {
            if (value == null)
                return null;

            if (value.Length == 0)
                return value;

            if (value.Length < length) return value;

            if (truncationString.Length > length) throw new ArgumentOutOfRangeException(nameof(truncationString), "cannot be longer than the max length");

            var words = value.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            int currentLength = 0;
            words = truncateFrom == TruncateFrom.Right ? words : words.Reverse().ToArray();
            foreach (var word in words)
            {
                if (currentLength + word.Length + truncationString.Length > length) break;
                currentLength += word.Length + 1;// +1 to add space character for length computation.
            }
            if (currentLength == 0) currentLength = length; // this is a special case where first word is longer than length, it will break it on characters.
            else currentLength = truncateFrom == TruncateFrom.Right
                ? currentLength - 1  // -1 removes the extra space character on Right direction.
                : currentLength + 1; // +1 removes the extra space character on Left direction.

            return truncateFrom == TruncateFrom.Right
                ? value.Substring(0, currentLength) + truncationString
                : truncationString + value.Substring(value.Length - currentLength);

        }
    }
}