using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using EcommerceWebApp.Models.UpdateViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApiService _apiService;

        public ProductsController(ILogger<HomeController> logger, IApiService apiService)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
                List<NewProductViewModel> products = await ProductsEndpointsHelperFuncs.GetProducts(
                                                            GlobalConstants.ProductsEndpoint, _apiService);
                List<CategoryViewModel> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                            GlobalConstants.CategoriesEndpoint, _apiService);

                ViewBag.FeaturedProduct = await ProductsEndpointsHelperFuncs.GetFeaturedProduct(
                                                            GlobalConstants.ProductsEndpoint, _apiService);
                ViewBag.Categories = CategoriesEndpointsHelperFuncs.GetCategoriesDictionaryWithNameCodeFields(categories);
                ViewBag.ProductsExists = products.Count > 0 ? true : false;
                ViewBag.CategoriesExist = categories.Count > 0 ? true : false;
                return View(products); // Return the view with the products data
        }

        public async Task<IActionResult> Details(int id)
        {
                var endpoint = GlobalConstants.ProductsEndpoint + $"{id}";
                var product = await ProductsEndpointsHelperFuncs.GetProductById(endpoint, _apiService);
            if (product == null)
            {
                return View("NotFound");
            }
                return View(product); // Return the view with the product's data
        }

        //get: products/Create
        public async Task<IActionResult> Create()
        {
            List<CategoryViewModel> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesEndpoint, _apiService);
            List<SubcategoryViewModel> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                                                           GlobalConstants.SubCategoriesEndpoint, _apiService);

            ViewBag.SubCategories = CategoriesEndpointsHelperFuncs
                                                            .GetSubCategoriesDictionaryWithNameCodeFields(subCategories)
                                                            .Select(kvp => new SelectListItem
                                                            {
                                                                Text = kvp.Key,
                                                                Value = kvp.Value
                                                            });
            ViewBag.Categories = CategoriesEndpointsHelperFuncs
                                                            .GetCategoriesDictionaryWithNameCodeFields(categories)
                                                            .Select(kvp => new SelectListItem
                                                            {
                                                                Text = kvp.Key,
                                                                Value = kvp.Value
                                                            });


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewProductViewModel product)
        {
            try
            {
                await _apiService.PostDataAsync(GlobalConstants.ProductCreateEndpoint, JsonSerializer.Serialize(product));
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = ex.Message;

                List<CategoryViewModel> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesEndpoint, _apiService);
                List<SubcategoryViewModel> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                                                               GlobalConstants.SubCategoriesEndpoint, _apiService);

                ViewBag.SubCategories = CategoriesEndpointsHelperFuncs
                                                            .GetSubCategoriesDictionaryWithNameCodeFields(subCategories)
                                                            .Select(kvp => new SelectListItem
                                                            {
                                                                Text = kvp.Key,
                                                                Value = kvp.Value
                                                            });

                ViewBag.Categories = CategoriesEndpointsHelperFuncs
                                                            .GetCategoriesDictionaryWithNameCodeFields(categories)
                                                            .Select(kvp => new SelectListItem
                                                            {
                                                                Text = kvp.Key,
                                                                Value = kvp.Value
                                                            });

                return View(product);
            }
            return RedirectToAction(nameof(Index));
        }

        //get: products/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            
                var product = await ProductsEndpointsHelperFuncs.GetProductById(
                                                            GlobalConstants.ProductsEndpoint + $"{id}", _apiService);

            if(product == null) { 
                List<CategoryViewModel> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                               GlobalConstants.CategoriesEndpoint, _apiService);
                List<SubcategoryViewModel> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                                                               GlobalConstants.SubCategoriesEndpoint, _apiService);
                ViewBag.SubCategories = CategoriesEndpointsHelperFuncs
                                                                            .GetSubCategoriesDictionaryWithNameCodeFields(subCategories)
                                                                            .Select(kvp => new SelectListItem
                                                                            {
                                                                                Text = kvp.Key,
                                                                                Value = kvp.Value
                                                                            });

                ViewBag.Categories = CategoriesEndpointsHelperFuncs
                                                            .GetCategoriesDictionaryWithNameCodeFields(categories)
                                                            .Select(kvp => new SelectListItem
                                                            {
                                                                Text = kvp.Key,
                                                                Value = kvp.Value
                                                            });

                return View("NotFound");
            }
            var productUpdateVM = new ProductUpdateVM
            {
                ProductId = product.Id,
                Name = product.Name,
                Brand = product.Brand,
                Photo = product.Photo,
                OtherPhotos = product.OtherPhotos,  
                Price = product.Price,  
                About = product.About,
                LongAbout = product.LongAbout,
                Stock = product.Stock,
                SubcategoryCode = product.SubcategoryCode,
                CategoryCode = product.CategoryCode,    
                IsActive = product.IsActive,
            };
            return View(productUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductUpdateVM product)
        {
            if (id != product.ProductId) return View("NotFound");

            try
            {
                await _apiService.UpdateDataAsync(GlobalConstants.ProductUpdateEndpoint + $"{id}",
                                                    JsonSerializer.Serialize(product));
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = ex.Message;

                List<CategoryViewModel> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesEndpoint, _apiService);
                List<SubcategoryViewModel> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                                                               GlobalConstants.SubCategoriesEndpoint, _apiService);

                ViewBag.SubCategories = CategoriesEndpointsHelperFuncs
                                                            .GetSubCategoriesDictionaryWithNameCodeFields(subCategories)
                                                            .Select(kvp => new SelectListItem
                                                            {
                                                                Text = kvp.Key,
                                                                Value = kvp.Value
                                                            });

                ViewBag.Categories = CategoriesEndpointsHelperFuncs
                                                            .GetCategoriesDictionaryWithNameCodeFields(categories)
                                                            .Select(kvp => new SelectListItem
                                                            {
                                                                Text = kvp.Key,
                                                                Value = kvp.Value
                                                            });

                return View(product);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var product = await ProductsEndpointsHelperFuncs.GetProductById(
                                                        GlobalConstants.ProductDeleteEndpoint + $"{id}", _apiService);

            if (product == null)
            {
                return View("NotFound");
            }
            return RedirectToAction("Index", "Products");
        }
    }
}
