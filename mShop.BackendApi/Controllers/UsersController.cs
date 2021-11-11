using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mShop.Application.System.Users;
using mShop.ViewModel.System.Users;

namespace mShop.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService mIUserService;

        public UsersController(IUserService nIUserService)
        {
            mIUserService = nIUserService;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        // AllowAnonymous cho phep chua login cung goi dc ham nay
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await mIUserService.Authenticate(request);

            if (string.IsNullOrEmpty(result.ResultObj))
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        // AllowAnonymous cho phep chua login cung goi dc ham nay
        // Frombody de post len json
        public async Task<IActionResult> Resgister([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await mIUserService.Register(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        // https://localhost:8080xx/api/users/paging?pageIndex=1&pagesize=10&keyword=xxx
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetUserPagingRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await mIUserService.GetUsersPaging(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await mIUserService.GetById(id);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await mIUserService.Update(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // end class
    }
}