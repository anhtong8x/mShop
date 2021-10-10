using mShop.ViewModel.Common;
using System.Collections.Generic;

namespace mShop.ViewModel.Catalog.Products.Manage
{
    public class GetProductPagingRequest : PagingRequestBase
    {
        public string KeyWord { get; set; }

        public List<int> CategoryIds { get; set; }  // truyen vao 1 mang de tim kiem
    }
}