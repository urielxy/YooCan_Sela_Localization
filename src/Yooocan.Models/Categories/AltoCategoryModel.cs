namespace Yooocan.Models.Categories
{
    public class AltoCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string ParentCategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
