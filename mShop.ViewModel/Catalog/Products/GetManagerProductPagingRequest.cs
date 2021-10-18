using mShop.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.ViewModel.Catalog.Products
{
    public class GetManagerProductPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }

        public List<int> CategoryIds { get; set; }
    }
}