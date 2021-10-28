using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mShop.Application.Catalog.Products;
using mShop.ViewModel.Catalog.ProductImage;
using mShop.ViewModel.Catalog.Products;

namespace mShop.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IPublicProductService mIPublicProductService;
        private readonly IManageProductService mIManageProductService;

        public ProductsController(IPublicProductService nIPublicProductService, IManageProductService nIManageProductService)
        {
            mIPublicProductService = nIPublicProductService;
            mIManageProductService = nIManageProductService;
        }

        [HttpGet("{languageId}")]
        public async Task<IActionResult> GetAll(string languageId)
        {
            var products = await mIPublicProductService.GetAll(languageId);
            return Ok(products);
        }

        //  http://localhost:port/products?pageIndex=1&pageSize=10&CategoryId=vn-VN
        //  Co nhieu Get nen ta phai chi ra path de swagger hieu. Bang cach khai bao tham so {languageId}
        [HttpGet("public-paging/{languageId}")]
        public async Task<IActionResult> GetAllPagingByLanguageId(string languageId, [FromQuery] GetPublicProductPagingRequest request)
        {
            var products = await mIPublicProductService.GetAllPagingByLanguageId(languageId, request);
            return Ok(products);
        }

        //  https://localhost:port/api/Product/vi-VN/1
        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetById(int productId, string languageId)
        {
            var product = await mIManageProductService.GetById(languageId, productId);
            if (product == null)
                return BadRequest($"Cannot find product {productId}");

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            // 1. Check du lieu
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. Save product thanh cong tra ve productId da luu
            var produtId = await mIManageProductService.Create(request);
            if (produtId == 0)
            {
                return BadRequest("Cannot save product");
            }

            // 3. lay ve product da save thanh cong
            var product = await mIManageProductService.GetById(request.LanguageId, produtId);

            return CreatedAtAction(nameof(GetById), new { id = produtId }, product);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {
            // 1. Check du lieu
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. Update
            var affectedResult = await mIManageProductService.Update(request);
            if (affectedResult == 0)
            {
                return BadRequest("Cannot update product");
            }

            return Ok();    // tra ve ma code 200 thanh cong
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int productId)
        {
            var affectedResult = await mIManageProductService.Delete(productId);
            if (affectedResult == 0)
            {
                return BadRequest("Cannot delete product");
            }
            return Ok();
        }

        // update 1 cot trong bang ta dung HttpPatch
        // phai chi ra {productId}/{price} de swagger hieu
        [HttpPatch("{productId}/{price}")]
        public async Task<IActionResult> UpdatePrice(int productId, decimal price)
        {
            var isSuccessfull = await mIManageProductService.UpdatePrice(productId, price);
            if (isSuccessfull)
                return Ok();
            return BadRequest("Cannot update price");
        }

        [HttpGet("{productId}/images/{imageId}")]
        public async Task<IActionResult> GetImageById(int productId, int imageId)
        {
            var image = await mIManageProductService.GetImageById(imageId);
            if (image == null) { return BadRequest("Cannot find image by id {imageId}"); }
            return Ok(image);
        }

        [HttpPost("{productId}/images")]
        public async Task<IActionResult> CreateImage(int productId, [FromForm] ProductImageCreateRequest request)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var imageId = await mIManageProductService.AddImage(productId, request);
            if (imageId == 0)
            {
                return BadRequest();
            }

            var image = await mIManageProductService.GetImageById(imageId);
            return CreatedAtAction(nameof(GetImageById), new { imageId }, image);
        }

        [HttpPut("{productId}/images/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest();
            var isSuccessfull = await mIManageProductService.UpdateImage(imageId, request);
            if (isSuccessfull == 0)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete("{productId}/image/{imageId}")]
        public async Task<IActionResult> RemoveImage(int imageId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var isSuccessfull = await mIManageProductService.RemoveImage(imageId);

            if (isSuccessfull == 0)
            {
                return BadRequest();
            }

            return Ok();
        }

        // end
    }
}