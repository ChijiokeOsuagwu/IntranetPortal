using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.BAMS.Models;
using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntranetPortal.Areas.BAMS.Controllers
{
    [Area("BAMS")]
    public class UpdatesController : Controller
    {
        private readonly IBamsManagerService _bamsManagerService;

        public UpdatesController(IBamsManagerService bamsManagerService)
        {
            _bamsManagerService = bamsManagerService;
        }

        [HttpGet]
        public async Task<IActionResult> List(int id)
        {
            AssignmentUpdatesListViewModel model = new AssignmentUpdatesListViewModel();
            model.AssignmentEventID = id;
            try
            {
                if (id > 0)
                {
                    var entities = await _bamsManagerService.GetAssignmentUpdatesByAssignmentEventIdAsync(id);
                    model.AssignmentUpdatesList = entities.ToList();
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult New(int id)
        {
            AssignmentUpdatesViewModel model = new AssignmentUpdatesViewModel();
            model.AssignmentEventID = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> New(AssignmentUpdatesViewModel model)
        {
            int assignmentEventId = 0;
            if (ModelState.IsValid)
            {
                assignmentEventId = model.AssignmentEventID;
                AssignmentUpdates assignmentUpdates = new AssignmentUpdates();
                try
                {
                    assignmentUpdates.UpdateTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                    assignmentUpdates.UpdateBy = HttpContext.User.Identity.Name;
                    assignmentUpdates.UpdateType = model.UpdateType;
                    assignmentUpdates.UpdateStatus = AssignmentUpdateStatus.Saved;
                    assignmentUpdates.UpdateDescription = model.UpdateDescription;
                    assignmentUpdates.AssignmentEventID = model.AssignmentEventID;

                    bool IsCreated = await _bamsManagerService.CreateAssignmentUpdateAsync(assignmentUpdates);
                    if (IsCreated)
                    {
                        model.UpdateDescription = string.Empty;
                        model.UpdateType = AssignmentUpdateType.Information;
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Assignment Update saved successfully!";
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Saving Update failed.";
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


        [HttpPost]
        public async Task<string> Delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    bool IsCreated = await _bamsManagerService.DeleteAssignmentUpdateAsync(id);
                    if (IsCreated)
                    {
                        return "done";
                    }
                    else
                    {
                        return "fail";
                    }
                }
                else
                {
                    return "none";
                }
            }
            catch (Exception)
            {
                return "fail";
            }
        }
    }
}