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