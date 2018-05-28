using Yooocan.Enums;
namespace Yooocan.Models.Admin
{
    public class FeaturedStoryModel
    {
        public int StoryId { get; set; }
        public FeaturedType FeaturedType { get; set; }
        public string Title { get; set; }
    }
}