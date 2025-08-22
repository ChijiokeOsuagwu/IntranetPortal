using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.PartnerServices.Models;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntranetPortal.Areas.PartnerServices.Controllers
{
    [Area("PartnerServices")]
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IBusinessManagerService _businessManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        public CustomersController(IConfiguration configuration, ISecurityService securityService,
                        IBaseModelService baseModelService, IBusinessManagerService businessManagerService,
                        IGlobalSettingsService globalSettingsService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _businessManagerService = businessManagerService;
            _globalSettingsService = globalSettingsService;
        }

        [Authorize(Roles = "BPSVWCUSR, BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> List(string cn = null)
        {
            BusinessesListViewModel model = new BusinessesListViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(cn))
                {
                    model.BusinessList = await _businessManagerService.SearchCustomersByNameAsync(cn);
                }
                else
                {
                    model.BusinessList = await _businessManagerService.GetCustomersAsync();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> AddCustomer()
        {
            BusinessPartnerViewModel model = new BusinessPartnerViewModel();
            model.IsCustomer = true;
            model.BusinessNumber = await _businessManagerService.GetNewCodeNumber();
            var industrySectors = await _baseModelService.GetIndustrySectorsAsync();
            ViewBag.IndustrySectorsList = new SelectList(industrySectors, "IndustrySectorId", "IndustrySectorName");
            var businessTypes = await _baseModelService.GetBusinessTypesAsync();
            ViewBag.BusinessTypesList = new SelectList(businessTypes, "BusinessTypeId", "BusinessTypeName");
            var locations = await _globalSettingsService.GetStationsAsync();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            var states = await _globalSettingsService.GetStatesAsync();
            ViewBag.StatesList = new SelectList(states, "Name", "Name");

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> AddCustomer(BusinessPartnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                Business business = new Business();
                BusinessContact contact = new BusinessContact();
                model.BusinessID = Guid.NewGuid().ToString();
                try
                {
                    business = model.ConvertToBusiness();
                    business.IsCustomer = true;
                    business.CreatedTime = DateTime.Now;
                    business.CreatedBy = HttpContext.User.Identity.Name;
                    business.ModifiedBy = HttpContext.User.Identity.Name;
                    business.ModifiedTime = DateTime.Now;
                    contact = model.FromModel_RetrieveBusinessContact();

                    if (await _businessManagerService.CreateBusinessAsync(business))
                    {
                        if (!string.IsNullOrWhiteSpace(contact.ContactName))
                        {
                            contact.BusinessID = business.BusinessID;
                            contact.Designation = model.ContactDesignation;
                            if (await _businessManagerService.CreateBusinessContactAsync(contact))
                            {
                                return RedirectToAction("List");
                            }
                            else
                            {
                                await _businessManagerService.DeleteBusinessAsync(business.BusinessID);
                                model.ViewModelErrorMessage = "Error! Sorry, an error was encountered. Creating New Customer failed.";
                            }
                        }
                        else
                        {
                            return RedirectToAction("List");
                        }
                    }
                    else
                    {
                        await _businessManagerService.DeleteBusinessAsync(business.BusinessID);
                        model.ViewModelErrorMessage = "Error! Sorry, an error was encountered. Creating New Customer failed.";
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

            var industrySectors = await _baseModelService.GetIndustrySectorsAsync();
            ViewBag.IndustrySectorsList = new SelectList(industrySectors, "IndustrySectorId", "IndustrySectorName");
            var businessTypes = await _baseModelService.GetBusinessTypesAsync();
            ViewBag.BusinessTypesList = new SelectList(businessTypes, "BusinessTypeId", "BusinessTypeName");
            var locations = await _globalSettingsService.GetStationsAsync();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            var states = await _globalSettingsService.GetStatesAsync();
            ViewBag.StatesList = new SelectList(states, "Name", "Name");

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> Edit(string id)
        {
            BusinessPartnerViewModel model = new BusinessPartnerViewModel();
            Business business = new Business();
            if (!string.IsNullOrWhiteSpace(id))
            {
                model.BusinessID = id;
                business = await _businessManagerService.GetCustomerByIdAsync(id);
                model.BusinessAddress = business.BusinessAddress;
                //model.BusinessID = business.BusinessID;
                model.BusinessName = business.BusinessName;
                model.BusinessNumber = business.BusinessNumber;
                model.BusinessStationID = business.BusinessStationId;
                model.BusinessType = business.BusinessType;
                model.BusinessTypeID = business.BusinessTypeId;
                model.IndustrySectorID = business.IndustrySectorId;
                model.IndustrySector = business.IndustrySector;
                model.Country = business.Country;
                model.Email1 = business.Email1;
                model.Email2 = business.Email2;
                model.ImagePath = business.ImagePath;
                model.PhoneNo1 = business.PhoneNo1;
                model.PhoneNo2 = business.PhoneNo2;
                model.State = business.State;
                model.WebLink1 = business.WebLink1;
                model.WebLink2 = business.WebLink2;
            }
            else
            {
                return RedirectToAction("AddCustomer");
            }

            var industrySectors = await _baseModelService.GetIndustrySectorsAsync();
            ViewBag.IndustrySectorsList = new SelectList(industrySectors, "IndustrySectorId", "IndustrySectorName");
            var businessTypes = await _baseModelService.GetBusinessTypesAsync();
            ViewBag.BusinessTypesList = new SelectList(businessTypes, "BusinessTypeId", "BusinessTypeName");
            var locations = await _globalSettingsService.GetStationsAsync();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            var states = await _globalSettingsService.GetStatesAsync();
            ViewBag.StatesList = new SelectList(states, "Name", "Name");

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> Edit(BusinessPartnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                Business business = new Business();
                try
                {
                    business = model.ConvertToBusiness();
                    business.ModifiedBy = HttpContext.User.Identity.Name;
                    business.ModifiedTime = DateTime.Now;

                    if (await _businessManagerService.UpdateBusinessAsync(business))
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "Customer updated successfully!";
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Sorry, an error was encountered. Updating customer failed.";
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

            var industrySectors = await _baseModelService.GetIndustrySectorsAsync();
            ViewBag.IndustrySectorsList = new SelectList(industrySectors, "IndustrySectorId", "IndustrySectorName");
            var businessTypes = await _baseModelService.GetBusinessTypesAsync();
            ViewBag.BusinessTypesList = new SelectList(businessTypes, "BusinessTypeId", "BusinessTypeName");
            var locations = await _globalSettingsService.GetStationsAsync();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            var states = await _globalSettingsService.GetStatesAsync();
            ViewBag.StatesList = new SelectList(states, "Name", "Name");

            return View(model);
        }

        [Authorize(Roles = "BPSVWCUSR, BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> Details(string id)
        {
            BusinessPartnerViewModel model = new BusinessPartnerViewModel();
            Business business = await _businessManagerService.GetCustomerByIdAsync(id);
            model.BusinessAddress = business.BusinessAddress;
            model.BusinessID = business.BusinessID;
            model.BusinessName = business.BusinessName;
            model.BusinessNumber = business.BusinessNumber;
            model.BusinessStationID = business.BusinessStationId;
            model.BusinessStationName = business.BusinessStationName;
            model.BusinessTypeID = business.BusinessTypeId;
            model.BusinessType = business.BusinessType;
            model.IndustrySectorID = business.IndustrySectorId;
            model.IndustrySector = business.IndustrySector;
            model.Country = business.Country;
            model.Email1 = business.Email1;
            model.Email2 = business.Email2;
            model.ImagePath = business.ImagePath;
            model.PhoneNo1 = business.PhoneNo1;
            model.PhoneNo2 = business.PhoneNo2;
            model.State = business.State;
            model.WebLink1 = business.WebLink1;
            model.WebLink2 = business.WebLink2;

            return View(model);
        }

        [Authorize(Roles = "BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> Delete(string id)
        {
            BusinessPartnerViewModel model = new BusinessPartnerViewModel();
            Business business = await _businessManagerService.GetCustomerByIdAsync(id);
            model.BusinessAddress = business.BusinessAddress;
            model.BusinessID = business.BusinessID;
            model.BusinessName = business.BusinessName;
            model.BusinessNumber = business.BusinessNumber;
            model.BusinessStationID = business.BusinessStationId;
            model.BusinessStationName = business.BusinessStationName;
            model.BusinessType = business.BusinessType;
            model.Country = business.Country;
            model.Email1 = business.Email1;
            model.Email2 = business.Email2;
            model.ImagePath = business.ImagePath;
            model.PhoneNo1 = business.PhoneNo1;
            model.PhoneNo2 = business.PhoneNo2;
            model.State = business.State;
            model.WebLink1 = business.WebLink1;
            model.WebLink2 = business.WebLink2;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> Delete(BusinessPartnerViewModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.BusinessID))
                {
                    bool linkContactIsDeleted = await _businessManagerService.DeleteBusinessContactsAsync(model.BusinessID);
                    if (linkContactIsDeleted)
                    {
                        if (await _businessManagerService.DeleteBusinessAsync(model.BusinessID))
                        {
                            return RedirectToAction("List");
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry, delete operation failed.";
                            return View(model);
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Sorry, delete operation failed. Key parameter has an invalid value.";
                        return View(model);
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = $"Sorry, delete operation failed. Key parameter has an invalid value.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
                return View(model);
            }
        }

        [Authorize(Roles = "BPSVWCUSR, BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> Contacts(string sp = null)
        {
            ContactListViewModel model = new ContactListViewModel();
            model.sp = sp;
            try
            {
                if (!string.IsNullOrEmpty(sp))
                {
                    var entities = await _businessManagerService.GetBusinessContactsByBusinessIdAsync(sp);
                    if (entities != null) { model.ContactsList = entities; }
                }
                else
                {
                    model.ContactsList = null;
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                model.ContactsList = null;
            }
            var customers = await _businessManagerService.GetCustomersAsync();
            ViewBag.CustomersList = new SelectList(customers, "BusinessID", "BusinessName");
            return View(model);
        }

        [Authorize(Roles = "BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> AddContact(string id)
        {
            ContactViewModel model = new ContactViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                model.BusinessID = id;
                Business business = await _businessManagerService.GetCustomerByIdAsync(id);
                if (business != null)
                {
                    model.BusinessName = business.BusinessName;
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> AddContact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                Business business = new Business();
                BusinessContact contact = new BusinessContact();
                try
                {
                    if (string.IsNullOrWhiteSpace(model.BusinessID) && !string.IsNullOrWhiteSpace(model.BusinessName))
                    {
                        business = await _businessManagerService.GetCustomerByNameAsync(model.BusinessName);
                        model.BusinessID = business.BusinessID;
                    }

                    contact = model.ConvertToBusinessContact();

                    if (!string.IsNullOrWhiteSpace(model.ContactName))
                    {
                        if (await _businessManagerService.CreateBusinessContactAsync(contact))
                        {
                            return RedirectToAction("Contacts", new { sp = model.BusinessID });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "Sorry, an error was encountered. Creating New Contact failed.";
                        }
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

        [Authorize(Roles = "BPSMGCUSR, XYALLACCZ")]
        public IActionResult EditContact(int id)
        {
            ContactViewModel model = new ContactViewModel();
            if (id > 0)
            {
                model.ContactID = id;
                BusinessContact businessContact = _businessManagerService.GetBusinessContactByIdAsync(id).Result;
                if (businessContact != null)
                {
                    model.BusinessName = businessContact.BusinessName;
                    model.ContactAddress = businessContact.ContactAddress;
                    model.BusinessID = businessContact.BusinessID;
                    model.ContactEmail1 = businessContact.ContactEmail1;
                    model.ContactEmail2 = businessContact.ContactEmail2;
                    model.ContactName = businessContact.ContactName;
                    model.ContactDesignation = businessContact.Designation;
                    model.ContactPhone1 = businessContact.ContactPhone1;
                    model.ContactPhone2 = businessContact.ContactPhone2;
                    model.ContactSex = businessContact.Sex;
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> EditContact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                Person person = new Person();
                BusinessContact businessContact = new BusinessContact();
                try
                {
                    businessContact = model.ConvertToBusinessContact();
                    if (await _businessManagerService.UpdateBusinessContactAsync(businessContact))
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "Contact updated successfully!";
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Updating Contact failed.";
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
                model.ViewModelErrorMessage = "Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        [Authorize(Roles = "BPSVWCUSR, BPSMGCUSR, XYALLACCZ")]
        public IActionResult ContactDetails(long id)
        {
            ContactViewModel model = new ContactViewModel();
            if (id > 0)
            {
                model.ContactID = id;
                BusinessContact businessContact = _businessManagerService.GetBusinessContactByIdAsync(id).Result;
                model.BusinessName = businessContact.BusinessName;
                model.ContactAddress = businessContact.ContactAddress;
                model.BusinessID = businessContact.BusinessID;
                model.ContactEmail1 = businessContact.ContactEmail1;
                model.ContactEmail2 = businessContact.ContactEmail2;

                model.ContactName = businessContact.ContactName;
                model.ContactDesignation = businessContact.Designation;
                model.ContactPhone1 = businessContact.ContactPhone1;
                model.ContactPhone2 = businessContact.ContactPhone2;
                model.ContactSex = businessContact.Sex;
            }
            return View(model);
        }

        [Authorize(Roles = "BPSMGCUSR, XYALLACCZ")]
        public IActionResult DeleteContact(long id)
        {
            ContactViewModel model = new ContactViewModel();
            if (id > 0)
            {
                model.ContactID = id;
                BusinessContact businessContact = _businessManagerService.GetBusinessContactByIdAsync(id).Result;
                model.BusinessName = businessContact.BusinessName;
                model.ContactAddress = businessContact.ContactAddress;
                model.BusinessID = businessContact.BusinessID;
                model.ContactEmail1 = businessContact.ContactEmail1;
                model.ContactEmail2 = businessContact.ContactEmail2;
                model.ContactName = businessContact.ContactName;
                model.ContactDesignation = businessContact.Designation;
                model.ContactPhone1 = businessContact.ContactPhone1;
                model.ContactPhone2 = businessContact.ContactPhone2;
                model.ContactSex = businessContact.Sex;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BPSMGCUSR, XYALLACCZ")]
        public async Task<IActionResult> DeleteContact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                long businessContactId = model.ContactID;
                string businessId = model.BusinessID;
                try
                {
                    bool businessContactIsDeleted = await _businessManagerService.DeleteBusinessContactAsync(businessContactId);
                    if (businessContactIsDeleted)
                    {
                        return RedirectToAction("Contacts", new { sp = businessId });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Contact could not be deleted.";
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

        //===== Customers Helper Methods =====//
        #region Customers Helper Methods
        [HttpGet]
        public JsonResult GetCustomerNames(string customerName)
        {
            List<string> customers = _businessManagerService.SearchCustomersByNameAsync(customerName).Result.Select(x => x.BusinessName).ToList();
            return Json(customers);
        }
        #endregion
    }
}