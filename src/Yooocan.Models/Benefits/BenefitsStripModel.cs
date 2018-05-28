using System.Collections.Generic;
using Yooocan.Models.Cards;

namespace Yooocan.Models.Benefits
{
    public class BenefitsStripModel
    {
        public string Title { get; set; }
        public string TitleLink { get; set; }
        public List<BenefitCardModel> Benefits { get; set; }
    }
}