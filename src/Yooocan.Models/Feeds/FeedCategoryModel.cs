using System.Collections.Generic;

namespace Yooocan.Models.Feeds
{
    public class FeedCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HeaderImageUrl { get; set; }
        public string MobileHeaderImageUrl { get; set; }
        public string CategoryColor { get; set; }
        public bool IsFollowed { get; set; }
        public List<StoryCardModel> Stories { get; set; }
        public List<FeedMenuModel> Categories { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
        public int SelectedStoryId { get; set; }
    }

    public class FeedMenuModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public List<FeedMenuModel> SubCategories { get; set; }
    }
}