using System.Collections.Generic;
using Yooocan.Enums;

namespace Yooocan.Models.Benefits
{
    public class BenefitModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public BenefitType Type { get; set; }
        public List<string> Images { get; set; }
        public string MainImageUrl { get; set; }
        public string HeaderImageUrl { get; set; }
        public string LogoUrl { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public int CompanyId { get; set; }
        public decimal? Discount { get; set; }
        public BenefitsStripModel RelatedBenefits { get; set; }
        public KeyValuePair<int, string>? Category { get; set; }
        public KeyValuePair<int, string>? ParentCategory { get; set; }
    }
}