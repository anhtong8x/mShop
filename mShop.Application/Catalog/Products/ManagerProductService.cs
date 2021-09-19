using mShop.Application.Catalog.Products.Dtos;
using mShop.Application.Dtos;
using mShop.Data.EF;
using mShop.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mShop.Application.Catalog.Products
{
    public class ManagerProductService : IManageProductService
    {
        private readonly MShopDbContext _MShopDbContext;

        public ManagerProductService(MShopDbContext dbConext)
        {
            _MShopDbContext = dbConext;
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var item = new Product()
            {
                Price = request.Price
            };

            _MShopDbContext.Products.Add(item);
            return await _MShopDbContext.SaveChangesAsync();
        }

        public Task<int> Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<int> Edit(ProductEditRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductViewModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<PageViewModel<ProductViewModel>> GetPaging(string keyWord, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}