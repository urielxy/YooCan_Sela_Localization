using System;
using System.Collections.Generic;
using Yooocan.Models.Products;

namespace Yooocan.Models.New.Stories
{
    public class StoryModel
    {        
        public int Id { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorLocation { get; set; }
        public string AuthorAboutMe { get; set; }
        public string AuthorProfileUrl { get; set; }
        public bool IsFollowed { get; set; }
        public bool IsLoggedIn { get; set; }
        public string HeaderImageUrl { get; set; }
        public string MobileHeaderImageUrl { get; set; }
        public string PrimaryImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublished { get; set; }
        public bool IsNoIndex { get; set; }
        public string YouTubeId { get; set; }
        public string ActivityLocation { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string Quote { get; set; }
        public string CategoryColor { get; set; }
        public string CategoryName { get; set; }
        public string LimitationName { get; set; }
        public int LikesCount { get; set; }
        public bool IsLiked { get; set; }
        public int ViewsCount { get; set; }
        public bool IsBlogPost { get; set; }
        public List<Tuple<int, string>> SubCategories { get; set; }
        public List<StoryParagraphModel> Paragraphs { get; set; }
        public List<string> Images { get; set; }
        public List<StoryCommentModel> Comments { get; set; }
        public List<StoryCardModel> RelatedStories { get; set; }
        public List<ProductCardModel> RelatedProducts { get; set; }
        public List<RelatedServiceProviderModel> RelatedServiceProviders { get; set; }   
        
        public StoryModel()
        {
            SubCategories = new List<Tuple<int, string>>();
            RelatedStories = new List<StoryCardModel>();
            RelatedProducts = new List<ProductCardModel>();
            RelatedServiceProviders = new List<RelatedServiceProviderModel>();
        }
    }
}