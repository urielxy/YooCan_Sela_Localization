using System.Collections.Generic;
using Alto.Models.Cards;
using Microsoft.Azure.Search.Models;

namespace Alto.Models.Search
{
    public class SearchResultModel
    {
        public List<BenefitCardModel> Benefits { get; set; }
        public FacetResults BenefitFacets { get; set; }
        public List<ProductCardModel> Products { get; set; }
        public FacetResults ProductFacets { get; set; }
    }
}
