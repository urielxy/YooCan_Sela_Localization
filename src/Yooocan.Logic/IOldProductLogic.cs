using System.Threading.Tasks;
using Yooocan.Entities;
using Yooocan.Models;
using Yooocan.Models.Products;
using Yooocan.Models.Vendors;

namespace Yooocan.Logic
{
    public interface IOldProductLogic
    {
        Task<Product> UploadProductAsync(Models.CreateProductModel product);
        Task<Product> EditProductAsync(VendorUploadProductModel model);
        Task<Models.ProductModel> GetProductModelAsync(int productId);
        Task<MyProductsPageModel> GetMyProducts(int vendorId);
        Task<Product> UploadProductAsync(VendorUploadProductModel model);
        Task<ProductCardModel> GetProductOfTheDay();
    }
}
