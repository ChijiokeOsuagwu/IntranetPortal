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
using Microsoft.AspNetCore.Authorization;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace IntranetPortal.Areas.AssetManager.Controllers
{
    [Area("AssetManager")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IAssetManagerService _assetManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IErmService _ermService;

        public HomeController(IConfiguration configuration, ISecurityService securityService,
                                IBaseModelService baseModelService, IAssetManagerService assetManagerService,
                                IErmService ermService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _assetManagerService = assetManagerService;
            _ermService = ermService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "AMSVWAINF, AMSVWATXN, AMSVWASTT, XYALLACCZ")]
        public async Task<IActionResult> AssetList(int? sp, int? pg = null)
        {
            IEnumerable<Asset> assetList = new List<Asset>();
            try
            {
                var claims = HttpContext.User.Claims.ToList();
                string userId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                if(string.IsNullOrWhiteSpace(userId))
                {
                    await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                    return LocalRedirect("/Home/Login");
                }

                if (sp > 0)
                {
                    assetList = await _assetManagerService.GetAssetsByAssetTypeIdAsync(sp.Value, userId);
                }
                else
                {
                    assetList = await _assetManagerService.GetAssetsAsync(userId);
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

       //=============== Helper Methods =================//
        #region Helper Methods
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
        public JsonResult GetAssetTypes(string text)
        {
            List<string> asset_types = _assetManagerService.SearchAssetTypesByNameAsync(text).Result.Select(x => x.Name).ToList();
            return Json(asset_types);
        }


        [HttpGet]
        public JsonResult GetAssetParameters(string asn)
        {
            var claims = HttpContext.User.Claims.ToList();
            string userId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Json(null);
            }

            Asset asset = new Asset();

            List<Asset> assets = _assetManagerService.SearchAssetsByNameAsync(asn, userId).Result.ToList();

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

        [HttpGet]
        public JsonResult GetBinLocationNames(string text)
        {
            Employee employee = new Employee();
            var entity = _ermService.GetEmployeeByNameAsync(HttpContext.User.Identity.Name).Result;
            if (entity != null && !string.IsNullOrWhiteSpace(entity.EmployeeID))
            {
                string UserID = entity.EmployeeID;
                List<string> binLocations = _assetManagerService.SearchAssetBinLocationsByNameAsync(text, UserID).Result.Select(x => x.AssetBinLocationName).ToList();
                return Json(binLocations);
            }
            return null;
        }

        #endregion
    }
}