using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.BAMS.Models;
using IntranetPortal.Areas.PartnerServices.Models;
using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntranetPortal.Areas.BAMS.Controllers
{
    [Area("BAMS")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IBamsManagerService _bamsManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IBusinessManagerService _businessManagerService;
        public HomeController(IConfiguration configuration, ISecurityService securityService,
                        IBaseModelService baseModelService, IBamsManagerService bamsManagerService,
                        IGlobalSettingsService globalSettingsService, IBusinessManagerService businessManagerService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _bamsManagerService = bamsManagerService;
            _globalSettingsService = globalSettingsService;
            _businessManagerService = businessManagerService;
        }

        [Authorize(Roles = "BAMVWASSG, XYALLACCZ")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "BAMVWASSG, XYALLACCZ")]
        public async Task<IActionResult> AssignmentLog(int? yr = null, int? mn = null)
        {
            ActiveAssignmentListViewModel model = new ActiveAssignmentListViewModel();
            if (yr == null || yr < 2000) { yr = DateTime.Today.Year; }
            var entities = await _bamsManagerService.GetAssignmentsByYearAndMonthAsync(yr, mn);
            model.AssignmentEventList = entities.ToList();
            if (yr > 0) { ViewData["StartYear"] = yr.Value; }
            if (mn > 0) { ViewData["StartMonth"] = mn.Value; }

            return View(model);
        }

        
        [Authorize(Roles = "BAMMGASSG, XYALLACCZ")]
        public IActionResult NewAssignment(string id = null, string nm = null)
        {
            AssignmentEventViewModel model = new AssignmentEventViewModel();
            model.StartTime = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
            if (!string.IsNullOrWhiteSpace(id)) { model.CustomerID = id; }
            if (!string.IsNullOrWhiteSpace(nm)) { model.CustomerName = nm; }
            var statusList = _bamsManagerService.GetOnlyAssignmentStatusAsync().Result.ToList();
            ViewBag.StatusList = new SelectList(statusList, "ID", "Description");
            var eventTypes = _bamsManagerService.GetAssignmentEventTypesAsync().Result.ToList();
            ViewBag.EventTypesList = new SelectList(eventTypes, "ID", "Description");
            var locations = _globalSettingsService.GetStationsAsync().Result.ToList();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGASSG, XYALLACCZ")]
        public async Task<IActionResult> NewAssignment(AssignmentEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                AssignmentEvent assignment = new AssignmentEvent();

                try
                {
                    assignment = model.ConvertToAssignmentEvent();
                    assignment.CreatedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                    assignment.CreatedBy = HttpContext.User.Identity.Name;
                    assignment.ModifiedBy = HttpContext.User.Identity.Name;
                    assignment.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                    Business business = await _businessManagerService.GetCustomerByNameAsync(model.CustomerName);
                    assignment.CustomerID = business.BusinessID;

                    bool IsCreated = await _bamsManagerService.CreateAssignmentEventAsync(assignment);
                    if (IsCreated)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Assignment created successfully!";
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Creating New Assignment failed.";
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
            var statusList = _bamsManagerService.GetOnlyAssignmentStatusAsync().Result.ToList();
            ViewBag.StatusList = new SelectList(statusList, "ID", "Description");
            var eventTypes = _bamsManagerService.GetAssignmentEventTypesAsync().Result.ToList();
            ViewBag.EventTypesList = new SelectList(eventTypes, "ID", "Description");
            var locations = _globalSettingsService.GetStationsAsync().Result.ToList();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [Authorize(Roles = "BAMMGASSG, XYALLACCZ")]
        public async Task<IActionResult> EditAssignment(int id)
        {
            AssignmentEventViewModel model = new AssignmentEventViewModel();
            if(id > 0)
            {
                AssignmentEvent assignmentEvent = await _bamsManagerService.GetAssignmentEventByIdAsync(id);
                if(assignmentEvent != null)
                {
                    model.CreatedBy = assignmentEvent.CreatedBy;
                    model.CreatedTime = assignmentEvent.CreatedTime;
                    model.CustomerID = assignmentEvent.CustomerID;
                    model.CustomerName = assignmentEvent.CustomerName;
                    model.Description = assignmentEvent.Description;
                    model.EndTime = assignmentEvent.EndTime;
                    model.EventTypeID = assignmentEvent.EventTypeID;
                    model.EventTypeName = assignmentEvent.EventTypeName;
                    model.ID = assignmentEvent.ID;
                    model.IsPaid = assignmentEvent.IsPaid;
                    model.LiaisonName = assignmentEvent.LiaisonName;
                    model.LiaisonPhone = assignmentEvent.LiaisonPhone;
                    model.ModifiedBy = assignmentEvent.ModifiedBy;
                    model.ModifiedTime = assignmentEvent.ModifiedTime;
                    model.StartTime = assignmentEvent.StartTime;
                    model.State = assignmentEvent.State;
                    model.StationID = assignmentEvent.StationID;
                    model.StationName = assignmentEvent.StationName;
                    model.StatusDescription = assignmentEvent.StatusDescription;
                    model.StatusID = assignmentEvent.StatusID;
                    model.Title = assignmentEvent.Title;
                    model.Venue = assignmentEvent.Venue;
                }
                else
                {
                    model.ViewModelErrorMessage = $"Sorry, the selected record could not be found.";
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Sorry, your request could not be processed. The required parameter [ID] has invalid value.";
            }

            var statusList = _bamsManagerService.GetOnlyAssignmentStatusAsync().Result.ToList();
            ViewBag.StatusList = new SelectList(statusList, "ID", "Description");
            var eventTypes = _bamsManagerService.GetAssignmentEventTypesAsync().Result.ToList();
            ViewBag.EventTypesList = new SelectList(eventTypes, "ID", "Description");
            var locations = _globalSettingsService.GetStationsAsync().Result.ToList();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGASSG, XYALLACCZ")]
        public async Task<IActionResult> EditAssignment(AssignmentEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                AssignmentEvent assignment = new AssignmentEvent();

                try
                {
                    assignment = model.ConvertToAssignmentEvent();
                    assignment.ModifiedBy = HttpContext.User.Identity.Name;
                    assignment.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                    Business business = await _businessManagerService.GetCustomerByNameAsync(model.CustomerName);
                    assignment.CustomerID = business.BusinessID;

                    bool IsUpdated = await _bamsManagerService.UpdateAssignmentEventAsync(assignment);
                    if (IsUpdated)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Assignment updated successfully!";
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Updating Assignment failed.";
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
            var statusList = _bamsManagerService.GetOnlyAssignmentStatusAsync().Result.ToList();
            ViewBag.StatusList = new SelectList(statusList, "ID", "Description");
            var eventTypes = _bamsManagerService.GetAssignmentEventTypesAsync().Result.ToList();
            ViewBag.EventTypesList = new SelectList(eventTypes, "ID", "Description");
            var locations = _globalSettingsService.GetStationsAsync().Result.ToList();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [Authorize(Roles = "BAMVWASSG, BAMMGASSG, XYALLACCZ")]
        public async Task<IActionResult> AssignmentDetails(int id)
        {
            AssignmentEventViewModel model = new AssignmentEventViewModel();
            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }
            AssignmentEvent assignmentEvent = await _bamsManagerService.GetAssignmentEventByIdAsync(id);
            if (assignmentEvent != null)
            {
                model.CreatedBy = assignmentEvent.CreatedBy;
                model.CreatedTime = assignmentEvent.CreatedTime;
                model.CustomerID = assignmentEvent.CustomerID;
                model.CustomerName = assignmentEvent.CustomerName;
                model.Description = assignmentEvent.Description;
                model.EndTime = assignmentEvent.EndTime;
                model.EventTypeID = assignmentEvent.EventTypeID;
                model.EventTypeName = assignmentEvent.EventTypeName;
                model.ID = assignmentEvent.ID;
                model.IsPaid = assignmentEvent.IsPaid;
                model.LiaisonName = assignmentEvent.LiaisonName;
                model.LiaisonPhone = assignmentEvent.LiaisonPhone;
                model.ModifiedBy = assignmentEvent.ModifiedBy;
                model.ModifiedTime = assignmentEvent.ModifiedTime;
                model.StartTime = assignmentEvent.StartTime;
                model.State = assignmentEvent.State;
                model.StationID = assignmentEvent.StationID;
                model.StationName = assignmentEvent.StationName;
                model.StatusDescription = assignmentEvent.StatusDescription;
                model.StatusID = assignmentEvent.StatusID;
                model.Title = assignmentEvent.Title;
                model.Venue = assignmentEvent.Venue;
            }
            return View(model);
        }

        [Authorize(Roles = "BAMMGASSG, XYALLACCZ")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            AssignmentEventViewModel model = new AssignmentEventViewModel();
            AssignmentEvent assignmentEvent = await _bamsManagerService.GetAssignmentEventByIdAsync(id);
            if (assignmentEvent != null)
            {
                model.CreatedBy = assignmentEvent.CreatedBy;
                model.CreatedTime = assignmentEvent.CreatedTime;
                model.CustomerID = assignmentEvent.CustomerID;
                model.CustomerName = assignmentEvent.CustomerName;
                model.Description = assignmentEvent.Description;
                model.EndTime = assignmentEvent.EndTime;
                model.EventTypeID = assignmentEvent.EventTypeID;
                model.EventTypeName = assignmentEvent.EventTypeName;
                model.ID = assignmentEvent.ID;
                model.IsPaid = assignmentEvent.IsPaid;
                model.LiaisonName = assignmentEvent.LiaisonName;
                model.LiaisonPhone = assignmentEvent.LiaisonPhone;
                model.ModifiedBy = assignmentEvent.ModifiedBy;
                model.ModifiedTime = assignmentEvent.ModifiedTime;
                model.StartTime = assignmentEvent.StartTime;
                model.State = assignmentEvent.State;
                model.StationID = assignmentEvent.StationID;
                model.StationName = assignmentEvent.StationName;
                model.StatusDescription = assignmentEvent.StatusDescription;
                model.StatusID = assignmentEvent.StatusID;
                model.Title = assignmentEvent.Title;
                model.Venue = assignmentEvent.Venue;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGASSG, XYALLACCZ")]
        public async Task<IActionResult> DeleteAssignment(AssignmentEventViewModel model)
        {
            try
            {
                if (model != null && model.ID > 0)
                {
                    bool isDeleted = await _bamsManagerService.DeleteAssignmentEventAsync(model.ID.Value);
                    if (isDeleted)
                    {
                        return RedirectToAction("ActiveAssignments");
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Delete operation failed. An error was encountered.";
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = $"The required parameter [ID] has invalid value.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = $"Delete operation failed. An error was encountered. {ex.Message}";
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "BAMMGASSG, XYALLACCZ")]
        public async Task<IActionResult> NewCustomer()
        {
            BusinessPartnerViewModel model = new BusinessPartnerViewModel();
            model.IsCustomer = true;
            var industryTypes = await _baseModelService.GetIndustryTypesAsync();
            ViewBag.IndustryTypesList = new SelectList(industryTypes, "IndustryTypeName", "IndustryTypeName");
            var locations = await _globalSettingsService.GetStationsAsync();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGASSG, XYALLACCZ")]
        public async Task<IActionResult> NewCustomer(BusinessPartnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                Business business = new Business();
                Person person = new Person();
                BusinessContact contact = new BusinessContact();
                model.BusinessID = Guid.NewGuid().ToString();
                model.ContactID = Guid.NewGuid().ToString();
                try
                {
                    business = model.ConvertToBusiness();
                    business.IsCustomer = true;
                    business.CreatedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                    business.CreatedBy = HttpContext.User.Identity.Name;
                    business.ModifiedBy = HttpContext.User.Identity.Name;
                    business.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                    person = model.FromModel_RetrieveContactInfo();
                    contact = model.FromModel_RetrieveBusinessContact();

                    if (await _businessManagerService.CreateBusinessAsync(business))
                    {
                        if (!string.IsNullOrWhiteSpace(person.FullName))
                        {
                            bool PersonCreated = await _baseModelService.CreatePersonAsync(person);
                            if (PersonCreated)
                            {
                                contact.PersonID = person.PersonID;
                                contact.BusinessID = business.BusinessID;
                                contact.PersonRole = model.Designation;
                                if (await _businessManagerService.CreateBusinessContactAsync(contact))
                                {
                                    model.OperationIsCompleted = true;
                                    model.OperationIsSuccessful = true;
                                    model.ViewModelSuccessMessage = $"New Customer created successfully!";
                                }
                                else
                                {
                                    await _baseModelService.DeletePersonAsync(person.PersonID, person.CreatedBy, person.CreatedTime);
                                    await _businessManagerService.DeleteBusinessAsync(business.BusinessID);
                                    model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Creating New Customer failed.";
                                }
                            }
                            else
                            {
                                await _businessManagerService.DeleteBusinessAsync(business.BusinessID);
                                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Creating New Customer failed.";
                            }
                        }
                        else
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = $"New Customer created successfully!";
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Creating New Customer failed.";
                    }
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(person.PersonID))
                    {
                        await _baseModelService.DeletePersonAsync(person.PersonID, person.CreatedBy, person.CreatedTime);
                    }
                    if (!string.IsNullOrEmpty(business.BusinessID))
                    {
                        await _businessManagerService.DeleteBusinessAsync(business.BusinessID);
                    }

                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
            }
            var industryTypes = await _baseModelService.GetIndustryTypesAsync();
            ViewBag.IndustryTypesList = new SelectList(industryTypes, "IndustryTypeName", "IndustryTypeName");
            var locations = await _globalSettingsService.GetStationsAsync();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [Authorize(Roles = "BAMVWDPLS, BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> Deployments()
        {
            ActiveAssignmentListViewModel model = new ActiveAssignmentListViewModel();
            var entities = await _bamsManagerService.GetOpenAssignmentsAsync();
            model.AssignmentEventList = entities.ToList();
            return View(model);
        }
    }
}