using Microsoft.AspNetCore.Mvc;
using mShop.AdminApp.Services;
using mShop.ViewModel.System.Users;
using System.Threading.Tasks;

namespace mShop.AdminApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserApiClient mIUserApiClient;

        public UserController(IUserApiClient nIUserApiClient)
        {
            mIUserApiClient = nIUserApiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid) return View(ModelState);

            var tokent = await mIUserApiClient.Authenticate(request);

            return View(tokent);
        }
    }
}