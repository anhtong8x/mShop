using mShop.ViewModel.System.Users;
using System.Threading.Tasks;

namespace mShop.AdminApp.Services
{
    public interface IUserApiClient
    {
        Task<string> Authenticate(LoginRequest request);
    }
}