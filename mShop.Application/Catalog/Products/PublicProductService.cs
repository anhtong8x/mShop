using mShop.Data.EF;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using mShop.ViewModel.Common;
using mShop.ViewModel.Catalog.Products;
using System.Collections.Generic;

namespace mShop.Application.Catalog.Products
{
    public class PublicProductService : IPublicProductService
    {
        private readonly MShopDbContext mDbContext;

        public PublicProductService(MShopDbContext dbConext)
        {
            mDbContext = dbConext;
        }

        public async Task<List<ProductViewModel>> GetAll(string languageId)
        {
            // 1. Select join
            var query = from p in mDbContext.Products
                        join pt in mDbContext.ProductTranslations on p.Id equals pt.ProductId
                        join pic in mDbContext.ProductInCategories on p.Id equals pic.ProductId
                        join c in mDbContext.Categories on pic.CategoryId equals c.Id
                        where pt.LanguageId == languageId
                        select new { p, pt, pic };

            var data = await query
                .Select(x => new ProductViewModel()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    ViewCount = x.p.ViewCount
                }).ToListAsync();

            return data;
        }

        public async Task<PageResult<ProductViewModel>> GetAllPagingByLanguageId(string languageId, GetPublicProductPagingRequest request)
        {
            // 1. Select join
            var query = from p in mDbContext.Products
                        join pt in mDbContext.ProductTranslations on p.Id equals pt.ProductId
                        join pic in mDbContext.ProductInCategories on p.Id equals pic.ProductId
                        join c in mDbContext.Categories on pic.CategoryId equals c.Id
                        where pt.LanguageId == languageId
                        select new { p, pt, pic };
            // 2. Filter
            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);

            // 3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductViewModel()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    ViewCount = x.p.ViewCount
                }).ToListAsync();

            // 4. Select and projection
            var pageResult = new PageResult<ProductViewModel>()
            {
                TotalPage = totalRow,
                Items = data
            };

            return pageResult;
        }
    }
}