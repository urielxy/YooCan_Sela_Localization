using System.Collections.Generic;

namespace Alto.Domain.Products
{
    public class JsonVariationRow
    {
        public Dictionary<int, int> Combinations { get; set; }
        public decimal? Price { get; set; }
        public string Sku { get; set; }
        public string Upc { get; set; }

        public JsonVariationRow()
        {
            Combinations = new Dictionary<int, int>();
        }
    }
}   