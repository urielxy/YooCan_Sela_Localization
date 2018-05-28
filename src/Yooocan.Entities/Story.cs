using System;
using System.Collections.Generic;
using Yooocan.Entities.ServiceProviders;

namespace Yooocan.Entities
{
    public class Story
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public List<StoryTip> Tips { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishDate { get; set; }
        public bool IsProductsReviewed { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsNoIndex { get; set; }
        public string YouTubeId { get; set; }
        public string ActivityLocation { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string GooglePlaceId { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string UsedProducts { get; set; }
        public string Quote { get; set; }
        public int LikesCount { get; set; }
        public int ViewsCount { get; set; }
        public bool CanNotBeFeaturedOnHomePage { get; set; }
        public bool IsInCompetition { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public List<StoryImage> Images { get; set; }
        public List<StoryComment> Comments { get; set; }
        public List<StoryCategory> StoryCategories { get; set; }

        public List<StoryProduct> StoryProducts { get; set; }
        public List<StoryServiceProvider> StoryServiceProviders { get; set; }
        public List<StoryLimitation> StoryLimitations { get; set; }
        public List<StoryParagraph> Paragraphs { get; set; }

        public Story()
        {
            Comments = new List<StoryComment>();
            Images = new List<StoryImage>();
            StoryLimitations = new List<StoryLimitation>();
            StoryProducts = new List<StoryProduct>();
            StoryServiceProviders = new List<StoryServiceProvider>();
            StoryCategories = new List<StoryCategory>();
        }
    }
}