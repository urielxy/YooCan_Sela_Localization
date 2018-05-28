using System;
using System.Collections.Generic;

namespace Yooocan.Entities.Blog
{
    public class Post
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
        public int Order { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public List<PostImage> Images { get; set; }
    }
}
