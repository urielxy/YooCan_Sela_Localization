using System;

namespace Yooocan.Entities
{
    public class ProductReview
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public byte Rating { get; set; }
        public DateTime InsertDate { get; set; }
        public bool IsDeleted { get; set; }

        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}