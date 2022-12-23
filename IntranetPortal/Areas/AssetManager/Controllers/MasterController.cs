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
    public class MasterController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IAssetManagerService _assetManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MasterController(IConfiguration configuration, ISecurityService securityService,
                                IBaseModelService baseModelService, IAssetManagerService assetManagerService,
                                IGlobalSettingsService globalSettingsService, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _assetManagerService = assetManagerService;
            _globalSettingsService = globalSettingsService;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> List(string an = null, int? tp = null, int? ct = null)
        {
            AssetListViewModel model = new AssetListViewModel();
            IEnumerable<Asset> assetList = new List<Asset>();
            try
            {
                if (!string.IsNullOrWhiteSpace(an))
                {
                    assetList = await _assetManagerService.SearchAssetsByNameAsync(an);
                }
                else if (tp != null && tp.Value > 0)
                {
                    assetList = await _assetManagerService.GetAssetsByAssetTypeIdAsync(tp.Value);
                }
                else if (ct != null && ct.Value > 0)
                {
                    assetList = await _assetManagerService.GetAssetsByCategoryIdAsync(ct.Value);
                }
                model.AssetList = assetList.ToList();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                assetList = null;
            }
            var assetTypes = await _assetManagerService.GetAssetTypesAsync();
            var assetCategories = await _assetManagerService.GetAssetCategoriesAsync();
            ViewBag.AssetTypeList = new SelectList(assetTypes, "ID", "Name");
            ViewBag.AssetCategoryList = new SelectList(assetCategories, "ID", "Name");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddAsset()
        {
            AssetViewModel model = new AssetViewModel();
            var types = await _assetManagerService.GetAssetTypesAsync();
            var locations = await _globalSettingsService.GetAllLocationsAsync();

            ViewBag.TypesList = new SelectList(types, "ID", "Name");
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsset(AssetViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string uploadsFolder = null;
                    string absoluteFilePath = null;
                    AssetType assetType = await _assetManagerService.GetAssetTypeByIdAsync(model.AssetTypeID);
                    int categoryId = assetType.CategoryID;

                    if (model.ImageUpload != null && model.ImageUpload.Length > 0)
                    {
                        var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        FileInfo fileInfo = new FileInfo(model.ImageUpload.FileName);
                        var fileExt = fileInfo.Extension;
                        if (!supportedTypes.Contains(fileExt))
                        {
                            model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                            return View(model);
                        }
                        uploadsFolder = "uploads/asm/" + Guid.NewGuid().ToString() + fileExt; //"_" + model.ImageUpload.FileName;
                        absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                        await model.ImageUpload.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));
                    }

                    Asset asset = model.ConvertToAsset();
                    asset.ImagePath = uploadsFolder;
                    asset.AssetCategoryID = categoryId;
                    asset.ModifiedBy = HttpContext.User.Identity.Name;
                    asset.ModifiedDate = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";
                    asset.CreatedBy = HttpContext.User.Identity.Name;
                    asset.CreatedDate = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";

                    if (await _assetManagerService.CreateAssetAsync(asset))
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Asset was added successfully!";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(absoluteFilePath))
                        {
                            FileInfo file = new FileInfo(absoluteFilePath);
                            if (file.Exists)
                            {
                                file.Delete();
                            }
                        }
                        model.ViewModelErrorMessage = $"Error! An error was encountered. New asset could not be added.";
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

            var types = await _assetManagerService.GetAssetTypesAsync();
            var locations = await _globalSettingsService.GetAllLocationsAsync();

            ViewBag.TypesList = new SelectList(types, "ID", "Name");
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditAsset(string id)
        {
            Asset asset = new Asset();
            AssetViewModel model = new AssetViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                asset = await _assetManagerService.GetAssetByIdAsync(id);
                model.AssetCategoryID = asset.AssetCategoryID;
                model.AssetCategoryName = asset.AssetCategoryName;
                model.AssetDescription = asset.AssetDescription;
                model.AssetID = asset.AssetID;
                model.AssetName = asset.AssetName;
                model.AssetNumber = asset.AssetNumber;
                model.AssetTypeID = asset.AssetTypeID;
                model.AssetTypeName = asset.AssetTypeName;
                model.ParentAssetID = asset.ParentAssetID;
                model.BaseLocationID = asset.BaseLocationID;
                model.BaseLocationName = asset.BaseLocationName;
                model.ConditionDescription = asset.ConditionDescription;
                model.ConditionStatus = asset.ConditionStatus;
                model.CreatedBy = asset.CreatedBy;
                model.CreatedDate = asset.CreatedDate;
                model.CurrentLocation = asset.CurrentLocation;
                model.ImagePath = asset.ImagePath;
                model.ModifiedBy = asset.ModifiedBy;
                model.ModifiedDate = asset.ModifiedDate;
                model.OldImagePath = asset.ImagePath;
                model.ParentAssetName = asset.ParentAssetName;
                model.PurchaseAmount = asset.PurchaseAmount;
                model.PurchaseDate = asset.PurchaseDate;
                model.UsageStatus = asset.UsageStatus;

            }
            else
            {
                return RedirectToAction("AddAsset");
            }

            var types = await _assetManagerService.GetAssetTypesAsync();
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            var assets = await _assetManagerService.GetAssetsAsync();

            ViewBag.TypesList = new SelectList(types, "ID", "Name");
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsset(AssetViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string uploadsFolder = null;
                    string absoluteFilePath = null;
                    AssetType assetType = await _assetManagerService.GetAssetTypeByIdAsync(model.AssetTypeID);
                    int categoryId = assetType.CategoryID;
                    bool ImageChanged = false;

                    if (model.ImageUpload != null && model.ImageUpload.Length > 0)
                    {
                        var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                        FileInfo fileInfo = new FileInfo(model.ImageUpload.FileName);
                        var fileExt = fileInfo.Extension;
                        if (!supportedTypes.Contains(fileExt))
                        {
                            model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                            return View(model);
                        }
                        uploadsFolder = "uploads/asm/" + Guid.NewGuid().ToString() + fileExt; //model.ImageUpload.FileName;
                        absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                        await model.ImageUpload.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));
                        ImageChanged = true;

                    }

                    Asset asset = model.ConvertToAsset();
                    asset.ImagePath = uploadsFolder;
                    asset.AssetCategoryID = categoryId;
                    asset.ModifiedBy = HttpContext.User.Identity.Name;
                    asset.ModifiedDate = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";

                    if (await _assetManagerService.UpdateAssetAsync(asset))
                    {
                        if (ImageChanged == true && !string.IsNullOrEmpty(model.OldImagePath))
                        {
                            string oldAbsoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, model.OldImagePath);
                            FileInfo file = new FileInfo(oldAbsoluteFilePath);
                            if (file.Exists)
                            {
                                file.Delete();
                            }
                        }
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Asset was updated successfully!";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(absoluteFilePath))
                        {
                            FileInfo file = new FileInfo(absoluteFilePath);
                            if (file.Exists)
                            {
                                file.Delete();
                            }
                        }
                        model.ViewModelErrorMessage = $"Error! An error was encountered. New asset was not added.";
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

            var types = await _assetManagerService.GetAssetTypesAsync();
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            var assets = await _assetManagerService.GetAssetsAsync();

            ViewBag.TypesList = new SelectList(types, "ID", "Name");
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            ViewBag.AssetsList = new SelectList(assets, "AssetID", "AssetName");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            AssetViewModel model = new AssetViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                Asset asset = await _assetManagerService.GetAssetByIdAsync(id);
                model.AssetCategoryID = asset.AssetCategoryID;
                model.AssetCategoryName = asset.AssetCategoryName;
                model.AssetDescription = asset.AssetDescription;
                model.AssetID = asset.AssetID;
                model.AssetName = asset.AssetName;
                model.AssetNumber = asset.AssetNumber;
                model.AssetTypeID = asset.AssetTypeID;
                model.AssetTypeName = asset.AssetTypeName;
                model.ParentAssetID = asset.ParentAssetID;
                model.BaseLocationID = asset.BaseLocationID;
                model.BaseLocationName = asset.BaseLocationName;
                model.ConditionDescription = asset.ConditionDescription;
                model.ConditionStatus = asset.ConditionStatus;
                model.CreatedBy = asset.CreatedBy;
                model.CreatedDate = asset.CreatedDate;
                model.CurrentLocation = asset.CurrentLocation;
                model.ImagePath = asset.ImagePath;
                model.ModifiedBy = asset.ModifiedBy;
                model.ModifiedDate = asset.ModifiedDate;
                model.OldImagePath = asset.ImagePath;
                model.ParentAssetName = asset.ParentAssetName;
                model.PurchaseAmount = asset.PurchaseAmount;
                model.PurchaseDate = asset.PurchaseDate;
                model.UsageStatus = asset.UsageStatus;
            }
            else
            {
                return RedirectToAction("List", "Master");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ShowImage(string id)
        {
            Asset asset = new Asset();
            AssetViewModel model = new AssetViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                asset = await _assetManagerService.GetAssetByIdAsync(id);
                model.AssetCategoryID = asset.AssetCategoryID;
                model.AssetCategoryName = asset.AssetCategoryName;
                model.AssetDescription = asset.AssetDescription;
                model.AssetID = asset.AssetID;
                model.AssetName = asset.AssetName;
                model.AssetNumber = asset.AssetNumber;
                model.AssetTypeID = asset.AssetTypeID;
                model.AssetTypeName = asset.AssetTypeName;
                model.ParentAssetID = asset.ParentAssetID;
                model.BaseLocationID = asset.BaseLocationID;
                model.BaseLocationName = asset.BaseLocationName;
                model.ConditionStatus = asset.ConditionStatus;
                model.ConditionDescription = asset.ConditionDescription;
                model.CreatedBy = asset.CreatedBy;
                model.CreatedDate = asset.CreatedDate;
                model.CurrentLocation = asset.CurrentLocation;
                model.ImagePath = asset.ImagePath;
                model.ModifiedBy = asset.ModifiedBy;
                model.ModifiedDate = asset.ModifiedDate;
                model.OldImagePath = asset.ImagePath;
                model.ParentAssetName = asset.ParentAssetName;
                model.PurchaseAmount = asset.PurchaseAmount;
                model.PurchaseDate = asset.PurchaseDate;
                model.UsageStatus = asset.UsageStatus;

            }
            else
            {
                return RedirectToAction("Assets", "Settings");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAsset(string id)
        {
            AssetViewModel model = new AssetViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                Asset asset = await _assetManagerService.GetAssetByIdAsync(id);
                model.AssetCategoryID = asset.AssetCategoryID;
                model.AssetCategoryName = asset.AssetCategoryName;
                model.AssetDescription = asset.AssetDescription;
                model.AssetID = asset.AssetID;
                model.AssetName = asset.AssetName;
                model.AssetNumber = asset.AssetNumber;
                model.AssetTypeID = asset.AssetTypeID;
                model.AssetTypeName = asset.AssetTypeName;
                model.ParentAssetID = asset.ParentAssetID;
                model.BaseLocationID = asset.BaseLocationID;
                model.BaseLocationName = asset.BaseLocationName;
                model.ConditionDescription = asset.ConditionDescription;
                model.ConditionStatus = asset.ConditionStatus;
                model.CreatedBy = asset.CreatedBy;
                model.CreatedDate = asset.CreatedDate;
                model.CurrentLocation = asset.CurrentLocation;
                model.ImagePath = asset.ImagePath;
                model.ModifiedBy = asset.ModifiedBy;
                model.ModifiedDate = asset.ModifiedDate;
                model.OldImagePath = asset.ImagePath;
                model.ParentAssetName = asset.ParentAssetName;
                model.PurchaseAmount = asset.PurchaseAmount;
                model.PurchaseDate = asset.PurchaseDate;
                model.UsageStatus = asset.UsageStatus;
            }
            else
            {
                return RedirectToAction("Assets", "Settings");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsset(AssetViewModel model)
        {
            if (model != null)
            {
                try
                {
                    string deletedBy = HttpContext.User.Identity.Name;
                    bool succeeded = await _assetManagerService.DeleteAssetAsync(model.AssetID, deletedBy);
                    if (succeeded)
                    {
                        return RedirectToAction("Assets", "Settings");
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