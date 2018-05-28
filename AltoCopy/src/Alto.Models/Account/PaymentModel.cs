namespace Alto.Models.Account
{
    public class PaymentModel
    {
        public bool IsTrialAvailable { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FullYearPrice { get; set; }
        public decimal PerMonthPrice { get; set; }
        public decimal MonthTrialPrice { get; set; }
        public int ExtendedTrialDuration { get; set; }
    }
}
