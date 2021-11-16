using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using mShop.AdminApp.Services;
using mShop.ViewModel.Common;
using mShop.ViewModel.System.Users;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace mShop.AdminApp.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserApiClient mIUserApiClient;
        private readonly IConfiguration mIConfiguration;    // khoi tao 1 lan dung private readonly
        private readonly IRoleApiClient mIRoleApiClient;

        public UserController(IUserApiClient nIUserApiClient, IConfiguration nIConfiguration, IRoleApiClient nIRoleApiClient)
        {
            mIUserApiClient = nIUserApiClient;
            mIConfiguration = nIConfiguration;
            mIRoleApiClient = nIRoleApiClient;
        }

        // pageIndex =1, pagesize = 10 gia tri mac dinh
        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 1)
        {
            // lay ve session da luu
            // var session = HttpContext.Session.GetString("Token");

            var request = new GetUserPagingRequest()
            {
                //BearerToken = session,
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var data = await mIUserApiClient.GetUsersPagings(request);

            // ViewBag truyen gia tri tu controller xuong view. Khi tim ta muon luu lai key nhap vao o tim, ta truyen qua viewbag
            // khai bao viewbag o day va o view index.cshtml
            ViewBag.Keyword = keyword;

            // Nhan tempData - de cap nhat xuong view
            if(TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }

            return View(data.ResultObj);
        }

        // chi tiet user
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await mIUserApiClient.GetById(id);
            return View(result.ResultObj);
        }

        // ham logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");    // logout thi remove token di
            // chuyen den trang home
            return RedirectToAction("Index", "Login"); ;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await mIUserApiClient.RegisterUser(request);

            if (result.IsSuccessed)
            {
                TempData["result"] = "Tạo người dùng thành công";
                return RedirectToAction("Index");   // thanh cong chuyen den action phan trang index ben tren
            }    

            ModelState.AddModelError("", result.Message);

            return View(request);   // tra lai cai view co san du lieu de ta sua trong truong hop loi
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await mIUserApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                var user = result.ResultObj;
                var updateRequest = new UserUpdateRequest()
                {
                    Dob = user.Dob,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Id = id
                };
                return View(updateRequest);
            }

            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await mIUserApiClient.UpdateUser(request.Id, request);
            if (result.IsSuccessed)
            {
                // truyen temData sang view
                TempData["result"] = "Cập nhật người dùng thành công";

                return RedirectToAction("Index");
            }
                

            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            return View(
                new UserDeleteRequest()
                {
                    Id = id
                });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await mIUserApiClient.Delete(request.Id);
            if (result.IsSuccessed)
            {
                // truyen temData sang view
                TempData["result"] = "Xóa người dùng thành công";

                return RedirectToAction("Index");
            }    
                
            ModelState.AddModelError("", result.Message);
            return View(request);
        }


        [HttpGet]
        public async Task<IActionResult> RoleAssign(Guid id)
        {
            var roleAssign = await GetRoleAssignRequest(id);
            return View(roleAssign);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign(RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await mIUserApiClient.RoleAssign(request.Id, request);
            if (result.IsSuccessed)
            {
                // truyen temData sang view
                TempData["result"] = "Cập nhật quyền thành công";

                return RedirectToAction("Index");
            }


            ModelState.AddModelError("", result.Message);

            // truong hop update loi van can show ra cac role cho user nay
            var roleAssign = await GetRoleAssignRequest(request.Id);

            return View(roleAssign);
        }

        private async Task<RoleAssignRequest> GetRoleAssignRequest(Guid id)
        {
            var userObj = await mIUserApiClient.GetById(id);
            var roleObj = await mIRoleApiClient.GetAll();
            var roleAssignRequest = new RoleAssignRequest();

            foreach (var role in roleObj.ResultObj)
            {
                roleAssignRequest.Roles.Add(new SelectItem()
                {
                    Id = role.Id.ToString(),
                    Name = role.Name,
                    Selected = userObj.ResultObj.Roles.Contains(role.Name)
                });
            }
            return roleAssignRequest;
        }
        // end class
    }
}