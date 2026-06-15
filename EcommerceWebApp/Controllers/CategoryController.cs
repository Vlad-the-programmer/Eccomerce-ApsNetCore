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
    [Route("categories")]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IApiService _apiService;

        public CategoryController(ILogger<CategoryController> logger, IApiService apiService)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<CategoryDTO> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesAdminEndpoint, _apiService);
            List<SubcategoryDTO> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                                                           GlobalConstants.SubCategoriesEndpoint, _apiService);

            //ViewBag.Categories = categories;
            ViewBag.SubCategories = subCategories;

            return View(categories);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            List<SubcategoryDTO> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                                                           GlobalConstants.SubCategoriesEndpoint, _apiService);
            ViewBag.SubCategories = CategoriesEndpointsHelperFuncs
                    .GetSubCategoriesDictionaryWithNameCodeFields(subCategories)
                    .Select(kvp => new SelectListItem
                    {
                        Text = kvp.Key,
                        Value = kvp.Value
                    });
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewCategoryViewModel newCategory)
        {
            try
            {
                await _apiService.PostDataAsync(GlobalConstants.CategoriesEndpoint, JsonSerializer.Serialize(newCategory));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;

                List<SubcategoryDTO> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                                                               GlobalConstants.SubCategoriesEndpoint, _apiService);

                ViewBag.SubCategories = CategoriesEndpointsHelperFuncs
                        .GetSubCategoriesDictionaryWithNameCodeFields(subCategories)
                        .Select(kvp => new SelectListItem
                        {
                            Text = kvp.Key,
                            Value = kvp.Value
                        });

                return View(newCategory);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await CategoriesEndpointsHelperFuncs.GetCategoryById(GlobalConstants.CategoriesEndpoint, id, _apiService);

            if (category?.Id == 0 && String.IsNullOrEmpty(category.Name))
            {
                return View("NotFound");
            }

            if (category == null)
            {
                return View("NotFound");
            }

            List<CategoryDTO> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesEndpoint, _apiService);
            List<SubcategoryDTO> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                                                           GlobalConstants.SubCategoriesEndpoint, _apiService);

            SubcategoryDTO? subcategory = await CategoriesEndpointsHelperFuncs.GetSubCategoryById(
                GlobalConstants.SubCategoriesEndpoint, category.SubcategoryId, _apiService);

            ViewBag.SubCategories = CategoriesEndpointsHelperFuncs
                                                                  .GetSubCategoriesDictionaryWithNameCodeFields(subCategories)
                                                                  .Select(kvp => new SelectListItem
                                                                  {
                                                                      Text = kvp.Key,
                                                                      Value = kvp.Value,
                                                                      Selected = kvp.Value == subcategory?.Code
                                                                                                    ? true : false
                                                                  });

            var categoryUpdateVM = new CategoryUpdateVM
            {
                Id = category.Id,
                Code = category.Code,
                Name = category.Name,
                About = category.About,
                IsActive = category.IsActive,
            };
            return View(categoryUpdateVM);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryUpdateVM category)
        {

            if (id != category.Id) return View("NotFound");

            try
            {
                await _apiService.UpdateDataAsync($"{GlobalConstants.CategoriesEndpoint}/{id}",
                                                    JsonSerializer.Serialize(category));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;

                List<CategoryDTO> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesEndpoint, _apiService);
                List<SubcategoryDTO> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                                                               GlobalConstants.SubCategoriesEndpoint, _apiService);

                CategoryDTO? categoryDto = categories.Where(c => c.Id == id).FirstOrDefault();

                SubcategoryDTO? subcategory = await CategoriesEndpointsHelperFuncs.GetSubCategoryById(
                                GlobalConstants.SubCategoriesEndpoint, categories.Where(c => c.Id == id).First().SubcategoryId, _apiService);

                ViewBag.SubCategories = CategoriesEndpointsHelperFuncs
                                        .GetSubCategoriesDictionaryWithNameCodeFields(subCategories)
                                        .Select(kvp => new SelectListItem
                                        {
                                            Text = kvp.Key,
                                            Value = kvp.Value,
                                            Selected = kvp.Value == subcategory?.Code
                                                                          ? true : false
                                        });
                return View(category);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                await _apiService.DeleteDataAsync($"{GlobalConstants.CategoriesEndpoint}/{id}");
                return RedirectToAction("Index", "Categories");
            }
            catch (HttpRequestException ex)
            {
                return View("NotFound");
            }
        }

    }
}
