using System;
using Yooocan.Enums;

namespace Yooocan.Entities.Benefits
{
    public class BenefitImage
    {
        public int Id { get; set; }
        public AltoImageType Type { get; set; }
        public string Url { get; set; }
        public string CdnUrl { get; set; }
        public int Order { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}