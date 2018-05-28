namespace Yooocan.Models
{
    public class StoryCardModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PublishDate{ get; set; }
        public string AuthorPictureUrl { get; set; }
        public string AuthorLocation { get; set; }
        public string AuthorName { get; set; }
        public string UserFirstName { get; set; }
        public string ImageUrl { get; set; }
        public float? HotAreaLeft { get; set; }
        public float? HotAreaTop { get; set; }

        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public int CategoryId { get; set; }
        public string LimitationName { get; set; }
        public bool ShouldShowLimitation { get; set; }
        public bool IsDarkTheme { get; set; }
        public bool IsImageCard { get; set; }

        public string Country { get; set; }
        public string CountryCode { get; set; }

        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public int ViewsCount { get; set; }
        public string Content { get; set; }
        public bool IsBlogPost { get; set; }
        public bool IsProductsReviewed { get; set; }
    }
}