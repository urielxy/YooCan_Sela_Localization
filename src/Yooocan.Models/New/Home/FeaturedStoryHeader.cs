namespace Yooocan.Models.New.Home
{
    public class FeaturedStoryHeader
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Author { get; set; }
        public string UserFirstName { get; set; }
        public string UserPictureUrl { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public string CategoryColor { get; set; }
        public float? HotAreaLeft { get; set; }
        public float? HotAreaTop { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
    }
}