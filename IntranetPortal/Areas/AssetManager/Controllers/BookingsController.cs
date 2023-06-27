using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.AssetManager.Models;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntranetPortal.Areas.AssetManager.Controllers
{
    [Area("AssetManager")]
    [Authorize]
    public class BookingsController : Controller
    {

        private readonly ILogger<BookingsController> _logger;
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IAssetManagerService _assetManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        public BookingsController(IConfiguration configuration, ISecurityService securityService,
                                IBaseModelService baseModelService, IAssetManagerService assetManagerService,
                                IGlobalSettingsService globalSettingsService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _assetManagerService = assetManagerService;
            _globalSettingsService = globalSettingsService;
        }

        [Authorize(Roles = "AMSBKGVWL, XYALLACCZ")]
        public async Task<IActionResult> Index(string id = null, int? tp = null, string sp = null)
        {
            AssetReservationListViewModel model = new AssetReservationListViewModel();
            IEnumerable<AssetReservation> assetReservationList;

            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    model.AssetID = id;
                    assetReservationList = await _assetManagerService.GetCurrentAssetReservationsByAssetIdAsync(id);
                }
                else
                {
                    if (tp > 0)
                    {
                        assetReservationList = await _assetManagerService.GetCurrentAssetReservationsByAssetTypeIdAsync(tp.Value);
                    }
                    else if (!string.IsNullOrWhiteSpace(sp))
                    {
                        model.sp = sp;
                        assetReservationList = await _assetManagerService.SearchCurrentAssetReservationsByAssetNameAsync(sp);
                    }
                    else
                    {
                        assetReservationList = await _assetManagerService.GetCurrentAssetReservationsAsync();
                    }
                }

                model.AssetReservationList = assetReservationList.ToList();

                var assetTypes = await _assetManagerService.GetAssetTypesAsync();
                if (tp != null && tp > 0) { ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name", tp.Value); }
                else { ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name"); }

                return View(model);
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
                model.AssetReservationList = null;

                var assetTypes = await _assetManagerService.GetAssetTypesAsync();
                if (tp != null && tp > 0) { ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name", tp.Value); }
                else { ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name"); }

                return View(model);
            }
        }

        [Authorize(Roles = "AMSBKGVWL, XYALLACCZ")]
        public async Task<IActionResult> List(string id = null, int? yr = null, int? mn = null)
        {
            AssetReservationListViewModel model = new AssetReservationListViewModel();
            IEnumerable<AssetReservation> assetReservationList;

            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    model.AssetID = id;
                    if(yr != null && yr.Value > 0)
                    {
                        model.yr = yr.Value;
                        if(mn != null && mn.Value > 0)
                        {
                            model.mn = mn.Value;
                            assetReservationList = await _assetManagerService.GetAssetReservationsByAssetIdAndYearAndMonthAsync(model.AssetID, model.yr.Value, model.mn.Value);
                        }
                        else
                        {
                            assetReservationList = await _assetManagerService.GetAssetReservationsByAssetIdAndYearAsync(model.AssetID, model.yr.Value);
                        }
                    }
                    else
                    {
                        model.yr = DateTime.Now.Year;
                        assetReservationList = await _assetManagerService.GetAssetReservationsByAssetIdAndYearAsync(model.AssetID, model.yr.Value);
                    }
                    assetReservationList = await _assetManagerService.GetCurrentAssetReservationsByAssetIdAsync(id);
                }
                else
                {
                    return RedirectToAction("Index");
                }

                model.AssetReservationList = assetReservationList.ToList();
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
                model.AssetReservationList = null;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSBKGADN, XYALLACCZ")]
        public async Task<IActionResult> Add(string id)
        {
            AssetReservationViewModel model = new AssetReservationViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                Asset asset = await _assetManagerService.GetAssetByIdAsync(id);
                model.AssetID = id;
                model.AssetTypeID = asset.AssetTypeID;
                model.AssetName = asset.AssetName;
                model.AssetTypeName = asset.AssetTypeName;
            }
            model.ReservedBy = HttpContext.User.Identity.Name;
            model.ReservedOn = DateTime.UtcNow;
            model.ReservedOnFormatted = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT";
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSBKGADN, XYALLACCZ")]
        public async Task<IActionResult> Add(AssetReservationViewModel model)
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

                    AssetReservation assetReservation = model.ConvertToAssetReservation();
                    assetReservation.LastModifiedBy = HttpContext.User.Identity.Name;
                    assetReservation.LastModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";
                    assetReservation.AssetID = asset.AssetID;
                    assetReservation.AssetTypeID = asset.AssetTypeID;
                    assetReservation.ReservedOn = DateTime.UtcNow;

                    if (await _assetManagerService.CreateAssetReservationAsync(assetReservation))
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Booking was added successfully!";
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Error! An error was encountered. New Booking was not added.";
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
        [Authorize(Roles = "AMSBKGEDT, XYALLACCZ")]
        public async Task<IActionResult> Edit(int id)
        {
            AssetReservationViewModel model = new AssetReservationViewModel();
            AssetReservation assetReservation = new AssetReservation();
            if (id > 0)
            {
                assetReservation = await _assetManagerService.GetAssetReservationByIdAsync(id);
                model.AssetDescription = assetReservation.AssetDescription;
                model.AssetID = assetReservation.AssetID;
                model.AssetName = assetReservation.AssetName;
                model.AssetReservationID = assetReservation.AssetReservationID;
                model.AssetTypeID = assetReservation.AssetTypeID;
                model.AssetTypeName = assetReservation.AssetTypeName;
                model.EventDescription = assetReservation.EventDescription;
                model.EventEndTime = assetReservation.EventEndTime;
                model.EventLocation = assetReservation.EventLocation;
                model.EventStartTime = assetReservation.EventStartTime;
                model.ReservationStatus = assetReservation.ReservationStatus;
                model.ReservedBy = assetReservation.ReservedBy;
                model.ReservedOn = assetReservation.ReservedOn;
                model.ReservedOnFormatted = $"{assetReservation.ReservedOn.Value.ToLongDateString() } {assetReservation.ReservedOn.Value.ToLongTimeString()} GMT";
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSBKGEDT, XYALLACCZ")]
        public async Task<IActionResult> Edit(AssetReservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetReservation assetReservation = model.ConvertToAssetReservation();
                    assetReservation.LastModifiedBy = HttpContext.User.Identity.Name;
                    assetReservation.LastModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";

                    if (await _assetManagerService.UpdateAssetReservationAsync(assetReservation))
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Booking was updated successfully!";
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Error! An error was encountered. Booking was not updated.";
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
        [Authorize(Roles = "AMSBKGVWD, XYALLACCZ")]
        public async Task<IActionResult> Details(int id)
        {
            AssetReservationViewModel model = new AssetReservationViewModel();
            AssetReservation assetReservation = new AssetReservation();
            if (id > 0)
            {
                assetReservation = await _assetManagerService.GetAssetReservationByIdAsync(id);
                model.AssetDescription = assetReservation.AssetDescription;
                model.AssetID = assetReservation.AssetID;
                model.AssetName = assetReservation.AssetName;
                model.AssetReservationID = assetReservation.AssetReservationID;
                model.AssetTypeID = assetReservation.AssetTypeID;
                model.AssetTypeName = assetReservation.AssetTypeName;
                model.EventDescription = assetReservation.EventDescription;
                model.EventEndTime = assetReservation.EventEndTime;
                model.EventLocation = assetReservation.EventLocation;
                model.EventStartTime = assetReservation.EventStartTime;
                model.ReservationStatus = assetReservation.ReservationStatus;
                model.ReservedBy = assetReservation.ReservedBy;
                model.ReservedOn = assetReservation.ReservedOn;
                model.ReservedOnFormatted = $"{assetReservation.ReservedOn.Value.ToLongDateString() } {assetReservation.ReservedOn.Value.ToLongTimeString()} GMT";
                model.EventStartTimeFormatted = $"{assetReservation.EventStartTime.Value.ToLongDateString() } {assetReservation.EventStartTime.Value.ToLongTimeString()} GMT";
                model.EventEndTimeFormatted = $"{assetReservation.EventEndTime.Value.ToLongDateString() } {assetReservation.EventEndTime.Value.ToLongTimeString()} GMT";

            }
            else
            {

            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSBKGDLT, XYALLACCZ")]
        public async Task<IActionResult> Delete(int id)
        {
            AssetReservationViewModel model = new AssetReservationViewModel();
            AssetReservation assetReservation = new AssetReservation();
            if (id > 0)
            {
                assetReservation = await _assetManagerService.GetAssetReservationByIdAsync(id);
                model.AssetDescription = assetReservation.AssetDescription;
                model.AssetID = assetReservation.AssetID;
                model.AssetName = assetReservation.AssetName;
                model.AssetReservationID = assetReservation.AssetReservationID;
                model.AssetTypeID = assetReservation.AssetTypeID;
                model.AssetTypeName = assetReservation.AssetTypeName;
                model.EventDescription = assetReservation.EventDescription;
                model.EventEndTime = assetReservation.EventEndTime;
                model.EventLocation = assetReservation.EventLocation;
                model.EventStartTime = assetReservation.EventStartTime;
                model.ReservationStatus = assetReservation.ReservationStatus;
                model.ReservedBy = assetReservation.ReservedBy;
                model.ReservedOn = assetReservation.ReservedOn;
                model.ReservedOnFormatted = $"{assetReservation.ReservedOn.Value.ToLongDateString() } {assetReservation.ReservedOn.Value.ToLongTimeString()} GMT";
                model.EventStartTimeFormatted = $"{assetReservation.EventStartTime.Value.ToLongDateString() } {assetReservation.EventStartTime.Value.ToLongTimeString()} GMT";
                model.EventEndTimeFormatted = $"{assetReservation.EventEndTime.Value.ToLongDateString() } {assetReservation.EventEndTime.Value.ToLongTimeString()} GMT";

            }
            else
            {

            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSBKGDLT, XYALLACCZ")]
        public async Task<IActionResult> Delete(AssetReservationViewModel model)
        {
            AssetReservation assetReservation = new AssetReservation();
            if (model != null && model.AssetReservationID > 0)
            {
                if (await _assetManagerService.DeleteAssetReservationAsync(model.AssetReservationID.Value))
                {
                    return RedirectToAction("Bookings");
                }
                else
                {
                    model.ViewModelErrorMessage = $"Sorry an error was encountered. Delete operation failed!";
                }
            }
            return View(model);
        }

    }
}