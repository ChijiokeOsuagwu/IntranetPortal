using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IntranetPortal.Areas.ATS.Models;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IntranetPortal.Areas.ATS.Controllers
{
    [Area("ATS")]
    public class AssignmentsController : Controller
    {
        private readonly IAssignmentService _assignmentService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IBaseModelService _baseModelService;
        private readonly IErmService _employeeService;
        private readonly IAssetManagerService _assetManagerService;

        public AssignmentsController(IAssignmentService assignmentService, IGlobalSettingsService globalSettingsService,
            IBaseModelService baseModelService, IErmService employeeService, IAssetManagerService assetManagerService)
        {
            _globalSettingsService = globalSettingsService;
            _assignmentService = assignmentService;
            _baseModelService = baseModelService;
            _employeeService = employeeService;
            _assetManagerService = assetManagerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }

        #region Assignment Controller Action Methods
        public async Task<IActionResult> Active(string cn, DateTime? sd = null, DateTime? ed = null)
        {
            ActiveAssignmentsListViewModel model = new ActiveAssignmentsListViewModel();
            try
            {
                model.cn = cn;
                model.sd = sd ?? DateTime.Now.AddMonths(-1);
                model.ed = ed ?? DateTime.Now.AddMonths(1);
                model.AssignmentList = await _assignmentService.GetAssignmentsAsync(model.cn, model.sd, model.ed);
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> CreateNew()
        {
            CreateAssignmentViewModel model = new CreateAssignmentViewModel();
            model.No = await _assignmentService.GetNewAssignmentNumberAsync();
            model.EventStartDate = DateTime.Now.Date;
            model.EventEndDate = DateTime.Now.Date;
            model.EventStartHour = 00;
            model.EventStartMinute = 00;
            model.EventEndHour = 00;
            model.EventEndMinute = 00;
            model.ReportDueDate = DateTime.Now.Date;

            var claims = HttpContext.User.Claims.ToList();
            model.AssignedById = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(model.AssignedById))
            {
                return RedirectToAction("Logout", "Home");
            }

            var entities = await _assignmentService.GetAssignmentEventTypesAsync();
            if (entities != null && entities.Count > 0)
            {
                ViewBag.EventTypes = new SelectList(entities, "Id", "Description");
            }

            var rolesEntities = await _assignmentService.GetAssignmentRolesAsync();
            if (rolesEntities != null && rolesEntities.Count > 0)
            {
                ViewBag.AssignmentRoles = new SelectList(rolesEntities, "Description", "Description");
            }

            var locationEntities = await _globalSettingsService.GetStationsAsync();
            if (locationEntities != null && locationEntities.Count > 0)
            {
                ViewBag.Stations = new SelectList(locationEntities, "LocationID", "LocationName");
            }

            var statesEntities = await _globalSettingsService.GetStatesAsync();
            if (statesEntities != null && statesEntities.Count > 0)
            {
                ViewBag.States = new SelectList(statesEntities, "Name", "Name");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNew(CreateAssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Assignment assignment = model.Convert();
                    if (assignment.EventStartTime >= assignment.EventEndTime)
                    {
                        model.ViewModelErrorMessage = "Incorrect Event Start Time or End Time.";
                    }
                    else
                    {
                        assignment.AssignedByName = assignment.CreatedBy = HttpContext.User.Identity.Name;
                        assignment.CreatedTime = DateTime.Now;
                        bool IsCreated = await _assignmentService.CreateNewAssignmentAsync(assignment);
                        if (IsCreated)
                        {
                            return RedirectToAction("Active");
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }

            var entities = await _assignmentService.GetAssignmentEventTypesAsync();
            if (entities != null && entities.Count > 0)
            {
                ViewBag.EventTypes = new SelectList(entities, "Id", "Description");
            }

            var rolesEntities = await _assignmentService.GetAssignmentRolesAsync();
            if (rolesEntities != null && rolesEntities.Count > 0)
            {
                ViewBag.AssignmentRoles = new SelectList(rolesEntities, "Description", "Description");
            }

            var locationEntities = await _globalSettingsService.GetStationsAsync();
            if (locationEntities != null && locationEntities.Count > 0)
            {
                ViewBag.Stations = new SelectList(locationEntities, "LocationID", "LocationName");
            }

            var statesEntities = await _globalSettingsService.GetStatesAsync();
            if (statesEntities != null && statesEntities.Count > 0)
            {
                ViewBag.States = new SelectList(statesEntities, "Name", "Name");
            }
            return View(model);
        }

        public async Task<IActionResult> EditAssignment(long? id)
        {
            EditAssignmentViewModel model = new EditAssignmentViewModel();
            if (id < 1) { return RedirectToAction("CreateNew"); }

            Assignment assignment = await _assignmentService.GetAssignmentAsync(id.Value);
            if (assignment == null)
            {
                model.ViewModelErrorMessage = "No record was found for the selected Assignment.";
            }
            else
            {
                model = model.Convert(assignment);
            }

            var entities = await _assignmentService.GetAssignmentEventTypesAsync();
            if (entities != null && entities.Count > 0)
            {
                ViewBag.EventTypes = new SelectList(entities, "Id", "Description");
            }


            var locationEntities = await _globalSettingsService.GetStationsAsync();
            if (locationEntities != null && locationEntities.Count > 0)
            {
                ViewBag.Stations = new SelectList(locationEntities, "LocationID", "LocationName");
            }

            var statesEntities = await _globalSettingsService.GetStatesAsync();
            if (statesEntities != null && statesEntities.Count > 0)
            {
                ViewBag.States = new SelectList(statesEntities, "Name", "Name");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssignment(EditAssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Assignment assignment = model.Convert();
                    assignment.ModifiedBy = HttpContext.User.Identity.Name;
                    assignment.ModifiedTime = DateTime.Now;
                    bool IsUpdated = await _assignmentService.EditAssignmentAsync(assignment);
                    if (IsUpdated)
                    {
                        model.ViewModelSuccessMessage = "Assignment was updated successfully!";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }

            var entities = await _assignmentService.GetAssignmentEventTypesAsync();
            if (entities != null && entities.Count > 0)
            {
                ViewBag.EventTypes = new SelectList(entities, "Id", "Description");
            }

            var locationEntities = await _globalSettingsService.GetStationsAsync();
            if (locationEntities != null && locationEntities.Count > 0)
            {
                ViewBag.Stations = new SelectList(locationEntities, "LocationID", "LocationName");
            }

            var statesEntities = await _globalSettingsService.GetStatesAsync();
            if (statesEntities != null && statesEntities.Count > 0)
            {
                ViewBag.States = new SelectList(statesEntities, "Name", "Name");
            }

            return View(model);
        }

        public async Task<IActionResult> Details(long id)
        {
            CreateAssignmentViewModel model = new CreateAssignmentViewModel();
            var entity = await _assignmentService.GetAssignmentAsync(id);
            if (entity != null) { model = model.Convert(entity); }
            return View(model);
        }

        public async Task<IActionResult> Notes(long id, string sp)
        {
            AssignmentNotesViewModel model = new AssignmentNotesViewModel();
            model.SourcePage = sp;
            model.AssignmentID = id;
            try
            {
                if (model.AssignmentID > 0)
                {
                    var assignmentNotes = await _assignmentService.GetAssignmentNotesAsync(model.AssignmentID);
                    if (assignmentNotes != null) { model.NoteList = assignmentNotes; }
                }

                model.LoggedInEmployeeName = HttpContext.User.Identity.Name;
                model.LoggedInEmployeeID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;

                if (string.IsNullOrWhiteSpace(model.LoggedInEmployeeID))
                {
                    await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                    return LocalRedirect("/Home/Login");
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        public async Task<IActionResult> History(long id)
        {
            AssignmentHistoryViewModel model = new AssignmentHistoryViewModel();
            model.AssignmentID = id;
            if (id < 1)
            {
                model.ViewModelErrorMessage = "Invalid parameter value [Assignment ID].";
            }
            else
            {
                var taskActivities = await _assignmentService.GetAssignmentHistoryAsync(model.AssignmentID.Value);
                if (taskActivities != null) { model.ActivityList = taskActivities; }
            }
            return View(model);
        }
        #endregion

        #region Assignment Crew Controller Action Methods
        public async Task<IActionResult> CrewList(long id, string tl, string lc)
        {
            AssignmentCrewListViewModel model = new AssignmentCrewListViewModel();
            try
            {
                model.AssignmentID = id;
                model.AssignmentTitle = tl;
                model.AssignmentState = lc;
                //model.AssignmentDate = dt;
                if (id > 0)
                {
                    model.CrewMemberList = await _assignmentService.GetAssignmentCrewMembersAsync(model.AssignmentID);
                }
                var rolesEntities = await _assignmentService.GetAssignmentRolesAsync();
                if (rolesEntities != null && rolesEntities.Count > 0)
                {
                    ViewBag.AssignmentRoles = new SelectList(rolesEntities, "Description", "Description");
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        public async Task<IActionResult> EquipmentList(long id, string tl, string lc)
        {
            AssignmentEquipmentListViewModel model = new AssignmentEquipmentListViewModel();
            try
            {
                model.AssignmentID = id;
                model.AssignmentTitle = tl;
                model.AssignmentState = lc;
                if (id > 0)
                {
                    model.EquipmentList = await _assignmentService.GetAssignmentEquipmentsAsync(model.AssignmentID);
                }

                var employeesEntities = await _assignmentService.GetAssignmentEmployeesAsync(model.AssignmentID);
                if (employeesEntities != null && employeesEntities.Count > 0)
                {
                    ViewBag.AssignedEmployees = new SelectList(employeesEntities, "EmployeeID", "FullName");
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        #endregion

        #region Assignment Participation Reports Controller Action Methods

        public async Task<IActionResult> CrewReports(long id, string tl)
        {
            CrewReportsListViewModel model = new CrewReportsListViewModel();
            model.AssignmentID = id;
            model.AssignmentTitle = tl;
            try
            {
                if(model.AssignmentID > 0)
                {
                    model.CrewReportList = await _assignmentService.GetAssignmentCrewReportsAsync(model.AssignmentID);
                }
                else
                {
                    model.ViewModelErrorMessage = "Ooops! It appears no Assignment was selected. Please select an Assignment to view Crew Reports.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        public async Task<IActionResult> AddParticipationReport(long id)
        {
            AddParticipationReportViewModel model = new AddParticipationReportViewModel();
            model.AssignmentId = id;

            var claims = HttpContext.User.Claims.ToList();
            model.EmployeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(model.EmployeeId))
            {
                return RedirectToAction("Logout", "Home");
            }

            var assignmentCrewReportEntities = await _assignmentService.GetAssignmentCrewReportAsync(model.AssignmentId, model.EmployeeId);
            if (assignmentCrewReportEntities == null || assignmentCrewReportEntities.Count < 1)
            {
                var assignmentEntity = await _assignmentService.GetAssignmentAsync(model.AssignmentId);
                if (assignmentEntity == null || string.IsNullOrWhiteSpace(assignmentEntity.Title))
                {
                    model.ViewModelErrorMessage = "Sorry, no record was found for this Assignment in the system.";
                }
                else
                {
                    model.AssignmentNumber = assignmentEntity.No;
                    model.CustomerName = assignmentEntity.ClientName;
                    model.AssignmentTitle = assignmentEntity.Title;
                    model.AssignmentDate = assignmentEntity.EventStartTime;
                    model.ArrivalDate = assignmentEntity.EventStartTime.Value.Date;
                    model.DepartureDate = assignmentEntity.EventEndTime.Value.Date;
                    model.ArrivalHour = assignmentEntity.EventStartTime.Value.Hour;
                    model.DepartureHour = assignmentEntity.EventEndTime.Value.Hour;
                    model.ArrivalMinute = assignmentEntity.EventStartTime.Value.Minute;
                    model.DepartureMinute = assignmentEntity.EventEndTime.Value.Minute;
                }
                var assignmentCrewMember = await _assignmentService.GetAssignmentCrewMemberAsync(model.AssignmentId, model.EmployeeId);
                if (assignmentCrewMember != null)
                {
                    model.AssignmentCrewId = assignmentCrewMember.Id ?? 0L;
                    model.IsTeamLead = assignmentCrewMember.IsTeamLead;
                }
            }
            else
            {
                AssignmentCrewReport assignmentCrewReport = assignmentCrewReportEntities.FirstOrDefault();
                model = model.Convert(assignmentCrewReport);
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddParticipationReport(AddParticipationReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssignmentCrewReport assignmentCrewReport = model.Convert();
                    if (assignmentCrewReport.ArrivalTime >= assignmentCrewReport.DepartureTime)
                    {
                        model.ViewModelErrorMessage = "Incorrect Arrival Time or Departure Time.";
                    }
                    else
                    {
                        assignmentCrewReport.ModifiedBy = assignmentCrewReport.EmployeeName = HttpContext.User.Identity.Name;
                        assignmentCrewReport.ModifiedTime = DateTime.Now;
                        if(assignmentCrewReport.CrewReportId < 1)
                        {
                            long crewReportId = await _assignmentService.AddAssignmentCrewReportAsync(assignmentCrewReport);
                            if (crewReportId > 0)
                            {
                                return RedirectToAction("Active");
                            }
                        }
                        else
                        {
                            if(await _assignmentService.UpdateAssignmentCrewReportAsync(assignmentCrewReport))
                            {
                                return RedirectToAction("Active");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        #endregion

        #region Assignment Post Editing Report Controller Action Methods
        public async Task<IActionResult> EngReports(long id, string tl)
        {
            EngReportListViewModel model = new EngReportListViewModel();
            model.AssignmentID = id;
            model.AssignmentTitle = tl;
            try
            {
                if (model.AssignmentID > 0)
                {
                    model.EngReportList = await _assignmentService.GetAssignmentEngReportsAsync(model.AssignmentID);
                }
                else
                {
                    model.ViewModelErrorMessage = "Ooops! It appears no Assignment was selected. Please select an Assignment to view Crew Reports.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        public async Task<IActionResult> AddEngReport(long id)
        {
            AddEngReportViewModel model = new AddEngReportViewModel();
            model.AssignmentId = id;
            model.EditingStartDate = DateTime.Now;
            model.EditingEndDate = DateTime.Now;
            model.ReporterArrivalDate = DateTime.Now;

            var claims = HttpContext.User.Claims.ToList();
            model.EmployeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(model.EmployeeId))
            {
                return RedirectToAction("Logout", "Home");
            }
            model.EmployeeName = HttpContext.User.Identity.Name;

            var assignmentEngReportEntities = await _assignmentService.GetAssignmentEngReportAsync(model.AssignmentId, model.EmployeeId);
            if (assignmentEngReportEntities == null || assignmentEngReportEntities.Count < 1)
            {
                var assignmentEntity = await _assignmentService.GetAssignmentAsync(model.AssignmentId);
                if (assignmentEntity == null || string.IsNullOrWhiteSpace(assignmentEntity.Title))
                {
                    model.ViewModelErrorMessage = "Sorry, no record was found for this Assignment in the system.";
                }
                else
                {
                    model.AssignmentNo = assignmentEntity.No;
                    model.CustomerName = assignmentEntity.ClientName;
                    model.AssignmentTitle = assignmentEntity.Title;
                }
            }
            else
            {
                AssignmentEngReport assignmentEngReport = assignmentEngReportEntities.FirstOrDefault();
                model = model.Convert(assignmentEngReport);
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEngReport(AddEngReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssignmentEngReport assignmentEngReport = model.Convert();
                    assignmentEngReport.ModifiedBy = assignmentEngReport.EmployeeName = HttpContext.User.Identity.Name;
                    assignmentEngReport.ModifiedTime = DateTime.Now;
                        if (assignmentEngReport.EngReportId < 1)
                        {
                            long engReportId = await _assignmentService.AddAssignmentEngReportAsync(assignmentEngReport);
                            if (engReportId > 0)
                            {
                                return RedirectToAction("Active");
                            }
                        }
                        else
                        {
                            if (await _assignmentService.UpdateAssignmentEngReportAsync(assignmentEngReport))
                            {
                                return RedirectToAction("Active");
                            }
                        }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        #endregion

        #region Assignments Helper Methods
        public string SaveNote(string nm, string msg, long id)
        {
            AssignmentNote note = new AssignmentNote()
            {
                NoteTime = DateTime.Now,
                NoteWrittenBy = nm,
                NoteContent = msg,
                AssignmentId = id,
            };

            if ((note.AssignmentId < 1) || string.IsNullOrWhiteSpace(nm) || string.IsNullOrWhiteSpace(msg)) { return "parameter"; }
            try
            {
                if (_assignmentService.AddAssignmentNoteAsync(note).Result)
                {
                    return "saved";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string DeleteAssignment(long id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_assignmentService.DeleteAssignmentAsync(id).Result)
                {
                    return "success";
                }
                else
                {
                    return "method failure";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        
        //==== Assignment Crew Helper Methods =====//
        public string AddCrewMember(long id, string nm, string r1, string r2, string r3)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nm) | string.IsNullOrWhiteSpace(r1))
                {
                    return "parameter";
                }

                Employee employee = _employeeService.GetEmployeeByNameAsync(nm).Result;
                if (employee == null || string.IsNullOrWhiteSpace(employee.EmployeeID))
                {
                    return "No record was found for this employee.";
                }

                AssignmentCrewMember crewMember = new AssignmentCrewMember()
                {
                    CrewMemberName = nm,
                    CrewMemberRole1 = r1,
                    CrewMemberRole2 = r2,
                    CrewMemberRole3 = r3,
                    AssignmentId = id,
                    CrewMemberId = employee.EmployeeID,
                    DepartmentId = employee.DepartmentID,
                    LocationId = employee.LocationID,
                    UnitId = employee.UnitID,

                    CreatedBy = HttpContext.User.Identity.Name,
                    CreatedTime = DateTime.Now
                };

                if (_assignmentService.AddAssignmentCrewMemberAsync(crewMember).Result > 0)
                {
                    return "saved";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string RemoveAssignmentCrewMember(long id)
        {
            try
            {
                if (id < 1)
                {
                    return "parameter";
                }

                AssignmentCrewMember crewMember = _assignmentService.GetAssignmentCrewMemberAsync(id).Result;
                if (crewMember == null || crewMember.Id < 1)
                {
                    return "No record was found for this employee.";
                }

                crewMember.ModifiedBy = HttpContext.User.Identity.Name;
                crewMember.ModifiedTime = DateTime.Now;
                if (_assignmentService.RemoveAssignmentCrewMemberAsync(crewMember).Result)
                {
                    return "success";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string UpdateCrewMember(long id, string nm, string r1, string r2, string r3)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nm) | string.IsNullOrWhiteSpace(r1))
                {
                    return "parameter";
                }

                Employee employee = _employeeService.GetEmployeeByNameAsync(nm).Result;
                if (employee == null || string.IsNullOrWhiteSpace(employee.EmployeeID))
                {
                    return "No record was found for this employee.";
                }

                AssignmentCrewMember crewMember = _assignmentService.GetAssignmentCrewMemberAsync(id).Result;
                if (crewMember == null || crewMember.Id < 1)
                {
                    return "An error was encountered. This record could not be retrieved.";
                }

                crewMember.ModifiedBy = HttpContext.User.Identity.Name;
                crewMember.ModifiedTime = DateTime.Now;
                crewMember.CrewMemberName = nm;
                crewMember.CrewMemberRole1 = r1;
                crewMember.CrewMemberRole2 = r2;
                crewMember.CrewMemberRole3 = r3;
                crewMember.Id = id;
                crewMember.CrewMemberId = employee.EmployeeID;
                crewMember.DepartmentId = employee.DepartmentID;
                crewMember.LocationId = employee.LocationID;
                crewMember.UnitId = employee.UnitID;

                if (_assignmentService.UpdateAssignmentCrewMemberAsync(crewMember).Result)
                {
                    return "saved";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string UpdateAssignmentCrewLead(long id, bool isl)
        {
            try
            {
                if (id < 1)
                {
                    return "parameter";
                }
                string updatedBy = HttpContext.User.Identity.Name;
                if (_assignmentService.UpdateAssignmentCrewLeadAsync(id, isl, updatedBy).Result)
                {
                    return "success";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //==== Assignment Equipment Helper Methods ====//
        public string AddEquipment(long id, string sd, string qn)
        {
            AssignmentEquipment equipment = new AssignmentEquipment();
            try
            {
                if (id < 1 || string.IsNullOrWhiteSpace(sd) | string.IsNullOrWhiteSpace(qn))
                {
                    return "parameter";
                }

                Employee employee = _employeeService.GetEmployeeByIdAsync(sd).Result;
                if (employee == null || string.IsNullOrWhiteSpace(employee.EmployeeID))
                {
                    return "No record was found for this employee.";
                }

                Asset asset = _assetManagerService.GetAssetByNameAsync(qn).Result;
                equipment.AssetCategoryId = asset.AssetCategoryID;
                equipment.AssetCategoryName = asset.AssetCategoryName;
                equipment.AssetClassId = asset.AssetClassID.Value;
                equipment.AssetClassName = asset.AssetClassName;
                equipment.AssetGroupId = asset.AssetGroupID.Value;
                equipment.AssetGroupName = asset.AssetGroupName;
                equipment.AssetId = asset.AssetID;
                equipment.AssetName = asset.AssetName;
                equipment.AssetTypeId = asset.AssetTypeID;
                equipment.AssetTypeName = asset.AssetTypeName;
                equipment.AssignedToEmployeeId = employee.EmployeeID;
                equipment.AssignedToEmployeeName = employee.FullName;
                equipment.AssignmentId = id;
                equipment.ModifiedBy = equipment.AssignedByEmployeeName = HttpContext.User.Identity.Name;
                equipment.ModifiedTime = DateTime.Now;

                var claims = HttpContext.User.Claims.ToList();
                equipment.AssignedByEmployeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                if (string.IsNullOrWhiteSpace(equipment.AssignedByEmployeeId))
                {
                    return "login";
                }

                if (_assignmentService.AddAssignmentEquipmentAsync(equipment).Result > 0)
                {
                    return "saved";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string UpdateEquipment(long id, string sd, string qn)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sd) | string.IsNullOrWhiteSpace(qn))
                {
                    return "parameter";
                }

                Employee employee = _employeeService.GetEmployeeByIdAsync(sd).Result;
                if (employee == null || string.IsNullOrWhiteSpace(employee.EmployeeID))
                {
                    return "No record was found for this employee.";
                }

                Asset asset = _assetManagerService.GetAssetByNameAsync(qn).Result;
                if (asset == null || string.IsNullOrWhiteSpace(asset.AssetID))
                {
                    return "No record was found for this equipment.";
                }

                AssignmentEquipment equipment = _assignmentService.GetAssignmentEquipmentAsync(id).Result;
                if (equipment == null || equipment.AssignmentEquipmentId < 1)
                {
                    return "An error was encountered. This record could not be retrieved.";
                }

                equipment.ModifiedBy = HttpContext.User.Identity.Name;
                equipment.ModifiedTime = DateTime.Now;
                equipment.AssetCategoryId = asset.AssetCategoryID;
                equipment.AssetCategoryName = asset.AssetCategoryName;
                equipment.AssetClassId = asset.AssetClassID.Value;
                equipment.AssetClassName = asset.AssetClassName;
                equipment.AssetGroupId = asset.AssetGroupID.Value;
                equipment.AssetGroupName = asset.AssetGroupName;
                equipment.AssetId = asset.AssetID;
                equipment.AssetName = asset.AssetName;
                equipment.AssetTypeId = asset.AssetTypeID;
                equipment.AssetTypeName = asset.AssetTypeName;
                equipment.AssignedToEmployeeId = employee.EmployeeID;
                equipment.AssignedToEmployeeName = employee.FullName;
                equipment.AssignmentEquipmentId = equipment.AssignmentEquipmentId;
                equipment.AssignmentId = id;

                var claims = HttpContext.User.Claims.ToList();
                equipment.AssignedByEmployeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                if (string.IsNullOrWhiteSpace(equipment.AssignedByEmployeeId))
                {
                    return "login";
                }

                if (_assignmentService.UpdateAssignmentEquipmentAsync(equipment).Result)
                {
                    return "saved";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string RemoveAssignmentEquipment(long id)
        {
            try
            {
                if (id < 1)
                {
                    return "parameter";
                }

                AssignmentEquipment equipment = _assignmentService.GetAssignmentEquipmentAsync(id).Result;
                if (equipment == null || equipment.AssignmentEquipmentId < 1)
                {
                    return "No record was found for this equipment assignment.";
                }

                equipment.ModifiedBy = HttpContext.User.Identity.Name;
                equipment.ModifiedTime = DateTime.Now;
                if (_assignmentService.RemoveAssignmentEquipmentAsync(equipment).Result)
                {
                    return "success";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion
    }
}