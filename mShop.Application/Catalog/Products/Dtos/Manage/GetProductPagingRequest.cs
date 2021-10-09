using mShop.Application.Dtos;
using System.Collections.Generic;

namespace mShop.Application.Catalog.Products.Dtos.Manage
{
    public class GetProductPagingRequest : PagingRequestBase
    {
        public string KeyWord { get; set; }

        public List<int> CategoryIds { get; set; }  // truyen vao 1 mang de tim kiem
    }
}