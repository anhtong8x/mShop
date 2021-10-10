using mShop.ViewModel.Common;

namespace mShop.ViewModel.Catalog.Products.Public
{
    public class GetProductPagingRequest : PagingRequestBase
    {
        public int? CategoryId { get; set; }    // ? cho phep gia tri null
    }
}