using System;
using Yooocan.Enums;

namespace Yooocan.Entities.Companies
{
    public class CompanyImage
    {
        public int Id { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public AltoImageType Type { get; set; }
        public string Url { get; set; }
        public string CdnUrl { get; set; }
        public Company Company { get; set; }
        public int CompanyId { get; set; }
    }
}