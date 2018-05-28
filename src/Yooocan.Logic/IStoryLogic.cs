using System.Collections.Generic;
using System.Threading.Tasks;
using Yooocan.Entities;
using Yooocan.Models;
using Yooocan.Models.New.Stories;
using Yooocan.Models.Products;
using Yooocan.Models.UploadStoryModels;

namespace Yooocan.Logic
{
    public interface IStoryLogic
    {
        Task<Story> UploadStoryAsync(UploadStoryModel story, string authorId);
        Task<Story> EditStoryAsync(UploadStoryModel story);
        Task ApproveStoryAsync(int storyId, string storyUrl);
        Task ToggleDeleteStoryAsync(int storyId);
        Task<List<Story>> GetUserStoriesAsync(string userId);
        Task<StoryModel> GetStoryModelForPage(int storyId);
        Task<List<Story>> GetRelatedStoriesAsync(int storyId, int primaryCategoryId, List<int> secenodaryCategoriesIds);
        Task<List<ProductCardModel>> GetRelatedProductsAsync(int storyId, int primaryCategoryId, List<int> secenodaryCategoriesIds, int maxRelatedProducts = 10);
        Task<List<RelatedServiceProviderModel>> GetRelatedServiceProvidersAsync(int storyId);
        Task SetRelatedProductsAsync(int storyId, IEnumerable<SetStoryProductModel> products);
        Task LikeAsync(int storyId, string userId);
        Task UnLikeAsync(int storyId, string userId);
        Task<PreviewStoryModel> PreviewStoryAsync(UploadStoryModel model, string userId);
    }
}