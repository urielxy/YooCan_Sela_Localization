using System.Collections.Generic;

namespace Alto.Models.Products
{
    public class PurchaseConfirmModel
    {
        public string ShippingAddress { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal PriceSaved { get; set; }
        public List<string> Variations { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public decimal TotalPaid { get; set; }
    }
}