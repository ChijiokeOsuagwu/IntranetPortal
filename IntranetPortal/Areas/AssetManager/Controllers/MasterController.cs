using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using IntranetPortal.Areas.AssetManager.Models;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ClosedXML.Excel;

namespace IntranetPortal.Areas.AssetManager.Controllers
{
    [Area("AssetManager")]
    [Authorize]
    public class MasterController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IAssetManagerService _assetManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IErmService _ermService;

        public MasterController(IConfiguration configuration, ISecurityService securityService,
                                IBaseModelService baseModelService, IAssetManagerService assetManagerService,
                                IGlobalSettingsService globalSettingsService, IWebHostEnvironment webHostEnvironment,
                                IErmService ermService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _assetManagerService = assetManagerService;
            _globalSettingsService = globalSettingsService;
            _webHostEnvironment = webHostEnvironment;
            _ermService = ermService;
        }

        [Authorize(Roles = "AMSVWAINF, AMSMGAINF, XYALLACCZ")]
        public async Task<IActionResult> List(string an = null, int? tp = null, int? gp = null)
        {
            AssetListViewModel model = new AssetListViewModel();
            IEnumerable<Asset> assetList = new List<Asset>();
            try
            {
                var claims = HttpContext.User.Claims.ToList();
                string userId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                if (string.IsNullOrWhiteSpace(userId))
                {
                    await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                    return LocalRedirect("/Home/Login");
                }

                AssetPermission assetPermission = new AssetPermission();
                Employee employee = new Employee();
                string empName = HttpContext.User.Identity.Name;
                employee = await _ermService.GetEmployeeByNameAsync(empName);
                if(employee != null && !string.IsNullOrWhiteSpace(employee.EmployeeID))
                {
                    assetPermission.UserID = employee.EmployeeID;
                }

                if (!string.IsNullOrWhiteSpace(an))
                {
                    assetList = await _assetManagerService.SearchAssetsByNameAsync(an, userId);
                }
                else if (tp != null && tp.Value > 0)
                {
                    assetList = await _assetManagerService.GetAssetsByAssetTypeIdAsync(tp.Value, userId);
                }
                else if (gp != null && gp.Value > 0)
                {
                    assetList = await _assetManagerService.GetAssetsByAssetTypeIdAsync(tp.Value, userId);
                }
                else
                {
                    assetList = await _assetManagerService.GetAssetsAsync(userId);
                }
                model.AssetList = assetList.ToList();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                assetList = null;
            }
            var assetTypes = await _assetManagerService.GetAssetTypesAsync();
            var assetGroups = await _assetManagerService.GetAssetGroupsAsync();

            ViewBag.AssetTypeList = new SelectList(assetTypes, "ID", "Name");
            ViewBag.AssetGroupList = new SelectList(assetGroups, "GroupID", "GroupName");

            return View(model);
        }

        #region Asset Master Write Action Controller Methods

