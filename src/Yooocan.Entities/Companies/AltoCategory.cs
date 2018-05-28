using System.Collections.Generic;

namespace Yooocan.Entities.Companies
{
    public class AltoCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AltoCategory ParentCategory { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHeader { get; set; }
        public List<AltoCategoryImage> Images { get; set; }
        public List<AltoCategory> SubCategories { get; set; }

        public AltoCategory()
        {
            SubCategories = new List<AltoCategory>();
            Images = new List<AltoCategoryImage>();
        }
    }
}