using System.Collections.Generic;
using System.Threading.Tasks;
using Yooocan.Entities;
using Yooocan.Models;

namespace Yooocan.Logic
{
    public interface IAdminLogic
    {
        DashboardModel GetDashboard();
        Task<List<SetStoryProductsDashboardModel>> GetSetStoryProductsDataAsync();
        Task<Story> SetStoryServiceProvidersAsync(int id, string serviceProviderIds);
        Task SendStoryOfTheDayAsync(int id, string text, string storyUrl, string recipient = null);
    }
}