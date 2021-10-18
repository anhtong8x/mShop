using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mShop.Application.Catalog.Products;
using mShop.Ultilities.Constants;

namespace mShop.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IPublicProductService _PublicProductService;

        public ProductController(IPublicProductService mPublicProductService)
        {
            _PublicProductService = mPublicProductService;
        }

        // test controller
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    return Ok("test");
        //}

        //
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //var products = await _PublicProductService.GetAll();
            //return Ok(products);
            return Ok(SystemConstants.MainConnectionString);
        }
    }
}