using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using mShop.ViewModel.Common;
using mShop.ViewModel.System.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace mShop.AdminApp.Services
{
    public class UserApiClient : IUserApiClient
    {
        // tao doi tuong nay de call api trong backendapi
        private readonly IHttpClientFactory mIHttpClientFactory;

        private readonly IConfiguration mIConfiguration;

        private readonly IHttpContextAccessor mIHttpContextAccessor;    // khoi tao 1 lan de lay ve session

        public UserApiClient(IHttpClientFactory nIHttpClientFactory, IHttpContextAccessor nIHttpContextAccessor,
            IConfiguration nIConfiguration)
        {
            mIHttpClientFactory = nIHttpClientFactory;
            mIConfiguration = nIConfiguration;
            mIHttpContextAccessor = nIHttpContextAccessor;
        }

        // https://localhost:44381/api/Users/authenticate
        public async Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = mIHttpClientFactory.CreateClient();
            //client.BaseAddress = new Uri("https://localhost:44381");
            client.BaseAddress = new Uri(mIConfiguration["BaseAddress"]);

            var response = await client.PostAsync("/api/Users/authenticate", httpContent);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(await response.Content.ReadAsStringAsync());
        }

        // https://localhost:44381/api/Users/paging?Keyword=a&PageIndex=1&PageSize=3&BearerToken=Bearer
        public async Task<ApiResult<PageResult<UserViewModel>>> GetUsersPagings(GetUserPagingRequest request)
        {
            var sessions = mIHttpContextAccessor.HttpContext.Session.GetString("Token");

            var client = mIHttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            client.BaseAddress = new Uri(mIConfiguration["BaseAddress"]);

            var response = await client.GetAsync($"/api/Users/paging?Keyword={request.Keyword}&PageIndex={request.PageIndex}&PageSize={request.PageSize}");

            var body = await response.Content.ReadAsStringAsync();

            var users = JsonConvert.DeserializeObject<ApiSuccessResult<PageResult<UserViewModel>>>(body);

            return users;
        }

        // https://localhost:44381/api/Users/register
        public async Task<ApiResult<bool>> RegisterUser(RegisterRequest request)
        {
            var client = mIHttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(mIConfiguration["BaseAddress"]);
            // Bearer da luu trong annonimos. Token duoc luu trong do

            // Tao moi user se post den UserController.cs .Post fai co 1 content
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Users/register", httpContent);

            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);
            }

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<UserViewModel>> GetById(Guid id)
        {
            var sessions = mIHttpContextAccessor.HttpContext.Session.GetString("Token");
            var client = mIHttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(mIConfiguration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            
            var response = await client.GetAsync($"/api/users/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<UserViewModel>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<UserViewModel>>(body);
        }

        public async Task<ApiResult<bool>> UpdateUser(Guid id, UserUpdateRequest request)
        {
            var client = mIHttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(mIConfiguration["BaseAddress"]);
            var sessions = mIHttpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/users/{id}", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<bool>> Delete(Guid id)
        {
            var sessions = mIHttpContextAccessor.HttpContext.Session.GetString("Token");
            var client = mIHttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(mIConfiguration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.DeleteAsync($"/api/users/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        // end class
    }
}