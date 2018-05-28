using System;
using Alto.Domain.Products;

namespace Alto.Domain.Orders
{
    public class OrderProduct
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ShippingPrice { get; set; }
        public string ShippingMethod { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status { get; set; }
        public string TrackingNumber { get; set; }
        public bool ReviewGiven { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
