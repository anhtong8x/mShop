using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using mShop.Application.Common;
using mShop.Data.EF;
using mShop.Data.Entities;
using mShop.Ultilities.Exceptions;
using mShop.ViewModel.Catalog.ProductImage;
using mShop.ViewModel.Catalog.Products;
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
        private readonly IStorageService _storageService;   // de luu file anh product

        public ManagerProductService(MShopDbContext dbConext, IStorageService storageService)
        {
            _dbContext = dbConext;
            _storageService = storageService;
        }

        public async Task<int> AddImage(int productId, ProductImageCreateRequest request)
        {
            var productImage = new ProductImage()
            {
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                ProductId = productId,
                SortOrder = request.SortOrder
            };

            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _dbContext.ProductImages.Add(productImage);
            await _dbContext.SaveChangesAsync();
            return productImage.Id;
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
            if (request.ThumbnailImage != null)
            {
                item.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = "Thumbnail image",
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        ImagePath = await this.SaveFile(request.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }

            _dbContext.Products.Add(item);

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null) throw new mShopException($"Cannot find a product: {productId}");
            // get tat ca cac anh cua product theo id va xoa het anh do
            var images = _dbContext.ProductImages.Where(i => i.ProductId == productId);
            foreach (var image in images)
            {
                await _storageService.DeleteFileAsync(image.ImagePath);
            }

            _dbContext.Products.Remove(product);
            return await _dbContext.SaveChangesAsync();
        }

        public Task<List<ProductViewModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ProductImageViewModel> GetImageById(int imageId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductImageViewModel>> GetListImages(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<PageResult<ProductViewModel>> GetPaging(GetManagerProductPagingRequest request)
        {
            // 1. Select join
            var query = from p in _dbContext.Products
                        join pt in _dbContext.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _dbContext.ProductInCategories on p.Id equals pic.ProductId
                        join c in _dbContext.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };

            // 2. Filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));
            }

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

        public async Task<int> RemoveImage(int imageId)
        {
            var productImage = await _dbContext.ProductImages.FindAsync(imageId);
            if (productImage == null)
                throw new mShopException($"Cannot find an image with id {imageId}");
            _dbContext.ProductImages.Remove(productImage);
            return await _dbContext.SaveChangesAsync();
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

            //Save image update
            if (request.ThumbnailImage != null)
            {
                var thumbnailImage = await _dbContext.ProductImages.FirstOrDefaultAsync(i => i.IsDefault == true && i.ProductId == request.Id);
                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                    _dbContext.ProductImages.Update(thumbnailImage);
                }
            }

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var productImage = await _dbContext.ProductImages.FindAsync(imageId);
            if (productImage == null)
                throw new mShopException($"Cannot find an image with id {imageId}");

            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _dbContext.ProductImages.Update(productImage);
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
            var originalFileName = System.Net.Http.Headers.ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        //
    }
}