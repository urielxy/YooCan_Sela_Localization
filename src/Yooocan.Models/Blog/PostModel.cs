using System;

namespace Yooocan.Models.Blog
{
    public class PostModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? PublishDate { get; set; }
        public int Order { get; set; }
        public bool IsDeleted { get; set; }        
        public string PrimaryImageUrl { get; set; }        
    }
}
