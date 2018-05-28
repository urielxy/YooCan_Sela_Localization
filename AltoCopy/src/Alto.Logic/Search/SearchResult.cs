using System.Collections.Generic;
using Alto.Domain.Benefits;
using Alto.Domain.Products;
using Microsoft.Azure.Search.Models;

namespace Alto.Logic.Search
{
    public class SearchResult
    {
        public List<Benefit> Benefits { get; set; }
        public FacetResults BenefitFacets { get; set; }
        public List<Product> Products { get; set; }
        public FacetResults ProductFacets { get; set; }
    }
}
