using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace mShop.Application.Catalog.Products
{
    public class ManagerProductService : IManageProductService
    {
        private readonly MShopDbContext mDbContext;
        private readonly IStorageService mIStorageService;   // de luu file anh product

        public ManagerProductService(MShopDbContext dbConext, IStorageService storageService)
        {
            mDbContext = dbConext;
            mIStorageService = storageService;
        }

        public async Task AddViewCount(int productId)
        {
            var product = await mDbContext.Products.FindAsync(productId);
            product.ViewCount += 1;
            await mDbContext.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            // 1. Tao doi tuong Product
            var produt = new Product()
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

            // 2. save image
            if (request.ThumbnailImage != null)
            {
                produt.ProductImages = new List<ProductImage>()
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

            // 3. Goi luu vao db
            mDbContext.Products.Add(produt);
            await mDbContext.SaveChangesAsync();

            return produt.Id;
        }

        public async Task<int> Delete(int productId)
        {
            var product = await mDbContext.Products.FindAsync(productId);
            if (product == null) throw new mShopException($"Cannot find a product: {productId}");
            // get tat ca cac anh cua product theo id va xoa het anh do
            var images = mDbContext.ProductImages.Where(i => i.ProductId == productId);
            foreach (var image in images)
            {
                await mIStorageService.DeleteFileAsync(image.ImagePath);
            }

            mDbContext.Products.Remove(product);
            return await mDbContext.SaveChangesAsync();
        }

        public Task<List<ProductViewModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<ProductViewModel> GetById(string languageId, int productId)
        {
            // 1. Tim dl theo dk o 2 bang
            var product = await mDbContext.Products.FindAsync(productId);
            var productTranslation = await mDbContext.ProductTranslations.FirstOrDefaultAsync(x => x.LanguageId == languageId && x.ProductId == productId);

            if (product == null || productTranslation == null) return null;

            // 2. Co the dung automap. Do du lieu vao doi tuong ProductViewModel
            var productViewModel = new ProductViewModel()
            {
                Id = product.Id,
                DateCreated = product.DateCreated,
                Description = productTranslation.Description != null ? productTranslation.Description : null,
                LanguageId = productTranslation.LanguageId,
                Details = productTranslation != null ? productTranslation.Details : null,
                Name = productTranslation != null ? productTranslation.Name : null,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                SeoAlias = productTranslation != null ? productTranslation.SeoAlias : null,
                SeoDescription = productTranslation != null ? productTranslation.SeoDescription : null,
                SeoTitle = productTranslation != null ? productTranslation.SeoTitle : null,
                Stock = product.Stock,
                ViewCount = product.ViewCount
            };

            return productViewModel;
        }

        public async Task<List<ProductImageViewModel>> GetListImages(int productId)
        {
            return await mDbContext.ProductImages.Where(x => x.ProductId == productId)
               .Select(i => new ProductImageViewModel()
               {
                   Caption = i.Caption,
                   DateCreated = i.DateCreated,
                   FileSize = i.FileSize,
                   Id = i.Id,
                   ImagePath = i.ImagePath,
                   IsDefault = i.IsDefault,
                   ProductId = i.ProductId,
                   SortOrder = i.SortOrder
               }).ToListAsync();
        }

        public async Task<PageResult<ProductViewModel>> GetPaging(GetManagerProductPagingRequest request)
        {
            // 1. Select join
            var query = from p in mDbContext.Products
                        join pt in mDbContext.ProductTranslations on p.Id equals pt.ProductId
                        join pic in mDbContext.ProductInCategories on p.Id equals pic.ProductId
                        join c in mDbContext.Categories on pic.CategoryId equals c.Id
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
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return pageResult;
        }

        public async Task<ProductImageViewModel> GetImageById(int imageId)
        {
            var image = await mDbContext.ProductImages.FindAsync(imageId);
            if (image == null)
                throw new mShopException($"Cannot find an image with id {imageId}");

            var viewModel = new ProductImageViewModel()
            {
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                FileSize = image.FileSize,
                Id = image.Id,
                ImagePath = image.ImagePath,
                IsDefault = image.IsDefault,
                ProductId = image.ProductId,
                SortOrder = image.SortOrder
            };
            return viewModel;
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
            mDbContext.ProductImages.Add(productImage);
            await mDbContext.SaveChangesAsync();
            return productImage.Id;
        }

        public async Task<int> RemoveImage(int imageId)
        {
            var productImage = await mDbContext.ProductImages.FindAsync(imageId);
            if (productImage == null)
                throw new mShopException($"Cannot find an image with id {imageId}");
            mDbContext.ProductImages.Remove(productImage);
            return await mDbContext.SaveChangesAsync();
        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await mDbContext.Products.FindAsync(request.Id);
            var productTranslations = await mDbContext.ProductTranslations
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
                var thumbnailImage = await mDbContext.ProductImages.FirstOrDefaultAsync(i => i.IsDefault == true && i.ProductId == request.Id);
                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                    mDbContext.ProductImages.Update(thumbnailImage);
                }
            }

            return await mDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var productImage = await mDbContext.ProductImages.FindAsync(imageId);
            if (productImage == null)
                throw new mShopException($"Cannot find an image with id {imageId}");

            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            mDbContext.ProductImages.Update(productImage);
            return await mDbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await mDbContext.Products.FindAsync(productId);
            if (product == null) throw new mShopException($"Cannot find a with: {productId}");
            product.Price = newPrice;
            return await mDbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await mDbContext.Products.FindAsync(productId);
            if (product == null) throw new mShopException($"Cannot find a with: {productId}");
            product.Price += addedQuantity;
            return await mDbContext.SaveChangesAsync() > 0;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            // phai check thu muc luu anh da ton tai chua neu chua fai tao thu muc
            // using System.Net.Http.Headers;
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await mIStorageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        //
    }
}