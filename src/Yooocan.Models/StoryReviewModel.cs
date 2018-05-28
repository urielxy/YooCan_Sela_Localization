using System;

namespace Yooocan.Models
{
    public class StoryCommentModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorPictureUrl { get; set; }
        public DateTime InsertDate { get; set; }
    }
}