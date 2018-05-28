using System.Collections.Generic;
using Yooocan.Entities.ServiceProviders;

namespace Yooocan.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActiveForFeed { get; set; }
        public bool IsActiveForShop { get; set; }
        public bool IsChoosableForProduct { get; set; }
        public bool IsChoosableForStory { get; set; }
        public string PictureUrl { get; set; }
        public string HeaderPictureUrl { get; set; }
        public string MobileHeaderPictureUrl { get; set; }
        public string ShopBackgroundColor { get; set; }
        public int FollowersCount { get; set; }
        public string RoundIcon { get; set; }
        public string MenuIconUrl { get; set; }
        public string Description { get; set; }
        public Category ParentCategory { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category RedirectCategory { get; set; }
        public int? RedirectCategoryId { get; set; }

        public List<Category> SubCategories { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }
        public List<StoryCategory> StoryCategories { get; set; }
        public List<ServiceProviderCategory> ServiceProviderCategories { get; set; }

        public Category()
        {
            SubCategories = new List<Category>();
            ProductCategories = new List<ProductCategory>();
            StoryCategories = new List<StoryCategory>();
        }
    }
}