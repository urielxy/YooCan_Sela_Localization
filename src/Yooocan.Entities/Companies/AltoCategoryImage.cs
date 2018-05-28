using System;
using Yooocan.Entities.Companies;
using Yooocan.Enums;

namespace Yooocan.Entities
{
    public class AltoCategoryImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string CdnUrl { get; set; }
        public AltoImageType Type { get; set; }

        public AltoCategory Category { get; set; }
        public int CategoryId { get; set; }

        public DateTime InsertDate { get; set; }
    }
}