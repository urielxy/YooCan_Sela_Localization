using System;
using System.Collections.Generic;
using Yooocan.Entities.Companies;
using Yooocan.Enums;

namespace Yooocan.Entities.Benefits
{
    public class Benefit
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public bool IsPublished { get; set; }

        public BenefitType Type { get; set; }
        //public List<BranchBenefit> BenefitBranches { get; set; }
        public List<BenefitCategory> Categories { get; set; }
        public List<BenefitImage> Images { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
        public decimal? Discount { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }

        public Benefit()
        {
            //BenefitBranches = new List<BranchBenefit>();
            Categories = new List<BenefitCategory>();
            Images = new List<BenefitImage>();
        }
    }
}