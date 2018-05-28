using System.Collections.Generic;
using System.Linq;

namespace Yooocan.Models.Feeds
{
    public class FeedModel
    {
        public Dictionary<int, List<StoryCardModel>> Categories { get; set; }
        public string MainCategoryName { get; set; }
        public int MainCategoryId { get; set; }
        public string HeaderPictureUrl { get; set; }
        public bool IsMainCategoryShopActive { get; set; }
    }
}