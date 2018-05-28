using System.Collections.Generic;
using Yooocan.Models.New.Stories;

namespace Yooocan.Models
{
    public class PreviewStoryModel
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string AuthorLocation { get; set; }
        public string AuthorAboutMe { get; set; }
        public string AuthorProfileUrl { get; set; }
        public string HeaderImageUrl { get; set; }
        public string PrimaryImageUrl { get; set; }
        public string Title { get; set; }
        public string YouTubeId { get; set; }
        public string ActivityLocation { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string CategoryColor { get; set; }
        public string CategoryName { get; set; }
        public List<string> SubCategories { get; set; }
        public List<StoryParagraphModel> Paragraphs { get; set; }
        public List<string> Images { get; set; }

        public PreviewStoryModel()
        {
            SubCategories = new List<string>();
            Paragraphs = new List<StoryParagraphModel>();
            Images = new List<string>();
        }
    }
}