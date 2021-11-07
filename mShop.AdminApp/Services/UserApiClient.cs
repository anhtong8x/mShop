using mShop.ViewModel.System.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace mShop.AdminApp.Services
{
    public class UserApiClient : IUserApiClient
    {
        // tao doi tuong nay de call api trong backendapi
        private readonly IHttpClientFactory mIHttpClientFactory;

        public UserApiClient(IHttpClientFactory nIHttpClientFactory)
        {
            mIHttpClientFactory = nIHttpClientFactory;
        }

        public async Task<string> Authenticate(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = mIHttpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:44381"); //https://localhost:44381/api/Users/authenticate

            var response = await client.PostAsync("/api/Users/authenticate", httpContent);

            var token = await response.Content.ReadAsStringAsync();
            return token;
        }
    }
}