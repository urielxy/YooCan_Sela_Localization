namespace Yooocan.Entities
{
    public class StoryProduct
    {
        public Story Story { get; set; }
        public int StoryId { get; set; }

        public Product Product { get; set; }
        public int ProductId { get; set; }
        public int Order { get; set; }
        public bool IsUsedInStory { get; set; }
    }
}