using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using IntranetPortal.Areas.AssetManager.Models;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IntranetPortal.Models;

namespace IntranetPortal.Areas.AssetManager.Controllers
{
    [Area("AssetManager")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IAssetManagerService _assetManagerService;
        private readonly IBaseModelService _baseModelService;
        public HomeController(IConfiguration configuration, ISecurityService securityService,
                                IBaseModelService baseModelService, IAssetManagerService assetManagerService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _assetManagerService = assetManagerService;
        }

        public IActionResult Index()
        {
            return View();
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
        public async Task<IActionResult> AssetDetails(string id)
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

        public async Task<IActionResult> AssetList(int? sp, int? pg = null)
        {
            IEnumerable<Asset> assetList = new List<Asset>();
            try
            {
                if (sp > 0)
                {
                    assetList = await _assetManagerService.GetAssetsByAssetTypeIdAsync(sp.Value);
                }
                else
                {
                    assetList = await _assetManagerService.GetAssetsAsync();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                assetList = null;
            }
            ViewData["CurrentFilter"] = ViewBag.sp = sp;
            ViewBag.pg = pg;
            var assetTypes = await _assetManagerService.GetAssetTypesAsync();
            ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name");

            return View(PaginatedList<Asset>.CreateAsync(assetList.AsQueryable(), pg ?? 1, 100));
        }

        public async Task<IActionResult> Bookings(string id = null, int? tp = null, string sp = null)
        {
            AssetReservationListViewModel model = new AssetReservationListViewModel();
            IEnumerable<AssetReservation> assetReservationList;

            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
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

        public async Task<IActionResult> BookingList(string id = null)
        {
            AssetReservationListViewModel model = new AssetReservationListViewModel();
            IEnumerable<AssetReservation> assetReservationList;

            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    ViewBag.AssetID = id;
                    assetReservationList = await _assetManagerService.GetCurrentAssetReservationsByAssetIdAsync(id);
                }
                else
                {
                    return RedirectToAction("Bookings");
                }

                model.AssetReservationList = assetReservationList.ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
                model.AssetReservationList = null;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult AddBooking()
        {
            AssetReservationViewModel model = new AssetReservationViewModel();
            model.ReservedBy = HttpContext.User.Identity.Name;
            model.ReservedOn = DateTime.UtcNow;
            model.ReservedOnFormatted = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddBooking(AssetReservationViewModel model)
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
        public async Task<IActionResult> EditBooking(int id)
        {
            AssetReservationViewModel model = new AssetReservationViewModel();
            AssetReservation assetReservation = new AssetReservation();
            if(id > 0)
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
        public async Task<IActionResult> EditBooking(AssetReservationViewModel model)
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
        public async Task<IActionResult> BookingDetails(int id)
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
        public async Task<IActionResult> DeleteBooking(int id)
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
        public async Task<IActionResult> DeleteBooking(AssetReservationViewModel model)
        {
            AssetReservation assetReservation = new AssetReservation();
            if (model != null && model.AssetReservationID > 0)
            {
                if( await _assetManagerService.DeleteAssetReservationAsync(model.AssetReservationID.Value))
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

        //=============================== Helper Methods ====================================================================//
        #region Helper Methods
        [HttpGet]
        public JsonResult GetAssetNames(string text)
        {
            List<string> assets = _assetManagerService.SearchAssetsByNameAsync(text).Result.Select(x => x.AssetName).ToList();
            return Json(assets);
        }

        [HttpGet]
        public JsonResult GetAssetParameters(string asn)
        {
            Asset asset = new Asset();

            List<Asset> assets = _assetManagerService.SearchAssetsByNameAsync(asn).Result.ToList();

            if (assets.Count == 1)
            {
                asset = assets.FirstOrDefault();
            }
            else
            {
                asset = new Asset
                {
                    AssetID = string.Empty,
                    AssetTypeID = -1
                };
            }
            string model = JsonConvert.SerializeObject(new { asset_id = asset.AssetID, asset_type_id = asset.AssetTypeID, asset_category_id = asset.AssetCategoryID }, Formatting.Indented);
            return Json(model);
        }
        #endregion
    }
}