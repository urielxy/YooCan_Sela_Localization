using System;
using Microsoft.Extensions.Logging;

namespace Yooocan.Logic.Extensions
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, int, int, int, int, long, Exception> _searchResult;

        static LoggerExtensions()
        {
            _searchResult = LoggerMessage.Define<string, int, int, int, int, long>(LogLevel.Information, (int)LoggingEvent.Search, "Search for {Term} yielded {StoriesCount}, {ProductsCount}, {ServiceProvidersCount}, {BenefitsCount} in {ElapsedMilliseconds}");
        }

        public static void LogSearchResults(this ILogger logger, string searchTerm, int storiesCount, int productsCount, int serviceProvidersCount, int benefitsCount, long elapsedMilliseconds)
        {
            _searchResult(logger, searchTerm, storiesCount, productsCount, serviceProvidersCount, benefitsCount, elapsedMilliseconds, null);
        }
    }
}
