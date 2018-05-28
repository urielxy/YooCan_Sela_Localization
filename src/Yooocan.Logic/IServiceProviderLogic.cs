using System.Threading.Tasks;
using Yooocan.Entities.ServiceProviders;
using Yooocan.Models.ServiceProviders;

namespace Yooocan.Logic
{
    public interface IServiceProviderLogic
    {
        Task<ServiceProvider> CreateAsync(CreateServiceProviderModel model);
        Task<ServiceProviderIndexModel> GetModelAsync(int id, string userId);
        Task<ServiceProvider> EditAsync(CreateServiceProviderModel model);

        Task<ServiceProviderContactRequest> ContactServiceProviderAsync(ContactServiceProviderModel mdoel);
        Task UnfollowyAsync(int serviceProviderId, string userId);
        Task FollowAsync(int serviceProviderId, string userId);
    }
}