using System;

namespace Alto.Domain.Imports
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

    public enum ImageType
    {
        Normal = 0,
        Primary = 1,
        Header = 2,
        Profile = 3,
        Brand = 4,
        Certification = 5
    }
}