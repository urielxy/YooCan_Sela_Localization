namespace Yooocan.Models
{
    public class StoryOfTheDayData
    {
        public string StoryUrl { get; set; }
        public string StoryTitle { get; set; }
        public int StoryId { get; set; }
        public string StoryText { get; set; }
        public string PrimaryImage { get; set; }
    }

    public class FollowingPublishedStoryUserData
    {
        public string StoryUrl { get; set; }
        public string StoryTitle { get; set; }
        public int StoryId { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public string AuhtorProfileImageUrl { get; set; }

    }

    public class EmailUserData
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string ProfileImageUrl { get; set; }
    } 
}