using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Yooocan.Models.ServiceProviders
{
    public class ServiceProviderIndexModel
    {
        public int Id { get; set; }
        public bool IsFollowed { get; set; }
        public bool IsPreview { get; set; }
        public string LogoUrl { get; set; }
        public string HeaderImageUrl { get; set; }
        public string MobileHeaderImageUrl { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string AboutTheCompany { get; set; }
        public string AdditionalInformation { get; set; }
        public string Email { get; set; }
        public string WebsiteUrl { get; set; }
        public List<CreateServiceProviderActivityModel> Activities { get; set; }
        public List<StoryCardModel> RelatedStories { get; set; }
        public List<string> YouTubeIds { get; set; }
        public string PrimaryImageUrl { get; set; }
        public List<string> Images { get; set; }
        public string PrimaryYouTubeId { get; set; }

        public ServiceProviderIndexModel()
        {
            Images = new List<string>();
            YouTubeIds = new List<string>();
            Activities = new List<CreateServiceProviderActivityModel>();
            RelatedStories = new List<StoryCardModel>();
        }
    }
}