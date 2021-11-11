using mShop.ViewModel.Common;
using mShop.ViewModel.System.Users;
using System.Threading.Tasks;

namespace mShop.AdminApp.Services
{
    public interface IUserApiClient
    {
        Task<string> Authenticate(LoginRequest request);

        Task<PageResult<UserViewModel>> GetUsersPagings(GetUserPagingRequest request);

        Task<bool> RegisterUser(RegisterRequest request);
    }
}