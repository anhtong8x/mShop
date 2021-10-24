﻿using mShop.ViewModel.Catalog.Products;
using mShop.ViewModel.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mShop.Application.Catalog.Products
{
    public interface IPublicProductService
    {
        Task<PageResult<ProductViewModel>> GetAllPagingByLanguageId(string languageId, GetPublicProductPagingRequest request);

        Task<List<ProductViewModel>> GetAll(string languageId);
    }
}