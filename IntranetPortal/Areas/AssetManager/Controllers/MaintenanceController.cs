using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.AssetManager.Models;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntranetPortal.Areas.AssetManager.Controllers
{
    [Area("AssetManager")]
    public class MaintenanceController : Controller
    {
        private readonly ILogger<MaintenanceController> _logger;
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IAssetManagerService _assetManagerService;
        private readonly IBaseModelService _baseModelService;
        public MaintenanceController(IConfiguration configuration, ISecurityService securityService,
                        IBaseModelService baseModelService, IAssetManagerService assetManagerService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _assetManagerService = assetManagerService;
        }


        public async Task<IActionResult> List(int? tp = null, int? pg = null)
        {
            IList<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            try
            {
                if (tp > 0)
                {
                    ViewBag.TypeID = tp;
                    assetMaintenanceList = await _assetManagerService.GetCurrentAssetMaintenancesByAssetTypeIdAsync(tp.Value);
                }
                else
                {
                    assetMaintenanceList = await _assetManagerService.GetCurrentAssetMaintenancesAsync();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            var assetTypes = await _assetManagerService.GetAssetTypesAsync();
            if (tp != null && tp > 0) { ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name", tp.Value); }
            else { ViewBag.AssetTypesList = new SelectList(assetTypes, "ID", "Name"); }

            return View(PaginatedList<AssetMaintenance>.CreateAsync(assetMaintenanceList.AsQueryable(), pg ?? 1, 100));
        }

        [HttpGet]
        public IActionResult AddMaintenance()
        {
            AssetMaintenanceViewModel model = new AssetMaintenanceViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddMaintenance(AssetMaintenanceViewModel model)
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
                            var assets = await _assetManagerService.SearchAssetsByNameAsync(model.AssetName);
                            asset = assets.ToList().FirstOrDefault();
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
                            AssetMaintenance assetMaintenance = model.ConvertToAssetMaintenance();
                            assetMaintenance.LoggedTime = DateTime.Now;
                            assetMaintenance.LoggedBy = HttpContext.User.Identity.Name;
                            assetMaintenance.ModifiedBy = HttpContext.User.Identity.Name;
                            assetMaintenance.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                            if (await _assetManagerService.AddAssetMaintenanceAsync(assetMaintenance))
                            {
                                model.OperationIsCompleted = true;
                                model.OperationIsSuccessful = true;
                                model.ViewModelSuccessMessage = $"Maintenance added successfully!";
                            }
                            else
                            {
                                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Adding new Maintenance failed.";
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
        public async Task<IActionResult> EditMaintenance(int id)
        {
            AssetMaintenanceViewModel model = new AssetMaintenanceViewModel();
            try
            {
                AssetMaintenance assetMaintenance = await _assetManagerService.GetAssetMaintenanceByIdAsync(id);
                model.AssetDescription = assetMaintenance.AssetDescription;
                model.AssetID = assetMaintenance.AssetID;
                model.AssetName = assetMaintenance.AssetName;
                model.AssetTypeID = assetMaintenance.AssetTypeID;
                model.AssetTypeName = assetMaintenance.AssetTypeName;
                model.AssetMaintenanceID = assetMaintenance.AssetMaintenanceID;
                model.AssetCategoryID = assetMaintenance.AssetCategoryID;
                model.IssueDescription = assetMaintenance.IssueDescription;
                model.SolutionDescription = assetMaintenance.SolutionDescription;
                model.PreviousCondition = assetMaintenance.PreviousCondition;
                model.FinalCondition = assetMaintenance.FinalCondition;
                model.AssetCategoryName = assetMaintenance.AssetCategoryName;
                model.Comments = assetMaintenance.Comments;
                model.MaintainedBy = assetMaintenance.MaintainedBy;
                model.SupervisedBy = assetMaintenance.SupervisedBy;
                model.StartTime = assetMaintenance.StartTime;
                model.EndTime = assetMaintenance.EndTime;
                model.MaintenanceTitle = assetMaintenance.MaintenanceTitle;
                model.LoggedBy = assetMaintenance.LoggedBy;
                model.LoggedTime = assetMaintenance.LoggedTime;
                model.ModifiedBy = assetMaintenance.ModifiedBy;
                model.ModifiedTime = assetMaintenance.ModifiedTime;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditMaintenance(AssetMaintenanceViewModel model)
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
                        if (model.AssetMaintenanceID < 1)
                        {
                            model.ViewModelErrorMessage = $"Error! Sorry, maintenance record was not found.";
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
                            }

                            if (string.IsNullOrWhiteSpace(model.AssetID) || model.AssetTypeID < 1 || model.AssetCategoryID < 1)
                            {
                                model.ViewModelErrorMessage = $"Ooops! The system could  not find any record of the equipment you entered. Please enter a valid equipment name and try again. ";
                            }
                            else
                            {
                                AssetMaintenance assetMaintenance = model.ConvertToAssetMaintenance();
                                assetMaintenance.ModifiedBy = HttpContext.User.Identity.Name;
                                assetMaintenance.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                                if (await _assetManagerService.UpdateAssetMaintenanceAsync(assetMaintenance))
                                {
                                    model.OperationIsCompleted = true;
                                    model.OperationIsSuccessful = true;
                                    model.ViewModelSuccessMessage = $"Maintenance updated successfully!";
                                }
                                else
                                {
                                    model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Updating Maintenance failed.";
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
        public async Task<IActionResult> MaintenanceDetails(int id)
        {
            AssetMaintenanceViewModel model = new AssetMaintenanceViewModel();
            try
            {
                AssetMaintenance assetMaintenance = await _assetManagerService.GetAssetMaintenanceByIdAsync(id);
                model.AssetDescription = assetMaintenance.AssetDescription;
                model.AssetID = assetMaintenance.AssetID;
                model.AssetName = assetMaintenance.AssetName;
                model.AssetTypeID = assetMaintenance.AssetTypeID;
                model.AssetTypeName = assetMaintenance.AssetTypeName;
                model.AssetMaintenanceID = assetMaintenance.AssetMaintenanceID;
                model.AssetCategoryID = assetMaintenance.AssetCategoryID;
                model.IssueDescription = assetMaintenance.IssueDescription;
                model.SolutionDescription = assetMaintenance.SolutionDescription;
                model.PreviousCondition = assetMaintenance.PreviousCondition;
                model.FinalCondition = assetMaintenance.FinalCondition;
                model.AssetCategoryName = assetMaintenance.AssetCategoryName;
                model.Comments = assetMaintenance.Comments;
                model.MaintainedBy = assetMaintenance.MaintainedBy;
                model.SupervisedBy = assetMaintenance.SupervisedBy;
                model.StartTime = assetMaintenance.StartTime;
                model.EndTime = assetMaintenance.EndTime;
                model.MaintenanceTitle = assetMaintenance.MaintenanceTitle;
                model.LoggedBy = assetMaintenance.LoggedBy;
                model.LoggedTime = assetMaintenance.LoggedTime;
                model.ModifiedBy = assetMaintenance.ModifiedBy;
                model.ModifiedTime = assetMaintenance.ModifiedTime;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMaintenance(int id)
        {
            AssetMaintenanceViewModel model = new AssetMaintenanceViewModel();
            try
            {
                AssetMaintenance assetMaintenance = await _assetManagerService.GetAssetMaintenanceByIdAsync(id);
                model.AssetDescription = assetMaintenance.AssetDescription;
                model.AssetID = assetMaintenance.AssetID;
                model.AssetName = assetMaintenance.AssetName;
                model.AssetTypeID = assetMaintenance.AssetTypeID;
                model.AssetTypeName = assetMaintenance.AssetTypeName;
                model.AssetMaintenanceID = assetMaintenance.AssetMaintenanceID;
                model.AssetCategoryID = assetMaintenance.AssetCategoryID;
                model.IssueDescription = assetMaintenance.IssueDescription;
                model.SolutionDescription = assetMaintenance.SolutionDescription;
                model.PreviousCondition = assetMaintenance.PreviousCondition;
                model.FinalCondition = assetMaintenance.FinalCondition;
                model.AssetCategoryName = assetMaintenance.AssetCategoryName;
                model.Comments = assetMaintenance.Comments;
                model.MaintainedBy = assetMaintenance.MaintainedBy;
                model.SupervisedBy = assetMaintenance.SupervisedBy;
                model.StartTime = assetMaintenance.StartTime;
                model.EndTime = assetMaintenance.EndTime;
                model.MaintenanceTitle = assetMaintenance.MaintenanceTitle;
                model.LoggedBy = assetMaintenance.LoggedBy;
                model.LoggedTime = assetMaintenance.LoggedTime;
                model.ModifiedBy = assetMaintenance.ModifiedBy;
                model.ModifiedTime = assetMaintenance.ModifiedTime;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMaintenance(AssetMaintenanceViewModel model)
        {
            AssetMaintenance assetMaintenance = new AssetMaintenance();
            bool IsSuccessful = false;
            try
            {
                if (model.AssetMaintenanceID > 0)
                {
                    IsSuccessful = await _assetManagerService.DeleteAssetMaintenanceAsync(model.AssetMaintenanceID.Value);
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
                    model.ViewModelErrorMessage = $"Sorry an error was encountered. Equipment Maintenance could not be deleted.";
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