using System.Collections.Generic;

namespace Yooocan.Models.Categories
{
    public class AltoCategoryMenuModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FooterImageUrl { get; set; }
        public string AlternativeUrl { get; set; }
        public List<KeyValuePair<int, string>> SubCategories { get; set; }

        public AltoCategoryMenuModel()
        {
            SubCategories = new List<KeyValuePair<int, string>>();
        }
    }
}