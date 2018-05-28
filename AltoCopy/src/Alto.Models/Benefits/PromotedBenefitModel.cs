using Alto.Enums;

namespace Alto.Models.Benefits
{
    public class PromotedBenefitModel
    {
        public int BenefitId { get; set; }
        public string Title { get; set; }
        public PromotionType PromotionType { get; set; }
    }
}