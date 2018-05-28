using System;
using Alto.Domain.Products;

namespace Alto.Domain.Referrals
{
    public class ProductReferral
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string Url { get; set; }
        public int? UserId { get; set; }
        public AltoUser User { get; set; }
        public string Ip { get; set; }
        public string UserAgent { get; set; }
        public string Referrer { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
