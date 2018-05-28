using System;

namespace Yooocan.Entities
{
    public class ReadHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime InsertDate { get; set; }
        public int? CategoryId { get; set; }
        public int? StoryId { get; set; }
    }
}