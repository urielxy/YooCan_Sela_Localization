using Humanizer;

namespace Yooocan.Logic.Extensions
{
    public static class TruncatorExt
    {
        public static ITruncator DiscardingIncompleteWord = new DiscardingIncompleteWordTruncator();
    }
}