using mShop.ViewModel.Catalog.Products;
using mShop.ViewModel.Catalog.Products.Public;
using mShop.ViewModel.Common;
using System.Threading.Tasks;

namespace mShop.Application.Catalog.Products
{
    public interface IPublicProductService
    {
        Task<PageResult<ProductViewModel>> GetAllByCategoryId(GetProductPagingRequest request);
    }
}