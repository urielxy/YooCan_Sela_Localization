using System;

namespace Yooocan.Entities.Companies
{
    public class CompanyCoupon
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public string Code { get; set; }
        public int? UsesLeft { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}