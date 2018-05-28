using System;
using Alto.Domain.Companies;

namespace Alto.Domain.Benefits
{
    public class BranchBenefit
    {
        public int Id { get; set; }
        public Branch Branch { get; set; }
        public int BranchId { get; set; }
        public Benefit Benefit { get; set; }
        public int BenefitId { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}