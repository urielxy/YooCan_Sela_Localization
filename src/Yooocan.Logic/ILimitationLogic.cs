using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yooocan.Logic
{
    public interface ILimitationLogic
    {
        Task<Dictionary<int, string>> GetLimitationsAsync();
    }
}