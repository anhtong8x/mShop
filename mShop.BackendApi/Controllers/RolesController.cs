using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mShop.Application.System.Roles;

namespace mShop.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService mIRoleService;

        public RolesController(IRoleService nIRoleService)
        {
            mIRoleService = nIRoleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await mIRoleService.GetAll();
            return Ok(roles);
        }
    }
}
