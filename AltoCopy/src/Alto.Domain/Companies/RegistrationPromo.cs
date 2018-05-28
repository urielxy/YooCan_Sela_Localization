using System;
using Alto.Enums.Account;

namespace Alto.Domain.Companies
{
    public class RegistrationPromo
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public int? ReferrerUserId { get; set; }
        public AltoUser ReferrerUser { get; set; }
        public string PromoCode { get; set; }
        public int? ReferralsRemaining { get; set; }
        public int ReferralsRegistered { get; set; }
        public RegistrationPromoType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsDisabled { get; set; }
        public bool NotOrganization { get; set; }
        public bool HideAltoDescriptionText { get; set; }
        public string Comments { get; set; }
    }
}
