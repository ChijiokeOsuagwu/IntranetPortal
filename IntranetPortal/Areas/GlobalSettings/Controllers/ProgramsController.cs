using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.GlobalSettings.Models;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.GlobalSettings.Controllers
{
    [Area("GlobalSettings")]
    [Authorize]
    public class ProgramsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _employeeRecordService;
        private readonly IDataProtector _dataProtector;
        public ProgramsController(IConfiguration configuration,
                                    IGlobalSettingsService globalSettingsService, IDataProtectionProvider dataProtectionProvider,
                                    IErmService employeeRecordService, DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _configuration = configuration;
            _globalSettingsService = globalSettingsService;
            _employeeRecordService = employeeRecordService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        //================================= Programs Action Methods =====================================================================//
        #region Program Action Methods

        [Authorize(Roles = "GBSVWASTT, GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> List(string tp, string bt, string st)
        {
            ProgramListViewModel model = new ProgramListViewModel();
            model.ProgramList = new List<Programme>();
            var entities = await _globalSettingsService.SearchProgramsAsync(tp,bt,st);
            if(entities != null) { model.ProgramList = entities.ToList();}

            var belt_entities = await _globalSettingsService.GetProgrammeBeltsAsync();
            if(belt_entities != null) 
            {
                ViewBag.ProgramBeltList = new SelectList(belt_entities, "Name", "Name", bt);
            }
            
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> ManageProgram(int id)
        {
            ManageProgramViewModel model = new ManageProgramViewModel();
            if(id > 0)
            {
                Programme programme = await _globalSettingsService.GetProgramAsync(id);
                if(programme != null)
                {
                    model.EndTimeFormatted = programme.EndTime;
                    model.Frequency = programme.Frequency;
                    model.HostStationID = programme.HostStationId ?? 0;
                    model.HostStationName = programme.HostStationName;
                    model.Platform = programme.Platform;
                    model.ProgramBelt = programme.ProgramBelt;
                    model.ProgramCode = programme.Code;
                    model.ProgramDescription = programme.Description;
                    model.ProgramID = programme.Id;
                    model.ProgramTitle = programme.Title;
                    model.ProgramType = programme.ProgramType;
                    model.StartTimeFormatted = programme.StartTime;
                    model.Status = programme.Status;
                    model.StartTime = DateTime.Parse(programme.StartTime);
                    model.EndTime = DateTime.Parse(programme.EndTime);
                }
            }

            var belt_entities = await _globalSettingsService.GetProgrammeBeltsAsync();
            if (belt_entities != null)
            {
                ViewBag.ProgramBeltList = new SelectList(belt_entities, "Name", "Name");
            }

            var platform_entities = await _globalSettingsService.GetProgramPlatformsAsync();
            if (platform_entities != null)
            {
                ViewBag.ProgramPlatformList = new SelectList(platform_entities, "Name", "Name");
            }

            var location_entities = await _globalSettingsService.GetStationsAsync();
            if(location_entities != null)
            {
                ViewBag.StationList = new SelectList(location_entities, "LocationID", "LocationName");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> ManageProgram(ManageProgramViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Programme programme = model.ConvertToProgram();
                    bool succeeded = false;
                    if(model.ProgramID < 1)
                    {
                       succeeded = await _globalSettingsService.CreateProgramAsync(programme);
                        if (succeeded)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = $"New Programme was created successfully!";
                        }
                    }
                    else
                    {
                        succeeded = await _globalSettingsService.UpdateProgramAsync(programme);
                        if (succeeded)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = $"Programme was updated successfully!";
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

            var belt_entities = await _globalSettingsService.GetProgrammeBeltsAsync();
            if (belt_entities != null)
            {
                ViewBag.ProgramBeltList = new SelectList(belt_entities, "Name", "Name");
            }

            var platform_entities = await _globalSettingsService.GetProgramPlatformsAsync();
            if (platform_entities != null)
            {
                ViewBag.ProgramPlatformList = new SelectList(platform_entities, "Name", "Name");
            }

            var location_entities = await _globalSettingsService.GetStationsAsync();
            if (location_entities != null)
            {
                ViewBag.StationList = new SelectList(location_entities, "LocationID", "LocationName");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Delete(int? id)
        {
            Programme programme = new Programme();
            ManageProgramViewModel model = new ManageProgramViewModel();
            if (id > 0)
            {
                programme = await _globalSettingsService.GetProgramAsync(id.Value);
                if (programme != null)
                {
                    model.EndTimeFormatted = programme.EndTime;
                    model.Frequency = programme.Frequency;
                    model.HostStationID = programme.HostStationId ?? 0;
                    model.HostStationName = programme.HostStationName;
                    model.Platform = programme.Platform;
                    model.ProgramBelt = programme.ProgramBelt;
                    model.ProgramCode = programme.Code;
                    model.ProgramDescription = programme.Description;
                    model.ProgramID = programme.Id;
                    model.ProgramTitle = programme.Title;
                    model.ProgramType = programme.ProgramType;
                    model.StartTimeFormatted = programme.StartTime;
                    model.Status = programme.Status;
                    model.StartTime = DateTime.Parse(programme.StartTime);
                    model.EndTime = DateTime.Parse(programme.EndTime);
                }
            }
            else
            {
                return RedirectToAction("ManageProgram", "Programs");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Delete(ManageProgramViewModel model)
        {
            if (model != null)
            {
                try
                {
                    bool succeeded = await _globalSettingsService.DeleteProgramAsync(model.ProgramID);
                    if (succeeded)
                    {
                        return RedirectToAction("List");
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

        [HttpGet]
        [Authorize(Roles = "GBSVWASTT, GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Details(int? id)
        {
            Programme programme = new Programme();
            ManageProgramViewModel model = new ManageProgramViewModel();
            if (id > 0)
            {
                programme = await _globalSettingsService.GetProgramAsync(id.Value);
                if (programme != null)
                {
                    model.EndTimeFormatted = programme.EndTime;
                    model.Frequency = programme.Frequency;
                    model.HostStationID = programme.HostStationId ?? 0;
                    model.HostStationName = programme.HostStationName;
                    model.Platform = programme.Platform;
                    model.ProgramBelt = programme.ProgramBelt;
                    model.ProgramCode = programme.Code;
                    model.ProgramDescription = programme.Description;
                    model.ProgramID = programme.Id;
                    model.ProgramTitle = programme.Title;
                    model.ProgramType = programme.ProgramType;
                    model.StartTimeFormatted = programme.StartTime;
                    model.Status = programme.Status;
                    model.StartTime = DateTime.Parse(programme.StartTime);
                    model.EndTime = DateTime.Parse(programme.EndTime);
                }
            }
            else
            {
                return RedirectToAction("ManageProgram", "Programs");
            }
            return View(model);
        }

        #endregion
    }
}