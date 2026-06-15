using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using EcommerceWebApp.Models.Dtos;
using EcommerceWebApp.Models.UpdateViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    [Route("products")]
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IApiService _apiService;

        public ProductsController(ILogger<ProductsController> logger, IApiService apiService)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> Index()
        {

            List<CategoryDTO> allCategories = await CategoriesEndpointsHelperFuncs.GetCategories(
                GlobalConstants.CategoriesEndpoint, _apiService);

            List<SubcategoryDTO> allSubcategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                GlobalConstants.SubCategoriesEndpoint, _apiService);

            List<ProductDTO> allProducts = await ProductsEndpointsHelperFuncs.GetProducts(
                GlobalConstants.ProductsEndpoint, _apiService);

            var categoryTree = new List<CategoryTreeViewModel>();
            foreach (var category in allCategories)
            {
                var categoryProductCount = allProducts.Count(p => p.CategoryCode == category.Code);

                var subcategoryTree = new List<SubcategoryTreeViewModel>();
                foreach (var subcategory in allSubcategories.Where(s => s.Category.Code == category.Code))
                {
                    var subcategoryProductCount = allProducts.Count(p => p.SubcategoryCode == subcategory.Code);
                    subcategoryTree.Add(new SubcategoryTreeViewModel
                    {
                        Code = subcategory.Code,
                        Name = subcategory.Name,
                        ProductCount = subcategoryProductCount
                    });
                }

                categoryTree.Add(new CategoryTreeViewModel
                {
                    Code = category.Code,
                    Name = category.Name,
                    ProductCount = categoryProductCount,
                    Subcategories = subcategoryTree
                });
            }

            ViewBag.CategoryTree = categoryTree;
            ViewBag.TotalProductCount = allProducts.Count;
            ViewBag.SearchComboxOptions = await ProductsEndpointsHelperFuncs.GetSearchComboBoxDtos(
                GlobalConstants.GetSearchComboboxDtosEndpoint, _apiService);
            ViewBag.OrderbyComboxOptions = await ProductsEndpointsHelperFuncs.GetOrderByComboBoxDtos(
                GlobalConstants.GetOrderByComboboxDtosEndpoint, _apiService);

            return View(allProducts);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
          [FromQuery] string searchString = "",
          [FromQuery] string? searchProperty = null,
          [FromQuery] string? sortProperty = null,
          [FromQuery] decimal? fromPrice = null,
          [FromQuery] decimal? ToPrice = null,
          [FromQuery] string? categoryCode = null,
          [FromQuery] string? subcategoryCode = null,
          [FromQuery] int? minRating = null,
          [FromQuery] bool sortAscending = false)
        {
            List<ProductDTO> filteredProducts = await ProductsEndpointsHelperFuncs.GetFilteredProducts(
                GlobalConstants.FilterProductsEndpoint, searchString, searchProperty, sortProperty,
                fromPrice, ToPrice, categoryCode, subcategoryCode, minRating, sortAscending, _apiService);

            List<CategoryDTO> allCategories = await CategoriesEndpointsHelperFuncs.GetCategories(
                GlobalConstants.CategoriesEndpoint, _apiService);

            List<SubcategoryDTO> allSubcategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                GlobalConstants.SubCategoriesEndpoint, _apiService);

            List<ProductDTO> allProducts = await ProductsEndpointsHelperFuncs.GetProducts(
                GlobalConstants.ProductsEndpoint, _apiService);

            var categoryTree = new List<CategoryTreeViewModel>();
            foreach (var category in allCategories)
            {
                var categoryProductCount = allProducts.Count(p => p.CategoryCode == category.Code);

                var subcategoryTree = new List<SubcategoryTreeViewModel>();
                foreach (var subcategory in allSubcategories.Where(s => s.Category.Code == category.Code))
                {
                    var subcategoryProductCount = allProducts.Count(p => p.SubcategoryCode == subcategory.Code);
                    subcategoryTree.Add(new SubcategoryTreeViewModel
                    {
                        Code = subcategory.Code,
                        Name = subcategory.Name,
                        ProductCount = subcategoryProductCount
                    });
                }

                categoryTree.Add(new CategoryTreeViewModel
                {
                    Code = category.Code,
                    Name = category.Name,
                    ProductCount = categoryProductCount,
                    Subcategories = subcategoryTree
                });
            }

            ViewBag.CategoryTree = categoryTree;
            ViewBag.TotalProductCount = allProducts.Count;
            ViewBag.SearchComboxOptions = await ProductsEndpointsHelperFuncs.GetSearchComboBoxDtos(
                GlobalConstants.GetSearchComboboxDtosEndpoint, _apiService);
            ViewBag.OrderbyComboxOptions = await ProductsEndpointsHelperFuncs.GetOrderByComboBoxDtos(
                GlobalConstants.GetOrderByComboboxDtosEndpoint, _apiService);

            return View("Index", filteredProducts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var endpoint = $"{GlobalConstants.ProductsEndpoint}/{id}";
            var product = await ProductsEndpointsHelperFuncs.GetProductById(endpoint, _apiService);
            if (product == null)
            {
                return View("NotFound");
            }

            var customerIdClaim = User.FindFirst("CustomerId");
            int? customerId = customerIdClaim != null && int.TryParse(customerIdClaim.Value, out var tempId) ? tempId : null;

            ViewBag.CustomerId = customerId ?? 0;

            return View(product);
        }

        //get: products/Create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            List<CategoryDTO> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesEndpoint, _apiService);
            List<SubcategoryDTO> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
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

            var subcategoryCategoryMap = new Dictionary<string, string>();
            foreach (var sub in subCategories)
            {
                subcategoryCategoryMap[sub.Code] = sub.Category?.Code;
            }
            ViewBag.SubcategoryCategoryMap = subcategoryCategoryMap;

            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewProductViewModel product)
        {
            try
            {
                await _apiService.PostDataAsync(GlobalConstants.ProductCreateEndpoint, JsonSerializer.Serialize(product));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;

                List<CategoryDTO> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesEndpoint, _apiService);
                List<SubcategoryDTO> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
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

                var subcategoryCategoryMap = new Dictionary<string, string>();
                foreach (var sub in subCategories)
                {
                    subcategoryCategoryMap[sub.Code] = sub.Category?.Code;
                }
                ViewBag.SubcategoryCategoryMap = subcategoryCategoryMap;

                return View(product);
            }
            return RedirectToAction(nameof(Index));
        }

        //get: products/Edit/1
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {

            var product = await ProductsEndpointsHelperFuncs.GetProductById(
                                                         $"{GlobalConstants.ProductsEndpoint}/{id}", _apiService);

            if (product == null)
            {

                return View("NotFound");
            }

            List<CategoryDTO> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesEndpoint, _apiService);
            List<SubcategoryDTO> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                                                           GlobalConstants.SubCategoriesEndpoint, _apiService);
            ViewBag.SubCategories = CategoriesEndpointsHelperFuncs
                                                                  .GetSubCategoriesDictionaryWithNameCodeFields(subCategories)
                                                                  .Select(kvp => new SelectListItem
                                                                  {
                                                                      Text = kvp.Key,
                                                                      Value = kvp.Value,
                                                                      Selected = kvp.Value == product.SubcategoryCode
                                                                                                    ? true : false
                                                                  });

            ViewBag.Categories = CategoriesEndpointsHelperFuncs
                                                        .GetCategoriesDictionaryWithNameCodeFields(categories)
                                                        .Select(kvp => new SelectListItem
                                                        {
                                                            Text = kvp.Key,
                                                            Value = kvp.Value,
                                                            Selected = kvp.Value == product.CategoryCode ? true : false
                                                        });

            var subcategoryCategoryMap = new Dictionary<string, string>();
            foreach (var sub in subCategories)
            {
                subcategoryCategoryMap[sub.Code] = sub.Category?.Code;
            }
            ViewBag.SubcategoryCategoryMap = subcategoryCategoryMap;

            var productUpdateVM = new ProductUpdateVM
            {
                Id = product.Id,
                Code = product.Code,
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

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductUpdateVM product)
        {
            if (id != product.Id) return View("NotFound");

            try
            {
                await _apiService.UpdateDataAsync($"{GlobalConstants.ProductUpdateEndpoint}/{id}",
                                                    JsonSerializer.Serialize(product));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;

                List<CategoryDTO> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesEndpoint, _apiService);
                List<SubcategoryDTO> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                                                               GlobalConstants.SubCategoriesEndpoint, _apiService);

                ViewBag.SubCategories = CategoriesEndpointsHelperFuncs
                                                                  .GetSubCategoriesDictionaryWithNameCodeFields(subCategories)
                                                                  .Select(kvp => new SelectListItem
                                                                  {
                                                                      Text = kvp.Key,
                                                                      Value = kvp.Value,
                                                                      Selected = kvp.Value == product.SubcategoryCode
                                                                                                    ? true : false
                                                                  });

                ViewBag.Categories = CategoriesEndpointsHelperFuncs
                                                            .GetCategoriesDictionaryWithNameCodeFields(categories)
                                                            .Select(kvp => new SelectListItem
                                                            {
                                                                Text = kvp.Key,
                                                                Value = kvp.Value,
                                                                Selected = kvp.Value == product.CategoryCode ? true : false
                                                            });
                var subcategoryCategoryMap = new Dictionary<string, string>();
                foreach (var sub in subCategories)
                {
                    subcategoryCategoryMap[sub.Code] = sub.Category?.Code;
                }
                ViewBag.SubcategoryCategoryMap = subcategoryCategoryMap;

                return View(product);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                await _apiService.DeleteDataAsync($"{GlobalConstants.ProductDeleteEndpoint}/{id}");
                return RedirectToAction("Index", "Products");
            }
            catch (HttpRequestException ex)
            {
                return View("NotFound");
            }
        }

    }
}
