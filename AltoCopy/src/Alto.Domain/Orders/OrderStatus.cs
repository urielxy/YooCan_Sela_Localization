namespace Alto.Domain.Orders
{
    public enum OrderStatus
    {
        AwaitingPayment = 1,
        PaymentAuthorized = 2,
        PaymentExecuted = 4,
        Shipped = 7,
        Completed = 10,
        Cancelled = 13,
        Refunded = 16,
        Disputed = 19
    }
}
