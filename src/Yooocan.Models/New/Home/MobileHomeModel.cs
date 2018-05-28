using System.Collections.Generic;
using Yooocan.Models.Blog;
using Yooocan.Models.Products;

namespace Yooocan.Models.New.Home
{
    public class MobileHomeModel
    {
        public ProductCardModel FeaturedProduct { get; set; }
        public FeaturedStoryHeader FeaturedStory { get; set; }
        public List<CategoryModel> ShopCategories { get; set; }
        public List<FeaturedStoryHeader> LatestStoriesPerCategory { get; set; }
        public PostModel FeatureBlogPost { get; set; }
    }
}
