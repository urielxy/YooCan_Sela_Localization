using System;

namespace Alto.Domain.Orders
{
    public class StatusHistory
    {
        public int Id { get; set; }
        public int OrderProductId { get; set; }
        public OrderProduct OrderProduct { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
