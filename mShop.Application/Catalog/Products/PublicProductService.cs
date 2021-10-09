using mShop.Application.Catalog.Products.Dtos;
using mShop.Application.Catalog.Products.Dtos.Public;
using mShop.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mShop.Application.Catalog.Products
{
    public class PublicProductService : IPublicProductService
    {
        public Task<PageResult<ProductViewModel>> GetAllByCategoryId(GetProductPagingRequest request)
        {
            throw new NotImplementedException();
        }
    }
}