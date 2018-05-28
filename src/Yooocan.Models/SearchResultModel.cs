using System.Collections.Generic;
using Yooocan.Models.Cards;
using Yooocan.Models.Products;

namespace Yooocan.Models
{
    public class SearchResultModel
    {
        public List<ProductCardModel> Products { get; set; }
        public List<StoryCardModel> Stories { get; set; }
        public List<RelatedServiceProviderModel> ServiceProviders { get; set; }
        public List<BenefitCardModel> Benefits { get; set; }
        public string Query { get; set; }
        public List<int> LimitationIds { get; set; }
        public int? CategoryId { get; set; }

        public SearchResultModel()
        {
            Products = new List<ProductCardModel>();
            Stories = new List<StoryCardModel>();
            ServiceProviders = new List<RelatedServiceProviderModel>();
            Benefits = new List<BenefitCardModel>();
        }
    }
}