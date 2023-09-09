using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.BAMS.Models;
using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntranetPortal.Areas.BAMS.Controllers
{
    [Area("BAMS")]
    public class ExtensionsController : Controller
    {
        private readonly IBamsManagerService _bamsManagerService;

        public ExtensionsController(IBamsManagerService bamsManagerService)
        {
            _bamsManagerService = bamsManagerService;
        }

        [Authorize(Roles = "BAMVWDPLS, BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> List(int id)
        {
            AssignmentExtensionListViewModel model = new AssignmentExtensionListViewModel();
            model.AssignmentEventID = id;
            try
            {
                if (id > 0)
                {
                    var entities = await _bamsManagerService.GetAssignmentExtensionsByAssignmentEventIdAsync(id);
                    model.AssignmentExtensionList = entities.ToList();
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [Authorize(Roles = "BAMVWDPLS, BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> Details(int id)
        {
            AssignmentExtensionViewModel model = new AssignmentExtensionViewModel();
            AssignmentExtension assignmentExtension = await _bamsManagerService.GetAssignmentExtensionByIdAsync(id);
            if (assignmentExtension != null)
            {
                model.CreatedBy = assignmentExtension.CreatedBy;
                model.CreatedTime = assignmentExtension.CreatedTime;
                model.AssignmentEventID = assignmentExtension.AssignmentEventID.Value;
                model.AssignmentEventTitle = assignmentExtension.AssignmentEventTitle;
                model.AssignmentExtensionID = assignmentExtension.AssignmentExtensionID;
                model.ExtensionReason = assignmentExtension.ExtensionReason;
                model.ExtensionType = assignmentExtension.ExtensionType;
                model.FromTime = assignmentExtension.FromTime;
                model.ToTime = assignmentExtension.ToTime;
                model.CreatedBy = assignmentExtension.CreatedBy;
                model.CreatedTime = assignmentExtension.CreatedTime;
                model.FromTimeFormatted = assignmentExtension.FromTimeFormatted;
                model.ToTimeFormatted = assignmentExtension.ToTimeFormatted;
            }
            return View(model);
        }

        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> New(int id)
        {
            AssignmentExtensionViewModel model = new AssignmentExtensionViewModel();
            model.AssignmentEventID = id;
            AssignmentEvent assignmentEvent = await _bamsManagerService.GetAssignmentEventByIdAsync(id);
            if(assignmentEvent != null)
            {
                model.AssignmentEventTitle = assignmentEvent.Title;
                model.FromTime = await _bamsManagerService.GetAssignmentEventClosingTime(id);
            }
            model.ToTime = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> New(AssignmentExtensionViewModel model)
        {
            if (ModelState.IsValid)
            {
                AssignmentExtension assignmentExtension = new AssignmentExtension();
                try
                {
                    if (model != null)
                    {
                        assignmentExtension = model.ConvertToAssignmentExtension();
                        assignmentExtension.CreatedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                        assignmentExtension.CreatedBy = HttpContext.User.Identity.Name;

                        bool IsCreated = await _bamsManagerService.CreateAssignmentExtensionAsync(assignmentExtension);
                        if (IsCreated)
                        {
                            return RedirectToAction("List", new { id = model.AssignmentEventID });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Event extension failed.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
            }
            return View(model);
        }

        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> Delete(int id)
        {
            AssignmentExtensionViewModel model = new AssignmentExtensionViewModel();
            AssignmentExtension assignmentExtension = await _bamsManagerService.GetAssignmentExtensionByIdAsync(id);
            if (assignmentExtension != null)
            {
                model.AssignmentExtensionID = assignmentExtension.AssignmentExtensionID;
                model.CreatedBy = assignmentExtension.CreatedBy;
                model.CreatedTime = assignmentExtension.CreatedTime;
                model.FromTimeFormatted = assignmentExtension.FromTimeFormatted;
                model.ToTimeFormatted = assignmentExtension.ToTimeFormatted;
                model.AssignmentEventID = assignmentExtension.AssignmentEventID.Value;
                model.AssignmentEventTitle = assignmentExtension.AssignmentEventTitle;
                model.ExtensionType = assignmentExtension.ExtensionType;
                model.ExtensionReason = assignmentExtension.ExtensionReason;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> Delete(AssignmentExtensionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model != null)
                    {
                        bool IsCreated = await _bamsManagerService.DeleteAssignmentExtensionAsync(model.AssignmentExtensionID.Value);
                        if (IsCreated)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = $"Extension deleted successfully!";
                            return RedirectToAction("List", new { id = model.AssignmentEventID});
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry, an error was encountered. Deleting Deployment Batch failed.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
            }
            return View(model);
        }
    }
}