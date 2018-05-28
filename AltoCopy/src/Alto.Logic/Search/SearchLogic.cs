using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain.Benefits;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Alto.Logic.Search
{
    public class SearchLogic
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SearchLogic> _logger;
        private readonly SearchServiceClient _searchClient;

        public SearchLogic(IServiceProvider serviceProvider, ILogger<SearchLogic> logger, SearchServiceClient searchClient)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _searchClient = searchClient;
        }

        public async Task<SearchResult> SearchAsync(string text, int? categoryId = null)
        {
            var benefitsTask = SearchBenefitsAsync(text, categoryId);
            var productsTask = SearchProductsAsync(text, categoryId);

            await Task.WhenAll(benefitsTask, productsTask);

            var result = benefitsTask.Result;

            result.Products = productsTask.Result.Products;
            result.ProductFacets = productsTask.Result.ProductFacets;

            return result;
        }

        private async Task<SearchResult> SearchBenefitsAsync(string text, int? categoryId = null)
        {
            var benefitsAzureSearchResult = await GetAzureSearchResult<BenefitIndexModel>(text, categoryId);

            var benefitIndexRows = benefitsAzureSearchResult.Results;
            var benefitIds = benefitIndexRows.OrderByDescending(x => x.Score)
                                            .Select(x => int.Parse(x.Document.BenefitId))
                                            .ToList();
            if (benefitIds.Count == 0)
                return new SearchResult { Benefits = new List<Benefit>() };

            using (var context = _serviceProvider.GetRequiredService<AltoDbContext>())
            {
                var benefits = await context.Benefits
                    .Include(x => x.Company)
                    .Include(x => x.Images)
                    .Include(x => x.Categories)
                    .Where(x => benefitIds.Contains(x.Id) && x.IsPublished && x.DeleteDate == null)
                    .AsNoTracking()
                    .ToListAsync();

                // http://stackoverflow.com/a/3946021/601179
                var benefitsOrderedByScore = (from id in benefitIds
                                              join benefit in benefits
                                              on id equals benefit.Id
                                              select benefit).ToList();

                return new SearchResult
                {
                    Benefits = benefitsOrderedByScore,
                    BenefitFacets = benefitsAzureSearchResult.Facets
                };
            }
        }

        private async Task<SearchResult> SearchProductsAsync(string text, int? categoryId = null)
        {
            var azureSearchResult = await GetAzureSearchResult<ProductIndexModel>(text, categoryId);
            var productIndexRows = azureSearchResult.Results;
            var ids = productIndexRows.OrderByDescending(x => x.Score)
                .Select(x => int.Parse(x.Document.ProductId))
                .ToList();
            if (ids.Count == 0)
                return new SearchResult { Benefits = new List<Benefit>() };

            using (var context = _serviceProvider.GetRequiredService<AltoDbContext>())
            {
                var products = await context.Products
                    .Include(x => x.Company)
                    .Include(x => x.Images)
                    .Where(x => ids.Contains(x.Id) && x.IsPublished && x.DeleteDate == null)
                    .AsNoTracking()
                    .ToListAsync();

                // http://stackoverflow.com/a/3946021/601179
                var productsOrderedByScore = (from id in ids
                                              join product in products
                                              on id equals product.Id
                                              select product).ToList();

                return new SearchResult
                {
                    Products = productsOrderedByScore,
                    ProductFacets = azureSearchResult.Facets
                };
            }
        }

        private async Task<DocumentSearchResult<T>> GetAzureSearchResult<T>(string text, int? categoryId) where T : class
        {
            string indexName;
            if (typeof(T) == typeof(BenefitIndexModel))
            {
                indexName = "benefits-simple";
            }
            else if (typeof(T) == typeof(ProductIndexModel))
            {
                indexName = "products";
            }
            else
            {
                throw new ArgumentException($"invalid type parameter {typeof(T)}, should be a search index model");
            }

            string filter = null;

            if (categoryId != null)
            {
                filter = $"CategoryIds/any(l: l eq '{categoryId}')";
            }

            //Use fuzzy search
            if (text != null && text.Length >= 3)
            {
                text = new string(text.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
                text = string.Join(" ",
                    text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(word => $"{word}~1"));
            }

            var azureStopwatch = new Stopwatch();
            azureStopwatch.Start();

            var azureSearchResult = await _searchClient.Indexes.GetClient(indexName)
                .Documents.SearchAsync<T>(text, new SearchParameters
                {
                    Top = 30,
                    Filter = filter,
                    QueryType = QueryType.Full,
                    Facets = new List<string> { "CategoryIds" }
                });

            azureStopwatch.Stop();
            _logger.LogInformation(76262562,
                    "Search in Azure for {index} took {ElapsedMilliseconds}", "benefits",
                    azureStopwatch.ElapsedMilliseconds);
            return azureSearchResult;
        }

        //public async Task SearchAsync(string text, float? latitude, float? longitude, int radius = 100)
        //{
        //    if (text.IsNullOrWhiteSpace())
        //        text = "*";

        //    var searchParameters = new SearchParameters();
        //    if (latitude != null && longitude != null)
        //    {
        //        searchParameters.Filter = $"geo.distance(Location, geography'POINT(-87.897000 42.167500)') lt {radius}";
        //    }
        //    await _searchClient.Indexes.GetClient("benefits").Documents
        //        .SearchAsync<Benefit>(text, searchParameters);
        //}
    }
}
