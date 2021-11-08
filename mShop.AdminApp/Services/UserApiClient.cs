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

        public UserApiClient(IHttpClientFactory nIHttpClientFactory, IConfiguration nIConfiguration)
        {
            mIHttpClientFactory = nIHttpClientFactory;
            mIConfiguration = nIConfiguration;
        }

        public async Task<string> Authenticate(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = mIHttpClientFactory.CreateClient();
            //client.BaseAddress = new Uri("https://localhost:44381");
            client.BaseAddress = new Uri(mIConfiguration["BaseAddress"]);

            var response = await client.PostAsync("/api/Users/authenticate", httpContent);

            var token = await response.Content.ReadAsStringAsync();
            return token;
        }

        // https://localhost:44381/api/Users/paging?Keyword=a&PageIndex=1&PageSize=3&BearerToken=Bearer
        public async Task<PageResult<UserViewModel>> GetUsersPagings(GetUserPagingRequest request)
        {
            var client = mIHttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.BearerToken);
            client.BaseAddress = new Uri(mIConfiguration["BaseAddress"]);

            var response = await client.GetAsync($"/api/Users/paging?Keyword={request.Keyword}&PageIndex={request.PageIndex}&PageSize={request.PageSize}");

            var body = await response.Content.ReadAsStringAsync();

            var users = JsonConvert.DeserializeObject<PageResult<UserViewModel>>(body);

            return users;
        }

        // end class
    }
}