using mShop.ViewModel.Common;

namespace mShop.ViewModel.Catalog.Products
{
    public class GetPublicProductPagingRequest : PagingRequestBase
    {
        public int? CategoryId { get; set; }    // ? cho phep gia tri null
    }
}