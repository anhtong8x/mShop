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
    public class UserController : Controller
    {
        private readonly IUserApiClient mIUserApiClient;
        private readonly IConfiguration mIConfiguration;    // khoi tao 1 lan dung private readonly

        public UserController(IUserApiClient nIUserApiClient, IConfiguration nIConfiguration)
        {
            mIUserApiClient = nIUserApiClient;
            mIConfiguration = nIConfiguration;
        }

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

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            // vao trang login thi login het seccsion cu
            // login bang skill nao the logout bang skill do
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid) return View(ModelState);

            var token = await mIUserApiClient.Authenticate(request);

            // giai ma token
            var userPrincipal = this.ValidateToken(token);
            var authoProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true // = true de khong fai login lai
            };

            // luu session. Day token vao session
            HttpContext.Session.SetString("Token", token);

            // bat dau sigin
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                authoProperties
                );

            // chuyen den trang home
            return RedirectToAction("Index", "Home");
        }

        // ham logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");    // logout thi remove token di
            // chuyen den trang home
            return RedirectToAction("Login", "User"); ;
        }

        // ham giai ma token
        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;

            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = mIConfiguration["Tokens:Issuer"];

            validationParameters.ValidIssuer = mIConfiguration["Tokens:Issuer"];

            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mIConfiguration["Tokens:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler()
                .ValidateToken(jwtToken, validationParameters, out validatedToken);
            return principal;
        }

        // end class
    }
}