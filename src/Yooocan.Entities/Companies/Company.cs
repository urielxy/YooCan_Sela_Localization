using System;
using System.Collections.Generic;
using Yooocan.Entities.Benefits;
using Yooocan.Entities.Products;
using Yooocan.Enums;
using Yooocan.Enums.Company;

namespace Yooocan.Entities.Companies
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public List<CompanyImage> Images { get; set; }
        public List<CompanyCategory> Categories { get; set; }
        public string OtherRequestedCategories { get; set; }
        public string WebsiteUrl { get; set; }
        public List<Benefit> Benefits { get; set; }
        public List<Product> Products { get; set; }
        public List<CompanyShipping> ShippingRules { get; set; }
        public string About { get; set; }
        public BusinessType BusinessType { get; set; }
        public bool IsForeignCurrency { get; set; }

        #region Terms
        public DateTime? OnBoardingDate { get; set; }
        public string OnBoardingContactPersonEmail { get; set; }
        public decimal? ReferralRate { get; set; }
        public RateType? ReferralRateType { get; set; }
        public decimal? MembersDiscountRate { get; set; }
        public RateType? DiscountRateType { get; set; }
        public string ReferrerFormat { get; set; }
        public List<CompanyCoupon> Coupons { get; set; }
        #endregion

        #region Location
        public string Address { get; set; }
        #endregion

        #region Company Contact Info
        public string TelephoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string TollFreeNumber { get; set; }
        #endregion

        #region Contact Person Info
        public List<CompanyContactPerson> ContactPersons { get; set; }
        #endregion

        public Company()
        {
            Images = new List<CompanyImage>();
            Categories = new List<CompanyCategory>();
            ContactPersons = new List<CompanyContactPerson>();
        }
    }
}