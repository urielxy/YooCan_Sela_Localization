namespace Yooocan.Models
{
    public class SearchStoryModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string PrimaryImageUrl { get; set; }
        public string AuthorName { get; set; }
        public string AuthorLocation { get; set; }
        public string PublishDate { get; set; }
    }
}