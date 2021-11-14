using Microsoft.AspNetCore.Mvc;
using mShop.ViewModel.Common;
using System.Threading.Tasks;

namespace mShop.AdminApp.Controllers.Components
{
    public class PagerViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(PagedResultBase result)
        {
            return Task.FromResult((IViewComponentResult)View("Default", result));
        }
    }
}