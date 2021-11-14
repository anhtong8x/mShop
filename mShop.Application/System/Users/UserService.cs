using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using mShop.Data.Entities;
using mShop.Ultilities.Exceptions;
using mShop.ViewModel.Common;
using mShop.ViewModel.System.Users;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace mShop.Application.System.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> mUserManager;
        private readonly SignInManager<AppUser> mSignInManager;
        private readonly RoleManager<AppRole> mRoleManager;
        private readonly IConfiguration mIConfiguration;

        public UserService(UserManager<AppUser> nUserManager, SignInManager<AppUser> nSignInManager, RoleManager<AppRole> nRoleManager, IConfiguration nIConfiguration)
        {
            mUserManager = nUserManager;
            mSignInManager = nSignInManager;
            mRoleManager = nRoleManager;
            mIConfiguration = nIConfiguration;
        }

        public async Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            var user = await mUserManager.FindByNameAsync(request.UserName);
            if (user == null) throw new mShopException($"Cannot find a user{request.UserName}");

            var result = await mSignInManager.PasswordSignInAsync(user, request.PassWord,
                request.RememberMe, true);
            if (!result.Succeeded)
            {
                return null;
            }

            // lay ra list cac role cua user
            var roles = await mUserManager.GetRolesAsync(user);

            // tao ra claim, day cac thong tin vao claim
            var claims = new[] {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.Email),
                new Claim(ClaimTypes.Role, string.Join(";",roles)),// noi list cac roles thanh chuoi ngan cach boi ;
                new Claim(ClaimTypes.Name, request.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mIConfiguration["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(mIConfiguration["Tokens:Issuer"],
                mIConfiguration["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new ApiSuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<ApiResult<PageResult<UserViewModel>>> GetUsersPaging(GetUserPagingRequest request)
        {
            // lay ve bang user
            var query = mUserManager.Users;

            // dung linq de tim theo keyword trong request
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(
                    x => x.UserName.Contains(request.Keyword) || x.PhoneNumber.Contains(request.Keyword)
                    );
            }

            // paging
            int totalRow = await query.CountAsync();

            // skip - lay ve
            var data = await query.Skip(
                    (request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(
                    x => new UserViewModel()
                    {
                        Email = x.Email,
                        UserName = x.UserName,
                        PhoneNumber = x.PhoneNumber,
                        Dob = x.Dob,
                        LastName = x.LastName,
                        FirstName = x.FirstName,
                        Id = x.Id
                    })
                .ToListAsync();

            // select
            var pageResult = new PageResult<UserViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PageResult<UserViewModel>>(pageResult);
        }

        public async Task<ApiResult<bool>> Register(RegisterRequest request)
        {
            var user = await mUserManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                return new ApiErrorResult<bool>("Tài khoản đã tồn tại");
            }
            if (await mUserManager.FindByEmailAsync(request.Email) != null)
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }

            user = new AppUser()
            {
                Dob = request.Dob,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber
            };

            var result = await mUserManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Đăng ký không thành công");
        }

        public async Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request)
        {
            if (await mUserManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id))
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }
            var user = await mUserManager.FindByIdAsync(id.ToString());
            user.Dob = request.Dob;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;

            var result = await mUserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Cập nhật không thành công");
        }

        public async Task<ApiResult<UserViewModel>> GetById(Guid id)
        {
            var user = await mUserManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<UserViewModel>("User không tồn tại");
            }
            var userVm = new UserViewModel()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                Dob = user.Dob,
                Id = user.Id,
                LastName = user.LastName,
                UserName = user.UserName
            };
            return new ApiSuccessResult<UserViewModel>(userVm);
        }

        public async Task<ApiResult<bool>> Delete(Guid id)
        {
            var user = await mUserManager.FindByIdAsync(id.ToString());
            if (user == null)
                return new ApiErrorResult<bool>("User không tồn tại");
            var result = await mUserManager.DeleteAsync(user);

            if(result.Succeeded)               
                return new ApiSuccessResult<bool>();

            return new ApiErrorResult<bool>("Không thể xóa User");
        }

        // end class
    }
}