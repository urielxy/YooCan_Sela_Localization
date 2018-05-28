using System.Collections.Generic;
using Yooocan.Models.Blog;
using Yooocan.Models.Products;

namespace Yooocan.Models.New.Home
{
    public class HomeModel
    {
        public List<FeaturedStoryHeader> HeaderStories { get; set; }
        public List<StoryCardModel> StoryCards { get; set; }
        public PostModel FeatureBlogPost { get; set; }
    }

    public class NewUserHomeModel
    {
        public FeaturedStoryHeader HeaderStory { get; set; }
        public List<FeaturedCategory> FeaturedCategories { get; set; }
        public List<StoryCardModel> StoryCards { get; set; }
        public List<ProductCardModel> ProductCards { get; set; }
    }
}