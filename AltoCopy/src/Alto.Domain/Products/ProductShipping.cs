using System;

namespace Alto.Domain.Products
{
    public class ProductShipping
    {
        public int Id { get; set; }
        public decimal ShippingPrice { get; set; }
        public string ShippingMethod { get; set; }
        public int? MinShippingDuration { get; set; }
        public int? MaxShippingDuration { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}