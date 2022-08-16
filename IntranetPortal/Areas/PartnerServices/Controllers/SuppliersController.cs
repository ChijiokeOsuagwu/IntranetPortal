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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntranetPortal.Areas.PartnerServices.Controllers
{
    [Area("PartnerServices")]
    public class SuppliersController : Controller
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IBusinessManagerService _businessManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        public SuppliersController(IConfiguration configuration, ISecurityService securityService,
                        IBaseModelService baseModelService, IBusinessManagerService businessManagerService,
                        IGlobalSettingsService globalSettingsService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _businessManagerService = businessManagerService;
            _globalSettingsService = globalSettingsService;
        }

        public async Task<IActionResult> List(string sp = null, int? pg = null)
        {
            IEnumerable<Business> businessList = new List<Business>();
            try
            {
                if (string.IsNullOrEmpty(sp))
                {
                    businessList = await _businessManagerService.GetSuppliersAsync();
                }
                else
                {
                    businessList = await _businessManagerService.SearchSuppliersByNameAsync(sp);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                businessList = null;
            }
            ViewData["CurrentFilter"] = ViewBag.sp = sp;
            ViewBag.pg = pg;

            return View(PaginatedList<Business>.CreateAsync(businessList.AsQueryable(), pg ?? 1, 100));
        }

        [HttpGet]
        public async Task<IActionResult> AddSupplier()
        {
            BusinessPartnerViewModel model = new BusinessPartnerViewModel();
            model.IsSupplier = true;
            var industryTypes = await _baseModelService.GetIndustryTypesAsync();
            ViewBag.IndustryTypesList = new SelectList(industryTypes, "IndustryTypeName", "IndustryTypeName");
            var locations = await _globalSettingsService.GetStationsAsync();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier(BusinessPartnerViewModel model)
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
                    business.IsSupplier = true;
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
                                    model.ViewModelSuccessMessage = $"New Supplier created successfully!";
                                }
                                else
                                {
                                    await _baseModelService.DeletePersonAsync(person.PersonID);
                                    await _businessManagerService.DeleteBusinessAsync(business.BusinessID);
                                    model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Creating New Supplier failed.";
                                }
                            }
                            else
                            {
                                await _businessManagerService.DeleteBusinessAsync(business.BusinessID);
                                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Creating New Supplier failed.";
                            }
                        }
                        else
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = $"New Supplier created successfully!";
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Creating New Supplier failed.";
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
                    model.OperationIsCompleted = true;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            var industryTypes = await _baseModelService.GetIndustryTypesAsync();
            ViewBag.IndustryTypesList = new SelectList(industryTypes, "IndustryTypeName", "IndustryTypeName");
            var locations = await _globalSettingsService.GetStationsAsync();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            BusinessPartnerViewModel model = new BusinessPartnerViewModel();
            Business business = new Business();
            if (!string.IsNullOrWhiteSpace(id))
            {
                business = await _businessManagerService.GetCustomerByIdAsync(id);
                model.BusinessAddress = business.BusinessAddress;
                model.BusinessID = business.BusinessID;
                model.BusinessName = business.BusinessName;
                model.BusinessNumber = business.BusinessNumber;
                model.BusinessStationID = business.BusinessStationID;
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
            }
            else
            {
                return RedirectToAction("AddSupplier");
            }

            var industryTypes = await _baseModelService.GetIndustryTypesAsync();
            ViewBag.IndustryTypesList = new SelectList(industryTypes, "IndustryTypeName", "IndustryTypeName");
            var locations = await _globalSettingsService.GetStationsAsync();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BusinessPartnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                Business business = new Business();
                try
                {
                    business = model.ConvertToBusiness();
                    business.ModifiedBy = HttpContext.User.Identity.Name;
                    business.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                    if (await _businessManagerService.UpdateBusinessAsync(business))
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Supplier updated successfully!";
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Sorry, an error was encountered. Updating supplier failed.";
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
            var industryTypes = await _baseModelService.GetIndustryTypesAsync();
            ViewBag.IndustryTypesList = new SelectList(industryTypes, "IndustryTypeName", "IndustryTypeName");
            var locations = await _globalSettingsService.GetStationsAsync();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            BusinessPartnerViewModel model = new BusinessPartnerViewModel();
            Business business = await _businessManagerService.GetSupplierByIdAsync(id);
            model.BusinessAddress = business.BusinessAddress;
            model.BusinessID = business.BusinessID;
            model.BusinessName = business.BusinessName;
            model.BusinessNumber = business.BusinessNumber;
            model.BusinessStationID = business.BusinessStationID;
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

        public async Task<IActionResult> Delete(string id)
        {
            BusinessPartnerViewModel model = new BusinessPartnerViewModel();
            Business business = await _businessManagerService.GetSupplierByIdAsync(id);
            model.BusinessAddress = business.BusinessAddress;
            model.BusinessID = business.BusinessID;
            model.BusinessName = business.BusinessName;
            model.BusinessNumber = business.BusinessNumber;
            model.BusinessStationID = business.BusinessStationID;
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

        public async Task<IActionResult> Contacts(string sp = null)
        {
            ContactListViewModel model = new ContactListViewModel();
            model.sp = sp;
            try
            {
                if (!string.IsNullOrEmpty(sp))
                {
                    var entities = await _businessManagerService.GetBusinessContactsByBusinessIdAsync(sp);
                    if (entities != null) { model.ContactsList = entities.ToList(); }
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
            var suppliers = await _businessManagerService.GetSuppliersAsync();
            ViewBag.SuppliersList = new SelectList(suppliers, "BusinessID", "BusinessName");
            return View(model);
        }

        public IActionResult AddContact(string id)
        {
            ContactViewModel model = new ContactViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                model.BusinessID = id;
                Business business = _businessManagerService.GetSupplierByIdAsync(id).Result;
                model.BusinessName = business.BusinessName;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddContact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                Person person = new Person();
                Business business = new Business();
                BusinessContact contact = new BusinessContact();
                business = await _businessManagerService.GetSupplierByNameAsync(model.BusinessName);
                model.BusinessID = business.BusinessID;

                model.PersonID = Guid.NewGuid().ToString();
                try
                {
                    person = model.ConvertToPerson();
                    person.CreatedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                    person.CreatedBy = HttpContext.User.Identity.Name;
                    person.ModifiedBy = HttpContext.User.Identity.Name;
                    person.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                    contact = model.ConvertToBusinessContact();

                    if (!string.IsNullOrWhiteSpace(person.FullName))
                    {
                        bool PersonCreated = await _baseModelService.CreatePersonAsync(person);
                        if (PersonCreated)
                        {
                            if (await _businessManagerService.CreateBusinessContactAsync(contact))
                            {
                                model.OperationIsCompleted = true;
                                model.OperationIsSuccessful = true;
                                model.ViewModelSuccessMessage = $"New Contact created successfully!";
                            }
                            else
                            {
                                await _baseModelService.DeletePersonAsync(person.PersonID);
                                model.ViewModelErrorMessage = $"Sorry, an error was encountered. Creating New Contact failed.";
                            }
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry, an error was encountered. Creating New Contact failed.";
                        }
                    }
                    else
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Contact created successfully!";
                    }
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(person.PersonID))
                    {
                        await _baseModelService.DeletePersonAsync(person.PersonID);
                    }
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

        public IActionResult EditContact(int id)
        {
            ContactViewModel model = new ContactViewModel();
            if (id > 0)
            {
                model.BusinessContactID = id;
                BusinessContact businessContact = _businessManagerService.GetBusinessContactByIdAsync(id).Result;
                model.BusinessName = businessContact.BusinessName;
                model.Address = businessContact.Address;
                model.BirthDay = businessContact.BirthDay;
                model.BirthMonth = businessContact.BirthMonth;
                model.BirthYear = businessContact.BirthYear;
                model.BusinessID = businessContact.BusinessID;
                model.Email = businessContact.Email;
                model.FirstName = businessContact.FirstName;
                model.FullName = $"{businessContact.Title} {businessContact.FirstName} {businessContact.OtherNames} {businessContact.Surname}";
                model.ImagePath = businessContact.ImagePath;
                model.MaritalStatus = businessContact.MaritalStatus;
                model.PersonID = businessContact.PersonID;
                model.PersonRole = businessContact.PersonRole;
                model.PhoneNo1 = businessContact.PhoneNo1;
                model.PhoneNo2 = businessContact.PhoneNo2;
                model.Sex = businessContact.Sex;
                model.Surname = businessContact.Surname;
                model.Title = businessContact.Title;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditContact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                Person person = new Person();
                BusinessContact businessContact = new BusinessContact();
                try
                {
                    person = model.ConvertToPerson();
                    person.ModifiedBy = HttpContext.User.Identity.Name;
                    person.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                    bool PersonCreated = await _baseModelService.UpdatePersonAsync(person);
                    if (PersonCreated)
                    {
                        businessContact = model.ConvertToBusinessContact();
                        if (await _businessManagerService.UpdateBusinessContactAsync(businessContact))
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = $"Contact updated successfully!";
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry, an error was encountered. Contact Designation could not be updated.";
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Sorry, an error was encountered. Updating Contact failed.";
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

        public IActionResult ContactDetails(int id)
        {
            ContactViewModel model = new ContactViewModel();
            if (id > 0)
            {
                model.BusinessContactID = id;
                BusinessContact businessContact = _businessManagerService.GetBusinessContactByIdAsync(id).Result;
                model.BusinessName = businessContact.BusinessName;
                model.Address = businessContact.Address;
                model.BirthDay = businessContact.BirthDay;
                model.BirthMonth = businessContact.BirthMonth;
                model.BirthYear = businessContact.BirthYear;
                model.BusinessID = businessContact.BusinessID;
                model.Email = businessContact.Email;
                model.FirstName = businessContact.FirstName;
                model.FullName = $"{businessContact.Title} {businessContact.FirstName} {businessContact.OtherNames} {businessContact.Surname}";
                model.ImagePath = businessContact.ImagePath;
                model.MaritalStatus = businessContact.MaritalStatus;
                model.PersonID = businessContact.PersonID;
                model.PersonRole = businessContact.PersonRole;
                model.PhoneNo1 = businessContact.PhoneNo1;
                model.PhoneNo2 = businessContact.PhoneNo2;
                model.Sex = businessContact.Sex;
                model.Surname = businessContact.Surname;
                model.Title = businessContact.Title;
                if (model.BirthDay > 0 && model.BirthMonth > 0)
                {
                    int birthYear = model.BirthYear == null ? 1900 : model.BirthYear.Value;
                    DateTime birthDate = new DateTime(model.BirthYear ?? 1900, model.BirthMonth.Value, model.BirthDay.Value);
                    model.DateOfBirth = $"{model.BirthDay} {birthDate.ToString("MMM", CultureInfo.InvariantCulture)} {model.BirthYear}";
                }
            }
            return View(model);
        }

        public IActionResult DeleteContact(int id)
        {
            ContactViewModel model = new ContactViewModel();
            if (id > 0)
            {
                model.BusinessContactID = id;
                BusinessContact businessContact = _businessManagerService.GetBusinessContactByIdAsync(id).Result;
                model.BusinessName = businessContact.BusinessName;
                model.Address = businessContact.Address;
                model.BirthDay = businessContact.BirthDay;
                model.BirthMonth = businessContact.BirthMonth;
                model.BirthYear = businessContact.BirthYear;
                model.BusinessID = businessContact.BusinessID;
                model.Email = businessContact.Email;
                model.FirstName = businessContact.FirstName;
                model.FullName = $"{businessContact.Title} {businessContact.FirstName} {businessContact.OtherNames} {businessContact.Surname}";
                model.ImagePath = businessContact.ImagePath;
                model.MaritalStatus = businessContact.MaritalStatus;
                model.PersonID = businessContact.PersonID;
                model.PersonRole = businessContact.PersonRole;
                model.PhoneNo1 = businessContact.PhoneNo1;
                model.PhoneNo2 = businessContact.PhoneNo2;
                model.Sex = businessContact.Sex;
                model.Surname = businessContact.Surname;
                model.Title = businessContact.Title;
                if (model.BirthDay > 0 && model.BirthMonth > 0)
                {
                    int birthYear = model.BirthYear == null ? 1900 : model.BirthYear.Value;
                    DateTime birthDate = new DateTime(model.BirthYear ?? 1900, model.BirthMonth.Value, model.BirthDay.Value);
                    model.DateOfBirth = $"{model.BirthDay} {birthDate.ToString("MMM", CultureInfo.InvariantCulture)} {model.BirthYear}";
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteContact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                int businessContactId = model.BusinessContactID;
                string personId = model.PersonID;
                string businessId = model.BusinessID;
                try
                {
                    bool businessContactIsDeleted = await _businessManagerService.DeleteBusinessContactAsync(businessContactId);
                    if (businessContactIsDeleted)
                    {
                        if (await _baseModelService.DeletePersonAsync(personId))
                        {
                            return RedirectToAction("Contacts", new { sp = businessId });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry, an error was encountered. Contact could not be deleted.";
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Sorry, an error was encountered. Contact could not be deleted.";
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

        //======================================= Suppliers Helper Methods ===========================================//
        #region Suppliers Helper Methods
        [HttpGet]
        public JsonResult GetSupplierNames(string supplierName)
        {
            List<string> suppliers = _businessManagerService.SearchSuppliersByNameAsync(supplierName).Result.Select(x => x.BusinessName).ToList();
            return Json(suppliers);
        }
        #endregion
    }
}