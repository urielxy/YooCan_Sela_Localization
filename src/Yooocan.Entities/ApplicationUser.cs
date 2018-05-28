using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Yooocan.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PictureUrl { get; set; }
        public string HeaderImageUrl { get; set; }
        public string Location { get; set; }
        public string AboutMe { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string InstagramUserName { get; set; }
        public string FacebookPageUrl { get; set; }
        public Gender? Gender { get; set; }
        public bool CustomizedFeedDone { get; set; }

        public DateTime InsertDate { get; set; }

        public List<Story> Stories { get; set; }
        public List<ProductReview> ProductReviews { get; set; }
        public List<StoryComment> StoryComments { get; set; }
        public List<FollowerFollowed> Followers { get; set; }
        public List<FollowerFollowed> Follows { get; set; }
        public List<NotificationLog> NotificationLogs { get; set; }
        public List<CategoryFollower> Categories { get; set; }
        public List<LimitationFollower> Limitations { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();

        public ApplicationUser()
        {
            Stories = new List<Story>();
            ProductReviews = new List<ProductReview>();
            StoryComments = new List<StoryComment>();
            Followers = new List<FollowerFollowed>();
            Follows = new List<FollowerFollowed>();
            NotificationLogs = new List<NotificationLog>();
            Categories = new List<CategoryFollower>();
            Limitations = new List<LimitationFollower>();
        }
    }

    public enum Gender
    {
        Male = 0,
        Female = 1
    }
}
