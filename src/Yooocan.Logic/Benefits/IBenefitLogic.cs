using System.Collections.Generic;
using System.Threading.Tasks;
using Yooocan.Models.Benefits;
using Yooocan.Models.Cards;

namespace Yooocan.Logic.Benefits
{
    public interface IBenefitLogic
    {
        Task<int> CreateAsync(BenefitEditModel model);
        Task DeleteAsync(int id);
        Task EditAsync(BenefitEditModel model);
        Task<BenefitEditModel> Get(int benefitId);
        Task<List<BenefitCardModel>> GetAllAsync();
        Task<BenefitModel> GetModelAsync(int id);
    }
}