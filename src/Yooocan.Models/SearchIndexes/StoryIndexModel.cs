using System.Collections.Generic;

namespace Yooocan.Models.SearchIndexes
{
    public class StoryIndexModel
    {
        public string StoryId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string StoryText { get; set; }
        public string Tags { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<string> CategoryNames { get; set; }
    }
}