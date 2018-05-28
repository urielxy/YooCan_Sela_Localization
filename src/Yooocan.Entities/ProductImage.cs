using System;
using Yooocan.Enums;

namespace Yooocan.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string OriginalUrl { get; set; }
        public string CdnUrl { get; set; }
        public int Order { get; set; }
        public ImageType Type { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime InsertDate { get; set; }
    }
}