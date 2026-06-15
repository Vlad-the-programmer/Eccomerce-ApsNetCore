using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using EcommerceWebApp.Models.Dtos;
using EcommerceWebApp.Models.UpdateVIewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    public class SubcategoryController : Controller
    {
        private readonly ILogger<SubcategoryController> _logger;
        private readonly IApiService _apiService;

        public SubcategoryController(ILogger<SubcategoryController> logger, IApiService apiService)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {


            List<CategoryDTO> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesEndpoint, _apiService);
            List<SubcategoryDTO> subCategories = await CategoriesEndpointsHelperFuncs.GetSubCategories(
                                                           GlobalConstants.SubCategoriesEndpoint, _apiService);

            ViewBag.SubCategories = subCategories;

            return View(categories);
        }


        [HttpGet("create/{categoryId}")]
        public async Task<IActionResult> Create(int categoryId)
        {
            List<CategoryDTO> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesEndpoint, _apiService);

            var category = categories.Where(c => c.Id == categoryId).FirstOrDefault();

            ViewBag.Categories = CategoriesEndpointsHelperFuncs
                    .GetCategoriesDictionaryWithNameCodeFields(categories)
                    .Select(kvp => new SelectListItem
                    {
                        Text = kvp.Key,
                        Value = kvp.Value,
                        Selected = kvp.Value == category?.Code ? true : false
                    });
            return View();
        }


        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewSubcategoryVM newSubCategory)
        {
            try
            {
                await _apiService.PostDataAsync(GlobalConstants.SubCategoriesEndpoint, JsonSerializer.Serialize(newSubCategory));
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

                return View(newSubCategory);
            }
            return RedirectToAction(nameof(Index), "Category");
        }


        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {

            var subCategory = await CategoriesEndpointsHelperFuncs.GetSubCategoryById(
                                                         $"{GlobalConstants.SubCategoriesEndpoint}", id, _apiService);

            if (subCategory?.CategoryId == 0 && String.IsNullOrEmpty(subCategory.Name) || subCategory == null)
            {
                return View("NotFound");
            }

            List<CategoryDTO> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                           GlobalConstants.CategoriesEndpoint, _apiService);

            CategoryDTO? category = await CategoriesEndpointsHelperFuncs.GetCategoryById(
                GlobalConstants.CategoriesEndpoint, subCategory.CategoryId, _apiService);

            ViewBag.Categories = CategoriesEndpointsHelperFuncs
                                                                  .GetCategoriesDictionaryWithNameCodeFields(categories)
                                                                  .Select(kvp => new SelectListItem
                                                                  {
                                                                      Text = kvp.Key,
                                                                      Value = kvp.Value,
                                                                      Selected = kvp.Value == category?.Code
                                                                                                    ? true : false
                                                                  });


            var subCategoryUpdateVM = new SubcategoryUpdateVM
            {
                Id = subCategory.Id,
                Code = subCategory.Code,
                Name = subCategory.Name,
                About = subCategory.About,
                CategoryCode = category.Code,
                IsActive = subCategory.IsActive,
            };
            return View(subCategoryUpdateVM);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubcategoryUpdateVM subCategory)
        {
            if (id != subCategory.Id) return View("NotFound");

            try
            {
                await _apiService.UpdateDataAsync($"{GlobalConstants.SubCategoriesEndpoint}/{id}",
                                                    JsonSerializer.Serialize(subCategory));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;


                var subCategoryObj = await CategoriesEndpointsHelperFuncs.GetSubCategoryById(
                                                             $"{GlobalConstants.SubCategoriesEndpoint}", id, _apiService);

                List<CategoryDTO> categories = await CategoriesEndpointsHelperFuncs.GetCategories(
                                                               GlobalConstants.CategoriesEndpoint, _apiService);

                CategoryDTO? category = await CategoriesEndpointsHelperFuncs.GetCategoryById(
                    GlobalConstants.SubCategoriesEndpoint, subCategoryObj.CategoryId, _apiService);

                ViewBag.Categories = CategoriesEndpointsHelperFuncs
                                                                      .GetCategoriesDictionaryWithNameCodeFields(categories)
                                                                      .Select(kvp => new SelectListItem
                                                                      {
                                                                          Text = kvp.Key,
                                                                          Value = kvp.Value,
                                                                          Selected = kvp.Value == category?.Code
                                                                                                        ? true : false
                                                                      });
                return View(subCategory);
            }
            return RedirectToAction(nameof(Index), "Category");
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _apiService.DeleteDataAsync($"{GlobalConstants.SubCategoriesEndpoint}/{id}");
                TempData["Success"] = "Subcategory deleted successfully.";
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                _logger.LogError(ex, "Error deleting subcategory {Id}", id);
            }

            return RedirectToAction("Index", "Category");
        }
    }
}
