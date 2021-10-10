using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using mShop.Data.EF;
using mShop.Data.Entities;
using mShop.Ultilities.Exceptions;
using mShop.ViewModel.Catalog.Products;
using mShop.ViewModel.Catalog.Products.Manage;
using mShop.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace mShop.Application.Catalog.Products
{
    public class ManagerProductService : IManageProductService
    {
        private readonly MShopDbContext _dbContext;

        public ManagerProductService(MShopDbContext dbConext)
        {
            _dbContext = dbConext;
        }

        public async Task AddViewCount(int productId)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            product.ViewCount += 1;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var item = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                DateCreated = DateTime.Now,
                // tao doi tuong con. id se tu tao do ta cau hinh roi
                ProductTranslations = new List<ProductTranslation> {
                    new ProductTranslation()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        SeoDescription = request.SeoDescription,
                        SeoAlias = request.SeoAlias,
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId
                    }
                }
            };

            // save image

            _dbContext.Products.Add(item);

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null) throw new mShopException($"Cannot find a product: {productId}");
            _dbContext.Products.Remove(product);
            return await _dbContext.SaveChangesAsync();
        }

        public Task<List<ProductViewModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<PageResult<ProductViewModel>> GetPaging(GetProductPagingRequest request)
        {
            // 1. Select join
            var query = from p in _dbContext.Products
                        join pt in _dbContext.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _dbContext.ProductInCategories on p.Id equals pic.ProductId
                        join c in _dbContext.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };
            // 2. Filter
            if (!string.IsNullOrEmpty(request.KeyWord))
                query = query.Where(x => x.pt.Name.Contains(request.KeyWord));

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
                TotlaPage = totalRow,
                Items = data
            };

            return pageResult;
        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _dbContext.Products.FindAsync(request.Id);
            var productTranslations = await _dbContext.ProductTranslations
                .FirstOrDefaultAsync(x => x.ProductId == request.Id
                && x.LanguageId == request.LanguageId);

            if (product == null || productTranslations == null)
                new mShopException($"Cannot find a product with id: {request.Id}");

            productTranslations.Name = request.Name;
            productTranslations.SeoAlias = request.SeoAlias;
            productTranslations.SeoDescription = request.SeoDescription;
            productTranslations.SeoTitle = request.SeoTitle;
            productTranslations.Description = request.Description;
            productTranslations.Details = request.Details;

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null) throw new mShopException($"Cannot find a with: {productId}");
            product.Price = newPrice;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null) throw new mShopException($"Cannot find a with: {productId}");
            product.Price += addedQuantity;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        //
    }
}