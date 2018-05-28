using System.Collections.Generic;

namespace Yooocan.Models.Feeds
{
    public class FeedCategoriesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<FeedCategoriesModel> SubCategories { get; set; }
    }
}
