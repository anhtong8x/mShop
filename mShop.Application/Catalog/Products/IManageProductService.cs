using mShop.ViewModel.Catalog.ProductImage;
using mShop.ViewModel.Catalog.Products;
using mShop.ViewModel.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mShop.Application.Catalog.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest request);

        Task<int> Update(ProductUpdateRequest request);

        Task<bool> UpdatePrice(int productId, decimal newPrice);

        Task<bool> UpdateStock(int productId, int addedQuantity);

        Task AddViewCount(int productId);   // kieu void

        Task<int> Delete(int productId);

        Task<List<ProductViewModel>> GetAll();

        Task<PageResult<ProductViewModel>> GetPaging(GetManagerProductPagingRequest request);

        Task<int> AddImage(int productId, ProductImageCreateRequest request);

        Task<int> RemoveImage(int imageId);

        Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request);

        Task<ProductImageViewModel> GetImageById(int imageId);

        Task<List<ProductImageViewModel>> GetListImages(int productId);
    }
}