        [Authorize(Roles = "AMSMGAINF, XYALLACCZ")]
        public async Task<IActionResult> SelectType(int? cl = null, int? gp = null, string tn = null)
        {
            SelectTypeListViewModel model = new SelectTypeListViewModel();
            IEnumerable<AssetType> assetTypeList = new List<AssetType>();
            try
            {

                if (!string.IsNullOrWhiteSpace(tn))
                {
                    assetTypeList = await _assetManagerService.SearchAssetTypesByNameAsync(tn);
                }
                else if (gp != null && gp.Value > 0)
                {
                    assetTypeList = await _assetManagerService.GetAssetTypesByGroupIdAsync(gp.Value);
                }
                else if (cl != null && cl.Value > 0)
                {
                    assetTypeList = await _assetManagerService.GetAssetTypesByClassIdAsync(cl.Value);
                }
                else
                {
                    assetTypeList = await _assetManagerService.GetAssetTypesAsync();
                }
                model.AssetTypeList = assetTypeList.ToList();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                model.AssetTypeList = null;
            }
            var assetGroups = await _assetManagerService.GetAssetGroupsAsync();
            var assetClasses = await _assetManagerService.GetAssetClassesAsync();

            ViewBag.AssetGroupList = new SelectList(assetGroups, "GroupID", "GroupName");
            ViewBag.AssetClassList = new SelectList(assetClasses, "ID", "Name");

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGAINF, XYALLACCZ")]
        public async Task<IActionResult> AddAsset(int tp)
        {
            AssetViewModel model = new AssetViewModel();
            if(tp > 0)
            {
                model.AssetTypeID = tp;
                AssetType assetType = await _assetManagerService.GetAssetTypeByIdAsync(tp);
                if(assetType != null && assetType.ID > 0)
                {
                    model.AssetCategoryID = assetType.CategoryID;
                    model.AssetCategoryID = assetType.CategoryID;
                    model.AssetClassID = assetType.ClassID.Value;
                    model.AssetClassName = assetType.ClassName;
                    model.AssetGroupID = assetType.GroupID.Value;
                    model.AssetGroupName = assetType.GroupName;
                    model.AssetTypeName = assetType.Name;
                }
            }
            model.ConditionStatus = AssetCondition.InGoodCondition;

            var claims = HttpContext.User.Claims.ToList();
            string userId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
            {
                await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                return LocalRedirect("/Home/Login");
            }

            var divisions = await _assetManagerService.GetAssetDivisionsAsync(userId);
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            ViewBag.DivisionsList = new SelectList(divisions, "ID", "Name");
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGAINF, XYALLACCZ")]
        public async Task<IActionResult> AddAsset(AssetViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string uploadsFolder = null;
                    string absoluteFilePath = null;

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

                    if (!string.IsNullOrWhiteSpace(model.BinLocationName))
                    {
                        AssetBinLocation binLocation = await _assetManagerService.GetAssetBinLocationByNameAsync(model.BinLocationName);
                        if (binLocation != null && binLocation.AssetBinLocationID > 0) { model.BinLocationID = binLocation.AssetBinLocationID; }
                    }

                    if (!string.IsNullOrWhiteSpace(model.ParentAssetName))
                    {
                        Asset parentAsset = await _assetManagerService.GetAssetByNameAsync(model.ParentAssetName);
                        if(parentAsset != null)
                        {
                            model.ParentAssetID = parentAsset.AssetID;
                            model.ParentAssetName = parentAsset.AssetName;
                        }
                    }

                    Asset asset = model.ConvertToAsset();
                    asset.UsageStatus = "Available";
                    asset.ImagePath = uploadsFolder;
                    asset.ModifiedBy = HttpContext.User.Identity.Name;
                    asset.ModifiedDate = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";
                    asset.CreatedBy = HttpContext.User.Identity.Name;
                    asset.CreatedDate = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";
                    if (string.IsNullOrWhiteSpace(asset.ConditionDescription))
                    {
                        switch (asset.ConditionStatus)
                        {
                            case AssetCondition.InGoodCondition:
                                asset.ConditionDescription = "In Good Working Condition";
                                break;
                            case AssetCondition.BeyondRepair:
                                asset.ConditionDescription = "Faulty (Beyond Repair)";
                                break;
                            case AssetCondition.RequiresRepair:
                                asset.ConditionDescription = "Faulty (Requires Repairs)";
                                break;
                        }
                    }

                    if (await _assetManagerService.CreateAssetAsync(asset))
                    {
                        return RedirectToAction("List", new { tp = asset.AssetTypeID});
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
        [Authorize(Roles = "AMSMGAINF, XYALLACCZ")]
        public async Task<IActionResult> EditAsset(string id)
        {
            Asset asset = new Asset();
            AssetViewModel model = new AssetViewModel();
            if (string.IsNullOrWhiteSpace(id))

            {
                return RedirectToAction("AddAsset");
            }
            else
            {
                asset = await _assetManagerService.GetAssetByIdAsync(id);
                model.AssetCategoryID = asset.AssetCategoryID;
                model.AssetCategoryName = asset.AssetCategoryName;
                model.AssetClassID = asset.AssetClassID.Value;
                model.AssetClassName = asset.AssetClassName;
                model.AssetGroupID = asset.AssetGroupID;
                model.AssetGroupName = asset.AssetGroupName;
                model.AssetTypeID = asset.AssetTypeID;
                model.AssetTypeName = asset.AssetTypeName;
                model.AssetDivisionID = asset.AssetDivisionID.Value;

                model.AssetDescription = asset.AssetDescription;
                model.AssetID = asset.AssetID;
                model.AssetName = asset.AssetName;
                model.AssetNumber = asset.AssetNumber;
                model.ParentAssetID = asset.ParentAssetID;
                model.BaseLocationID = asset.BaseLocationID;
                model.BaseLocationName = asset.BaseLocationName;
                model.BinLocationID = asset.BinLocationID;
                model.BinLocationName = asset.BinLocationName;
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

            var claims = HttpContext.User.Claims.ToList();
            string userId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
            {
                await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                return LocalRedirect("/Home/Login");
            }

            var types = await _assetManagerService.GetAssetTypesByClassIdAsync(asset.AssetClassID.Value);
            var locations = await _globalSettingsService.GetAllLocationsAsync(userId);

            ViewBag.TypesList = new SelectList(types, "ID", "Name");
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGAINF, XYALLACCZ")]
        public async Task<IActionResult> EditAsset(AssetViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string uploadsFolder = null;
                    string absoluteFilePath = null;
                    AssetType assetType = await _assetManagerService.GetAssetTypeByIdAsync(model.AssetTypeID);
                    if(assetType != null)
                    {
                        model.AssetCategoryID = assetType.CategoryID;
                        model.AssetClassID = assetType.ClassID.Value;
                        model.AssetGroupID = assetType.GroupID;
                    }

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

                    if (!string.IsNullOrWhiteSpace(model.BinLocationName))
                    {
                        AssetBinLocation binLocation = await _assetManagerService.GetAssetBinLocationByNameAsync(model.BinLocationName);
                        if (binLocation != null && binLocation.AssetBinLocationID > 0) { model.BinLocationID = binLocation.AssetBinLocationID; }
                    }

                    if (!string.IsNullOrWhiteSpace(model.ParentAssetName))
                    {
                        Asset parentAsset = await _assetManagerService.GetAssetByNameAsync(model.ParentAssetName);
                        if (parentAsset != null)
                        {
                            model.ParentAssetID = parentAsset.AssetID;
                            model.ParentAssetName = parentAsset.AssetName;
                        }
                    }

                    Asset asset = model.ConvertToAsset();
                    if (ImageChanged)
                    {
                        asset.ImagePath = uploadsFolder;
                    }
                    else
                    {
                        asset.ImagePath = model.OldImagePath;
                    }

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
                        return RedirectToAction("Details", new { id = model.AssetID });
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

            var claims = HttpContext.User.Claims.ToList();
            string userId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
            {
                await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                return LocalRedirect("/Home/Login");
            }

            var types = await _assetManagerService.GetAssetTypesAsync();
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            var assets = await _assetManagerService.GetAssetsAsync(userId);

            ViewBag.TypesList = new SelectList(types, "ID", "Name");
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            ViewBag.AssetsList = new SelectList(assets, "AssetID", "AssetName");

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSVWAINF, AMSMGAINF, XYALLACCZ")]
        public async Task<IActionResult> Details(string id)
        {
            AssetViewModel model = new AssetViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                Asset asset = await _assetManagerService.GetAssetByIdAsync(id);
                model.AssetCategoryID = asset.AssetCategoryID;
                model.AssetCategoryName = asset.AssetCategoryName;
                model.AssetClassID = asset.AssetClassID.Value;
                model.AssetClassName = asset.AssetClassName;
                model.AssetGroupID = asset.AssetGroupID;
                model.AssetGroupName = asset.AssetGroupName;
                model.AssetTypeID = asset.AssetTypeID;
                model.AssetTypeName = asset.AssetTypeName;
                model.AssetDescription = asset.AssetDescription;
                model.AssetID = asset.AssetID;
                model.AssetName = asset.AssetName;
                model.AssetNumber = asset.AssetNumber;
                model.ParentAssetID = asset.ParentAssetID;
                model.BaseLocationID = asset.BaseLocationID;
                model.BaseLocationName = asset.BaseLocationName;
                model.BinLocationID = asset.BinLocationID;
                model.BinLocationName = asset.BinLocationName;
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
        [Authorize(Roles = "AMSVWAINF, AMSMGAINF, XYALLACCZ")]
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
        [Authorize(Roles = "AMSMGAINF, XYALLACCZ")]
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
                model.BinLocationID = asset.BinLocationID;
                model.BinLocationName = asset.BinLocationName;
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
        [Authorize(Roles = "AMSMGAINF, XYALLACCZ")]
        public async Task<IActionResult> DeleteAsset(AssetViewModel model)
        {
            string absoluteFilePath = string.Empty;
            if (model != null)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(model.ImagePath))
                    {
                        absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, model.ImagePath);
                    }

                    string deletedBy = HttpContext.User.Identity.Name;
                    bool succeeded = await _assetManagerService.DeleteAssetAsync(model.AssetID, deletedBy);
                    if (succeeded)
                    {
                        if (!string.IsNullOrEmpty(absoluteFilePath))
                        {
                            FileInfo file = new FileInfo(absoluteFilePath);
                            if (file.Exists)
                            {
                                file.Delete();
                            }
                        }
                        return RedirectToAction("List");
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

        #region Asset Master Reports
        
        [Authorize(Roles = "AMSMGAINF, XYALLACCZ")]
        public async Task<IActionResult> StatusReport(int? bsl = null, int? bnl = null, int? grp = null, int? typ = null, int? cnd = null)
        {
            string userName = HttpContext.User.Identity.Name;
            string userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            if (string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(userName))
            {
                await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                return LocalRedirect("/Home/Login");
            }

            AssetStatusReportViewModel model = new AssetStatusReportViewModel();
            model.AssetMasterList = new List<Asset>();
            model.bsl = bsl;
            model.bnl = bnl;
            model.grp = grp;
            model.typ = typ;
            model.cnd = cnd;

            try
            {
                var entities = await _assetManagerService.GetAssetStatusReportAsync(userId, model.bsl, model.bnl, model.grp, model.typ, model.cnd);
                if(entities != null)
                {
                    model.AssetMasterList = entities;
                    model.RecordCount = entities.Count;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var baselocations_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (baselocations_entities != null && baselocations_entities.Count > 0)
            {
                ViewBag.BaseLocationList = new SelectList(baselocations_entities, "LocationID", "LocationName", bsl);
            }

            var binlocation_entities = await _assetManagerService.GetAssetBinLocationsAsync(userId);
            if (binlocation_entities != null && binlocation_entities.Count > 0)
            {
                ViewBag.BinLocationList = new SelectList(binlocation_entities, "AssetBinLocationID", "AssetBinLocationName", bnl);
            }

            var asset_group_entities = await _assetManagerService.GetAssetGroupsAsync();
            if (asset_group_entities != null && asset_group_entities.Count > 0)
            {
                ViewBag.AssetGroupList = new SelectList(asset_group_entities, "GroupID", "GroupName", grp);
            }

            var asset_type_entities = await _assetManagerService.GetAssetTypesAsync();
            if (asset_type_entities != null && asset_type_entities.Count > 0)
            {
                ViewBag.AssetTypeList = new SelectList(asset_type_entities, "ID", "Name", typ);
            }
            return View(model);
        }

        [Authorize(Roles = "AMSMGAINF, XYALLACCZ")]
        public async Task<FileResult> DownloadStatusReport(int? bsl = null, int? bnl = null, int? grp = null, int? typ = null, int? cnd = null)
        {
            string userName = HttpContext.User.Identity.Name;
            string userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            if (string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(userName))
            {
                return null;
            }

            AssetStatusReportViewModel model = new AssetStatusReportViewModel();
            model.AssetMasterList = new List<Asset>();
            model.bsl = bsl;
            model.bnl = bnl;
            model.cnd = cnd;
            model.grp = grp;
            model.typ = typ;

            string fileName = $"Assets & Equipment Status Report {DateTime.Now.Ticks.ToString()}.xlsx";
            try
            {
                var entities = await _assetManagerService.GetAssetStatusReportAsync(userId, model.bsl, model.bnl, model.grp, model.typ, model.cnd);
                if (entities != null && entities.Count > 0)
                {
                    model.AssetMasterList = entities;
                    model.RecordCount = entities.Count;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return GenerateStatusReportExcel(fileName, model.AssetMasterList);
        }



        #endregion

        //=============== Assets Helper Methods =====================//
        #region Assets Helper Methods

        [HttpGet]
        public JsonResult GetAssetNames(string text)
        {
            var claims = HttpContext.User.Claims.ToList();
            string userId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Json(null);
            }
            List<string> assets = _assetManagerService.SearchAssetsByNameAsync(text, userId).Result.Select(x => x.AssetName).ToList();
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

        private FileResult GenerateStatusReportExcel(string fileName, IEnumerable<Asset> results)
        {
            DataTable dataTable = new DataTable("tbAssetList");
            dataTable.Columns.AddRange(new DataColumn[]
            {
              new DataColumn("No"),
              new DataColumn("Name"),
              new DataColumn("Type"),
              new DataColumn("Condition"),
              new DataColumn("Base Location"),
              new DataColumn("Bin Location"),
              new DataColumn("Current Location"),
              new DataColumn("Usage Status"),
              new DataColumn("Master"),
            });

            foreach (var result in results)
            {
                dataTable.Rows.Add(
                    result.AssetNumber,
                    result.AssetName,
                    result.AssetTypeName,
                    result.ConditionDescription,
                    result.BaseLocationName,
                    result.BinLocationName,
                    result.CurrentLocation,
                    result.UsageStatus,
                    result.ParentAssetName
                    );
            }

            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        #endregion


    }
}