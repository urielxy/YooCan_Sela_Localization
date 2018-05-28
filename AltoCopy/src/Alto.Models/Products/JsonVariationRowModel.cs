using System.Collections.Generic;

namespace Alto.Models.Products
{
    public class JsonVariationRowModel
    {
        public Dictionary<string, string> Combinations { get; set; }
        public decimal? Price { get; set; }
        public string Sku { get; set; }
        public string Upc { get; set; }

        public JsonVariationRowModel()
        {
            Combinations = new Dictionary<string, string>();
        }
    }
}