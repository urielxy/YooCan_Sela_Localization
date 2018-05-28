namespace Yooocan.Entities
{
    public class Feed
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PrimaryImageUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
        public string UserImageUrl { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int? ParentCategoryId { get; set; }

    }
}