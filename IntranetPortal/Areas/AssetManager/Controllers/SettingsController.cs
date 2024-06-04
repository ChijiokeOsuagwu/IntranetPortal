using System;
using System.Collections.Generic;
using System.Linq;
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

namespace IntranetPortal.Areas.AssetManager.Controllers
{
    [Area("AssetManager")]
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IAssetManagerService _assetManagerService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IErmService _ermService;

        public SettingsController(IConfiguration configuration, IAssetManagerService assetManagerService,
                                    IGlobalSettingsService globalSettingsService, IWebHostEnvironment webHostingEnvironment,
                                    IErmService ermService)
        {
            _configuration = configuration;
            _assetManagerService = assetManagerService;
            _globalSettingsService = globalSettingsService;
            _webHostEnvironment = webHostingEnvironment;
            _ermService = ermService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //=============== Asset Division Controller Actions ======================//
        #region Asset Division Controller Actions

        [Authorize(Roles = "AMSVWASTT, AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Divisions(string searchString = null)
        {
            AssetDivisionListViewModel model = new AssetDivisionListViewModel();
            IEnumerable<AssetDivision> divisionList;
            if (string.IsNullOrEmpty(searchString))
            {
                divisionList = await _assetManagerService.GetAssetDivisionsAsync();
            }
            else
            {
                divisionList = await _assetManagerService.SearchAssetDivisionsByNameAsync(searchString);
            }
            ViewData["CurrentFilter"] = searchString;
            model.AssetDivisionList = divisionList.ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddDivision()
        {
            AssetDivisionViewModel model = new AssetDivisionViewModel();
            var location_entities = await _globalSettingsService.GetAllLocationsAsync();
            if(location_entities != null)
            {
                ViewBag.LocationList = new SelectList(location_entities, "LocationID", "LocationName");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddDivision(AssetDivisionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetDivision division = model.ConvertToAssetDivision();
                    bool succeeded = await _assetManagerService.CreateAssetDivisionAsync(division);
                    if (succeeded)
                    {
                        return RedirectToAction("Divisions");
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
            var location_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (location_entities != null)
            {
                ViewBag.LocationList = new SelectList(location_entities, "LocationID", "LocationName");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditDivision(int id)
        {
            AssetDivision division = new AssetDivision();
            AssetDivisionViewModel model = new AssetDivisionViewModel();
            if (id > 0)
            {
                division = await _assetManagerService.GetAssetDivisionByIdAsync(id);
                model.ID = division.ID;
                model.Name = division.Name;
                model.Description = division.Description;
                model.LocationID = division.LocationID.Value;
            }
            else
            {
                return RedirectToAction("AddDivision");
            }
            var location_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (location_entities != null)
            {
                ViewBag.LocationList = new SelectList(location_entities, "LocationID", "LocationName");
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditDivision(AssetDivisionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetDivision division = model.ConvertToAssetDivision();
                    bool succeeded = await _assetManagerService.UpdateAssetDivisionAsync(division);
                    if (succeeded)
                    {
                        return RedirectToAction("Divisions");
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
            var location_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (location_entities != null)
            {
                ViewBag.LocationList = new SelectList(location_entities, "LocationID", "LocationName");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteDivision(int id)
        {
            var claims = HttpContext.User.Claims.ToList();
            string userId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
            {
                await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                return LocalRedirect("/Home/Login");
            }

            AssetDivision division = new AssetDivision();
            AssetDivisionViewModel model = new AssetDivisionViewModel();
            List<Asset> assets = new List<Asset>();
            var entities = await _assetManagerService.GetAssetsByDivisionIdAsync(id, userId);
            assets = entities.ToList();
            if (assets != null && assets.Count > 0)
            {
                model.OperationIsCompleted = true;
                model.ViewModelErrorMessage = $"This item cannot be deleted because there are other records in the system that depend on it.";
            }

            if (id > 0)
            {
                division = await _assetManagerService.GetAssetDivisionByIdAsync(id);
                model.ID = division.ID;
                model.Name = division.Name;
                model.Description = division.Description;
            }
            else
            {
                return RedirectToAction("Divisions", "Settings");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteDivision(AssetDivisionViewModel model)
        {
            if (model != null)
            {
                try
                {
                    bool succeeded = await _assetManagerService.DeleteAssetDivisionAsync(model.ID.Value);
                    if (succeeded)
                    {
                        return RedirectToAction("Divisions");
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

        //=============== Asset Category Controller Actions ======================//
        #region Asset Category Controller Actions

        [Authorize(Roles = "AMSVWASTT, AMSMGASTT, XYALLACCZ")]
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
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public IActionResult AddCategory()
        {
            AssetCategoryViewModel model = new AssetCategoryViewModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
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
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
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
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
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
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
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
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
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

        //=============== Asset Types Controller Actions =========================//
        #region Asset Types Controller Action

        [Authorize(Roles = "AMSVWASTT, AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AssetTypes(int? cd = null, int? gd = null, string searchString = null)
        {
            AssetTypeListViewModel model = new AssetTypeListViewModel();
            IEnumerable<AssetType> typeList = new List<AssetType>();
            if (cd != null && cd.Value > 0)
            {
                typeList = await _assetManagerService.GetAssetTypesByClassIdAsync(cd.Value);
            }
            else if (gd != null && gd.Value > 0)
            {
                typeList = await _assetManagerService.GetAssetTypesByGroupIdAsync(gd.Value);
            }
            else if (!string.IsNullOrEmpty(searchString))
            {
                typeList = await _assetManagerService.SearchAssetTypesByNameAsync(searchString);
            }
            else
            {
                typeList = await _assetManagerService.GetAssetTypesAsync();
            }

            ViewBag.cd = cd;
            ViewBag.gd = gd;

            var assetClasses = await _assetManagerService.GetAssetClassesAsync();
            ViewBag.AssetClassesList = new SelectList(assetClasses, "ID", "Name");

            var assetGroups = await _assetManagerService.GetAssetGroupsAsync();
            ViewBag.AssetGroupsList = new SelectList(assetGroups, "GroupID", "GroupName");

            ViewData["CurrentFilter"] = searchString;
            model.AssetTypeList = typeList.ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddAssetType()
        {
            AssetTypeViewModel model = new AssetTypeViewModel();

            var assetGroups = await _assetManagerService.GetAssetGroupsAsync();
            ViewBag.AssetGroupsList = new SelectList(assetGroups, "GroupID", "GroupName");

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddAssetType(AssetTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetGroup assetGroup = await _assetManagerService.GetAssetGroupByIdAsync(model.GroupID);
                    if (assetGroup != null && !string.IsNullOrWhiteSpace(assetGroup.GroupName))
                    {
                        model.CategoryID = assetGroup.CategoryID.Value;
                        model.ClassID = assetGroup.ClassID.Value;
                    }

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

            var assetGroups = await _assetManagerService.GetAssetGroupsAsync();
            ViewBag.AssetGroupsList = new SelectList(assetGroups, "GroupID", "GroupName");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
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
            var assetClasses = await _assetManagerService.GetAssetClassesAsync();
            ViewBag.AssetClassesList = new SelectList(assetClasses, "ID", "Name");

            var assetGroups = await _assetManagerService.GetAssetGroupsAsync();
            ViewBag.AssetGroupsList = new SelectList(assetGroups, "GroupID", "GroupName");

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditAssetType(AssetTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetGroup assetGroup = await _assetManagerService.GetAssetGroupByIdAsync(model.GroupID);
                    if (assetGroup != null && !string.IsNullOrWhiteSpace(assetGroup.GroupName))
                    {
                        model.CategoryID = assetGroup.CategoryID.Value;
                        model.ClassID = assetGroup.ClassID.Value;
                    }

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
            var assetClasses = await _assetManagerService.GetAssetClassesAsync();
            ViewBag.AssetClassesList = new SelectList(assetClasses, "ID", "Name");

            var assetGroups = await _assetManagerService.GetAssetGroupsAsync();
            ViewBag.AssetGroupsList = new SelectList(assetGroups, "GroupID", "GroupName");

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSVWASTT, AMSMGASTT, XYALLACCZ")]
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

                model.GroupID = assetType.GroupID ?? 0;
                model.GroupName = assetType.GroupName;

                model.ClassID = assetType.ClassID ?? 0;
                model.ClassName = assetType.ClassName;

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
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
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
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
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

        //=============== Asset Class Controller Actions =========================//
        #region Asset Class Controller Action

        [Authorize(Roles = "AMSVWASTT, AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AssetClasses(int? cd = null, string searchString = null)
        {
            AssetClassListViewModel model = new AssetClassListViewModel();
            IEnumerable<AssetClass> classList = new List<AssetClass>();
            if (cd != null && cd.Value > 0)
            {
                classList = await _assetManagerService.GetAssetClassesByCategoryIdAsync(cd.Value);
            }
            else if (!string.IsNullOrEmpty(searchString))
            {
                classList = await _assetManagerService.SearchAssetClassesByNameAsync(searchString);
            }

            ViewBag.cd = cd;
            var assetCategories = await _assetManagerService.GetAssetCategoriesAsync();
            ViewBag.AssetCategoriesList = new SelectList(assetCategories, "ID", "Name");

            ViewData["CurrentFilter"] = searchString;
            model.AssetClassList = classList.ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddAssetClass()
        {
            AssetClassViewModel model = new AssetClassViewModel();
            var categories = await _assetManagerService.GetAssetCategoriesAsync();
            ViewBag.CategoryList = new SelectList(categories, "ID", "Name");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddAssetClass(AssetClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetClass assetClass = model.ConvertToAssetClass();
                    bool succeeded = await _assetManagerService.CreateAssetClassAsync(assetClass);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Asset Class was created successfully!";
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
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditAssetClass(int id)
        {
            AssetClass assetClass = new AssetClass();
            AssetClassViewModel model = new AssetClassViewModel();
            if (id > 0)
            {
                assetClass = await _assetManagerService.GetAssetClassByIdAsync(id);
                model.ID = assetClass.ID;
                model.Name = assetClass.Name;
                model.Description = assetClass.Description;
                model.CategoryID = assetClass.CategoryID;
                model.CategoryName = assetClass.CategoryName;
            }
            else
            {
                return RedirectToAction("AddAssetClass");
            }
            var categories = await _assetManagerService.GetAssetCategoriesAsync();
            ViewBag.CategoryList = new SelectList(categories, "ID", "Name");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditAssetClass(AssetClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetClass assetClass = model.ConvertToAssetClass();
                    bool succeeded = await _assetManagerService.UpdateAssetClassAsync(assetClass);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Asset Class was updated successfully!";
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
        [Authorize(Roles = "AMSVWASTT, AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AssetClassDetails(int id)
        {
            AssetClass assetClass = new AssetClass();
            AssetClassViewModel model = new AssetClassViewModel();
            if (id > 0)
            {
                assetClass = await _assetManagerService.GetAssetClassByIdAsync(id);
                model.ID = assetClass.ID;
                model.Name = assetClass.Name;
                model.Description = assetClass.Description;
                model.CategoryID = assetClass.CategoryID;
                model.CategoryName = assetClass.CategoryName;
            }
            else
            {
                return RedirectToAction("AssetClasses");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteAssetClass(int id)
        {
            AssetClass assetClass = new AssetClass();
            AssetClassViewModel model = new AssetClassViewModel();
            if (id > 0)
            {
                assetClass = await _assetManagerService.GetAssetClassByIdAsync(id);
                model.ID = assetClass.ID;
                model.Name = assetClass.Name;
                model.Description = assetClass.Description;
                model.CategoryID = assetClass.CategoryID;
                model.CategoryName = assetClass.CategoryName;
            }
            else
            {
                return RedirectToAction("AssetClasses");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteAssetClass(AssetClassViewModel model)
        {
            if (model != null)
            {
                try
                {
                    bool succeeded = await _assetManagerService.DeleteAssetClassAsync(model.ID.Value);
                    if (succeeded)
                    {
                        return RedirectToAction("AssetClasses");
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

        //=============== Asset Bin Location Controller Actions ===================//
        #region Asset Bin Location Controller Action

        [Authorize(Roles = "AMSVWASTT, AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AssetBinLocations(int? loc = null, string ss = null)
        {
            AssetBinLocationListViewModel model = new AssetBinLocationListViewModel();
            IEnumerable<AssetBinLocation> assetBinLocationList = new List<AssetBinLocation>();
            Employee employee = new Employee();
            var entity = await _ermService.GetEmployeeByNameAsync(HttpContext.User.Identity.Name);
            if (entity != null && !string.IsNullOrWhiteSpace(entity.EmployeeID))
            {
                AssetPermission assetPermission = new AssetPermission();
                assetPermission.UserID = entity.EmployeeID;

                if (!string.IsNullOrEmpty(ss))
                {
                    assetBinLocationList = await _assetManagerService.SearchAssetBinLocationsByNameAsync(ss, entity.EmployeeID);
                }
                else
                {
                    if (loc != null && loc > 0)
                    {
                        assetBinLocationList = await _assetManagerService.GetAssetBinLocationsByLocationIdAsync(loc.Value, entity.EmployeeID);
                    }
                }
            }

            var locations = await _globalSettingsService.GetAllLocationsAsync();
            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");

            model.loc = loc;
            model.sp = ss;
            model.AssetBinLocationList = assetBinLocationList.ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddAssetBinLocation()
        {
            AssetBinLocationViewModel model = new AssetBinLocationViewModel();
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddAssetBinLocation(AssetBinLocationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetBinLocation assetBinLocation = model.ConvertToAssetBinLocation();
                    bool succeeded = await _assetManagerService.CreateAssetBinLocationAsync(assetBinLocation);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Bin Location was created successfully!";
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
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, AXYALLACCZ")]
        public async Task<IActionResult> EditAssetBinLocation(int id)
        {
            AssetBinLocation assetBinLocation = new AssetBinLocation();
            AssetBinLocationViewModel model = new AssetBinLocationViewModel();
            if (id > 0)
            {
                assetBinLocation = await _assetManagerService.GetAssetBinLocationByIdAsync(id);
                model.AssetBinLocationID = assetBinLocation.AssetBinLocationID;
                model.AssetBinLocationName = assetBinLocation.AssetBinLocationName;
                model.AssetBinLocationDescription = assetBinLocation.AssetBinLocationDescription;
                model.AssetLocationID = assetBinLocation.AssetLocationID;
            }
            else
            {
                return RedirectToAction("AddAssetBinLocation");
            }
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditAssetBinLocation(AssetBinLocationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetBinLocation assetBinLocation = model.ConvertToAssetBinLocation();
                    bool succeeded = await _assetManagerService.UpdateAssetBinLocationAsync(assetBinLocation);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Bin Location was updated successfully!";
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
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSVWASTT, AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AssetBinLocationDetails(int id)
        {
            AssetBinLocation assetBinLocation = new AssetBinLocation();
            AssetBinLocationViewModel model = new AssetBinLocationViewModel();
            if (id > 0)
            {
                assetBinLocation = await _assetManagerService.GetAssetBinLocationByIdAsync(id);
                model.AssetBinLocationID = assetBinLocation.AssetBinLocationID;
                model.AssetBinLocationName = assetBinLocation.AssetBinLocationName;
                model.AssetBinLocationDescription = assetBinLocation.AssetBinLocationDescription;
                model.AssetLocationID = assetBinLocation.AssetLocationID;
                model.AssetLocationName = assetBinLocation.AssetLocationName;
            }
            else
            {
                return RedirectToAction("AssetBinLocations");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteAssetBinLocation(int id)
        {
            AssetBinLocation assetBinLocation = new AssetBinLocation();
            AssetBinLocationViewModel model = new AssetBinLocationViewModel();
            if (id > 0)
            {
                assetBinLocation = await _assetManagerService.GetAssetBinLocationByIdAsync(id);
                model.AssetBinLocationID = assetBinLocation.AssetBinLocationID;
                model.AssetBinLocationName = assetBinLocation.AssetBinLocationName;
                model.AssetBinLocationDescription = assetBinLocation.AssetBinLocationDescription;
                model.AssetLocationID = assetBinLocation.AssetLocationID;
                model.AssetLocationName = assetBinLocation.AssetLocationName;
            }
            else
            {
                return RedirectToAction("AssetBinLocations");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteAssetBinLocation(AssetBinLocationViewModel model)
        {
            if (model != null)
            {
                try
                {
                    bool succeeded = await _assetManagerService.DeleteAssetBinLocationAsync(model.AssetBinLocationID.Value);
                    if (succeeded)
                    {
                        return RedirectToAction("AssetBinLocations");
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


        //=============== Asset Groups Controller Actions =========================//
        #region Asset Groups Controller Action

        [Authorize(Roles = "AMSVWASTT, AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AssetGroups(int? cd = null, string searchString = null)
        {
            AssetGroupListViewModel model = new AssetGroupListViewModel();
            IEnumerable<AssetGroup> groupList = new List<AssetGroup>();
            if (cd != null && cd.Value > 0)
            {
                groupList = await _assetManagerService.GetAssetGroupsByClassIdAsync(cd.Value);
            }
            else if (!string.IsNullOrEmpty(searchString))
            {
                groupList = await _assetManagerService.SearchAssetGroupsByNameAsync(searchString);
            }
            else
            {
                groupList = await _assetManagerService.GetAssetGroupsAsync();
            }

            ViewBag.cd = cd;
            var assetClasses = await _assetManagerService.GetAssetClassesAsync();
            ViewBag.AssetClassesList = new SelectList(assetClasses, "ID", "Name");

            ViewData["CurrentFilter"] = searchString;
            model.AssetGroupList = groupList.ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddAssetGroup()
        {
            AssetGroupViewModel model = new AssetGroupViewModel();
            var assetClasses = await _assetManagerService.GetAssetClassesAsync();
            ViewBag.AssetClassesList = new SelectList(assetClasses, "ID", "Name");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddAssetGroup(AssetGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetClass assetClass = await _assetManagerService.GetAssetClassByIdAsync(model.ClassID);
                    if (assetClass != null && !string.IsNullOrWhiteSpace(assetClass.Name))
                    {
                        model.CategoryID = assetClass.CategoryID;
                    }

                    AssetGroup assetGroup = model.ConvertToAssetGroup();
                    bool succeeded = await _assetManagerService.CreateAssetGroupAsync(assetGroup);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Asset Group was created successfully!";
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
            var assetClasses = await _assetManagerService.GetAssetClassesAsync();
            ViewBag.AssetClassesList = new SelectList(assetClasses, "ID", "Name");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditAssetGroup(int id)
        {
            AssetGroup assetGroup = new AssetGroup();
            AssetGroupViewModel model = new AssetGroupViewModel();
            if (id > 0)
            {
                assetGroup = await _assetManagerService.GetAssetGroupByIdAsync(id);
                model.GroupID = assetGroup.GroupID;
                model.GroupName = assetGroup.GroupName;
                model.ClassID = assetGroup.ClassID ?? 0;
                model.ClassName = assetGroup.ClassName;
                model.CategoryID = assetGroup.CategoryID ?? 0;
                model.CategoryName = assetGroup.CategoryName;
            }
            else
            {
                return RedirectToAction("AddAssetGroup");
            }
            var assetClasses = await _assetManagerService.GetAssetClassesAsync();
            ViewBag.AssetClassesList = new SelectList(assetClasses, "ID", "Name");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditAssetGroup(AssetGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetClass assetClass = await _assetManagerService.GetAssetClassByIdAsync(model.ClassID);
                    if (assetClass != null && !string.IsNullOrWhiteSpace(assetClass.Name))
                    {
                        model.CategoryID = assetClass.CategoryID;
                    }

                    AssetGroup assetGroup = model.ConvertToAssetGroup();
                    bool succeeded = await _assetManagerService.UpdateAssetGroupAsync(assetGroup);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Asset Group was updated successfully!";
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
            var assetClasses = await _assetManagerService.GetAssetClassesAsync();
            ViewBag.AssetClassesList = new SelectList(assetClasses, "ID", "Name");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSVWASTT, AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AssetGroupDetails(int id)
        {
            AssetGroup assetGroup = new AssetGroup();
            AssetGroupViewModel model = new AssetGroupViewModel();
            if (id > 0)
            {
                assetGroup = await _assetManagerService.GetAssetGroupByIdAsync(id);
                model.GroupID = assetGroup.GroupID;
                model.GroupName = assetGroup.GroupName;
                model.ClassID = assetGroup.ClassID ?? 0;
                model.ClassName = assetGroup.ClassName;
                model.CategoryID = assetGroup.CategoryID ?? 0;
                model.CategoryName = assetGroup.CategoryName;
            }
            else
            {
                return RedirectToAction("AssetGroups");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteAssetGroup(int id)
        {
            AssetGroup assetGroup = new AssetGroup();
            AssetGroupViewModel model = new AssetGroupViewModel();
            if (id > 0)
            {
                assetGroup = await _assetManagerService.GetAssetGroupByIdAsync(id);
                model.GroupID = assetGroup.GroupID;
                model.GroupName = assetGroup.GroupName;
                model.ClassID = assetGroup.ClassID ?? 0;
                model.ClassName = assetGroup.ClassName;
                model.CategoryID = assetGroup.CategoryID ?? 0;
                model.CategoryName = assetGroup.CategoryName;
            }
            else
            {
                return RedirectToAction("AssetGroups");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AMSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteAssetGroup(AssetGroupViewModel model)
        {
            if (model != null)
            {
                try
                {
                    bool succeeded = await _assetManagerService.DeleteAssetGroupAsync(model.GroupID.Value);
                    if (succeeded)
                    {
                        return RedirectToAction("AssetGroups");
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


        //=============== Assets Helper Methods ==================================//
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

        #endregion
    }
}