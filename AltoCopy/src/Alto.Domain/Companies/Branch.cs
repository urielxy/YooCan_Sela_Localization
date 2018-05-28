using System;
using System.Collections.Generic;
using Alto.Domain.Benefits;

namespace Alto.Domain.Companies
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Company Company { get; set; }
        public int CompanyId { get; set; }
        public List<BranchBenefit> BranchBenefits { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
    }
}