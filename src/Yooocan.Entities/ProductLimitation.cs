namespace Yooocan.Entities
{
    public class ProductLimitation
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }

        public Limitation Limitation { get; set; }
        public int LimitationId { get; set; }
    }
}