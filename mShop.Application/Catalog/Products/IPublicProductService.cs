using mShop.Application.Catalog.Products.Dtos;
using mShop.Application.Catalog.Products.Dtos.Public;
using mShop.Application.Dtos;
using System.Threading.Tasks;

namespace mShop.Application.Catalog.Products
{
    public interface IPublicProductService
    {
        Task<PageResult<ProductViewModel>> GetAllByCategoryId(GetProductPagingRequest request);
    }
}