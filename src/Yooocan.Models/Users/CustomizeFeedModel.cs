using System.Collections.Generic;

namespace Yooocan.Models.Users
{
    public class CustomizeFeedModel
    {
        public class CategoryModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string RoundIcon { get; set; }
            public bool Selected { get; set; }
        }

        public class LimitationModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Color { get; set; }
            public bool Selected { get; set; }
        }

        public List<CategoryModel> Categories { get; set; }
        public List<LimitationModel> Limitations { get; set; }
    }
}