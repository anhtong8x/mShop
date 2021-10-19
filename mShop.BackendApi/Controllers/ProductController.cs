using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mShop.Application.Catalog.Products;

namespace mShop.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IPublicProductService mIPublicProductService;

        public ProductController(IPublicProductService nIPublicProductService)
        {
            mIPublicProductService = nIPublicProductService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await mIPublicProductService.GetAll();
            return Ok(products);
        }
    }
}