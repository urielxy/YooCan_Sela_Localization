using System;
using Alto.Enums.Account;

namespace Alto.Models.Account
{
    public class RegistrationPromoSession
    {
        public string PromoCode { get; set; }
        public DateTime StartDate { get; set; }
        public int RegistrationPromoId { get; set; }
        public string Guid { get; set; }
        public RegistrationPromoType PromoType { get; set; }
        public decimal PromoAmount { get; set; }
    }
}