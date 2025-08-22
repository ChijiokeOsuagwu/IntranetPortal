using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.PartnerServices.Models;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntranetPortal.Areas.PartnerServices.Controllers
{
    [Area("PartnerServices")]
    public class HomeController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IBusinessManagerService _businessManagerService;
        private readonly IBaseModelService _baseModelService;
        public HomeController(IConfiguration configuration, ISecurityService securityService,
                        IBaseModelService baseModelService, IBusinessManagerService businessManagerService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _businessManagerService = businessManagerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Business Sectors Controller Actions

        [Authorize(Roles = "ATSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> Sectors()
        {
            BusinessSectorsListViewModel model = new BusinessSectorsListViewModel();
            try
            {
                var entities = await _baseModelService.GetIndustrySectorsAsync();
                if (entities != null && entities.Count > 0)
                {
                    model.IndustrySectorsList = entities.ToList();
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            if (TempData["Error"] != null)
            {
                model.ViewModelErrorMessage = TempData["Error"].ToString();
            }

            if (TempData["Success"] != null)
            {
                model.ViewModelSuccessMessage = TempData["Success"].ToString();
            }
            return View(model);
        }

        
        [Authorize(Roles = "ATSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> ManageSector(int id)
        {
            ManageSectorViewModel model = new ManageSectorViewModel();
            try
            {
                if (id > 0)
                {
                    IndustrySector industrySector = await _baseModelService.GetIndustrySectorAsync(id);
                    if (industrySector != null && !string.IsNullOrWhiteSpace(industrySector.IndustrySectorName))
                    {
                        model.IndustrySectorID = industrySector.IndustrySectorId;
                        model.IndustrySectorName = industrySector.IndustrySectorName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [Authorize(Roles = "ATSSTTMGA, XYALLACCZ")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageSector(ManageSectorViewModel model)
        {
            try
            {
                IndustrySector industrySector = new IndustrySector();
                if (ModelState.IsValid)
                {
                    industrySector.IndustrySectorId = model.IndustrySectorID ?? 0;
                    industrySector.IndustrySectorName = model.IndustrySectorName;

                    if (industrySector.IndustrySectorId < 1)
                    {
                        if (await _baseModelService.CreateIndustrySectorAsync(industrySector))
                        {
                            return RedirectToAction("Sectors");
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        if (await _baseModelService.EditIndustrySectorAsync(industrySector))
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Industry Sector was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }
        

        [Authorize(Roles = "ATSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> DeleteSector(int id)
        {
            try
            {
                if (id > 0)
                {
                    bool IsDeleted = await _baseModelService.DeleteIndustrySectorAsync(id);
                    if (IsDeleted)
                    {
                        TempData["Success"] = "Sector deleted successfully!";
                    }
                    else
                    {
                        TempData["Error"] = "Sorry, an error was encountered. Delete operation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Sectors");
        }
        
        #endregion

    }
}