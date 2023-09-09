using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.AssetManager.Models;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntranetPortal.Areas.AssetManager.Controllers
{
    [Area("AssetManager")]
    [Authorize]
    public class UsageController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IAssetManagerService _assetManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        public UsageController(IConfiguration configuration, ISecurityService securityService,
                                IBaseModelService baseModelService, IAssetManagerService assetManagerService,
                                IGlobalSettingsService globalSettingsService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _assetManagerService = assetManagerService;
            _globalSettingsService = globalSettingsService;
        }

        [Authorize(Roles = "AMSVWATXN, AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> Index(string id, int? yr = null, int? mn = null)
        {
            AssetUsageListViewModel model = new AssetUsageListViewModel();
            IList<AssetUsage> assetUsageList = new List<AssetUsage>();
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("CheckOutList");
            }
            else { model.AssetID = id; }
            try
            {
                if (yr > 0)
                {
                    model.yr = yr.Value;
                    if (mn > 0)
                    {
                        model.mn = mn.Value;
                        assetUsageList = await _assetManagerService.GetAssetUsagesByAssetIdAndDateAsync(id, yr.Value, mn.Value);
                        model.AssetUsageList = assetUsageList.ToList();
                    }
                    else
                    {
                        assetUsageList = await _assetManagerService.GetAssetUsagesByAssetIdAndDateAsync(id, yr.Value);
                        model.AssetUsageList = assetUsageList.ToList();
                    }
                }
                else
                {
                    model.yr = DateTime.Now.Year;
                    assetUsageList = await _assetManagerService.GetAssetUsagesByAssetIdAndDateAsync(id,model.yr.Value);
                    model.AssetUsageList = assetUsageList.ToList();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View(model);
        }

        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> CheckOutList(int? tp = null, string sp = null, int? pg = null)
        {
            IList<AssetUsage> assetUsageList = new List<AssetUsage>();
            try
            {
                if (tp > 0)
                {
                    ViewBag.TypeID = tp;
                    assetUsageList = await _assetManagerService.GetCurrentAssetUsagesByAssetTypeIdAsync(tp.Value);
                }
                else if (!string.IsNullOrWhiteSpace(sp))
                {
                    ViewBag.SearchParameter = sp;
                    assetUsageList = await _assetManagerService.SearchCurrentAssetUsagesByAssetNameAsync(sp);
                }
                else
                {
                    assetUsageList = await _assetManagerService.GetCurrentAssetUsagesAsync();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            var assetTypes = await _assetManagerService.GetAssetTypesAsync();
            if (tp != null && tp > 0) { ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name", tp.Value); }
            else { ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name"); }

            return View(PaginatedList<AssetUsage>.CreateAsync(assetUsageList.AsQueryable(), pg ?? 1, 100));
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> CheckOut(string id)
        {
            AssetUsageViewModel model = new AssetUsageViewModel();
            try
            {
                Asset asset = await _assetManagerService.GetAssetByIdAsync(id);
                model.AssetDescription = asset.AssetDescription;
                model.AssetID = asset.AssetID;
                model.AssetName = asset.AssetName;
                model.AssetTypeID = asset.AssetTypeID;
                model.AssetTypeName = asset.AssetTypeName;
                model.CheckedOutFromLocation = asset.CurrentLocation == null || asset.CurrentLocation == "" ? asset.BaseLocationName : asset.CurrentLocation;
                model.CheckOutCondition = asset.ConditionStatus;
                model.UsageStartTime = DateTime.Now;
                model.UsageEndTime = DateTime.Now;
            }
            catch(Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> CheckOut(AssetUsageViewModel model)
        {
            if (ModelState.IsValid)
            {
                Asset asset = new Asset();
                try
                {
                    if (model.AssetTypeID < 1 || string.IsNullOrEmpty(model.AssetID))
                    {
                        var assets = await _assetManagerService.SearchAssetsByNameAsync(model.AssetName);
                        asset = assets.ToList().FirstOrDefault();
                    }
                    else
                    {
                        asset.AssetID = model.AssetID;
                        asset.AssetTypeID = model.AssetTypeID.Value;
                    }

                    AssetUsage assetUsage = model.ConvertToAssetUsage();
                    assetUsage.ModifiedBy = HttpContext.User.Identity.Name;
                    assetUsage.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                    assetUsage.AssetID = asset.AssetID;
                    assetUsage.AssetTypeID = asset.AssetTypeID;
                    assetUsage.CheckedOutTime = DateTime.Now;
                    assetUsage.CheckedOutBy = HttpContext.User.Identity.Name;
                    assetUsage.CheckStatus = "Checked Out";

                    if (await _assetManagerService.CheckOutEquipmentAsync(assetUsage))
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Equipment Checked Out successfully!";
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Error! An error was encountered. Equipment Checked Out failed.";
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
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> EditCheckOut(int id)
        {
            AssetUsageViewModel model = new AssetUsageViewModel();
            try
            {
                AssetUsage assetUsage = await _assetManagerService.GetAssetUsageByIdAsync(id);
                model.AssetDescription = assetUsage.AssetDescription;
                model.AssetID = assetUsage.AssetID;
                model.AssetName = assetUsage.AssetName;
                model.AssetTypeID = assetUsage.AssetTypeID;
                model.AssetTypeName = assetUsage.AssetTypeName;
                model.CheckedOutFromLocation = assetUsage.CheckedOutFromLocation;
                model.CheckOutCondition = assetUsage.CheckOutCondition;
                model.UsageStartTime = assetUsage.UsageStartTime;
                model.UsageEndTime = assetUsage.UsageEndTime;
                model.Purpose = assetUsage.Purpose;
                model.UsageLocation = assetUsage.UsageLocation;
                model.UsageDescription = assetUsage.UsageDescription;
                model.CheckedOutTo = assetUsage.CheckedOutTo;
                model.CheckedOutComment = assetUsage.CheckedOutComment;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSVWATXN, AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> CheckOutDetails(int id)
        {
            AssetUsageViewModel model = new AssetUsageViewModel();
            try
            {
                AssetUsage assetUsage = await _assetManagerService.GetAssetUsageByIdAsync(id);
                model.AssetDescription = assetUsage.AssetDescription;
                model.AssetID = assetUsage.AssetID;
                model.AssetName = assetUsage.AssetName;
                model.AssetTypeID = assetUsage.AssetTypeID;
                model.AssetTypeName = assetUsage.AssetTypeName;
                model.CheckedOutFromLocation = assetUsage.CheckedOutFromLocation; 
                model.CheckOutCondition = assetUsage.CheckOutCondition;
                model.UsageStartTime = assetUsage.UsageStartTime;
                model.UsageEndTime = assetUsage.UsageEndTime;
                model.Purpose = assetUsage.Purpose;
                model.UsageLocation = assetUsage.UsageLocation;
                model.UsageDescription = assetUsage.UsageDescription;
                model.CheckedOutTo = assetUsage.CheckedOutTo;
                model.CheckedOutComment = assetUsage.CheckedOutComment;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> DeleteCheckOut(int id)
        {
            AssetUsageViewModel model = new AssetUsageViewModel();
            try
            {
                AssetUsage assetUsage = await _assetManagerService.GetAssetUsageByIdAsync(id);
                model.UsageID = assetUsage.UsageID;
                model.AssetDescription = assetUsage.AssetDescription;
                model.AssetID = assetUsage.AssetID;
                model.AssetName = assetUsage.AssetName;
                model.AssetTypeID = assetUsage.AssetTypeID;
                model.AssetTypeName = assetUsage.AssetTypeName;
                model.CheckedOutFromLocation = assetUsage.CheckedOutFromLocation;
                model.CheckOutCondition = assetUsage.CheckOutCondition;
                model.UsageStartTime = assetUsage.UsageStartTime;
                model.UsageEndTime = assetUsage.UsageEndTime;
                model.Purpose = assetUsage.Purpose;
                model.UsageLocation = assetUsage.UsageLocation;
                model.UsageDescription = assetUsage.UsageDescription;
                model.CheckedOutTo = assetUsage.CheckedOutTo;
                model.CheckedOutComment = assetUsage.CheckedOutComment;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> DeleteCheckOut(AssetUsageViewModel model)
        {
                AssetUsage assetUsage = new AssetUsage();
                bool IsSuccessful = false;
                try
                {
                    if (model.UsageID > 0 )
                    {
                        IsSuccessful = await _assetManagerService.DeleteAssetUsageAsync(model.UsageID.Value);
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Ooops! It appears some key form parameters are missing or have invalid values. Please reload the form and try again.";
                        model.OperationIsCompleted = true;
                    }

                    if (IsSuccessful)
                    {
                        return RedirectToAction("CheckOutList");
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Sorry an error was encountered. Equipment Checked Out could not be deleted."; 
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                    model.OperationIsCompleted = true;
                }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> CheckIn(int id)
        {
            AssetCheckInViewModel model = new AssetCheckInViewModel();
            try
            {
                AssetUsage assetUsage = await _assetManagerService.GetAssetUsageByIdAsync(id);
                model.AssetDescription = assetUsage.AssetDescription;
                model.AssetID = assetUsage.AssetID;
                model.AssetName = assetUsage.AssetName;
                model.CheckedInBy = HttpContext.User.Identity.Name;
                model.CheckedInTime = DateTime.Now;
                model.UsageID = assetUsage.UsageID;
                model.CheckedInFromLocation = assetUsage.UsageLocation;
                model.CheckedInToLocation = string.Empty;
                model.CheckedInCondition = AssetCondition.InGoodCondition;
                model.CheckedInComment = string.Empty;

            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            ViewBag.LocationsList = new SelectList(await _globalSettingsService.GetAllLocationsAsync(),"LocationName","LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> CheckIn(AssetCheckInViewModel model)
        {
            if (ModelState.IsValid)
            {
                AssetUsage assetUsage = new AssetUsage();
                try
                {
                    assetUsage = model.ConvertToAssetUsage();
                    assetUsage.ModifiedBy = HttpContext.User.Identity.Name;
                    assetUsage.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                    assetUsage.CheckStatus = "Checked In";

                    if (await _assetManagerService.CheckOutEquipmentAsync(assetUsage))
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Equipment Checked Out successfully!";
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Error! An error was encountered. Equipment Checked Out failed.";
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
    }
}