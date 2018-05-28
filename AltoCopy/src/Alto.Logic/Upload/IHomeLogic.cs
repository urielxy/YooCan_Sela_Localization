using System.Threading.Tasks;
using Alto.Models.Home;

namespace Alto.Logic.Upload
{
    public interface IHomeLogic
    {
        Task<HomeModel> GetModelAsync();
        Task<HomeModel> GetRandomModelAsync();
    }
}