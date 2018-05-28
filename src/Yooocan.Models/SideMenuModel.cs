using System.Collections.Generic;

namespace Yooocan.Models
{
    public class SideMenuModel
    {
        public List<CategoryModel> StoriesCategories{ get; set; }
        public List<CategoryModel> ServiceProvidersCategories{ get; set; }
        public List<CategoryModel> BenefitsCategories { get; set; }
    }
}
