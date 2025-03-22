using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceWebApp.Models;
using EcommerceWebApp.ApiServices;
using System.Text.Json.Serialization;
using System.Text.Json;
using EcommerceWebApp.Helpers;
namespace EcommerceWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IApiService _apiService;
    public HomeController(ILogger<HomeController> logger, IApiService apiService)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var endpoint = "api/products";

            List<NewProductViewModel> products = await ProductsEndpointsHelperFuncs.GetProducts("api/products", _apiService);
            List<CategoryViewModel> categories = await CategoriesEndpointsHelperFuncs.GetCategories("api/categories", _apiService);

            ViewBag.FeaturedProduct = await ProductsEndpointsHelperFuncs.GetFeaturedProduct(endpoint, _apiService);
            ViewBag.Categories = CategoriesEndpointsHelperFuncs.GetCategoriesDictionaryWithNameCodeFields(categories);
            ViewBag.ProductsExists = products.Count > 0 ? true : false;
            ViewBag.CategoriesExist = categories.Count > 0 ? true : false;
            return View(products); // Return the view with the products data
        }
        catch (HttpRequestException ex)
        {
            // Handle API errors
            ViewBag.ErrorMessage = ex.Message;
            return View(); // Return the view with an error message
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var endpoint = "api/products/{id}";
            var product = ProductsEndpointsHelperFuncs.GetProductById(endpoint, _apiService);
            return View(product); // Return the view with the product's data
        }
        catch (HttpRequestException ex)
        {
            return View("NotFound"); 
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
