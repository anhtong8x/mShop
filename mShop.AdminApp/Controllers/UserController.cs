using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using mShop.AdminApp.Services;
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

        public UserController(IUserApiClient nIUserApiClient, IConfiguration nIConfiguration)
        {
            mIUserApiClient = nIUserApiClient;
            mIConfiguration = nIConfiguration;
        }

        // pageIndex =1, pagesize = 10 gia tri mac dinh
        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            // lay ve session da luu
            var session = HttpContext.Session.GetString("Token");

            var request = new GetUserPagingRequest()
            {
                BearerToken = session,
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var data = await mIUserApiClient.GetUsersPagings(request);

            return View(data);
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

            if (result)
                return RedirectToAction("Index");   // thanh cong chuyen den action phan trang index ben tren

            return View(request);   // tra lai cai view co san du lieu de ta sua trong truong hop loi
        }

        // end class
    }
}