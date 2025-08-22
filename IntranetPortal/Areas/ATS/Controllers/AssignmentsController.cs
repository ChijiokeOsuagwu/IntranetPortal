using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IntranetPortal.Areas.ATS.Models;
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

        public AssignmentsController(IAssignmentService assignmentService, IGlobalSettingsService globalSettingsService,
            IBaseModelService baseModelService, IErmService employeeService)
        {
            _globalSettingsService = globalSettingsService;
            _assignmentService = assignmentService;
            _baseModelService = baseModelService;
            _employeeService = employeeService;
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
                model.AssignmentList = await _assignmentService.GetAssignments(model.cn, model.sd, model.ed);
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

            Assignment assignment = await _assignmentService.GetAssignment(id.Value);
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
                    assignment.CreatedBy = HttpContext.User.Identity.Name;
                    assignment.CreatedTime = DateTime.Now;
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
            var entity = await _assignmentService.GetAssignment(id);
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
                    model.CrewMemberList = await _assignmentService.GetAssignmentCrewMembers(model.AssignmentID);
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

        #endregion

        #region Assignment Reports Controller Action Methods


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
                    UnitId = employee.UnitID
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

                AssignmentCrewMember crewMember = _assignmentService.GetAssignmentCrewMember(id).Result;
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

                AssignmentCrewMember crewMember = _assignmentService.GetAssignmentCrewMember(id).Result;
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

        #endregion
    }
}