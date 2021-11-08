using mShop.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.ViewModel.System.Users
{
    public class GetUserPagingRequest : PagingRequestBase
    {
        // de ke thua thuoc tinh trong pagingrequestbase - so trang / trang hien tai
        // ta them cac thuoc tinh can them. vd tim theo keyword
        public string Keyword { get; set; }
    }
}