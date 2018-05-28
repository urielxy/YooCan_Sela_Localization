namespace Yooocan.Models.Users
{
    public class FollowerModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool IsFollowed { get; set; }
    }
}