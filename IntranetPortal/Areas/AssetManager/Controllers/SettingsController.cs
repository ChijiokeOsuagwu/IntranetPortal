using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.AssetManager.Models;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IntranetPortal.Areas.AssetManager.Controllers
{
    [Area("AssetManager")]
    public class SettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IAssetManagerService _assetManagerService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SettingsController(IConfiguration configuration, IAssetManagerService assetManagerService,
                                    IGlobalSettingsService globalSettingsService, IWebHostEnvironment webHostingEnvironment)
        {
            _configuration = configuration;
            _assetManagerService = assetManagerService;
            _globalSettingsService = globalSettingsService;
            _webHostEnvironment = webHostingEnvironment;
        }

        //======================== Asset Category Controller Actions =======================================================//
        #region Asset Category Controller Actions
        public async Task<IActionResult> Categories(string searchString = null)
        {
            AssetCategoryListViewModel model = new AssetCategoryListViewModel();
            IEnumerable<AssetCategory> categoryList;
            if (string.IsNullOrEmpty(searchString))
            {
                categoryList = await _assetManagerService.GetAssetCategoriesAsync();
            }
            else
            {
                categoryList = await _assetManagerService.SearchAssetCategoriesByNameAsync(searchString);
            }
            ViewData["CurrentFilter"] = searchString;
            model.AssetCategoryList = categoryList.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            AssetCategoryViewModel model = new AssetCategoryViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(AssetCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetCategory category = model.ConvertToAssetCategory();
                    bool succeeded = await _assetManagerService.CreateAssetCategoryAsync(category);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Asset Category was created successfully!";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                    model.OperationIsCompleted = true;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            AssetCategory category = new AssetCategory();
            AssetCategoryViewModel model = new AssetCategoryViewModel();
            if (id > 0)
            {
                category = await _assetManagerService.GetAssetCategoryByIdAsync(id);
                model.ID = category.ID;
                model.Name = category.Name;
                model.Description = category.Description;
            }
            else
            {
                return RedirectToAction("AddCategory");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(AssetCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetCategory category = model.ConvertToAssetCategory();
                    bool succeeded = await _assetManagerService.UpdateAssetCategoryAsync(category);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Asset Category was updated successfully!";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                    model.OperationIsCompleted = true;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            AssetCategory category = new AssetCategory();
            AssetCategoryViewModel model = new AssetCategoryViewModel();
            List<AssetType> assetTypes = new List<AssetType>();
            var entities = await _assetManagerService.GetAssetTypesByCategoryIdAsync(id);
            assetTypes = entities.ToList();
            if (assetTypes != null && assetTypes.Count > 0)
            {
                model.OperationIsCompleted = true;
                model.ViewModelErrorMessage = $"This item cannot be deleted because there are other records in the system that depend on it.";
            }

            if (id > 0)
            {
                category = await _assetManagerService.GetAssetCategoryByIdAsync(id);
                model.ID = category.ID;
                model.Name = category.Name;
                model.Description = category.Description;
            }
            else
            {
                return RedirectToAction("Categories", "Settings");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(AssetCategoryViewModel model)
        {
            if (model != null)
            {
                try
                {
                    bool succeeded = await _assetManagerService.DeleteAssetCategoryAsync(model.ID.Value);
                    if (succeeded)
                    {
                        return RedirectToAction("Categories");
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                    model.OperationIsCompleted = true;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }
        #endregion

        //======================== Asset Types Controller Actions =========================================================//
        #region Asset Types Controller Action
        public async Task<IActionResult> AssetTypes(string searchString = null)
        {
            AssetTypeListViewModel model = new AssetTypeListViewModel();
            IEnumerable<AssetType> typeList;
            if (string.IsNullOrEmpty(searchString))
            {
                typeList = await _assetManagerService.GetAssetTypesAsync();
            }
            else
            {
                typeList = await _assetManagerService.SearchAssetTypesByNameAsync(searchString);
            }
            ViewData["CurrentFilter"] = searchString;
            model.AssetTypeList = typeList.ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddAssetType()
        {
            AssetTypeViewModel model = new AssetTypeViewModel();
            var categories = await _assetManagerService.GetAssetCategoriesAsync();
            ViewBag.CategoryList = new SelectList(categories, "ID", "Name");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddAssetType(AssetTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetType assetType = model.ConvertToAssetType();
                    bool succeeded = await _assetManagerService.CreateAssetTypeAsync(assetType);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Asset Type was created successfully!";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                    model.OperationIsCompleted = true;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            var categories = await _assetManagerService.GetAssetCategoriesAsync();
            ViewBag.CategoryList = new SelectList(categories, "ID", "Name");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditAssetType(int id)
        {
            AssetType assetType = new AssetType();
            AssetTypeViewModel model = new AssetTypeViewModel();
            if (id > 0)
            {
                assetType = await _assetManagerService.GetAssetTypeByIdAsync(id);
                model.ID = assetType.ID;
                model.Name = assetType.Name;
                model.Description = assetType.Description;
                model.CategoryID = assetType.CategoryID;
                model.CategoryName = assetType.CategoryName;
            }
            else
            {
                return RedirectToAction("AddAssetType");
            }
            var categories = await _assetManagerService.GetAssetCategoriesAsync();
            ViewBag.CategoryList = new SelectList(categories, "ID", "Name");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAssetType(AssetTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetType assetType = model.ConvertToAssetType();
                    bool succeeded = await _assetManagerService.UpdateAssetTypeAsync(assetType);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Asset Type was updated successfully!";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                    model.OperationIsCompleted = true;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            var categories = await _assetManagerService.GetAssetCategoriesAsync();
            ViewBag.CategoryList = new SelectList(categories, "ID", "Name");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AssetTypeDetails(int id)
        {
            AssetType assetType = new AssetType();
            AssetTypeViewModel model = new AssetTypeViewModel();
            if (id > 0)
            {
                assetType = await _assetManagerService.GetAssetTypeByIdAsync(id);
                model.ID = assetType.ID;
                model.Name = assetType.Name;
                model.Description = assetType.Description;
                model.CategoryID = assetType.CategoryID;
                model.CategoryName = assetType.CategoryName;
            }
            else
            {
                return RedirectToAction("AssetTypes");
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteAssetType(int id)
        {
            AssetType assetType = new AssetType();
            AssetTypeViewModel model = new AssetTypeViewModel();
            if (id > 0)
            {
                assetType = await _assetManagerService.GetAssetTypeByIdAsync(id);
                model.ID = assetType.ID;
                model.Name = assetType.Name;
                model.Description = assetType.Description;
                model.CategoryID = assetType.CategoryID;
                model.CategoryName = assetType.CategoryName;
            }
            else
            {
                return RedirectToAction("AssetTypes");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAssetType(AssetTypeViewModel model)
        {
            if (model != null)
            {
                try
                {
                    bool succeeded = await _assetManagerService.DeleteAssetTypeAsync(model.ID.Value);
                    if (succeeded)
                    {
                        return RedirectToAction("AssetTypes");
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                    model.OperationIsCompleted = true;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }
        #endregion

        //======================== Assets Helper Methods ======================================//
        #region Assets Helper Methods

        [HttpGet]
        public JsonResult GetAssetNames(string text)
        {
            List<string> assets = _assetManagerService.SearchAssetsByNameAsync(text).Result.Select(x => x.AssetName).ToList();
            return Json(assets);
        }

        [HttpGet]
        public JsonResult GetAssetParameters(string nm)
        {
            Asset asset = new Asset();

            asset = _assetManagerService.GetAssetByNameAsync(nm).Result;

            if (asset == null || string.IsNullOrWhiteSpace(asset.AssetName))
            {
                asset = new Asset
                {
                    AssetID = string.Empty,
                    AssetName = string.Empty
                };
            }

            string model = JsonConvert.SerializeObject(new
            {
                asset_id = asset.AssetID,
                asset_name = asset.AssetName,
                asset_description = asset.AssetDescription,
                asset_status = asset.UsageStatus,
                asset_type_id = asset.AssetTypeID,
                asset_type_name = asset.AssetTypeName,
            }, Formatting.Indented);
            return Json(model);
        }

        #endregion

    }
}