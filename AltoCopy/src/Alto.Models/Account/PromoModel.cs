using Alto.Enums.Account;

namespace Alto.Models.Account
{
    public class PromoModel
    {
        public string CompanyLogo { get; set; }
        public string CompanyName { get; set; }
        public RegistrationPromoType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal? PerMonthPrice { get; set; }
        public bool NotOrganization { get; set; }
        public bool HideAltoDescriptionText { get; set; }
    }
}
