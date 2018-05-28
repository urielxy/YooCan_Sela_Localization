using System;
using Alto.Enums;

namespace Alto.Domain.Benefits
{
    public class PromotedBenefit
    {
        public int Id { get; set; }
        public int BenefitId { get; set; }
        public int Order { get; set; }
        public Benefit Benefit { get; set; }
        public PromotionType PromotionType { get; set; }
        public DateTime InsertDate { get; set; }
    }
}