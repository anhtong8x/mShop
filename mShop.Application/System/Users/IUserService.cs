using mShop.ViewModel.Common;
using mShop.ViewModel.System.Users;
using System;
using System.Threading.Tasks;

namespace mShop.Application.System.Users
{
    public interface IUserService
    {
        public Task<ApiResult<string>> Authenticate(LoginRequest request);

        public Task<ApiResult<bool>> Register(RegisterRequest request);

        public Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request);

        public Task<ApiResult<UserViewModel>> GetById(Guid id);

        public Task<ApiResult<PageResult<UserViewModel>>> GetUsersPaging(GetUserPagingRequest request);
    }
}