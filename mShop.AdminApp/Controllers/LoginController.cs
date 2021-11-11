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
    public class LoginController : Controller
    {
        private readonly IUserApiClient mIUserApiClient;
        private readonly IConfiguration mIConfiguration;    // khoi tao 1 lan dung private readonly

        public LoginController(IUserApiClient nIUserApiClient, IConfiguration nIConfiguration)
        {
            mIUserApiClient = nIUserApiClient;
            mIConfiguration = nIConfiguration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // vao trang login thi login het seccsion cu
            // login bang skill nao the logout bang skill do
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginRequest request)
        {
            if (!ModelState.IsValid) return View(ModelState);

            var token = await mIUserApiClient.Authenticate(request);

            // giai ma token
            var userPrincipal = this.ValidateToken(token.ResultObj);
            var authoProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true // = true de khong fai login lai
            };

            // luu session. Day token vao session
            HttpContext.Session.SetString("Token", token.ResultObj);

            // bat dau sigin
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                authoProperties
                );

            // chuyen den trang home
            return RedirectToAction("Index", "Home");
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