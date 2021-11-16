using mShop.ViewModel.Common;
using mShop.ViewModel.System.Roles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mShop.AdminApp.Services
{
    public interface IRoleApiClient
    {
        Task<ApiResult<List<RoleViewModel>>> GetAll();
    }
}
