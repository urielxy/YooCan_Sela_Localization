using System;
using System.Collections.Generic;

namespace Alto.Domain.Products
{
    public class ProductVariationValue
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public Variation Variation { get; set; }
        public int VariationId { get; set; }
        public string Value { get; set; }
        public DateTime InsertDate { get; set; }
    }
}