using System;
using Alto.Enums;

namespace Alto.Domain.Products
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string CdnUrl { get; set; }
        public string OriginalUrl { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ImageType Type { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime InsertDate { get; set; }
    }
}