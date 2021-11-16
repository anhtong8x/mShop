using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using mShop.ViewModel.Common;
using mShop.ViewModel.System.Roles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace mShop.AdminApp.Services
{
    public class RoleApiClient : IRoleApiClient
    {
        // tao doi tuong nay de call api trong backendapi
        private readonly IHttpClientFactory mIHttpClientFactory;

        private readonly IConfiguration mIConfiguration;

        private readonly IHttpContextAccessor mIHttpContextAccessor;    // khoi tao 1 lan de lay ve session

        public RoleApiClient(IHttpClientFactory nIHttpClientFactory, IHttpContextAccessor nIHttpContextAccessor,
            IConfiguration nIConfiguration)
        {
            mIHttpClientFactory = nIHttpClientFactory;
            mIConfiguration = nIConfiguration;
            mIHttpContextAccessor = nIHttpContextAccessor;
        }

        public async Task<ApiResult<List<RoleViewModel>>> GetAll()
        {
            var sessions = mIHttpContextAccessor.HttpContext.Session.GetString("Token");
            var client = mIHttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(mIConfiguration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            // goi den  rolescontroller backendapi
            var response = await client.GetAsync($"/api/roles/");
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                List<RoleViewModel> myDeserializedObjList = (List<RoleViewModel>)JsonConvert.DeserializeObject(body, typeof(List<RoleViewModel>));
                return new ApiSuccessResult<List<RoleViewModel>>(myDeserializedObjList);
            }
            return JsonConvert.DeserializeObject<ApiErrorResult<List<RoleViewModel>>>(body);
        }
    }
}
