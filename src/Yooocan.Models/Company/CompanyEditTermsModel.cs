using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yooocan.Enums;

namespace Yooocan.Models.Company
{
    public class CompanyEditTermsModel
    {
        public string Name { get; set; }

        public DateTime? OnBoardingDate { get; set; }

        public string OnBoardingContactPersonEmail { get; set; }

        [Required]
        [Display(Name = "Referral Rate")]
        public decimal? ReferralRate { get; set; }

        [Required]
        [Display(Name = "Referral Rate Type")]
        public RateType? ReferralRateType { get; set; }

        [Required]
        [Display(Name = "Members Discount Rate")]
        public decimal? MembersDiscountRate { get; set; }

        [Required]
        [Display(Name = "Discount Rate Type")]
        public RateType? DiscountRateType { get; set; }

        [Display(Name = "Coupon Code")]
        public string CouponCode { get; set; }

        public List<CompanyShippingModel> ShippingRules { get; set; }
    }
}
