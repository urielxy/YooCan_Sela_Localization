using System.Threading.Tasks;
using Yooocan.Models.Products;
using Yooocan.Entities;

namespace Yooocan.Logic.Products
{
    public interface IProductLogic
    {
        Task<ProductModel> GetModelAsync(int id);
        Task<Product> CreateAsync(CreateProductModel model);
        Task<Product> EditAsync(CreateProductModel model);
        Task DeleteAsync(int id);
    }
}