using System;

namespace Yooocan.Entities
{
    public class StoryComment
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public DateTime InsertDate { get; set; }
        public bool IsDeleted { get; set; }

        public Story Story { get; set; }
        public int StoryId { get; set; }
    }
}