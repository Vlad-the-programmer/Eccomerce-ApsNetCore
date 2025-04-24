using EcommerceWebApp.ApiServices;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApp.Helpers.ViewComponents
{
    public class ShoppingCartSummaryViewComponent : ViewComponent
    {
        public readonly IApiService _apiService;

        public ShoppingCartSummaryViewComponent(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await CartEndpointsHelperFuncs.GetCartItems(GlobalConstants.GetCartItemsEndpoint, _apiService);
            return View(items.Count);
        }
    }
}
