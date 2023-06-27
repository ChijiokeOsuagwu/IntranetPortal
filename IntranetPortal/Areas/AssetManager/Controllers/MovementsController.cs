using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.AssetManager.Models;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
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
    public class MovementsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IAssetManagerService _assetManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        public MovementsController(IConfiguration configuration, ISecurityService securityService,
                                IBaseModelService baseModelService, IAssetManagerService assetManagerService,
                                IGlobalSettingsService globalSettingsService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _assetManagerService = assetManagerService;
            _globalSettingsService = globalSettingsService;
        }

        [Authorize(Roles = "AMSMVTVWL, XYALLACCZ")]
        public async Task<IActionResult> List(int? tp = null, int? pg = null)
        {
            IList<AssetMovement> assetMovementList = new List<AssetMovement>();
            try
            {
                if (tp > 0)
                {
                    ViewBag.TypeID = tp;
                    assetMovementList = await _assetManagerService.GetAssetMovementsByAssetTypeIdAsync(tp.Value);
                }
                else
                {
                    assetMovementList = await _assetManagerService.GetAssetMovementAsync();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            var assetTypes = await _assetManagerService.GetAssetTypesAsync();
            if (tp != null && tp > 0) { ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name", tp.Value); }
            else { ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name"); }

            return View(PaginatedList<AssetMovement>.CreateAsync(assetMovementList.AsQueryable(), pg ?? 1, 100));
        }

        [Authorize(Roles = "AMSMVTVWL, XYALLACCZ")]
        public async Task<IActionResult> Index(string id, int? yr = null, int? mn = null)
        {
            IList<AssetMovement> assetMovementList = new List<AssetMovement>();
            AssetMovementListViewModel model = new AssetMovementListViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    model.AssetID = id;
                    if (yr != null && yr.Value > 0)
                    {
                        model.yr = yr.Value;
                        if(mn != null && mn.Value > 0)
                        {
                            model.mn = mn.Value;
                            assetMovementList = await _assetManagerService.GetAssetMovementsByAssetIdAndYearAndMonthAsync(model.AssetID, model.yr.Value, model.mn.Value);
                        }
                        else
                        {
                            assetMovementList = await _assetManagerService.GetAssetMovementsByAssetIdAndYearAsync(model.AssetID, model.yr.Value);
                        }
                    }
                    else
                    {
                        if(mn != null && mn.Value > 0)
                        {
                            model.mn = mn.Value;
                            model.yr = DateTime.Now.Year;
                            assetMovementList = await _assetManagerService.GetAssetMovementsByAssetIdAndYearAndMonthAsync(model.AssetID, model.yr.Value, model.mn.Value);
                        }
                        else
                        {
                            model.yr = DateTime.Now.Year;
                            model.mn = DateTime.Now.Month;
                            assetMovementList = await _assetManagerService.GetAssetMovementsByAssetIdAndYearAndMonthAsync(model.AssetID, model.yr.Value, model.mn.Value);
                        }
                    }
                }
                else
                {
                    return RedirectToAction("List");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMVTADN, XYALLACCZ")]
        public async Task<IActionResult> AddMovement(string id = null)
        {
            AssetMovementViewModel model = new AssetMovementViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                Asset asset = await _assetManagerService.GetAssetByIdAsync(id);
                model.AssetID = id;
                model.AssetName = asset.AssetName;
                model.AssetTypeID = asset.AssetTypeID;
                model.AssetCategoryID = asset.AssetCategoryID;
            }
            model.MovedOn = DateTime.Today;
            var locations = _globalSettingsService.GetAllLocationsAsync().Result;
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMVTADN, XYALLACCZ")]
        public async Task<IActionResult> AddMovement(AssetMovementViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Asset asset = new Asset();
                try
                {
                    if (string.IsNullOrWhiteSpace(model.AssetName))
                    {
                        model.ViewModelErrorMessage = $"Error! Invalid equipment name.";
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(model.AssetID) || model.AssetTypeID < 1 || model.AssetCategoryID < 1)
                        {
                            var assets = await _assetManagerService.SearchAssetsByNameAsync(model.AssetName);
                            Asset asset = assets.ToList().FirstOrDefault();
                            model.AssetID = asset.AssetID;
                            model.AssetTypeID = asset.AssetTypeID;
                            model.AssetCategoryID = asset.AssetCategoryID;
                            model.AssetConditionStatus = asset.ConditionStatus;
                            model.AssetConditionDescription = asset.ConditionDescription;
                        }

                        if (string.IsNullOrWhiteSpace(model.AssetID) || model.AssetTypeID < 1 || model.AssetCategoryID < 1)
                        {
                            model.ViewModelErrorMessage = $"Ooops! The system could  not find any record of the equipment you entered. Please enter a valid equipment name and try again. ";
                        }
                        else
                        {
                            Location toLocation = await _globalSettingsService.GetLocationByIdAsync(model.MovedToLocationID.Value);
                            model.MovedToLocationName = toLocation.LocationName;
                            Location fromLocation = await _globalSettingsService.GetLocationByIdAsync(model.MovedFromLocationID.Value);
                            model.MovedFromLocationName = fromLocation.LocationName;

                            Asset asset = await _assetManagerService.GetAssetByIdAsync(model.AssetID);

                            AssetMovement assetMovement = model.ConvertToAssetMovement();
                            assetMovement.AssetConditionStatus = asset.ConditionStatus;
                            assetMovement.AssetConditionDescription = asset.ConditionDescription;
                            assetMovement.LoggedTime = DateTime.Now;
                            assetMovement.LoggedBy = HttpContext.User.Identity.Name;
                            assetMovement.ModifiedBy = HttpContext.User.Identity.Name;
                            assetMovement.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                            if (await _assetManagerService.AddAssetMovementAsync(assetMovement))
                            {
                                model.OperationIsCompleted = true;
                                model.OperationIsSuccessful = true;
                                model.ViewModelSuccessMessage = $"Equipment Transfer added successfully!";
                            }
                            else
                            {
                                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Adding new Equipment Transfer failed.";
                            }
                        }
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
            var locations = _globalSettingsService.GetAllLocationsAsync().Result;
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMVTEDT, XYALLACCZ")]
        public async Task<IActionResult> EditMovement(int id)
        {
            AssetMovementViewModel model = new AssetMovementViewModel();
            try
            {
                AssetMovement assetMovement = await _assetManagerService.GetAssetMovementByIdAsync(id);
                model.AssetDescription = assetMovement.AssetDescription;
                model.AssetID = assetMovement.AssetID;
                model.AssetName = assetMovement.AssetName;
                model.AssetTypeID = assetMovement.AssetTypeID;
                model.AssetTypeName = assetMovement.AssetTypeName;
                model.AssetMovementID = assetMovement.AssetMovementID;
                model.AssetCategoryID = assetMovement.AssetCategoryID;
                model.MovedFromLocationID = assetMovement.MovedFromLocationID;
                model.MovedFromLocationName = assetMovement.MovedFromLocationName;
                model.MovedToLocationID = assetMovement.MovedToLocationID;
                model.MovedToLocationName = assetMovement.MovedToLocationName;
                model.AssetConditionStatus = assetMovement.AssetConditionStatus;
                model.AssetConditionDescription = assetMovement.AssetConditionDescription;
                model.MovementPurpose = assetMovement.MovementPurpose;
                model.AssetCategoryName = assetMovement.AssetCategoryName;
                model.Comments = assetMovement.Comments;
                model.ApprovedBy = assetMovement.ApprovedBy;
                model.SupervisedBy = assetMovement.SupervisedBy;
                model.MovedOn = assetMovement.MovedOn;
                model.LoggedBy = assetMovement.LoggedBy;
                model.LoggedTime = assetMovement.LoggedTime;
                model.ModifiedBy = assetMovement.ModifiedBy;
                model.ModifiedTime = assetMovement.ModifiedTime;

                Location toLocation = await _globalSettingsService.GetLocationByNameAsync(model.MovedToLocationName);
                model.MovedToLocationID = toLocation.LocationID;
                Location fromLocation = await _globalSettingsService.GetLocationByNameAsync(model.MovedFromLocationName);
                model.MovedFromLocationID = fromLocation.LocationID;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            var locations = _globalSettingsService.GetAllLocationsAsync().Result;
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMVTEDT, XYALLACCZ")]
        public async Task<IActionResult> EditMovement(AssetMovementViewModel model)
        {
            if (ModelState.IsValid)
            {
                Asset asset = new Asset();
                try
                {
                    if (string.IsNullOrWhiteSpace(model.AssetName))
                    {
                        model.ViewModelErrorMessage = $"Error! Invalid equipment name.";
                        return View(model);
                    }
                    else
                    {
                        if (model.AssetMovementID < 1)
                        {
                            model.ViewModelErrorMessage = $"Error! Sorry, movement record was not found.";
                            return View(model);
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(model.AssetID) || model.AssetTypeID < 1 || model.AssetCategoryID < 1)
                            {
                                var assets = await _assetManagerService.SearchAssetsByNameAsync(model.AssetName);
                                asset = assets.ToList().FirstOrDefault();
                                model.AssetID = asset.AssetID;
                                model.AssetTypeID = asset.AssetTypeID;
                                model.AssetCategoryID = asset.AssetCategoryID;
                                model.AssetConditionDescription = asset.ConditionDescription;
                                model.AssetConditionStatus = asset.ConditionStatus;
                            }

                            if (string.IsNullOrWhiteSpace(model.AssetID) || model.AssetTypeID < 1 || model.AssetCategoryID < 1)
                            {
                                model.ViewModelErrorMessage = $"Ooops! The system could  not find any record of the equipment you entered. Please enter a valid equipment name and try again. ";
                            }
                            else
                            {
                                AssetMovement assetMovement = model.ConvertToAssetMovement();
                                assetMovement.ModifiedBy = HttpContext.User.Identity.Name;
                                assetMovement.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                                if (await _assetManagerService.UpdateAssetMovementAsync(assetMovement))
                                {
                                    model.OperationIsCompleted = true;
                                    model.OperationIsSuccessful = true;
                                    model.ViewModelSuccessMessage = $"Equipment Transfer updated successfully!";
                                }
                                else
                                {
                                    model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Updating Equipment Transfer failed.";
                                }
                            }
                        }
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
            var locations = _globalSettingsService.GetAllLocationsAsync().Result;
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMVTVWD, XYALLACCZ")]
        public async Task<IActionResult> MovementDetails(int id)
        {
            AssetMovementViewModel model = new AssetMovementViewModel();
            try
            {
                AssetMovement assetMovement = await _assetManagerService.GetAssetMovementByIdAsync(id);
                model.AssetDescription = assetMovement.AssetDescription;
                model.AssetID = assetMovement.AssetID;
                model.AssetName = assetMovement.AssetName;
                model.AssetTypeID = assetMovement.AssetTypeID;
                model.AssetTypeName = assetMovement.AssetTypeName;
                model.AssetMovementID = assetMovement.AssetMovementID;
                model.AssetCategoryID = assetMovement.AssetCategoryID;
                model.MovedFromLocationID = assetMovement.MovedFromLocationID;
                model.MovedFromLocationName = assetMovement.MovedFromLocationName;
                model.MovedToLocationID = assetMovement.MovedToLocationID;
                model.MovedToLocationName = assetMovement.MovedToLocationName;
                model.AssetConditionStatus = assetMovement.AssetConditionStatus;
                model.AssetConditionDescription = assetMovement.AssetConditionDescription;
                model.MovementPurpose = assetMovement.MovementPurpose;
                model.AssetCategoryName = assetMovement.AssetCategoryName;
                model.AssetConditionDescription = assetMovement.AssetConditionDescription;
                model.Comments = assetMovement.Comments;
                model.ApprovedBy = assetMovement.ApprovedBy;
                model.SupervisedBy = assetMovement.SupervisedBy;
                model.MovedOn = assetMovement.MovedOn;
                model.LoggedBy = assetMovement.LoggedBy;
                model.LoggedTime = assetMovement.LoggedTime;
                model.ModifiedBy = assetMovement.ModifiedBy;
                model.ModifiedTime = assetMovement.ModifiedTime;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMVTDLT, XYALLACCZ")]
        public async Task<IActionResult> DeleteMovement(int id)
        {
            AssetMovementViewModel model = new AssetMovementViewModel();
            try
            {
                AssetMovement assetMovement = await _assetManagerService.GetAssetMovementByIdAsync(id);
                model.AssetDescription = assetMovement.AssetDescription;
                model.AssetID = assetMovement.AssetID;
                model.AssetName = assetMovement.AssetName;
                model.AssetTypeID = assetMovement.AssetTypeID;
                model.AssetTypeName = assetMovement.AssetTypeName;
                model.AssetMovementID = assetMovement.AssetMovementID;
                model.AssetCategoryID = assetMovement.AssetCategoryID;
                model.MovedFromLocationID = assetMovement.MovedFromLocationID;
                model.MovedFromLocationName = assetMovement.MovedFromLocationName;
                model.MovedToLocationID = assetMovement.MovedToLocationID;
                model.MovedToLocationName = assetMovement.MovedToLocationName;
                model.AssetConditionStatus = assetMovement.AssetConditionStatus;
                model.AssetConditionDescription = assetMovement.AssetConditionDescription;
                model.MovementPurpose = assetMovement.MovementPurpose;
                model.AssetCategoryName = assetMovement.AssetCategoryName;
                model.AssetConditionDescription = assetMovement.AssetConditionDescription;
                model.Comments = assetMovement.Comments;
                model.ApprovedBy = assetMovement.ApprovedBy;
                model.SupervisedBy = assetMovement.SupervisedBy;
                model.MovedOn = assetMovement.MovedOn;
                model.LoggedBy = assetMovement.LoggedBy;
                model.LoggedTime = assetMovement.LoggedTime;
                model.ModifiedBy = assetMovement.ModifiedBy;
                model.ModifiedTime = assetMovement.ModifiedTime;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMVTDLT, XYALLACCZ")]
        public async Task<IActionResult> DeleteMovement(AssetMovementViewModel model)
        {
            AssetMovement assetMovement = new AssetMovement();
            bool IsSuccessful = false;
            try
            {
                if (model.AssetMovementID > 0)
                {
                    IsSuccessful = await _assetManagerService.DeleteAssetMovementAsync(model.AssetMovementID.Value);
                }
                else
                {
                    model.ViewModelErrorMessage = $"Ooops! It appears some key form parameters are missing or have invalid values. Please reload the form and try again.";
                    model.OperationIsCompleted = true;
                }

                if (IsSuccessful)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    model.ViewModelErrorMessage = $"Sorry an error was encountered. Equipment Transfer could not be deleted.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
                model.OperationIsCompleted = true;
            }
            return View(model);
        }
    }
}