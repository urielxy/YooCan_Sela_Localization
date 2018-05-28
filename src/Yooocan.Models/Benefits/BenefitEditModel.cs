using System;
using System.Collections.Generic;
using Yooocan.Enums;
using Yooocan.Models.Company;

namespace Yooocan.Models.Benefits
{
    public class BenefitEditModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public BenefitType Type { get; set; }
        public decimal? Discount { get; set; }
        public List<int> BenefitBranches { get; set; }
        public List<string> Images { get; set; }
        public string ImageDataUri { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<CompanySelectModel> CompaniesOptions { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<CategoryModel> CategoriesOptions { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}