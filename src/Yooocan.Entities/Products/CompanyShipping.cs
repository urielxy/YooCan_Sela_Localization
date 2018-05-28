using System;
using Yooocan.Entities.Companies;

namespace Yooocan.Entities.Products
{
    public class CompanyShipping
    {
        public int Id { get; set; }
        public decimal ShippingPrice { get; set; }
        public string ShippingMethod { get; set; }
        public int? MinShippingDuration { get; set; }
        public int? MaxShippingDuration { get; set; }
        public decimal MinProductPrice { get; set; }
        public decimal MaxProductPrice { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}