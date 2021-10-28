using mShop.ViewModel.System.Users;
using System.Threading.Tasks;

namespace mShop.Application.System.Users
{
    public interface IUserService
    {
        public Task<string> Authenticate(LoginRequest request);

        public Task<bool> Register(RegisterRequest request);
    }
}