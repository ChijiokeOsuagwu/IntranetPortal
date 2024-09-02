using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.ERM.Models;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntranetPortal.Areas.ERM.Controllers
{
    [Area("ERM")]
    [Authorize(Roles = "ERMSTTMGR, XYALLACCZ")]
    public class SettingsController : Controller
    {
        private readonly IErmService _ermService;

        public SettingsController(IErmService ermService)
        {
            _ermService = ermService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Separation Types Controller Actions

        public async Task<IActionResult> SeparationTypes()
        {
            SeparationTypeListViewModel model = new SeparationTypeListViewModel();
            var entities = await _ermService.GetEmployeeSeparationTypesAsync();
            if (entities != null && entities.Count > 0)
            {
                model.SeparationTypeList = entities.ToList();
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

        public async Task<IActionResult> ManageSeparationType(int id)
        {
            EmployeeSeparationTypeViewModel model = new EmployeeSeparationTypeViewModel();
            if (id > 0)
            {
                EmployeeSeparationType separationType = await _ermService.GetEmployeeSeparationTypeByIdAsync(id);
                if (separationType != null && !string.IsNullOrWhiteSpace(separationType.Description))
                {
                    model.Id = separationType.Id;
                    model.Description = separationType.Description;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageSeparationType(EmployeeSeparationTypeViewModel model)
        {
            try
            {
                EmployeeSeparationType separationType = new EmployeeSeparationType();
                if (ModelState.IsValid)
                {
                    separationType.Id = model.Id;
                    separationType.Description = model.Description;

                    if (separationType.Id < 1)
                    {
                        bool IsAdded = await _ermService.CreateEmployeeSeparationTypeAsync(separationType);
                        if (IsAdded)
                        {
                            return RedirectToAction("SeparationTypes");
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _ermService.UpdateEmployeeSeparationTypeAsync(separationType);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Separation Type was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "Sorry, an error was encountered. Please try again.";
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

        public async Task<IActionResult> DeleteSeparationType(int id)
        {
            try
            {
                if (id > 0)
                {
                    bool IsDeleted = await _ermService.DeleteEmployeeSeparationTypeAsync(id);
                    if (IsDeleted)
                    {
                        TempData["Success"] = "Record deleted successfully!";
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
            return RedirectToAction("SeparationTypes");
        }
        #endregion


        #region Separation Reasons Controller Actions

        public async Task<IActionResult> SeparationReasons()
        {
            SeparationReasonListViewModel model = new SeparationReasonListViewModel();
            var entities = await _ermService.GetEmployeeSeparationReasonsAsync();
            if (entities != null && entities.Count > 0)
            {
                model.SeparationReasonList = entities.ToList();
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

        public async Task<IActionResult> ManageSeparationReason(int id)
        {
            SeparationReasonViewModel model = new SeparationReasonViewModel();
            if (id > 0)
            {
                EmployeeSeparationReason separationReason = await _ermService.GetEmployeeSeparationReasonByIdAsync(id);
                if (separationReason != null && !string.IsNullOrWhiteSpace(separationReason.Description))
                {
                    model.Id = separationReason.Id;
                    model.Description = separationReason.Description;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageSeparationReason(SeparationReasonViewModel model)
        {
            try
            {
                EmployeeSeparationReason separationReason = new EmployeeSeparationReason();
                if (ModelState.IsValid)
                {
                    separationReason.Id = model.Id;
                    separationReason.Description = model.Description;

                    if (separationReason.Id < 1)
                    {
                        bool IsAdded = await _ermService.CreateEmployeeSeparationReasonAsync(separationReason);
                        if (IsAdded)
                        {
                            return RedirectToAction("SeparationReasons");
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _ermService.UpdateEmployeeSeparationReasonAsync(separationReason);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Separation Reason was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "Sorry, an error was encountered. Please try again.";
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

        public async Task<IActionResult> DeleteSeparationReason(int id)
        {
            try
            {
                if (id > 0)
                {
                    bool IsDeleted = await _ermService.DeleteEmployeeSeparationReasonAsync(id);
                    if (IsDeleted)
                    {
                        TempData["Success"] = "Record deleted successfully!";
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
            return RedirectToAction("SeparationReasons");
        }
        #endregion
    }
}