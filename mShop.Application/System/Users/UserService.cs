using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using mShop.Data.Entities;
using mShop.Ultilities.Exceptions;
using mShop.ViewModel.System.Users;
using System;
using System.IdentityModel.Tokens.Jwt;
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

        public async Task<string> Authenticate(LoginRequest request)
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
                new Claim(ClaimTypes.Role, string.Join(";",roles)) // noi list cac roles thanh chuoi ngan cach boi ;
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mIConfiguration["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(mIConfiguration["Tokens:Issuer"],
                mIConfiguration["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            var user = new AppUser()
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
                return true;
            }
            return false;
        }
    }
}