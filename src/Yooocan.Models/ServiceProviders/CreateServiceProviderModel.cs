using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Yooocan.Models.ServiceProviders
{
    public class CreateServiceProviderModel
    {
        public int Id { get; set; }
        public string LogoUrl { get; set; }
        public string HeaderImageUrl { get; set; }
        [Required]
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
        public string ContactPresonName { get; set; }
        [Required]
        [Url(ErrorMessage = "Please enter a valid url that starts with http:// or https://")]
        public string WebsiteUrl { get; set; }
        public string PhoneNumber { get; set; }
        [EmailAddress]        
        public string Email { get; set; }
        public string AdditionalInformation { get; set; }
        public string AboutTheCompany { get; set; }
        public bool IsChapter { get; set; }
        public string TollFreePhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string SuggestedCategory { get; set; }
        public string UserId { get; set; }
        public Dictionary<int, string> LimitationsOptions { get; set; }
        public Dictionary<string, Dictionary<int, string>> CategoriesOptions { get; set; }
        public List<int> Categories { get; set; }
        public Dictionary<string, int> MainCategoryNamesToIds { get; set; }
        public List<int> Limitations { get; set; }

        public List<CreateServiceProviderActivityModel> Activities { get; set; }
        public List<string> YouTubeIds { get; set; }
        public List<string> Images { get; set; }


        public CreateServiceProviderModel()
        {
            Categories = new List<int>();
            Limitations = new List<int>();
            Activities = new List<CreateServiceProviderActivityModel>();
            YouTubeIds = new List<string>();
            Images = new List<string>();
        }
    }
}