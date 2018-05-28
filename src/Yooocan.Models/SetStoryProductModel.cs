namespace Yooocan.Models
{
    public class SetStoryProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PrimaryImageUrl { get; set; }
        public string VendorName { get; set; }
        public bool IsUsedInStory { get; set; }
    }
}