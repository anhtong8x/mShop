using mShop.Application.Catalog.Products.Dtos;
using mShop.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mShop.Application.Catalog.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest request);

        Task<int> Edit(ProductEditRequest request);

        Task<int> Delete(int Id);

        Task<List<ProductViewModel>> GetAll();

        Task<PageViewModel<ProductViewModel>> GetPaging(string keyWord, int pageIndex, int pageSize);
    }
}