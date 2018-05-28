using System.Collections.Generic;
using Yooocan.Models.Benefits;

namespace Yooocan.Models.Categories
{
    public class AltoCategoryFeedModel
    {
        public int Id { get; set; }
        //public string Name { get; set; }
        public string HeaderImageUrl { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }

        public List<BenefitsStripModel> BenefitsStrips { get; set; }        
        public Dictionary<int, string> SubCategories { get; set; }
        public string CategoryName { get; set; }

        public AltoCategoryFeedModel()
        {
            BenefitsStrips = new List<BenefitsStripModel>();

            SubCategories = new Dictionary<int, string>();
        }
    }
}