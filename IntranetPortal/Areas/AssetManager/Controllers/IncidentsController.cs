using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IntranetPortal.Areas.AssetManager.Models;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntranetPortal.Areas.AssetManager.Controllers
{
    [Area("AssetManager")]
    [Authorize]
    public class IncidentsController : Controller
    {
        private readonly ILogger<IncidentsController> _logger;
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IAssetManagerService _assetManagerService;
        private readonly IBaseModelService _baseModelService;
        public IncidentsController(IConfiguration configuration, ISecurityService securityService,
                        IBaseModelService baseModelService, IAssetManagerService assetManagerService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _assetManagerService = assetManagerService;
        }

        [Authorize(Roles = "AMSVWATXN, AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> List(int? tp = null, int? pg = null)
        {
            IList<AssetIncident> assetIncidentList = new List<AssetIncident>();
            try
            {
                if (tp > 0)
                {
                    ViewBag.TypeID = tp;
                    assetIncidentList = await _assetManagerService.GetCurrentAssetIncidentsByAssetTypeIdAsync(tp.Value);
                }
                else
                {
                    assetIncidentList = await _assetManagerService.GetCurrentAssetIncidentsAsync();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            var assetTypes = await _assetManagerService.GetAssetTypesAsync();
            if (tp != null && tp > 0) { ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name", tp.Value); }
            else { ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name"); }

            return View(PaginatedList<AssetIncident>.CreateAsync(assetIncidentList.AsQueryable(), pg ?? 1, 100));
        }

        [Authorize(Roles = "AMSVWATXN, AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> index(string id, int? yr = null, int? mn = null)
        {
            IList<AssetIncident> assetIncidentList = new List<AssetIncident>();
            AssetIncidentListViewModel model = new AssetIncidentListViewModel();
            try
            {
                if (yr != null && yr.Value > 0)
                {
                    if(mn != null && mn.Value > 0)
                    {
                        model.yr = yr.Value;
                        model.mn = mn.Value;
                        assetIncidentList = await _assetManagerService.GetAssetIncidentsByAssetIdAndYearAndMonthAsync(id, yr.Value, mn.Value);
                    }
                    else
                    {
                        model.yr = yr.Value;
                        assetIncidentList = await _assetManagerService.GetAssetIncidentsByAssetIdAndYearAsync(id, yr.Value);
                    }
                }
                else
                {
                    if(mn != null && mn.Value > 0)
                    {
                        model.yr = DateTime.Now.Year;
                        model.mn = mn.Value;
                        assetIncidentList = await _assetManagerService.GetAssetIncidentsByAssetIdAndYearAndMonthAsync(id, DateTime.Now.Year, mn.Value);
                    }
                    else
                    {
                        model.yr = DateTime.Now.Year;
                        model.mn = DateTime.Now.Month;
                        assetIncidentList = await _assetManagerService.GetAssetIncidentsByAssetIdAndYearAndMonthAsync(id, DateTime.Now.Year, DateTime.Now.Month);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> AddIncident(string id = null)
        {
            AssetIncidentViewModel model = new AssetIncidentViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                Asset asset = await _assetManagerService.GetAssetByIdAsync(id);
                if(asset != null && !string.IsNullOrWhiteSpace(asset.AssetName))
                {
                    model.AssetID = asset.AssetID;
                    model.AssetName = asset.AssetName;
                    model.AssetTypeID = asset.AssetTypeID;
                    model.AssetCategoryID = asset.AssetCategoryID;
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> AddIncident(AssetIncidentViewModel model)
        {
            if (ModelState.IsValid)
            {
                Asset asset = new Asset();
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
                            asset = await _assetManagerService.GetAssetByNameAsync(model.AssetName);
                            model.AssetID = asset.AssetID;
                            model.AssetTypeID = asset.AssetTypeID;
                            model.AssetCategoryID = asset.AssetCategoryID;
                        }

                        if (string.IsNullOrWhiteSpace(model.AssetID) || model.AssetTypeID < 1 || model.AssetCategoryID < 1)
                        {
                            model.ViewModelErrorMessage = $"Ooops! The system could  not find any record of the equipment you entered. Please enter a valid equipment name and try again. ";
                        }
                        else
                        {
                            AssetIncident assetIncident = model.ConvertToAssetIncident();
                            assetIncident.LoggedTime = DateTime.Now;
                            assetIncident.LoggedBy = HttpContext.User.Identity.Name;
                            assetIncident.ModifiedBy = HttpContext.User.Identity.Name;
                            assetIncident.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                            if (await _assetManagerService.AddAssetIncidentAsync(assetIncident))
                            {
                                model.OperationIsCompleted = true;
                                model.OperationIsSuccessful = true;
                                model.ViewModelSuccessMessage = $"Incident added successfully!";
                            }
                            else
                            {
                                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Adding new Incident failed.";
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
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> EditIncident(int id)
        {
            AssetIncidentViewModel model = new AssetIncidentViewModel();
            try
            {
                AssetIncident assetIncident = await _assetManagerService.GetAssetIncidentByIdAsync(id);
                model.AssetDescription = assetIncident.AssetDescription;
                model.AssetID = assetIncident.AssetID;
                model.AssetName = assetIncident.AssetName;
                model.AssetTypeID = assetIncident.AssetTypeID;
                model.AssetTypeName = assetIncident.AssetTypeName;
                model.AssetIncidentID = assetIncident.AssetIncidentID;
                model.AssetCategoryID = assetIncident.AssetCategoryID;
                model.ActionTaken = assetIncident.ActionTaken;
                model.AssetCondition = assetIncident.AssetCondition;
                model.AssetCategoryName = assetIncident.AssetCategoryName;
                model.Comments = assetIncident.Comments;
                model.IncidentDescription = assetIncident.IncidentDescription;
                model.IncidentTime = assetIncident.IncidentTime;
                model.IncidentTitle = assetIncident.IncidentTitle;
                model.LoggedBy = assetIncident.LoggedBy;
                model.LoggedTime = assetIncident.LoggedTime;
                model.Recommendation = assetIncident.Recommendation;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> EditIncident(AssetIncidentViewModel model)
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
                        if (model.AssetIncidentID < 1)
                        {
                            model.ViewModelErrorMessage = $"Error! Sorry, incident record was not found.";
                            return View(model);
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(model.AssetID) || model.AssetTypeID < 1 || model.AssetCategoryID < 1)
                            {
                                asset = await _assetManagerService.GetAssetByNameAsync(model.AssetName);
                                model.AssetID = asset.AssetID;
                                model.AssetTypeID = asset.AssetTypeID;
                                model.AssetCategoryID = asset.AssetCategoryID;
                            }

                            if (string.IsNullOrWhiteSpace(model.AssetID) || model.AssetTypeID < 1 || model.AssetCategoryID < 1)
                            {
                                model.ViewModelErrorMessage = $"Ooops! The system could  not find any record of the equipment you entered. Please enter a valid equipment name and try again. ";
                            }
                            else
                            {
                                AssetIncident assetIncident = model.ConvertToAssetIncident();
                                //assetIncident.LoggedTime = DateTime.Now;
                                //assetIncident.LoggedBy = HttpContext.User.Identity.Name;
                                assetIncident.ModifiedBy = HttpContext.User.Identity.Name;
                                assetIncident.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                                if (await _assetManagerService.UpdateAssetIncidentAsync(assetIncident))
                                {
                                    model.OperationIsCompleted = true;
                                    model.OperationIsSuccessful = true;
                                    model.ViewModelSuccessMessage = $"Incident updated successfully!";
                                }
                                else
                                {
                                    model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Updating Incident failed.";
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
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSVWATXN, AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> IncidentDetails(int id)
        {
            AssetIncidentViewModel model = new AssetIncidentViewModel();
            try
            {
                AssetIncident assetIncident = await _assetManagerService.GetAssetIncidentByIdAsync(id);
                model.AssetDescription = assetIncident.AssetDescription;
                model.AssetID = assetIncident.AssetID;
                model.AssetName = assetIncident.AssetName;
                model.AssetTypeID = assetIncident.AssetTypeID;
                model.AssetTypeName = assetIncident.AssetTypeName;
                model.AssetIncidentID = assetIncident.AssetIncidentID;
                model.AssetCategoryID = assetIncident.AssetCategoryID;
                model.ActionTaken = assetIncident.ActionTaken;
                model.AssetCondition = assetIncident.AssetCondition;
                model.AssetCategoryName = assetIncident.AssetCategoryName;
                model.Comments = assetIncident.Comments;
                model.IncidentDescription = assetIncident.IncidentDescription;
                model.IncidentTime = assetIncident.IncidentTime;
                model.IncidentTitle = assetIncident.IncidentTitle;
                model.LoggedBy = assetIncident.LoggedBy;
                model.LoggedTime = assetIncident.LoggedTime;
                model.Recommendation = assetIncident.Recommendation;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> DeleteIncident(int id)
        {
            AssetIncidentViewModel model = new AssetIncidentViewModel();
            try
            {
                AssetIncident assetIncident = await _assetManagerService.GetAssetIncidentByIdAsync(id);
                model.AssetDescription = assetIncident.AssetDescription;
                model.AssetID = assetIncident.AssetID;
                model.AssetName = assetIncident.AssetName;
                model.AssetTypeID = assetIncident.AssetTypeID;
                model.AssetTypeName = assetIncident.AssetTypeName;
                model.AssetIncidentID = assetIncident.AssetIncidentID;
                model.AssetCategoryID = assetIncident.AssetCategoryID;
                model.ActionTaken = assetIncident.ActionTaken;
                model.AssetCondition = assetIncident.AssetCondition;
                model.AssetCategoryName = assetIncident.AssetCategoryName;
                model.Comments = assetIncident.Comments;
                model.IncidentDescription = assetIncident.IncidentDescription;
                model.IncidentTime = assetIncident.IncidentTime;
                model.IncidentTitle = assetIncident.IncidentTitle;
                model.LoggedBy = assetIncident.LoggedBy;
                model.LoggedTime = assetIncident.LoggedTime;
                model.Recommendation = assetIncident.Recommendation;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);

        }

        [HttpPost]
        [Authorize(Roles = "AMSMGATXN, XYALLACCZ")]
        public async Task<IActionResult> DeleteIncident(AssetIncidentViewModel model)
        {
            AssetIncident assetIncident = new AssetIncident();
            bool IsSuccessful = false;
            try
            {
                if (model.AssetIncidentID > 0)
                {
                    IsSuccessful = await _assetManagerService.DeleteAssetIncidentAsync(model.AssetIncidentID.Value);
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
                    model.ViewModelErrorMessage = $"Sorry an error was encountered. Equipment Incident could not be deleted.";
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