using System;

namespace Yooocan.Models.Company
{
    public class CompanyIndexModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WebsiteUrl { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPosition { get; set; }
        public string ContactPersonEmail { get; set; }
        public DateTime? OnBoardingDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
