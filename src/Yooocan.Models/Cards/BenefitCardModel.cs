using Yooocan.Enums;

namespace Yooocan.Models.Cards
{
    public class BenefitCardModel
    {
        public int Id { get; set; }
        public bool IsDarkTheme { get; set; }
        public string ImageUrl { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public float? Discount { get; set; }
        public BenefitType Type { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
