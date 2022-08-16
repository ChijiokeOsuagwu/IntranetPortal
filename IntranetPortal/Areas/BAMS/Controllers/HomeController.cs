using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.BAMS.Models;
using IntranetPortal.Areas.PartnerServices.Models;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntranetPortal.Areas.BAMS.Controllers
{
    [Area("BAMS")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
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

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ActiveAssignments()
        {
            ActiveAssignmentListViewModel model = new ActiveAssignmentListViewModel();
            var entities = await _bamsManagerService.GetOpenAssignmentsAsync();
            model.AssignmentEventList = entities.ToList();
            return View(model);
        }

        public IActionResult NewAssignment(string id = null, string nm = null)
        {
            AssignmentEventViewModel model = new AssignmentEventViewModel();
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

        [HttpGet]
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
                                    await _baseModelService.DeletePersonAsync(person.PersonID);
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
                        await _baseModelService.DeletePersonAsync(person.PersonID);
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


    }
}