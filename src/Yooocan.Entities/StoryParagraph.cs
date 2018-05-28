using System;

namespace Yooocan.Entities
{
    public class StoryParagraph
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public Story Story { get; set; }
        public int StoryId { get; set; }
        public int Order { get; set; }
        public DateTime InsertDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}