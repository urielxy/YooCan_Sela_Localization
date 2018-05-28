namespace Alto.Domain.Products
{
    public class ProductLimitation
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }

        public Limitation Limitation { get; set; }
        public int LimitationId { get; set; }
    }
}