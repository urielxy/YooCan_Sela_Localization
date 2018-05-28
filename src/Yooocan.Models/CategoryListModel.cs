namespace Yooocan.Models
{
    public class CategoryListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public string HeaderPictureUrl { get; set; }
        public string ParentCategoryName { get; set; }
    }

    public class CreateCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public string HeaderPictureUrl { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ShopBackgroundColor { get; set; }

        public bool IsActiveForFeed { get; set; }
        public bool IsActiveForShop { get; set; }
        public bool IsChoosableForProduct { get; set; }
        public bool IsChoosableForStory { get; set; }
    }
}