namespace Alto.Logic.PayPal
{
    public class PayPalOptions
    {
        public string PayPalClientId { get; set; }
        public string PayPalSecret { get; set; }
        public bool PayPalIsProduction { get; set; }
        public string PayPalExperienceProfileId { get; set; }
        public string PayPalNoShippingExperienceProfileId { get; set; }
    }
}
