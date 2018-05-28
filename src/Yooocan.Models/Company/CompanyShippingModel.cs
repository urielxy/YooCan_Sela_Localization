namespace Yooocan.Models.Company
{
    public class CompanyShippingModel
    {
        public int Id { get; set; }
        public decimal ShippingPrice { get; set; }
        public string ShippingMethod { get; set; }
        public int? MinShippingDuration { get; set; }
        public int? MaxShippingDuration { get; set; }
        public decimal MinProductPrice { get; set; }
        public decimal MaxProductPrice { get; set; }
    }
}
