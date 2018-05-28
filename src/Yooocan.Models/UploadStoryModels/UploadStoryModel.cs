using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Yooocan.Models.New.Stories;

namespace Yooocan.Models.UploadStoryModels
{
    public class UploadStoryModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string YoutubeId { get; set; }
        public string HeaderImageUrl { get; set; }
        public List<string> Images { get; set; }
        public List<int> Categories { get; set; }
        public List<int> Limitations { get; set; }
        public Dictionary<int, string> LimitationsOptions { get; set; }
        public Dictionary<string, Dictionary<int, string>> CategoriesOptions { get; set; }
        public List<StoryParagraphModel> Paragraphs { get; set; }
        public string SuggestedCategory { get; set; }
        public string ActivityLocation { get; set; }
        public string GooglePlaceId { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string UserInstagramUserName { get; set; }
        public string UserFacebookPageUrl { get; set; }

        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public double? UserLongitude { get; set; }
        public double? UserLatitude { get; set; }
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;
        public int PrimaryCategoryId { get; set; }
        public int PrimaryLimitationId { get; set; }        
        public string UsedProducts { get; set; }
        public bool IsInCompetition { get; set; }
        public string Template { get; set; }
        public bool IsProductsReviewed { get; set; }

        public UploadStoryModel()
        {
            Categories = new List<int>();
            Limitations = new List<int>();
            Images = new List<string>();
            Paragraphs = new List<StoryParagraphModel>();
        }
    }

    public class SetRelatedProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VendorName { get; set; }
        public string PrimaryImageUrl { get; set; }
    }

    public class InternalCreateStoryModel
    {
        [Required]
        public string Title { get; set; }
        public string YoutubeId { get; set; }
        public List<string> Images { get; set; }
        public List<StoryParagraphModel> Paragraphs { get; set; }
        public List<int> Categories { get; set; }
        public List<int> Limitations { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string AboutMe { get; set; }
        public string PictureUrl { get; set; }
        public int PrimaryCategoryId { get; set; }
    }

}