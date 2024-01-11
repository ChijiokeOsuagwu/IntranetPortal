using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntranetPortal.Areas.AssetManager.Models;
using IntranetPortal.Areas.BAMS.Models;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using SelectPdf;

namespace IntranetPortal.Areas.BAMS.Controllers
{
    [Area("BAMS")]
    [Authorize]
    public class DeploymentController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IBamsManagerService _bamsManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IBusinessManagerService _businessManagerService;
        private readonly IAssetManagerService _assetManagerService;
        private readonly IErmService _employeeRecordService;

        public DeploymentController(IConfiguration configuration, ISecurityService securityService,
                        IBaseModelService baseModelService, IBamsManagerService bamsManagerService,
                        IGlobalSettingsService globalSettingsService, IBusinessManagerService businessManagerService,
                        IAssetManagerService assetManagerService, IErmService employeeRecordService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _bamsManagerService = bamsManagerService;
            _globalSettingsService = globalSettingsService;
            _businessManagerService = businessManagerService;
            _assetManagerService = assetManagerService;
            _employeeRecordService = employeeRecordService;
        }

        #region Assignment Deployment Action Methods

        [Authorize(Roles = "BAMVWDPLS, BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> List(int id)
        {
            AssignmentDeploymentListViewModel model = new AssignmentDeploymentListViewModel();
            model.AssignmentEventID = id;
            if (id > 0)
            {
                var entities = await _bamsManagerService.GetAssignmentDeploymentsByAssignmentEventIdAsync(id);
                model.AssignmentDeploymentList = entities.ToList();
            }
            return View(model);
        }

        [Authorize(Roles = "BAMVWDPLS, BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> AssignmentDetails(int id)
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

        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public IActionResult NewDeployment(int id)
        {
            AssignmentDeploymentViewModel model = new AssignmentDeploymentViewModel();
            model.AssignmentEventID = id;
            model.DepartureTime = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
            model.ArrivalTime = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy HH:mm"));

            var statusList = _bamsManagerService.GetOnlyDeploymentStatusAsync().Result.ToList();
            ViewBag.StatusList = new SelectList(statusList, "ID", "Description");
            var locations = _globalSettingsService.GetStationsAsync().Result.ToList();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> NewDeployment(AssignmentDeploymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                AssignmentDeployment deployment = new AssignmentDeployment();
                try
                {
                    if (model != null)
                    {
                        deployment = model.ConvertToAssignmentDeployment();
                        deployment.CreatedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                        deployment.CreatedBy = HttpContext.User.Identity.Name;
                        deployment.ModifiedBy = HttpContext.User.Identity.Name;
                        deployment.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                        bool IsCreated = await _bamsManagerService.CreateAssignmentDeploymentAsync(deployment);
                        if (IsCreated)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = $"New Deployment created successfully!";
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Creating New Deployment failed.";
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
            var statusList = _bamsManagerService.GetOnlyDeploymentStatusAsync().Result.ToList();
            ViewBag.StatusList = new SelectList(statusList, "ID", "Description");
            var locations = _globalSettingsService.GetStationsAsync().Result.ToList();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> EditDeployment(int id)
        {
            AssignmentDeploymentViewModel model = new AssignmentDeploymentViewModel();
            if (id > 0)
            {
                AssignmentDeployment assignmentDeployment = await _bamsManagerService.GetAssignmentDeploymentByIdAsync(id);
                if (assignmentDeployment != null)
                {
                    model.CreatedBy = assignmentDeployment.CreatedBy;
                    model.CreatedTime = assignmentDeployment.CreatedTime;
                    model.ModifiedBy = assignmentDeployment.ModifiedBy;
                    model.ModifiedTime = assignmentDeployment.ModifiedTime;
                    model.StationID = assignmentDeployment.StationID;
                    model.StationName = assignmentDeployment.StationName;
                    model.StatusDescription = assignmentDeployment.StatusDescription;
                    model.StatusID = assignmentDeployment.StatusID;
                    model.ArrivalTime = assignmentDeployment.ArrivalTime;
                    model.AssignmentEventDescription = assignmentDeployment.AssignmentEventDescription;
                    model.AssignmentEventID = assignmentDeployment.AssignmentEventID;
                    model.AssignmentEventTitle = assignmentDeployment.AssignmentEventTitle;
                    model.DepartureTime = assignmentDeployment.DepartureTime;
                    model.DeploymentID = assignmentDeployment.DeploymentID;
                    model.DeploymentTitle = assignmentDeployment.DeploymentTitle;
                    model.TeamLeadName = assignmentDeployment.TeamLeadName;
                    model.TeamLeadPhone = assignmentDeployment.TeamLeadPhone;
                }
                else
                {
                    model.ViewModelErrorMessage = "Sorry, the item was not found. Or it may have been deleted.";
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Sorry, the item could not be retrieved. Key parameter [DeploymentID] has invalid value.";
            }

            var statusList = _bamsManagerService.GetOnlyDeploymentStatusAsync().Result.ToList();
            ViewBag.StatusList = new SelectList(statusList, "ID", "Description");
            var locations = _globalSettingsService.GetStationsAsync().Result.ToList();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> EditDeployment(AssignmentDeploymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                AssignmentDeployment deployment = new AssignmentDeployment();
                try
                {
                    if (model != null)
                    {
                        deployment = model.ConvertToAssignmentDeployment();
                        deployment.ModifiedBy = HttpContext.User.Identity.Name;
                        deployment.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                        bool IsCreated = await _bamsManagerService.UpdateAssignmentDeploymentAsync(deployment);
                        if (IsCreated)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = $"Deployment updated successfully!";
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry, an error was encountered. Editing Deployment failed.";
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
            var statusList = _bamsManagerService.GetOnlyDeploymentStatusAsync().Result.ToList();
            ViewBag.StatusList = new SelectList(statusList, "ID", "Description");
            var locations = _globalSettingsService.GetStationsAsync().Result.ToList();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [Authorize(Roles = "BAMVWDPLS, BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> DeploymentDetails(int id)
        {
            AssignmentDeploymentViewModel model = new AssignmentDeploymentViewModel();
            AssignmentDeployment assignmentDeployment = await _bamsManagerService.GetAssignmentDeploymentByIdAsync(id);
            if (assignmentDeployment != null)
            {
                model.CreatedBy = assignmentDeployment.CreatedBy;
                model.CreatedTime = assignmentDeployment.CreatedTime;
                model.ModifiedBy = assignmentDeployment.ModifiedBy;
                model.ModifiedTime = assignmentDeployment.ModifiedTime;
                model.StationID = assignmentDeployment.StationID;
                model.StationName = assignmentDeployment.StationName;
                model.StatusDescription = assignmentDeployment.StatusDescription;
                model.StatusID = assignmentDeployment.StatusID;

                model.ArrivalTime = assignmentDeployment.ArrivalTime;
                model.AssignmentEventDescription = assignmentDeployment.AssignmentEventDescription;
                model.AssignmentEventID = assignmentDeployment.AssignmentEventID;
                model.AssignmentEventTitle = assignmentDeployment.AssignmentEventTitle;
                model.DepartureTime = assignmentDeployment.DepartureTime;
                model.DeploymentID = assignmentDeployment.DeploymentID;
                model.DeploymentTitle = assignmentDeployment.DeploymentTitle;
                model.TeamLeadName = assignmentDeployment.TeamLeadName;
                model.TeamLeadPhone = assignmentDeployment.TeamLeadPhone;
            }
            return View(model);
        }

        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> DeleteDeployment(int id)
        {
            AssignmentDeploymentViewModel model = new AssignmentDeploymentViewModel();
            AssignmentDeployment assignmentDeployment = await _bamsManagerService.GetAssignmentDeploymentByIdAsync(id);
            if (assignmentDeployment != null)
            {
                model.CreatedBy = assignmentDeployment.CreatedBy;
                model.CreatedTime = assignmentDeployment.CreatedTime;
                model.ModifiedBy = assignmentDeployment.ModifiedBy;
                model.ModifiedTime = assignmentDeployment.ModifiedTime;
                model.StationID = assignmentDeployment.StationID;
                model.StationName = assignmentDeployment.StationName;
                model.StatusDescription = assignmentDeployment.StatusDescription;
                model.StatusID = assignmentDeployment.StatusID;

                model.ArrivalTime = assignmentDeployment.ArrivalTime;
                model.AssignmentEventDescription = assignmentDeployment.AssignmentEventDescription;
                model.AssignmentEventID = assignmentDeployment.AssignmentEventID;
                model.AssignmentEventTitle = assignmentDeployment.AssignmentEventTitle;
                model.DepartureTime = assignmentDeployment.DepartureTime;
                model.DeploymentID = assignmentDeployment.DeploymentID;
                model.DeploymentTitle = assignmentDeployment.DeploymentTitle;
                model.TeamLeadName = assignmentDeployment.TeamLeadName;
                model.TeamLeadPhone = assignmentDeployment.TeamLeadPhone;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> DeleteDeployment(AssignmentDeploymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model != null)
                    {
                        bool teamDeleted = await _bamsManagerService.DeleteDeploymentTeamMembersByDeploymentIdAsync(model.DeploymentID);
                        bool IsCreated = await _bamsManagerService.DeleteAssignmentDeploymentAsync(model.DeploymentID);
                        if (IsCreated)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = $"Deployment deleted successfully!";
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
            var statusList = _bamsManagerService.GetOnlyDeploymentStatusAsync().Result.ToList();
            ViewBag.StatusList = new SelectList(statusList, "ID", "Description");
            var locations = _globalSettingsService.GetStationsAsync().Result.ToList();
            ViewBag.LocationsList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }
        #endregion

        #region Deployment Teams Action Methods

        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> SelectTeamBatch(int? id)
        {
            AssignmentDeploymentListViewModel model = new AssignmentDeploymentListViewModel();
            if (id.HasValue)
            {
                if (id > 0)
                {
                    var entities = await _bamsManagerService.GetAssignmentDeploymentsByAssignmentEventIdAsync(id.Value);
                    model.AssignmentDeploymentList = entities.ToList();
                    model.AssignmentEventID = id.Value;
                }
            }
            var assignments = await _bamsManagerService.GetOpenAssignmentsAsync();
            ViewBag.AssignmentEventList = new SelectList(assignments.ToList(), "ID", "Title");

            return View(model);
        }

        [Authorize(Roles = "BAMVWDPLS, BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> DeploymentTeam(int dd, int ad)
        {
            DeploymentTeamMemberViewModel model = new DeploymentTeamMemberViewModel();
            List<DeploymentTeamMember> teamMembers = new List<DeploymentTeamMember>();
            if (dd > 0 && ad > 0)
            {
                model.DeploymentID = dd;
                model.AssignmentEventID = ad;

                var entities = await _bamsManagerService.GetDeploymentTeamMembersByDeploymentIdAsync(dd);
                teamMembers = entities.ToList();
            }

            ViewBag.TeamMembersList = teamMembers;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<string> AddTeamMember(int ad, int dd, string mn, string mr)
        {
            if (ad < 1 || dd < 1 || string.IsNullOrWhiteSpace(mn) || string.IsNullOrWhiteSpace(mr))
            {
                return "missing";
            }
            try
            {
                DeploymentTeamMember teamMember = new DeploymentTeamMember();
                teamMember.TeamMemberRole = mr;
                teamMember.AssignmentEventID = ad;
                teamMember.DeploymentID = dd;
                teamMember.ModifiedBy = HttpContext.User.Identity.Name;
                teamMember.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                teamMember.CreatedBy = HttpContext.User.Identity.Name;
                teamMember.CreatedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                Person person = await _baseModelService.GetPersonbyNameAsync(mn);
                if (person != null && !string.IsNullOrWhiteSpace(person.PersonID))
                {
                    teamMember.TeamMemberID = person.PersonID;

                    var existing_entities = await _bamsManagerService.GetDeploymentTeamMembersByAssignmentEventIdAndPersonIdAsync(ad, person.PersonID);

                    if (existing_entities == null || existing_entities.Count > 0)
                    {
                        return "exist"; //model.ViewModelWarningMessage = "Sorry, this member has already been deployed for this Assignment.";
                    }
                    else
                    {
                        Employee employee = await _employeeRecordService.GetEmployeeByIdAsync(person.PersonID);
                        if (employee != null)
                        {
                            teamMember.TeamMemberUnit = employee.UnitName;
                            teamMember.TeamMemberStation = employee.LocationName;
                        }
                        bool IsAdded = await _bamsManagerService.CreateDeploymentTeamMemberAsync(teamMember);
                        if (IsAdded)
                        {
                            return "done";
                        }
                        else
                        {
                            return "error";//model.ViewModelErrorMessage = "Sorry, an error was encountered. New team member could not be added.";
                        }
                    }
                }
                else
                {
                    return "none"; //model.ViewModelErrorMessage = "Sorry, no record was found for the name you entered. Team Member was not added.";
                }
            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public string DeleteTeamMember(int td)
        {
            if (td > 0)
            {
                bool IsDeleted = _bamsManagerService.DeleteDeploymentTeamMemberAsync(td).Result;
                if (IsDeleted) { return "done"; } else { return "error"; }
            }
            else
            {
                return "none";
            }
        }


        [Authorize(Roles = "BAMVWDPLS, BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> AssignmentTeamList(int ed, int dd)
        {
            DeploymentTeamMemberViewModel model = new DeploymentTeamMemberViewModel();
            List<DeploymentTeamMember> teamMembers = new List<DeploymentTeamMember>();
            if (dd > 0 && ed > 0)
            {
                model.DeploymentID = dd;
                model.AssignmentEventID = ed;
                var entities = await _bamsManagerService.GetDeploymentTeamMembersByDeploymentIdAsync(dd);
                teamMembers = entities.ToList();
            }

            StringBuilder sb = new StringBuilder();
            if (teamMembers == null || teamMembers.Count < 1)
            {
                sb.Append("<div></div>");
            }
            else
            {
                int serialNo = 0;
                sb.Append("<!DOCTYPE html><html><head><style>.main-container{ font-family: verdana, sans-serif; ");
                sb.Append("font-size:14px; padding:20px;} table {border-collapse:collapse; width:98%;} ");
                sb.Append("td, th {border:1px solid #dddddd; text-align:left; padding:8px;} </style></head>");
                sb.Append("<body><div class=\"main-container\"><h3>Assignment Team List</h3><hr/>");
                sb.Append($"<h5>Event: {teamMembers.FirstOrDefault().AssignmentEventTitle}</h5>");
                sb.Append($"<h5>Batch: {teamMembers.FirstOrDefault().DeploymentTitle}</h5><hr/>");
                sb.Append($"<table><tr bgcolor=\"#dddddd\"><th>#</th><th>Name</th><th>Role</th>");
                sb.Append($"<th>Unit</th><th>Station</th></tr>");
                foreach (var item in teamMembers)
                {
                    serialNo++;
                    sb.Append($"<tr><td>{serialNo}</td><td>{item.TeamMemberName}</td><td>{item.TeamMemberRole}</td>");
                    sb.Append($"<td>{item.TeamMemberUnit}</td><td>{item.TeamMemberStation}</td></tr>");
                }
                sb.Append("</table></div><div style=\"text-align:center; font-size:14px\">This document is auto-generated by OfficeManager application.</div></body></html>");
            }
            var stdView = new HtmlToPdf();
            stdView.Options.WebPageWidth = 1024;
            var pdf = stdView.ConvertHtmlString(sb.ToString());
            var pdfbytes = pdf.Save();

            return File(pdfbytes, "application/pdf");
        }
        #endregion

        #region Deployment Equipment Action Methods

        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> SelectEquipmentBatch(int? id)
        {
            AssignmentDeploymentListViewModel model = new AssignmentDeploymentListViewModel();
            if (id.HasValue)
            {
                if (id > 0)
                {
                    var entities = await _bamsManagerService.GetAssignmentDeploymentsByAssignmentEventIdAsync(id.Value);
                    model.AssignmentDeploymentList = entities.ToList();
                    model.AssignmentEventID = id.Value;
                }
            }
            var assignments = await _bamsManagerService.GetOpenAssignmentsAsync();
            ViewBag.AssignmentEventList = new SelectList(assignments.ToList(), "ID", "Title");

            return View(model);
        }

        [Authorize(Roles = "BAMVWDPLS, BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> DeploymentEquipment(int dd, int ad)
        {
            DeploymentEquipmentViewModel model = new DeploymentEquipmentViewModel();
            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }

            if (TempData["WarningMessage"] != null)
            {
                model.ViewModelWarningMessage = TempData["WarningMessage"].ToString();
            }

            if (TempData["SuccessMessage"] != null)
            {
                model.ViewModelSuccessMessage = TempData["SuccessMessage"].ToString();
            }

            List<DeploymentEquipment> equipments = new List<DeploymentEquipment>();
            if (dd > 0 && ad > 0)
            {
                model.DeploymentID = dd;
                model.AssignmentEventID = ad;

                var entities = await _bamsManagerService.GetDeploymentEquipmentsByDeploymentIdAsync(dd);
                equipments = entities.ToList();
            }

            ViewBag.EquipmentsList = equipments;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> DeploymentEquipment(DeploymentEquipmentViewModel model)
        {
            List<DeploymentEquipment> equipments = new List<DeploymentEquipment>();
            if (ModelState.IsValid)
            {
                try
                {
                    DeploymentEquipment equipment = model.ConvertToDeploymentEquipment();
                    equipment.ModifiedBy = HttpContext.User.Identity.Name;
                    equipment.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                    equipment.CreatedBy = HttpContext.User.Identity.Name;
                    equipment.CreatedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";

                    Asset asset = await _assetManagerService.GetAssetByNameAsync(model.AssetName);
                    if (asset != null && !string.IsNullOrWhiteSpace(asset.AssetID))
                    {
                        equipment.AssetID = asset.AssetID;
                        equipment.AssetTypeID = asset.AssetTypeID;
                        equipment.EquipmentDeploymentStatus = asset.UsageStatus;
                        string assetTypeName = asset.AssetTypeName;
                        var existing_entities = await _bamsManagerService.GetDeploymentEquipmentsByAssignmentEventIdAndAssetIdAsync(model.AssignmentEventID, asset.AssetID);

                        if (existing_entities != null && existing_entities.Count > 0)
                        {
                            model.ViewModelWarningMessage = $"Sorry, this {assetTypeName} has already been deployed for this Assignment.";
                        }
                        else
                        {
                            var checkedout_entities = await _assetManagerService.GetAssetUsagesCheckedOutByAssetIdAsync(asset.AssetID);
                            if (checkedout_entities != null && checkedout_entities.Count > 0)
                            {
                                string reason = checkedout_entities.FirstOrDefault().Purpose;
                                model.ViewModelWarningMessage = $"Sorry, this {assetTypeName} is currently Checked Out for {reason}. If you are sure it is currently not in use kindly Check it In before you proceed.";
                            }
                            else
                            {
                                TempData["EventID"] = model.AssignmentEventID;
                                TempData["BatchID"] = model.DeploymentID;

                                //Redirect To Equipment Check Out Form
                                return RedirectToAction("DeploymentCheckOut", new { id = asset.AssetID });
                            }
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, no record was found for the equipment you entered. Equipment was not added.";
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
            var entities = await _bamsManagerService.GetDeploymentEquipmentsByDeploymentIdAsync(model.DeploymentID);
            equipments = entities.ToList();
            ViewBag.EquipmentsList = equipments;
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> DeploymentCheckOut(string id)
        {
            AssetUsageViewModel model = new AssetUsageViewModel();
            int AssignmentEventID = 0;
            AssignmentEvent assignmentEvent = new AssignmentEvent();
            int DeploymentID = 0;
            try
            {
                if (TempData["BatchID"] != null)
                {
                    DeploymentID = Convert.ToInt32(TempData["BatchID"]);
                    model.DeploymentID = DeploymentID;
                }

                if (TempData["EventID"] != null)
                {
                    AssignmentEventID = Convert.ToInt32(TempData["EventID"]);
                    model.AssignmentEventID = AssignmentEventID;
                    assignmentEvent = await _bamsManagerService.GetAssignmentEventByIdAsync(AssignmentEventID);
                    if (assignmentEvent != null && !string.IsNullOrWhiteSpace(assignmentEvent.Title))
                    {
                        model.UsageStartTime = assignmentEvent.StartTime;
                        model.UsageEndTime = assignmentEvent.EndTime;
                        model.Purpose = "Assignment";
                        model.UsageDescription = assignmentEvent.Description;
                        model.UsageLocation = assignmentEvent.Venue;
                    }
                }

                Asset asset = await _assetManagerService.GetAssetByIdAsync(id);
                model.AssetDescription = asset.AssetDescription;
                model.AssetID = asset.AssetID;
                model.AssetName = asset.AssetName;
                model.AssetTypeID = asset.AssetTypeID;
                model.AssetTypeName = asset.AssetTypeName;
                model.CheckedOutFromLocation = asset.CurrentLocation == null || asset.CurrentLocation == "" ? asset.BaseLocationName : asset.CurrentLocation;
                model.CheckOutCondition = asset.ConditionStatus;
                model.UsageStartTime = DateTime.Now;
                model.UsageEndTime = DateTime.Now;

                switch (asset.ConditionStatus)
                {
                    case AssetCondition.BeyondRepair:
                        model.CheckOutConditionFormatted = "Beyond Repair";
                        break;
                    case AssetCondition.InGoodCondition:
                        model.CheckOutConditionFormatted = "In Good Condition";
                        break;
                    case AssetCondition.RequiresRepair:
                        model.CheckOutConditionFormatted = "Requires Repair";
                        break;
                    default:
                        model.CheckOutConditionFormatted = "Unspecified";
                        break;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            var teamMembers = await _bamsManagerService.GetDeploymentTeamMembersByAssignmentEventIdAsync(model.AssignmentEventID.Value);
            ViewBag.TeamMembersList = new SelectList(teamMembers.ToList(), "TeamMemberName", "TeamMemberName");

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<IActionResult> DeploymentCheckOut(AssetUsageViewModel model)
        {
            if (ModelState.IsValid)
            {
                Asset asset = new Asset();
                try
                {
                    if (model.AssetTypeID < 1 || string.IsNullOrEmpty(model.AssetID))
                    {
                        asset = await _assetManagerService.GetAssetByNameAsync(model.AssetName);
                    }
                    else
                    {
                        asset = await _assetManagerService.GetAssetByIdAsync(model.AssetID);
                    }

                    AssetUsage assetUsage = model.ConvertToAssetUsage();
                    assetUsage.ModifiedBy = HttpContext.User.Identity.Name;
                    assetUsage.ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + UTC";
                    assetUsage.AssetID = asset.AssetID;
                    assetUsage.AssetTypeID = asset.AssetTypeID;
                    assetUsage.CheckedOutTime = DateTime.Now;
                    assetUsage.CheckedOutBy = HttpContext.User.Identity.Name;
                    assetUsage.CheckStatus = "Checked Out";


                    if (await _assetManagerService.CheckOutEquipmentAsync(assetUsage))
                    {
                        DeploymentEquipment equipmentDeployed = new DeploymentEquipment();
                        equipmentDeployed.AssetID = asset.AssetID;
                        equipmentDeployed.AssetTypeID = asset.AssetTypeID;
                        equipmentDeployed.AssignmentEventID = model.AssignmentEventID.Value;
                        equipmentDeployed.DeploymentID = model.DeploymentID.Value;
                        equipmentDeployed.PreviousAvailabilityStatus = asset.UsageStatus;
                        equipmentDeployed.PreviousLocation = asset.CurrentLocation;

                        var usageList = await _assetManagerService.GetAssetUsagesByAssetIdAsync(asset.AssetID);
                        AssetUsage equipmentUsage = usageList.FirstOrDefault();
                        if (equipmentUsage != null && equipmentUsage.UsageID > 0)
                        {
                            equipmentDeployed.EquipmentUsageID = equipmentUsage.UsageID;
                        }

                        bool IsAdded = await _bamsManagerService.CreateDeploymentEquipmentAsync(equipmentDeployed);
                        if (IsAdded)
                        {
                            TempData["SuccessMessage"] = "New Equipment was added and Checked Out Successfully!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Sorry, an error was encountered. New equipment could not be added.";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Error! An error was encountered. Equipment Check Out failed.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = $"Ooops! It appears some fields in the Check Out form have missing or invalid values. Please correct this and try again.";
            }
            return RedirectToAction("DeploymentEquipment", new { dd = model.DeploymentID, ad = model.AssignmentEventID });
        }

        [Authorize(Roles = "BAMMGDPLS, XYALLACCZ")]
        public async Task<string> DeleteEquipment(int qd)
        {
            DeploymentEquipment deployedEquipment = new DeploymentEquipment();
            string modifiedBy = HttpContext.User.Identity.Name;
            if (qd > 0)
            {
                deployedEquipment = await _bamsManagerService.GetDeploymentEquipmentByIdAsync(qd);
                if (deployedEquipment == null || string.IsNullOrWhiteSpace(deployedEquipment.AssetID))
                { return "none"; }
                int assetUsageId = default;
                if (deployedEquipment.EquipmentUsageID.HasValue) { assetUsageId = deployedEquipment.EquipmentUsageID.Value; }
                string previousLocation = deployedEquipment.PreviousLocation;
                string previousAvailabilityStatus = deployedEquipment.PreviousAvailabilityStatus;
                string equipmentId = deployedEquipment.AssetID;
                bool checkOutIsCancelled = false;
                bool deploymentIsDeleted = await _bamsManagerService.DeleteDeploymentEquipmentAsync(qd);
                if (deploymentIsDeleted)
                {
                    checkOutIsCancelled = await _assetManagerService.CancelCheckOutEquipmentAsync(assetUsageId, equipmentId, deployedEquipment.PreviousLocation, previousAvailabilityStatus, modifiedBy);
                    return "done";
                }
                else { return "error"; }
            }
            else
            {
                return "none";
            }
        }

        #endregion
    }
}