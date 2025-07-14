using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.ATS.Models;
using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntranetPortal.Areas.ATS.Controllers
{
    [Area("ATS")]
    public class HomeController : Controller
    {
        private readonly IAssignmentService _assignmentService;

        public HomeController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

        #region Assignment Event Types Controller Actions

        [Authorize(Roles = "ATSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> AssignmentEventTypes()
        {
            AssignmentEventTypesListViewModel model = new AssignmentEventTypesListViewModel();
            try
            {
                var entities = await _assignmentService.GetAssignmentEventTypesAsync();
                if (entities != null && entities.Count > 0)
                {
                    model.AssignmentEventTypeList = entities.ToList();
                }
            }
            catch(Exception ex)
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
        public async Task<IActionResult> ManageEventType(int id)
        {
            ManageEventTypeViewModel model = new ManageEventTypeViewModel();
            try
            {
                if (id > 0)
                {
                    AssignmentEventType eventType = await _assignmentService.GetAssignmentEventTypeAsync(id, null);
                    if (eventType != null && !string.IsNullOrWhiteSpace(eventType.Description))
                    {
                        model.Id = eventType.Id;
                        model.Description = eventType.Description;
                    }
                }
            }
            catch(Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        
        [Authorize(Roles = "ATSSTTMGA, XYALLACCZ")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageEventType(ManageEventTypeViewModel model)
        {
            try
            {
                AssignmentEventType eventType = new AssignmentEventType();
                if (ModelState.IsValid)
                {
                    eventType.Id = model.Id ?? 0;
                    eventType.Description = model.Description;

                    if (eventType.Id < 1)
                    {
                        long newId = await _assignmentService.CreateAssignmentEventTypeAsync(eventType);
                        if (newId > 0)
                        {
                            return RedirectToAction("AssignmentEventTypes");
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _assignmentService.UpdateAssignmentEventTypeAsync(eventType);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Assignment Event Type was updated successfully!";
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
        public async Task<IActionResult> DeleteEventType(int id)
        {
            try
            {
                if (id > 0)
                {
                    bool IsDeleted = await _assignmentService.DeleteAssignmentEventTypeAsync(id);
                    if (IsDeleted)
                    {
                        TempData["Success"] = "Event Type deleted successfully!";
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
            return RedirectToAction("AssignmentEventTypes");
        }

        #endregion


        #region Assignment Roles Controller Actions

        [Authorize(Roles = "ATSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> AssignmentRoles()
        {
            AssignmentRolesListViewModel model = new AssignmentRolesListViewModel();
            try
            {
                var entities = await _assignmentService.GetAssignmentRolesAsync();
                if (entities != null && entities.Count > 0)
                {
                    model.AssignmentRoleList = entities.ToList();
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
        public async Task<IActionResult> ManageAssignmentRole(int id)
        {
            ManageAssignmentRoleViewModel model = new ManageAssignmentRoleViewModel();
            try
            {
                if (id > 0)
                {
                    AssignmentRole role = await _assignmentService.GetAssignmentRoleAsync(id, null);
                    if (role != null && !string.IsNullOrWhiteSpace(role.Description))
                    {
                        model.Id = role.Id;
                        model.Description = role.Description;
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
        public async Task<IActionResult> ManageAssignmentRole(ManageAssignmentRoleViewModel model)
        {
            try
            {
                AssignmentRole role = new AssignmentRole();
                if (ModelState.IsValid)
                {
                    role.Id = model.Id ?? 0;
                    role.Description = model.Description;

                    if (role.Id < 1)
                    {
                        long newId = await _assignmentService.CreateAssignmentRoleAsync(role);
                        if (newId > 0)
                        {
                            return RedirectToAction("AssignmentRoles");
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _assignmentService.UpdateAssignmentRoleAsync(role);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Assignment Role was updated successfully!";
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
        public async Task<IActionResult> DeleteAssignmentRole(int id)
        {
            try
            {
                if (id > 0)
                {
                    bool IsDeleted = await _assignmentService.DeleteAssignmentRoleAsync(id);
                    if (IsDeleted)
                    {
                        TempData["Success"] = "Assignment Role deleted successfully!";
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
            return RedirectToAction("AssignmentRoles");
        }

        #endregion

    }
}