using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClosedXML.Excel;
using IntranetPortal.Areas.WSP.Models;
using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Helpers;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.WSP.Controllers
{
    [Area("WSP")]
    public class WorkspaceController : Controller
    {
        private readonly IWspService _wspService;
        private readonly IErmService _ermService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IConfiguration _configuration;
        public WorkspaceController(IWspService wspService, IErmService ermService, IBaseModelService baseModelService,
            IGlobalSettingsService globalSettingsService, IConfiguration configuration)
        {
            _wspService = wspService;
            _ermService = ermService;
            _baseModelService = baseModelService;
            _globalSettingsService = globalSettingsService;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "WSPVWAEER, WSPVWAETK, XYALLACCZ")]
        public IActionResult Reports()
        {
            return View();
        }

        #region Task Folders Controller Action Methods
        public async Task<IActionResult> MyTaskFolders(string id = null, DateTime? sd = null, DateTime? ed = null)
        {
            TaskFoldersViewModel model = new TaskFoldersViewModel();
            string ownerName = HttpContext.User.Identity.Name;
            model.id = id;
            model.sd = sd ?? DateTime.Now.AddMonths(-3);
            model.ed = ed ?? DateTime.Now.AddMonths(+3);
            try
            {
                if (string.IsNullOrWhiteSpace(model.id))
                {
                    var claims = HttpContext.User.Claims.ToList();
                    model.id = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                }

                if (!string.IsNullOrWhiteSpace(model.id))
                {
                    var entities = await _wspService.GetWorkItemFoldersByOwnerIdAsync(model.id, false, model.sd, model.ed);
                    if (entities != null && entities.Count > 0)
                    {
                        model.TaskFolders = entities;
                    }
                    model.NoOfPendingTasks = await _wspService.GetTasksPendingCountAsync(model.id);
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }
        public async Task<IActionResult> TeamTaskFolders(string id = null, DateTime? sd = null, DateTime? ed = null)
        {
            List<EmployeeReportLine> directReports = new List<EmployeeReportLine>();
            TaskFoldersViewModel model = new TaskFoldersViewModel();
            List<WorkItemFolder> TaskFolders = new List<WorkItemFolder>();
            model.id = id;
            model.sd = sd ?? DateTime.Now.AddMonths(-3);
            model.ed = ed ?? DateTime.Now.AddMonths(1);
            try
            {
                if (!string.IsNullOrWhiteSpace(model.id))
                {
                    var entities = await _wspService.GetWorkItemFoldersByOwnerIdAsync(model.id, false, model.sd, model.ed);
                    if (entities != null && entities.Count > 0)
                    {
                        model.TaskFolders = entities;
                    }
                    model.NoOfPendingTasks = await _wspService.GetTasksPendingCountAsync(model.id);
                }

                var claims = HttpContext.User.Claims.ToList();
                string reportsToEmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                var employee_entities = await _ermService.GetEmployeeReportsByReportsToEmployeeIdAsync(reportsToEmployeeID);
                if (employee_entities != null && employee_entities.Count > 0)
                {
                    directReports = employee_entities;
                }
                ViewBag.ReportsList = new SelectList(directReports, "EmployeeID", "EmployeeName", model.id);
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }
        public async Task<IActionResult> MyArchivedTaskFolders(string id = null, DateTime? fd = null, DateTime? td = null)
        {
            MyArchivedTaskFoldersViewModel model = new MyArchivedTaskFoldersViewModel();
            string ownerName = HttpContext.User.Identity.Name;
            model.Id = id;

            if (fd == null) { model.fd = DateTime.Now.AddMonths(-6); }
            if (td == null) { model.td = DateTime.Now.AddDays(1); }

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                var claims = HttpContext.User.Claims.ToList();
                model.Id = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(model.Id))
            {
                var entities = await _wspService.GetWorkItemFoldersArchivedAsync(model.Id, fd, td);
                if (entities != null && entities.Count > 0)
                {
                    model.ArchivedFolders = entities;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> ManageTaskFolder(long? id = null, string od = null, string src = null)
        {
            ManageTaskFolderViewModel model = new ManageTaskFolderViewModel();
            model.Id = id ?? 0;
            try
            {
                if (id == null || id < 1)
                {
                    model.PeriodStartDate = DateTime.Today;
                    model.PeriodEndDate = DateTime.Today.AddDays(7);
                    model.Title = $"Weekly Tasks Due by {model.PeriodEndDate.Value.ToString("dd-MMM-yyyy")}";
                    if (!string.IsNullOrWhiteSpace(od))
                    {
                        model.OwnerId = od;
                    }
                    else
                    {
                        var claims = HttpContext.User.Claims.ToList();
                        model.OwnerId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    }
                }
                else
                {
                    var entity = await _wspService.GetWorkItemFolderAsync(id.Value);
                    if (entity != null)
                    {
                        model = model.Convert(entity);
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            model.SourcePage = src;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageTaskFolder(ManageTaskFolderViewModel model)
        {
            if (ModelState.IsValid)
            {
                string _pageSource = model.SourcePage;
                WorkItemFolder folder = new WorkItemFolder();
                folder = model.Convert();
                folder.FolderTypeId = (int)WorkItemFolderType.TaskFolder;
                try
                {
                    if (model.Id < 1)
                    {
                        if (string.IsNullOrWhiteSpace(model.OwnerId))
                        {
                            var claims = HttpContext.User.Claims.ToList();
                            folder.OwnerId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                        }
                        else { folder.OwnerId = model.OwnerId; }
                        folder.CreatedBy = HttpContext.User.Identity.Name;
                        folder.CreatedTime = DateTime.Now;
                        long _newTaskFolderId = await _wspService.CreateWorkItemFolderAsync(folder);
                        if (_newTaskFolderId > 0)
                        {
                            if (model.SourcePage == "ttf")
                            {
                                return RedirectToAction("TeamTaskFolders", new { id = folder.OwnerId });
                            }
                            else if (model.SourcePage == "mtf")
                            {
                                return RedirectToAction("MyTaskFolders", new { id = folder.OwnerId });
                            }
                            else { return View(model); }
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "Sorry, the operation could not be completed. Please try again.";
                            return View(model);
                        }
                    }
                    else
                    {
                        folder.UpdatedBy = HttpContext.User.Identity.Name;
                        folder.UpdatedTime = DateTime.Now;
                        bool IsUpdated = await _wspService.UpdateWorkItemFolderAsync(folder);
                        if (IsUpdated)
                        {
                            if (model.SourcePage == "ttf")
                            {
                                return RedirectToAction("TeamTaskFolders", new { id = folder.OwnerId });
                            }
                            else if (model.SourcePage == "mtf")
                            {
                                return RedirectToAction("MyTaskFolders", new { id = folder.OwnerId });
                            }
                            else { return View(model); }
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "Sorry, the operation could not be completed. Please try again.";
                            return View(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }
        public async Task<IActionResult> TaskFolderInfo(long? id)
        {
            ManageTaskFolderViewModel model = new ManageTaskFolderViewModel();
            try
            {
                var entity = await _wspService.GetWorkItemFolderAsync(id.Value);
                if (entity != null)
                {
                    model = model.Convert(entity);
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        public async Task<IActionResult> DeleteTaskFolder(long? id, string src = null)
        {
            ManageTaskFolderViewModel model = new ManageTaskFolderViewModel();
            try
            {
                var entity = await _wspService.GetWorkItemFolderAsync(id.Value);
                if (entity != null)
                {
                    model = model.Convert(entity);
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            model.SourcePage = src;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTaskFolder(ManageTaskFolderViewModel model)
        {
            if (model.Id > 0)
            {
                try
                {
                    bool IsDeleted = await _wspService.DeleteWorkItemFolderAsync(model.Id);
                    if (IsDeleted)
                    {
                        if (model.SourcePage == "ttf")
                        {
                            return RedirectToAction("TeamTaskFolders", new { id = model.OwnerId });
                        }
                        else if (model.SourcePage == "mtf")
                        {
                            return RedirectToAction("MyTaskFolders", new { id = model.OwnerId });
                        }
                        else { return View(model); }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, the operation could not be completed. Please try again.";
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Notes(string sp, long? td, long? fd, long? pd)
        {
            WorkItemNotesViewModel model = new WorkItemNotesViewModel();
            model.SourcePage = sp;
            model.TaskID = td;
            model.FolderID = fd;
            model.ProjectID = pd;
            if (td < 1 && fd < 1 && pd < 1) { return View(model); }
            else
            {
                if (model.TaskID > 0)
                {
                    var taskNotes = await _wspService.GetTaskItemNotesAsync(model.TaskID.Value);
                    if (taskNotes != null) { model.NoteList = taskNotes; }
                }
                else if (model.TaskID < 1 && model.FolderID > 0)
                {
                    var folderNotes = await _wspService.GetWorkItemFolderNotesAsync(model.FolderID.Value);
                    if (folderNotes != null) { model.NoteList = folderNotes; }
                }
                else if (model.ProjectID > 0)
                {

                }
                else { }
            }

            model.LoggedInEmployeeName = HttpContext.User.Identity.Name;
            model.LoggedInEmployeeID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;

            if (string.IsNullOrWhiteSpace(model.LoggedInEmployeeID))
            {
                await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                return LocalRedirect("/Home/Login");
            }
            return View(model);
        }
        public async Task<IActionResult> Activities(long? td, long? fd, long? pd)
        {
            WorkItemActivitiesViewModel model = new WorkItemActivitiesViewModel();
            model.TaskID = td;
            model.FolderID = fd;
            model.ProjectID = pd;
            if (td < 1 && fd < 1 && pd < 1) { return View(model); }

            if (model.TaskID > 0)
            {
                var taskActivities = await _wspService.GetWorkItemActivitiesByTaskIdAsync(model.TaskID.Value);
                if (taskActivities != null) { model.ActivityList = taskActivities; }
            }
            else if (model.FolderID > 0 && (model.TaskID == null || model.TaskID < 1))
            {
                var folderActivities = await _wspService.GetWorkItemActivitiesByFolderIdAsync(model.FolderID.Value);
                if (folderActivities != null) { model.ActivityList = folderActivities; }
            }
            else if (model.ProjectID > 0)
            {

            }
            return View(model);
        }

        #endregion

        #region Task Items Controller Action Methods
        public async Task<IActionResult> MyTaskList(long id, string nm, int? ps = null, int? dm = null, string src = null)
        {
            TaskListViewModel model = new TaskListViewModel();
            model.SourcePage = src;
            model.FolderID = id;
            model.FolderTitle = nm;
            model.ProgressStatusID = ps;
            if (id < 1)
            {
                model.FolderID = 0;
                model.FolderTitle = "Pending Tasks";
                model.IsPendingTasks = true;
                model.FolderOwnerName = HttpContext.User.Identity.Name;
                var claims = HttpContext.User.Claims.ToList();
                model.FolderOwnerID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                var entities = await _wspService.GetTasksPendingAsync(model.FolderOwnerID);
                if (entities != null) { model.TaskItems = entities; }
            }
            else
            {
                WorkItemFolder folder = await _wspService.GetWorkItemFolderAsync(model.FolderID);
                if (folder != null)
                {
                    model.FolderIsArchived = folder.IsArchived;
                    model.FolderIsLocked = folder.IsLocked;
                    model.FolderTitle = folder.Title;
                    model.FolderOwnerID = folder.OwnerId;
                    model.FolderOwnerName = folder.OwnerName;
                }
                var entities = await _wspService.GetTasksByFolderIdAsync(model.FolderID);
                if (entities != null) { model.TaskItems = entities; }
            }
            var folder_entities = await _wspService.GetActiveWorkItemFoldersAsync(model.FolderOwnerID);
            if (folder_entities != null && folder_entities.Count > 0)
            {
                model.TaskFolderList = folder_entities;
            }
            return View(model);
        }
        public async Task<IActionResult> TeamTaskList(long id, string od, string nm, int? ps = null, int? dm = null, string src = null)
        {
            TaskListViewModel model = new TaskListViewModel();
            model.FolderOwnerID = od;
            model.SourcePage = src;
            model.FolderID = id;
            model.FolderTitle = nm;
            model.ProgressStatusID = ps;
            try
            {
                Employee _folderOwner = await _ermService.GetEmployeeByIdAsync(model.FolderOwnerID);
                if (_folderOwner != null && !string.IsNullOrWhiteSpace(_folderOwner.EmployeeID))
                {
                    model.FolderOwnerName = _folderOwner.FullName;
                    model.FolderOwnerUnitName = _folderOwner.UnitName;
                }

                if (id < 1)
                {
                    model.FolderID = 0;
                    model.FolderTitle = "Pending Tasks";
                    model.IsPendingTasks = true;

                    var entities = await _wspService.GetTasksPendingAsync(model.FolderOwnerID);
                    if (entities != null) { model.TaskItems = entities; }

                    var folder_entities = await _wspService.GetActiveWorkItemFoldersAsync(model.FolderOwnerID);
                    if (folder_entities != null && folder_entities.Count > 0)
                    {
                        model.TaskFolderList = folder_entities;
                    }
                }
                else
                {
                    WorkItemFolder folder = await _wspService.GetWorkItemFolderAsync(model.FolderID);
                    if (folder != null)
                    {
                        model.FolderIsArchived = folder.IsArchived;
                        model.FolderIsLocked = folder.IsLocked;
                        model.FolderTitle = folder.Title;
                        model.FolderOwnerID = folder.OwnerId;
                        model.FolderOwnerName = folder.OwnerName;
                    }
                    var entities = await _wspService.GetTasksByFolderIdAsync(model.FolderID);
                    if (entities != null) { model.TaskItems = entities; }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        public async Task<IActionResult> MyArchivedEvaluations(long id, string fn, string od)
        {
            SubmittedEvaluationsViewModel model = new SubmittedEvaluationsViewModel();
            model.FolderID = id;
            model.FolderOwnerID = od;
            model.FolderName = fn;
            model.SubmittedToEmployeeName = HttpContext.User.Identity.Name;

            if (!string.IsNullOrWhiteSpace(model.FolderOwnerID))
            {
                var taskOwner = await _ermService.GetEmployeeByIdAsync(model.FolderOwnerID);
                if (taskOwner != null)
                {
                    model.FolderOwnerName = taskOwner.FullName;
                    model.FolderOwnerUnitName = taskOwner.UnitName;
                    model.FolderOwnerLocationName = taskOwner.LocationName;
                    model.FolderOwnerDesignation = taskOwner.CurrentDesignation;
                }
            }

            if (model.FolderID > 0)
            {
                var entities = await _wspService.GetTaskItemEvaluationsAsync(model.FolderID);
                if (entities != null && entities.Count > 0)
                {
                    model.TaskItemEvaluations = entities;
                    model.SubmittedToEmployeeName = entities[0].EvaluatorName;

                }
            }
            return View(model);
        }
        public async Task<IActionResult> ManageTask(long id, long fd, string od = null, string src = null, long? sd = null)
        {
            ManageTaskViewModel model = new ManageTaskViewModel();
            model.Id = id;
            model.WorkFolderId = fd;
            model.TaskOwnerId = od;
            model.SourcePage = src;
            model.FolderSubmissionId = sd;
            Employee employee = new Employee();
            try
            {
                if (model.Id < 1)
                {
                    string taskNo = await _baseModelService.GenerateAutoNumberAsync("taskno");
                    if (!string.IsNullOrWhiteSpace(taskNo)) { model.Number = $"T{taskNo}"; }
                    if (string.IsNullOrWhiteSpace(model.TaskOwnerId))
                    {
                        WorkItemFolder folder = new WorkItemFolder();
                        if (model.WorkFolderId > 0)
                        {
                            folder = await _wspService.GetWorkItemFolderAsync(model.WorkFolderId.Value);
                            if (folder != null)
                            {
                                model.TaskOwnerId = folder.OwnerId;
                            }
                        }
                        var claims = HttpContext.User.Claims.ToList();
                        model.TaskOwnerId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                        if (string.IsNullOrWhiteSpace(model.TaskOwnerId))
                        {
                            model.ViewModelErrorMessage = "Oops! It appears you session has expired. Please login and try again.";
                            return View(model);
                        }
                    }
                    employee = await _ermService.GetEmployeeByIdAsync(model.TaskOwnerId);
                    if (employee == null || string.IsNullOrWhiteSpace(employee.FullName))
                    {
                        model.ViewModelErrorMessage = "No employee record was found for the active user. Please login and try again.";
                        return View(model);
                    }
                    model.TaskOwnerId = employee.EmployeeID;
                    model.TaskOwnerName = employee.FullName;
                    model.UnitId = employee.UnitID;
                    model.DepartmentId = employee.DepartmentID;
                    model.LocationId = employee.LocationID;
                    model.ExpectedStartTime = DateTime.Today;
                    model.ExpectedDueTime = DateTime.Today.AddDays(7);
                }
                else
                {
                    var task = await _wspService.GetTaskItemByIdAsync(model.Id);
                    if (task != null)
                    {
                        model = model.Convert(task);
                        model.SourcePage = src;
                        model.FolderSubmissionId = sd;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageTask(ManageTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TaskItem task = model.Convert();
                    if (task.Id > 0)
                    {
                        task.LastModifiedBy = HttpContext.User.Identity.Name;
                        task.LastModifiedTime = DateTime.UtcNow;
                        bool isUpdated = await _wspService.UpdateTaskItemAsync(task);
                        if (isUpdated)
                        {
                            if (model.SourcePage == "mtl")
                            {
                                return RedirectToAction("MyTaskList", new { id = model.WorkFolderId, nm = model.WorkFolderName });
                            }
                            else if (model.SourcePage == "sbt")
                            {
                                return RedirectToAction("SubmittedTasks", new { id = model.WorkFolderId, sd = model.FolderSubmissionId, tp = "Approval", od = model.TaskOwnerId });
                            }
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                    else
                    {
                        switch (task.ProgressStatusId)
                        {
                            case 1:
                            case 3:
                                task.ActualStartTime = DateTime.Now;
                                break;
                            case 2:
                                task.ActualStartTime = DateTime.Now;
                                task.ActualDueTime = DateTime.Now;
                                break;
                            default:
                                break;
                        }
                        task.CreatedBy = HttpContext.User.Identity.Name;
                        task.CreatedTime = DateTime.UtcNow;
                        long newTaskId = await _wspService.CreateTaskItemAsync(task);
                        if (newTaskId > 0)
                        {
                            if (model.SourcePage == "mtl")
                            {
                                return RedirectToAction("MyTaskList", new { id = model.WorkFolderId, nm = model.WorkFolderName });
                            }
                            else if (model.SourcePage == "sbt")
                            {
                                return RedirectToAction("SubmittedTasks", new { id = model.WorkFolderId, sd = model.FolderSubmissionId, tp = "Approval", od = model.TaskOwnerId });
                            }
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
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
        public async Task<IActionResult> TaskDetails(long id)
        {
            ManageTaskViewModel model = new ManageTaskViewModel();
            model.Id = id;
            try
            {
                if (model.Id > 0)
                {
                    var task = await _wspService.GetTaskItemByIdAsync(model.Id);
                    if (task != null)
                    {
                        model = model.Convert(task);
                    }
                }
                else { model.ViewModelErrorMessage = "Sorry, invalid parameter value [Task ID]."; }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        public async Task<IActionResult> ManageTaskResolution(long id, long fd, string od = null)
        {
            ManageTaskResolutionViewModel model = new ManageTaskResolutionViewModel();
            model.TaskItemID = id;
            model.TaskFolderID = fd;
            model.TaskOwnerID = od;
            Employee employee = new Employee();
            try
            {
                var task = await _wspService.GetTaskItemByIdAsync(model.TaskItemID);
                if (task != null)
                {
                    model.TaskResolution = task.MoreInformation;
                    model.TaskFolderName = task.WorkFolderName;
                    model.TaskItemDescription = task.Description;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageTaskResolution(ManageTaskResolutionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string UpdatedBy = HttpContext.User.Identity.Name;
                    bool isUpdated = await _wspService.UpdateTaskItemResolutionAsync(model.TaskItemID, model.TaskResolution, UpdatedBy);
                    if (isUpdated)
                    {
                        return RedirectToAction("MyTaskList", new { id = model.TaskFolderID, nm = model.TaskFolderName });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> RescheduleTask(long id, long fd, string src = null)
        {
            RescheduleTaskViewModel model = new RescheduleTaskViewModel();
            model.Source = src;
            model.TaskItemID = id;
            model.TaskFolderID = fd;
            var entity = await _wspService.GetTaskItemByIdAsync(id);
            model.CurrentEndDate = entity.ExpectedDueTime;
            if (entity.ExpectedDueTime != null) { model.CurrentEndDateDescription = entity.ExpectedDueTime.Value.ToString("yyyy-MMM-dd"); }
            model.CurrentStartDate = entity.ExpectedStartTime;
            if (entity.ExpectedStartTime != null) { model.CurrentStartDateDescription = entity.ExpectedStartTime.Value.ToString("yyyy-MMM-dd"); }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RescheduleTask(RescheduleTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                TaskTimelineChange taskTimelineChange = new TaskTimelineChange();
                try
                {
                    taskTimelineChange.NewEndDate = model.NewEndDate;
                    taskTimelineChange.NewStartDate = model.NewStartDate;
                    taskTimelineChange.PreviousEndDate = model.CurrentEndDate;
                    taskTimelineChange.PreviousStartDate = model.CurrentStartDate;
                    taskTimelineChange.TaskItemId = model.TaskItemID;
                    taskTimelineChange.WorkItemFolderId = model.TaskFolderID;
                    if (model.NewEndDate != null && model.CurrentEndDate != null)
                    {
                        TimeSpan timeSpan = model.NewEndDate.Value.Subtract(model.CurrentEndDate.Value);
                        if (timeSpan != null)
                        {
                            taskTimelineChange.DifferentInDays = timeSpan.Days;
                        }
                    }
                    taskTimelineChange.ModifiedBy = HttpContext.User.Identity.Name;
                    bool IsUpdated = await _wspService.UpdateTaskTimelineAsync(taskTimelineChange);
                    if (IsUpdated)
                    {
                        if (model.Source == "ttl")
                        {
                            return RedirectToAction("TeamTaskList", new { id = model.TaskFolderID });
                        }
                        else if (model.Source == "mtl")
                        {
                            return RedirectToAction("MyTaskList", new { id = model.TaskFolderID });
                        }
                        else if (model.Source == "sbt")
                        {
                            return RedirectToAction("SubmittedTasks", new { id = model.TaskFolderID });
                        }
                        else
                        {
                            return RedirectToAction("Index");
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
        public async Task<IActionResult> ScheduleHistory(long id, long fd)
        {
            ScheduleHistoryViewModel model = new ScheduleHistoryViewModel();
            model.TaskItemID = id;
            model.TaskFolderID = fd;

            var entities = await _wspService.GetTaskTimelineChangesByTaskItemIdAsync(model.TaskItemID);
            if (entities != null && entities.Count > 0)
            {
                model.TaskTimelineChanges = entities;
            }
            return View(model);
        }
        public IActionResult DeclineTaskApproval(long id, long fd, long sd, string od, string fn, string st)
        {
            DeclineTaskApprovalViewModel model = new DeclineTaskApprovalViewModel();
            model.TaskItemID = id;
            model.TaskFolderID = fd;
            model.TaskFolderName = fn;
            model.TaskOwnerID = od;
            model.FolderSubmissionID = sd;
            model.FolderSubmissionType = st;
            model.FromEmployeeName = HttpContext.User.Identity.Name;
            var claims = HttpContext.User.Claims.ToList();
            model.FromEmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeclineTaskApproval(DeclineTaskApprovalViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    WorkItemNote note = new WorkItemNote
                    {
                        NoteContent = model.NoteContent,
                        NoteWrittenBy = model.FromEmployeeName,
                        TaskItemId = model.TaskItemID,
                        NoteTime = DateTime.Now,
                    };
                    bool NoteIsAdded = await _wspService.AddWorkItemNoteAsync(note);
                    if (NoteIsAdded)
                    {
                        bool TaskIsDeclined = await _wspService.UpdateTaskItemApprovalStatusAsync(model.TaskItemID, ApprovalStatus.Declined, model.FromEmployeeName);
                        if (TaskIsDeclined)
                        {

                            return RedirectToAction("SubmittedTasks", new { id = model.TaskFolderID, fn = model.TaskFolderName, sd = model.FolderSubmissionID, od = model.TaskOwnerID });
                        }
                        else { model.ViewModelErrorMessage = "Sorry,an error was encountered. Please try again."; }
                    }
                    else { model.ViewModelErrorMessage = "Sorry,an error was encountered. Please try again."; }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> DelegatedTaskList(string id = null, string ed = null, int? ps = null, DateTime? fd = null, DateTime? td = null)
        {
            DelegatedTaskListViewModel model = new DelegatedTaskListViewModel();
            model.id = id;
            model.ed = ed;
            model.ps = ps;
            model.fd = fd ?? DateTime.Now.AddMonths(-6);
            model.td = td ?? DateTime.Now.AddDays(1);
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    var claims = HttpContext.User.Claims.ToList();
                    model.id = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                }
                model.TaskItems = await _wspService.SearchDelegatedTaskItemsAsync(model.id, model.ed, model.ps, model.fd, model.td);

                var entities = await _ermService.GetEmployeeReportsByReportsToEmployeeIdAsync(model.id);
                if (entities != null && entities.Count > 0)
                {
                    ViewBag.ReportsList = new SelectList(entities, "EmployeeID", "EmployeeName", model.ed);
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        public async Task<IActionResult> DelegateTask(string ed = null, long? fd = null, string src = null, long? sd = null)
        {
            DelegateTaskViewModel model = new DelegateTaskViewModel();
            model.WorkFolderId = fd;
            model.TaskOwnerId = ed;
            model.SourcePage = src;
            model.FolderSubmissionId = sd;
            model.DelegatedByEmployeeName = HttpContext.User.Identity.Name;
            model.DelegatedTime = DateTime.Now;
            Employee employee = new Employee();
            try
            {
                string taskNo = await _baseModelService.GenerateAutoNumberAsync("taskno");
                if (!string.IsNullOrWhiteSpace(taskNo)) { model.Number = $"T{taskNo}"; }
                if (string.IsNullOrWhiteSpace(model.DelegatedByEmployeeId))
                {
                    var claims = HttpContext.User.Claims.ToList();
                    model.DelegatedByEmployeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (string.IsNullOrWhiteSpace(model.DelegatedByEmployeeId))
                    {
                        model.ViewModelErrorMessage = "Oops! It appears you session has expired. Please login and try again.";
                        return View(model);
                    }
                }
                model.ExpectedStartTime = DateTime.Today;
                model.ExpectedDueTime = DateTime.Today.AddDays(7);
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DelegateTask(DelegateTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.DelegatedByEmployeeName = HttpContext.User.Identity.Name;
                    if (string.IsNullOrWhiteSpace(model.DelegatedByEmployeeId))
                    {
                        var claims = HttpContext.User.Claims.ToList();
                        model.DelegatedByEmployeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                        if (string.IsNullOrWhiteSpace(model.DelegatedByEmployeeId))
                        {
                            model.ViewModelErrorMessage = "Oops! It appears you session has expired. Please login and try again.";
                            return View(model);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(model.DelegatedToEmployeeName))
                    {
                        Employee employee = await _ermService.GetEmployeeByNameAsync(model.DelegatedToEmployeeName);
                        if (employee != null)
                        {
                            model.DelegatedToEmployeeId = employee.PersonID;
                            model.TaskOwnerId = employee.EmployeeID;
                            model.TaskOwnerName = employee.FullName;
                            model.UnitId = employee.UnitID;
                            model.DepartmentId = employee.DepartmentID;
                            model.LocationId = employee.LocationID;
                        }
                    }

                    DelegatedTaskItem task = model.Convert();

                    switch (task.ProgressStatusId)
                    {
                        case 1:
                        case 3:
                            task.ActualStartTime = DateTime.Now;
                            break;
                        case 2:
                            task.ActualStartTime = DateTime.Now;
                            task.ActualDueTime = DateTime.Now;
                            break;
                        default:
                            break;
                    }
                    task.CreatedBy = HttpContext.User.Identity.Name;
                    task.CreatedTime = DateTime.UtcNow;
                    task.AssignedByEmployeeId = model.DelegatedByEmployeeId;
                    bool IsCreated = await _wspService.CreateDelegatedTaskItemAsync(task);
                    if (IsCreated)
                    {
                        //============= Notification Code Starts Here =========================//

                        Employee sender = new Employee();
                        sender = await _ermService.GetEmployeeByIdAsync(model.DelegatedByEmployeeId);
                        Employee receiver = new Employee();
                        receiver = await _ermService.GetEmployeeByIdAsync(model.DelegatedToEmployeeId);

                        //===== Send Notificiation Message to Approver ========//
                        Message message = new Message
                        {
                            MessageID = Guid.NewGuid().ToString(),
                            RecipientID = receiver.EmployeeID,
                            RecipientName = receiver.FullName,
                            SentBy = sender.FullName
                        };

                        //===== Send Email Notifications =========//
                        bool emailCopySent = false;
                        UtilityHelper utilityHelper = new UtilityHelper(_configuration);
                        EmailModel recipientEmailCopy = new EmailModel();
                        recipientEmailCopy.RecipientName = receiver.FullName;
                        if (!string.IsNullOrWhiteSpace(receiver.OfficialEmail))
                        {
                            recipientEmailCopy.RecipientEmail = receiver.OfficialEmail;
                        }
                        else
                        {
                            recipientEmailCopy.RecipientEmail = receiver.Email;
                        }

                        if (!string.IsNullOrWhiteSpace(sender.OfficialEmail))
                        {
                            recipientEmailCopy.SenderEmail = sender.OfficialEmail;
                        }
                        else
                        {
                            recipientEmailCopy.SenderEmail = sender.Email;
                        }

                        recipientEmailCopy.SenderName = sender.FullName;
                        recipientEmailCopy.Subject = "A New Task Has Been Delegated To You.";
                        recipientEmailCopy.HtmlContent = UtilityHelper.GetWorkspaceTaskDelegationNotificationEmailHtmlContent(receiver.FullName, sender.FullName);
                        recipientEmailCopy.PlainContent = UtilityHelper.GetWorkspaceTaskDelegationNotificationEmailPlainContent(receiver.FullName, sender.FullName);

                        message.Subject = "A New Task Has Been Delegated To You.";
                        message.MessageBody = UtilityHelper.GetWorkspaceTaskDelegationNotificationMessageContent(receiver.FullName, sender.FullName);

                        bool messageSent = await _baseModelService.SendMessageAsync(message);
                        if (!string.IsNullOrWhiteSpace(recipientEmailCopy.RecipientEmail))
                        {
                            emailCopySent = await utilityHelper.SendEmailWithSendGridAsync(recipientEmailCopy);
                        }

                        //============= Notification Code Ends Here ===========================//


                        if (model.SourcePage == "dlg")
                        {
                            return RedirectToAction("DelegatedTaskList", new { id = model.DelegatedByEmployeeId });
                        }
                        else if (model.SourcePage == "mtl")
                        {
                            return RedirectToAction("MyTaskList", new { id = model.WorkFolderId, nm = model.WorkFolderName });
                        }
                        else if (model.SourcePage == "sbt")
                        {
                            return RedirectToAction("SubmittedTasks", new { id = model.WorkFolderId, sd = model.FolderSubmissionId, tp = "Approval", od = model.TaskOwnerId });
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> ReassignTask(long id, long td, string src = null, long? sd = null)
        {
            ReassignTaskViewModel model = new ReassignTaskViewModel();
            model.OldTaskDelegationId = id;
            model.TaskItemId = td;
            model.SourcePage = src;
            model.DelegatedByEmployeeName = HttpContext.User.Identity.Name;
            model.DelegatedTime = DateTime.Now;
            Employee employee = new Employee();
            try
            {
                if (string.IsNullOrWhiteSpace(model.DelegatedByEmployeeId))
                {
                    var claims = HttpContext.User.Claims.ToList();
                    model.DelegatedByEmployeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (string.IsNullOrWhiteSpace(model.DelegatedByEmployeeId))
                    {
                        model.ViewModelErrorMessage = "Oops! It appears you session has expired. Please login and try again.";
                        return View(model);
                    }
                }
                model.ExpectedStartTime = DateTime.Today;
                model.ExpectedDueTime = DateTime.Today.AddDays(7);

                if (model.TaskItemId > 0)
                {
                    DelegatedTaskItem d = await _wspService.GetDelegatedTaskItemAsync(model.OldTaskDelegationId);
                    if (d != null)
                    {
                        model.ProgressStatusId = d.ProgressStatusId;
                        model.TaskItemDescription = d.Description;
                        model.TaskNumber = d.Number;
                        model.ExpectedDueTime = d.ExpectedDueTime;
                        model.ExpectedStartTime = d.ExpectedStartTime;
                        model.OldTaskDelegationId = d.TaskDelegationId;
                        model.TaskItemId = d.Id;
                    }
                    else
                    {
                        TaskItem t = await _wspService.GetTaskItemByIdAsync(model.TaskItemId);
                        if (t != null)
                        {
                            model.TaskItemId = t.Id;
                            model.TaskNumber = t.Number;
                            model.TaskItemDescription = t.Description;
                            model.MoreInformation = t.MoreInformation;
                            model.ProgressStatusId = t.ProgressStatusId;
                            model.ExpectedDueTime = t.ExpectedDueTime;
                            model.ExpectedStartTime = t.ExpectedStartTime;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReassignTask(ReassignTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                DelegatedTaskItem delegatedTaskItem = await _wspService.GetDelegatedTaskItemAsync(model.OldTaskDelegationId);
                delegatedTaskItem.DelegatedByEmployeeId = model.DelegatedByEmployeeId;
                delegatedTaskItem.DelegatedByEmployeeName = model.DelegatedByEmployeeName;
                delegatedTaskItem.DelegatedTime = model.DelegatedTime;
                delegatedTaskItem.DelegatedToEmployeeId = model.DelegatedToEmployeeId;
                delegatedTaskItem.DelegatedToEmployeeName = model.DelegatedToEmployeeName;
                delegatedTaskItem.IsReAssigned = true;
                delegatedTaskItem.ReassignedTime = model.DelegatedTime;
                delegatedTaskItem.AssignedTime = model.DelegatedTime;
                delegatedTaskItem.TaskOwnerName = model.DelegatedToEmployeeName;
                delegatedTaskItem.Description = model.TaskItemDescription;
                delegatedTaskItem.MoreInformation = model.MoreInformation;
                delegatedTaskItem.ProgressStatusId = model.ProgressStatusId;
                delegatedTaskItem.ExpectedDueTime = model.ExpectedDueTime;
                delegatedTaskItem.ExpectedStartTime = model.ExpectedStartTime;

                try
                {
                    delegatedTaskItem.AssignedByEmployeeName = delegatedTaskItem.LastModifiedBy = delegatedTaskItem.DelegatedByEmployeeName = HttpContext.User.Identity.Name;
                    if (string.IsNullOrWhiteSpace(delegatedTaskItem.DelegatedByEmployeeId))
                    {
                        var claims = HttpContext.User.Claims.ToList();
                        delegatedTaskItem.AssignedByEmployeeId = delegatedTaskItem.DelegatedByEmployeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                        if (string.IsNullOrWhiteSpace(delegatedTaskItem.DelegatedByEmployeeId))
                        {
                            model.ViewModelErrorMessage = "Oops! It appears you session has expired. Please login and try again.";
                            return View(model);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(model.DelegatedToEmployeeName))
                    {
                        Employee employee = await _ermService.GetEmployeeByNameAsync(model.DelegatedToEmployeeName);
                        if (employee != null)
                        {
                            delegatedTaskItem.DelegatedToEmployeeId = employee.PersonID;
                            delegatedTaskItem.TaskOwnerId = employee.EmployeeID;
                            delegatedTaskItem.UnitId = employee.UnitID;
                            delegatedTaskItem.DepartmentId = employee.DepartmentID;
                            delegatedTaskItem.LocationId = employee.LocationID;
                        }
                    }

                    delegatedTaskItem.LastModifiedTime = DateTime.Now;
                    bool IsReassigned = await _wspService.ReDelegateTaskItemAsync(delegatedTaskItem, model.OldTaskDelegationId);
                    if (IsReassigned)
                    {
                        //============= Notification Code Starts Here =========================//

                        Employee sender = new Employee();
                        sender = await _ermService.GetEmployeeByIdAsync(model.DelegatedByEmployeeId);
                        Employee receiver = new Employee();
                        receiver = await _ermService.GetEmployeeByIdAsync(model.DelegatedToEmployeeId);

                        //===== Send Notificiation Message ========//
                        Message message = new Message
                        {
                            MessageID = Guid.NewGuid().ToString(),
                            RecipientID = receiver.EmployeeID,
                            RecipientName = receiver.FullName,
                            SentBy = sender.FullName
                        };

                        //===== Send Email Notifications =========//
                        bool emailCopySent = false;
                        UtilityHelper utilityHelper = new UtilityHelper(_configuration);
                        EmailModel recipientEmailCopy = new EmailModel();
                        recipientEmailCopy.RecipientName = receiver.FullName;
                        if (!string.IsNullOrWhiteSpace(receiver.OfficialEmail))
                        {
                            recipientEmailCopy.RecipientEmail = receiver.OfficialEmail;
                        }
                        else
                        {
                            recipientEmailCopy.RecipientEmail = receiver.Email;
                        }

                        if (!string.IsNullOrWhiteSpace(sender.OfficialEmail))
                        {
                            recipientEmailCopy.SenderEmail = sender.OfficialEmail;
                        }
                        else
                        {
                            recipientEmailCopy.SenderEmail = sender.Email;
                        }

                        recipientEmailCopy.SenderName = sender.FullName;
                        recipientEmailCopy.Subject = "A New Task Has Been Delegated To You.";
                        recipientEmailCopy.HtmlContent = UtilityHelper.GetWorkspaceTaskDelegationNotificationEmailHtmlContent(receiver.FullName, sender.FullName);
                        recipientEmailCopy.PlainContent = UtilityHelper.GetWorkspaceTaskDelegationNotificationEmailPlainContent(receiver.FullName, sender.FullName);

                        message.Subject = "A New Task Has Been Delegated To You.";
                        message.MessageBody = UtilityHelper.GetWorkspaceTaskDelegationNotificationMessageContent(receiver.FullName, sender.FullName);

                        bool messageSent = await _baseModelService.SendMessageAsync(message);
                        if (!string.IsNullOrWhiteSpace(recipientEmailCopy.RecipientEmail))
                        {
                            emailCopySent = await utilityHelper.SendEmailWithSendGridAsync(recipientEmailCopy);
                        }

                        //============= Notification Code Ends Here ===========================//

                        if (model.SourcePage == "dlg")
                        {
                            return RedirectToAction("DelegatedTaskList", new { id = model.DelegatedByEmployeeId });
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> CheckDuplicates(string id, string kw, DateTime? sd = null, DateTime? ed = null)
        {
            CheckDuplicatesViewModel model = new CheckDuplicatesViewModel();
            if (string.IsNullOrWhiteSpace(kw)) { model.ViewModelErrorMessage = "Please enter a keyword to search for duplicates."; }
            model.kw = kw;
            if (string.IsNullOrWhiteSpace(id)) { model.ViewModelErrorMessage = "Invalid Request. No staff was specified."; }
            model.id = id;
            if (sd == null) { model.sd = DateTime.Now.AddMonths(-12); } else { model.sd = sd.Value; }
            if (ed == null) { model.ed = DateTime.Now.AddDays(1); } else { model.ed = ed.Value; }
            var taskOwner = await _ermService.GetEmployeeByIdAsync(model.id);
            if (taskOwner != null)
            {
                model.TaskOwnerLocation = taskOwner.LocationName;
                model.TaskOwnerName = taskOwner.FullName;
                model.TaskOwnerUnit = taskOwner.UnitName;
            }
            if (!string.IsNullOrWhiteSpace(model.kw))
            {
                model.TaskItemList = await _wspService.GetTaskItemsWithSameKeyword(model.id, model.kw, model.sd, model.ed);
            }
            return View(model);
        }

        #endregion

        #region Task Submissions Controller Action Methods

        public IActionResult SubmitTaskFolder(long id)
        {
            SubmitTaskFolderViewModel model = new SubmitTaskFolderViewModel();
            model.TaskFolderID = id;
            model.FromEmployeeName = HttpContext.User.Identity.Name;
            var claims = HttpContext.User.Claims.ToList();
            model.FromEmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitTaskFolder(SubmitTaskFolderViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string _submissionPurpose = string.Empty;
                    var employee = await _ermService.GetEmployeeByNameAsync(model.ToEmployeeName);
                    if (employee == null || string.IsNullOrWhiteSpace(employee.EmployeeID))
                    {
                        model.ViewModelErrorMessage = "No record was found for the selected employee. Please select an employee from the dropdown. Do not type in the name.";
                        return View(model);
                    }
                    else { model.ToEmployeeID = employee.EmployeeID; }

                    FolderSubmission submission = new FolderSubmission();
                    submission.IsActioned = false;
                    submission.Comment = model.Comment;
                    submission.DateActioned = null;
                    submission.DateSubmitted = DateTime.UtcNow;
                    submission.FromEmployeeId = model.FromEmployeeID;
                    submission.FromEmployeeName = model.FromEmployeeName;
                    submission.SubmissionType = (WorkItemSubmissionType)model.SubmissionTypeID;
                    submission.FolderId = model.TaskFolderID;
                    submission.ToEmployeeId = model.ToEmployeeID;
                    submission.ToEmployeeName = model.ToEmployeeName;
                    if (submission.SubmissionType == WorkItemSubmissionType.Review)
                    {
                        var entities = await _wspService.GetTasksByFolderIdAsync(submission.FolderId);
                        if (entities != null && entities.Count > 0)
                        {
                            foreach (var task in entities)
                            {
                                if (string.IsNullOrWhiteSpace(task.MoreInformation))
                                {
                                    model.ViewModelErrorMessage = "One or more tasks in this folder have blank Resolution fields. All tasks must have Resolutions before you can submit for Close Out. Please add Resolution to all tasks and try again. ";
                                    return View(model);
                                }

                                if (task.ProgressStatus == WorkItemProgressStatus.NotStarted)
                                {
                                    model.ViewModelErrorMessage = "One or more tasks in this folder still have Progress Status of [Not Yet Started]. Kindly update the Progress Status and try again. ";
                                    return View(model);
                                }
                            }
                        }
                    }

                    bool IsAdded = await _wspService.AddFolderSubmissionAsync(submission);
                    if (IsAdded)
                    {
                        await _wspService.UpdateWorkItemFolderLockStatusAsync(model.TaskFolderID, true, model.FromEmployeeName);
                        if (submission.SubmissionType == WorkItemSubmissionType.Review)
                        {
                            _submissionPurpose = "Evaluation and Close Out";
                            TaskEvaluationHeader t = new TaskEvaluationHeader();
                            var entity = await _wspService.GetTaskEvaluationHeaderAsync(model.TaskFolderID, model.ToEmployeeID);
                            if (entity == null || entity.Id < 1)
                            {
                                Employee e = await _ermService.GetEmployeeByIdAsync(model.FromEmployeeID);
                                t.EvaluatorId = model.ToEmployeeID;
                                t.TaskFolderId = model.TaskFolderID;
                                t.TaskOwnerDeptId = e.DepartmentID ?? 0;
                                t.TaskOwnerId = model.FromEmployeeID;
                                t.TaskOwnerLocationId = e.LocationID ?? 0;
                                t.TaskOwnerUnitId = e.UnitID ?? 0;
                                t.TotalNumberOfTasks = await _wspService.GetTaskItemsCountByFolderIdAsync(model.TaskFolderID);
                                await _wspService.CreateTaskEvaluationHeaderAsync(t);
                            }
                            else
                            {
                                _submissionPurpose = "Approval";
                                entity.TotalNumberOfTasks = await _wspService.GetTaskItemsCountByFolderIdAsync(model.TaskFolderID);
                                await _wspService.UpdateTaskEvaluationHeaderAsync(entity);
                            }
                        }


                        //============= Notification Code Starts Here =========================//

                        Employee sender = new Employee();
                        sender = await _ermService.GetEmployeeByIdAsync(model.FromEmployeeID);
                        Employee receiver = new Employee();
                        receiver = await _ermService.GetEmployeeByIdAsync(model.ToEmployeeID);

                        //===== Send Notificiation Message  ========//
                        Message message = new Message
                        {
                            MessageID = Guid.NewGuid().ToString(),
                            RecipientID = receiver.EmployeeID,
                            RecipientName = receiver.FullName,
                            SentBy = sender.FullName
                        };

                        //===== Send Email Notifications =========//
                        bool emailCopySent = false;
                        UtilityHelper utilityHelper = new UtilityHelper(_configuration);
                        EmailModel recipientEmailCopy = new EmailModel();
                        recipientEmailCopy.RecipientName = receiver.FullName;
                        if (!string.IsNullOrWhiteSpace(receiver.OfficialEmail))
                        {
                            recipientEmailCopy.RecipientEmail = receiver.OfficialEmail;
                        }
                        else
                        {
                            recipientEmailCopy.RecipientEmail = receiver.Email;
                        }

                        if (!string.IsNullOrWhiteSpace(sender.OfficialEmail))
                        {
                            recipientEmailCopy.SenderEmail = sender.OfficialEmail;
                        }
                        else
                        {
                            recipientEmailCopy.SenderEmail = sender.Email;
                        }

                        recipientEmailCopy.SenderName = sender.FullName;
                        recipientEmailCopy.Subject = "A Task Folder Has Been Submitted To You.";
                        recipientEmailCopy.HtmlContent = UtilityHelper.GetWorkspaceTaskSubmissionNotificationEmailHtmlContent(receiver.FullName, sender.FullName, _submissionPurpose);
                        recipientEmailCopy.PlainContent = UtilityHelper.GetWorkspaceTaskSubmissionNotificationEmailPlainContent(receiver.FullName, sender.FullName, _submissionPurpose);

                        message.Subject = "A Task Folder Has Been Submitted To You.";
                        message.MessageBody = UtilityHelper.GetWorkspaceTaskSubmissionNotificationMessageContent(receiver.FullName, sender.FullName, _submissionPurpose);

                        bool messageSent = await _baseModelService.SendMessageAsync(message);
                        if (!string.IsNullOrWhiteSpace(recipientEmailCopy.RecipientEmail))
                        {
                            emailCopySent = await utilityHelper.SendEmailWithSendGridAsync(recipientEmailCopy);
                        }
                        //============= Notification Code Ends Here ===========================//

                        return RedirectToAction("MyTaskFolders", new { id = model.FromEmployeeID });
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> SubmitHistory(long fd, string fn)
        {
            SubmitHistoryViewModel model = new SubmitHistoryViewModel();
            model.FolderID = fd;
            model.FolderName = fn;
            if (model.FolderID > 0)
            {
                var entities = await _wspService.GetFolderSubmissionsByFolderIdAsync(model.FolderID);
                if (entities != null && entities.Count > 0)
                {
                    model.FolderSumissions = entities;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> SubmittedToMe(SubmittedToMeViewModel model)
        {
            if (model == null) { model = new SubmittedToMeViewModel(); }
            var claims = HttpContext.User.Claims.ToList();
            model.EmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            if (!string.IsNullOrWhiteSpace(model.EmployeeID))
            {
                var entities = await _wspService.SearchFolderSubmissionsAsync(model.EmployeeID, model.SubmittedYear, model.SubmittedMonth, model.FromEmployeeName);
                if (entities != null) { model.SubmissionList = entities.ToList(); }
                if (model.SubmittedYear < 2025) { model.SubmittedYear = DateTime.Now.Year; }
            }
            else { model.ViewModelErrorMessage = "Sorry, it appears your session expired. Please login again to continue."; }
            return View(model);
        }
        public async Task<IActionResult> SubmittedTasks(long id, string fn, long sd, string ed, string tp, string od)
        {
            SubmittedTasksViewModel model = new SubmittedTasksViewModel();
            model.FolderID = id;
            model.FolderName = fn;
            model.FolderSubmissionID = sd;
            model.FolderOwnerID = od;
            //model.PurposeOfSubmission = tp;

            FolderSubmission submission = await _wspService.GetFolderSubmissionByIdAsync(sd);
            if (submission != null)
            {
                model.FolderIsReturned = submission.IsActioned;
            }

            if (!string.IsNullOrWhiteSpace(model.FolderOwnerID))
            {
                var taskOwner = await _ermService.GetEmployeeByIdAsync(model.FolderOwnerID);
                if (taskOwner != null)
                {
                    model.FolderOwnerName = taskOwner.FullName;
                    model.FolderOwnerUnitName = taskOwner.UnitName;
                    model.FolderOwnerLocationName = taskOwner.LocationName;
                    model.FolderOwnerDesignation = taskOwner.CurrentDesignation;
                }
            }

            if (!string.IsNullOrWhiteSpace(ed))
            {
                model.SubmittedToEmployeeID = ed;
            }
            else
            {
                var claims = HttpContext.User.Claims.ToList();
                model.SubmittedToEmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            }

            model.SubmittedToEmployeeName = HttpContext.User.Identity.Name;
            if (!string.IsNullOrEmpty(tp))
            {
                if (tp == "Approval")
                {
                    model.PurposeOfSubmission = WorkItemSubmissionType.Approval;
                    if (id > 0)
                    {
                        var entities = await _wspService.GetTasksByFolderIdAsync(model.FolderID);
                        if (entities != null && entities.Count > 0)
                        {
                            model.TaskItems = entities;
                        }
                    }
                }
                else if (tp == "Evaluation")
                {
                    //model.PurposeOfSubmission = WorkItemSubmissionType.Evaluation;
                    //TaskEvaluationHeader evaluationHeader = await _workspaceService.GetTaskEvaluationHeaderAsync(id, ed);
                    //if (evaluationHeader != null && evaluationHeader.Id > 0)
                    //{
                    //    model.TaskEvaluationHeaderID = evaluationHeader.Id;
                    //}
                    //else
                    //{
                    //    Employee taskOwner = new Employee();
                    //    taskOwner = await _ermService.GetEmployeeByIdAsync(od);
                    //    if (!string.IsNullOrWhiteSpace(taskOwner.FullName))
                    //    {

                    //        evaluationHeader.TaskListId = id;
                    //        evaluationHeader.EvaluatorId = ed;
                    //        evaluationHeader.TaskOwnerId = od;
                    //        evaluationHeader.TaskOwnerUnitId = taskOwner.UnitID ?? 0;
                    //        evaluationHeader.TaskOwnerDeptId = taskOwner.DepartmentID ?? 0;
                    //        evaluationHeader.TaskOwnerLocationId = taskOwner.LocationID ?? 0;
                    //        evaluationHeader.EvaluationDate = DateTime.UtcNow;
                    //        model.TaskEvaluationHeaderID = await _workspaceService.CreateTaskEvaluationHeaderAsync(evaluationHeader);
                    //    }

                    //}
                }
            }
            else
            {
                if (id > 0)
                {
                    var entities = await _wspService.GetTasksByFolderIdAsync(model.FolderID);
                    if (entities != null && entities.Count > 0)
                    {
                        model.TaskItems = entities;
                    }
                }
            }

            return View(model);
        }
        public async Task<IActionResult> SubmittedEvaluations(long id, string fn, long sd, string ed, string od)
        {
            SubmittedEvaluationsViewModel model = new SubmittedEvaluationsViewModel();
            model.FolderID = id;
            model.FolderSubmissionID = sd;
            model.SubmittedToEmployeeID = ed;
            model.EvaluatorID = ed;
            model.FolderOwnerID = od;
            model.FolderName = fn;
            model.PurposeOfSubmission = WorkItemSubmissionType.Review;
            model.SubmittedToEmployeeName = HttpContext.User.Identity.Name;

            FolderSubmission submission = await _wspService.GetFolderSubmissionByIdAsync(sd);
            if (submission != null)
            {
                model.FolderIsReturned = submission.IsActioned;
            }

            if (!string.IsNullOrWhiteSpace(model.FolderOwnerID))
            {
                var taskOwner = await _ermService.GetEmployeeByIdAsync(model.FolderOwnerID);
                if (taskOwner != null)
                {
                    model.FolderOwnerName = taskOwner.FullName;
                    model.FolderOwnerUnitName = taskOwner.UnitName;
                    model.FolderOwnerLocationName = taskOwner.LocationName;
                    model.FolderOwnerDesignation = taskOwner.CurrentDesignation;
                }
            }
            if (string.IsNullOrWhiteSpace(model.EvaluatorID) && string.IsNullOrWhiteSpace(model.SubmittedToEmployeeID))
            {
                var claims = HttpContext.User.Claims.ToList();
                string current_employeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                model.EvaluatorID = current_employeeId;
                model.SubmittedToEmployeeID = current_employeeId;
            }
            model.PurposeOfSubmission = WorkItemSubmissionType.Review;
            TaskEvaluationHeader evaluationHeader = await _wspService.GetTaskEvaluationHeaderAsync(model.FolderID, model.EvaluatorID);
            if (evaluationHeader != null && evaluationHeader.Id > 0)
            {
                model.TaskEvaluationHeaderID = evaluationHeader.Id;
            }
            else
            {
                Employee taskOwner = new Employee();
                taskOwner = await _ermService.GetEmployeeByIdAsync(od);
                if (!string.IsNullOrWhiteSpace(taskOwner.FullName))
                {
                    evaluationHeader.TaskFolderId = id;
                    evaluationHeader.EvaluatorId = ed;
                    evaluationHeader.TaskOwnerId = od;
                    evaluationHeader.TaskOwnerUnitId = taskOwner.UnitID ?? 0;
                    evaluationHeader.TaskOwnerDeptId = taskOwner.DepartmentID ?? 0;
                    evaluationHeader.TaskOwnerLocationId = taskOwner.LocationID ?? 0;
                    model.TaskEvaluationHeaderID = await _wspService.CreateTaskEvaluationHeaderAsync(evaluationHeader);
                }
            }

            if (id > 0)
            {
                var entities = await _wspService.GetTaskItemEvaluationsAsync(model.FolderID, model.EvaluatorID);
                if (entities != null && entities.Count > 0)
                {
                    model.TaskItemEvaluations = entities;
                }
            }

            return View(model);
        }
        #endregion

        #region Task Evaluation Controller Action Methods

        public async Task<IActionResult> ReturnTaskItem(long id, long fd, long sd, string od, string fn, long hd, long dd)
        {
            ReturnTaskItemViewModel model = new ReturnTaskItemViewModel();
            model.TaskItemID = id;
            model.TaskFolderID = fd;
            model.TaskFolderName = fn;
            model.TaskOwnerID = od;
            model.FolderSubmissionID = sd;
            model.TaskEvaluationHeaderID = hd;
            model.TaskEvaluationDetailID = dd;

            model.FromEmployeeName = HttpContext.User.Identity.Name;
            var claims = HttpContext.User.Claims.ToList();
            model.FromEmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

            var entity = await _wspService.GetTaskEvaluationHeaderAsync(model.TaskFolderID, model.FromEmployeeID);
            if (entity != null && entity.Id > 0)
            {
                model.TaskEvaluationHeaderID = entity.Id;
                var evaluationDetail = await _wspService.GetTaskEvaluationDetailAsync(model.TaskEvaluationHeaderID, model.TaskItemID);
                if (evaluationDetail != null && evaluationDetail.TaskEvaluationDetailId > 0)
                {
                    model.TaskEvaluationDetailID = evaluationDetail.TaskEvaluationDetailId;
                }
            }

            var entities = await _wspService.GetWorkItemReturnReasonsAsync();
            if (entities != null && entities.Count > 0)
            {
                ViewBag.ReturnReasons = new SelectList(entities, "Description", "Description");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnTaskItem(ReturnTaskItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                TaskEvaluationDetail detail = new TaskEvaluationDetail();
                detail.TaskItemId = model.TaskItemID;
                detail.TaskFolderId = model.TaskFolderID;
                detail.TaskEvaluatorId = model.FromEmployeeID;
                detail.TaskEvaluatorName = HttpContext.User.Identity.Name;
                detail.QualityScore = model.QualityRating ?? 0;
                detail.TaskEvaluationHeaderId = model.TaskEvaluationHeaderID;
                detail.TaskEvaluationDetailId = model.TaskEvaluationDetailID;

                bool _taskEvaluationIsUpdated = false;

                try
                {
                    if (detail.TaskItemId < 1) { throw new Exception("Required parameter [TaskItemId] is missing."); }
                    if (string.IsNullOrWhiteSpace(detail.TaskEvaluatorId))
                    {
                        var claims = HttpContext.User.Claims.ToList();
                        string current_employeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                        if (string.IsNullOrWhiteSpace(current_employeeId)) { throw new Exception("Sorry, it appears your session has expired. Please login again and try again."); }
                        detail.TaskEvaluatorId = current_employeeId;
                    }

                    if (model.ExemptFromEvaluation == false)
                    {
                        if (detail.TaskEvaluationDetailId > 0)
                        {
                            var old_detail = await _wspService.GetTaskEvaluationDetailByIdAsync(detail.TaskEvaluationDetailId);
                            if (old_detail != null)
                            {
                                detail = old_detail;
                                detail.QualityScore = 0;
                                detail.CompletionScore = 0;
                                _taskEvaluationIsUpdated = await _wspService.UpdateTaskEvaluationDetailAsync(detail);
                            }
                            else
                            {
                                throw new Exception("Sorry no evaluation record was found for this task. Please try again.");
                            }
                        }
                        else
                        {
                            TaskItem task = await _wspService.GetTaskItemByIdAsync(detail.TaskItemId);
                            if (task == null || task.Id < 1) { throw new Exception("Ooops! No record was found for this task item. Please try again."); }
                            detail.TaskItemId = task.Id;
                            detail.CompletionScore = 0;
                            detail.EvaluationDate = DateTime.Now;
                            detail.TaskOwnerDeptId = task.DepartmentId ?? 0;
                            detail.TaskOwnerUnitId = task.UnitId ?? 0;
                            detail.TaskOwnerLocationId = task.LocationId ?? 0;
                            detail.TaskOwnerId = task.TaskOwnerId;

                            TaskEvaluationHeader evaluationHeader = new TaskEvaluationHeader();
                            if (model.TaskEvaluationHeaderID < 1)
                            {
                                evaluationHeader = _wspService.GetTaskEvaluationHeaderAsync(detail.TaskFolderId, detail.TaskEvaluatorId).Result;
                                if (evaluationHeader == null || evaluationHeader.Id < 1)
                                {
                                    evaluationHeader = new TaskEvaluationHeader();
                                    evaluationHeader.EvaluatorId = detail.TaskEvaluatorId;
                                    evaluationHeader.TaskFolderId = detail.TaskFolderId;
                                    evaluationHeader.TaskOwnerDeptId = task.DepartmentId ?? 0;
                                    evaluationHeader.TaskOwnerId = task.TaskOwnerId;
                                    evaluationHeader.TaskOwnerLocationId = task.LocationId ?? 0;
                                    evaluationHeader.TaskOwnerUnitId = task.UnitId ?? 0;
                                    long evaluationHeaderId = await _wspService.CreateTaskEvaluationHeaderAsync(evaluationHeader);
                                    if (evaluationHeaderId > 0) { detail.TaskEvaluationHeaderId = evaluationHeaderId; }
                                }
                                else
                                {
                                    detail.TaskEvaluationHeaderId = evaluationHeader.Id;
                                }
                            }
                            _taskEvaluationIsUpdated = await _wspService.AddTaskEvaluationDetailAsync(detail);
                        }
                    }

                    TaskEvaluationReturns taskEvaluationReturns = model.Convert();
                    WorkItemNote note = new WorkItemNote
                    {
                        NoteContent = model.ReasonDetails,
                        NoteWrittenBy = model.FromEmployeeName,
                        TaskItemId = model.TaskItemID,
                        NoteTime = DateTime.Now,
                    };
                    bool NoteIsAdded = await _wspService.AddWorkItemNoteAsync(note);
                    if (NoteIsAdded)
                    {
                        if (model.CloseTask)
                        {
                            return RedirectToAction("SubmittedEvaluations", new { id = model.TaskFolderID, fn = model.TaskFolderName, sd = model.FolderSubmissionID, ed = model.FromEmployeeID, od = model.TaskOwnerID });
                        }
                        else
                        {
                            Employee taskOwner = await _ermService.GetEmployeeByIdAsync(taskEvaluationReturns.TaskOwnerId);
                            if (taskOwner != null)
                            {
                                taskEvaluationReturns.TaskOwnerUnitId = taskOwner.UnitID;
                                taskEvaluationReturns.TaskOwnerDepartmentId = taskOwner.DepartmentID;
                                taskEvaluationReturns.TaskOwnerLocationId = taskOwner.LocationID;
                                taskEvaluationReturns.ExemptFromEvaluation = model.ExemptFromEvaluation;
                            }

                            bool _evaluationReturnIsAdded = await _wspService.ReturnTaskEvaluationAsync(taskEvaluationReturns);
                            if (_evaluationReturnIsAdded)
                            {
                                return RedirectToAction("SubmittedEvaluations", new { id = model.TaskFolderID, fn = model.TaskFolderName, sd = model.FolderSubmissionID, ed = model.FromEmployeeID, od = model.TaskOwnerID });
                            }
                            else { model.ViewModelErrorMessage = "Sorry,an error was encountered. Please try again."; }
                        }

                    }
                    else { model.ViewModelErrorMessage = "Sorry,an error was encountered. Please try again."; }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ReturnDeclinedTaskItem(long id, long fd, long sd, string od, string fn, long hd, long dd)
        {
            ReturnTaskItemViewModel model = new ReturnTaskItemViewModel();
            model.TaskItemID = id;
            model.TaskFolderID = fd;
            model.TaskFolderName = fn;
            model.TaskOwnerID = od;
            model.FolderSubmissionID = sd;
            model.TaskEvaluationHeaderID = hd;
            model.TaskEvaluationDetailID = dd;

            model.FromEmployeeName = HttpContext.User.Identity.Name;
            var claims = HttpContext.User.Claims.ToList();
            model.FromEmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

            var entity = await _wspService.GetTaskEvaluationHeaderAsync(model.TaskFolderID, model.FromEmployeeID);
            if (entity != null && entity.Id > 0)
            {
                model.TaskEvaluationHeaderID = entity.Id;
                var evaluationDetail = await _wspService.GetTaskEvaluationDetailAsync(model.TaskEvaluationHeaderID, model.TaskItemID);
                if (evaluationDetail != null && evaluationDetail.TaskEvaluationDetailId > 0)
                {
                    model.TaskEvaluationDetailID = evaluationDetail.TaskEvaluationDetailId;
                }
            }
            model.ReasonType = "Unapproved Task";
            model.QualityRating = 1;
            model.ExemptFromEvaluation = false;
            //var entities = await _wspService.GetWorkItemReturnReasonsAsync();
            //if (entities != null && entities.Count > 0)
            //{
            //    ViewBag.ReturnReasons = new SelectList(entities, "Description", "Description");
            //}
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnDeclinedTaskItem(ReturnTaskItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                TaskEvaluationDetail detail = new TaskEvaluationDetail();
                detail.TaskItemId = model.TaskItemID;
                detail.TaskFolderId = model.TaskFolderID;
                detail.TaskEvaluatorId = model.FromEmployeeID;
                detail.TaskEvaluatorName = HttpContext.User.Identity.Name;
                detail.QualityScore = model.QualityRating ?? 0;
                detail.TaskEvaluationHeaderId = model.TaskEvaluationHeaderID;
                detail.TaskEvaluationDetailId = model.TaskEvaluationDetailID;

                bool _taskEvaluationIsUpdated = false;

                try
                {
                    if (detail.TaskItemId < 1) { throw new Exception("Required parameter [TaskItemId] is missing."); }
                    await _wspService.UpdateTaskItemApprovalStatusAsync(model.TaskItemID, ApprovalStatus.Declined, model.FromEmployeeName);

                    if (string.IsNullOrWhiteSpace(detail.TaskEvaluatorId))
                    {
                        var claims = HttpContext.User.Claims.ToList();
                        string current_employeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                        if (string.IsNullOrWhiteSpace(current_employeeId)) { throw new Exception("Sorry, it appears your session has expired. Please login again and try again."); }
                        detail.TaskEvaluatorId = current_employeeId;
                    }

                    if (model.ExemptFromEvaluation == false)
                    {
                        if (detail.TaskEvaluationDetailId > 0)
                        {
                            var old_detail = await _wspService.GetTaskEvaluationDetailByIdAsync(detail.TaskEvaluationDetailId);
                            if (old_detail != null)
                            {
                                detail = old_detail;
                                detail.QualityScore = 0;
                                detail.CompletionScore = 0;
                                _taskEvaluationIsUpdated = await _wspService.UpdateTaskEvaluationDetailAsync(detail);
                            }
                            else
                            {
                                throw new Exception("Sorry no evaluation record was found for this task. Please try again.");
                            }
                        }
                        else
                        {
                            TaskItem task = await _wspService.GetTaskItemByIdAsync(detail.TaskItemId);
                            if (task == null || task.Id < 1) { throw new Exception("Ooops! No record was found for this task item. Please try again."); }
                            detail.TaskItemId = task.Id;
                            detail.CompletionScore = 0;
                            detail.EvaluationDate = DateTime.Now;
                            detail.TaskOwnerDeptId = task.DepartmentId ?? 0;
                            detail.TaskOwnerUnitId = task.UnitId ?? 0;
                            detail.TaskOwnerLocationId = task.LocationId ?? 0;
                            detail.TaskOwnerId = task.TaskOwnerId;

                            TaskEvaluationHeader evaluationHeader = new TaskEvaluationHeader();
                            if (model.TaskEvaluationHeaderID < 1)
                            {
                                evaluationHeader = _wspService.GetTaskEvaluationHeaderAsync(detail.TaskFolderId, detail.TaskEvaluatorId).Result;
                                if (evaluationHeader == null || evaluationHeader.Id < 1)
                                {
                                    evaluationHeader = new TaskEvaluationHeader();
                                    evaluationHeader.EvaluatorId = detail.TaskEvaluatorId;
                                    evaluationHeader.TaskFolderId = detail.TaskFolderId;
                                    evaluationHeader.TaskOwnerDeptId = task.DepartmentId ?? 0;
                                    evaluationHeader.TaskOwnerId = task.TaskOwnerId;
                                    evaluationHeader.TaskOwnerLocationId = task.LocationId ?? 0;
                                    evaluationHeader.TaskOwnerUnitId = task.UnitId ?? 0;
                                    long evaluationHeaderId = await _wspService.CreateTaskEvaluationHeaderAsync(evaluationHeader);
                                    if (evaluationHeaderId > 0) { detail.TaskEvaluationHeaderId = evaluationHeaderId; }
                                }
                                else
                                {
                                    detail.TaskEvaluationHeaderId = evaluationHeader.Id;
                                }
                            }
                            _taskEvaluationIsUpdated = await _wspService.AddTaskEvaluationDetailAsync(detail);
                        }
                    }

                    TaskEvaluationReturns taskEvaluationReturns = model.Convert();
                    WorkItemNote note = new WorkItemNote
                    {
                        NoteContent = model.ReasonDetails,
                        NoteWrittenBy = model.FromEmployeeName,
                        TaskItemId = model.TaskItemID,
                        NoteTime = DateTime.Now,
                    };
                    bool NoteIsAdded = await _wspService.AddWorkItemNoteAsync(note);
                    if (NoteIsAdded)
                    {
                        Employee taskOwner = await _ermService.GetEmployeeByIdAsync(taskEvaluationReturns.TaskOwnerId);
                        if (taskOwner != null)
                        {
                            taskEvaluationReturns.TaskOwnerUnitId = taskOwner.UnitID;
                            taskEvaluationReturns.TaskOwnerDepartmentId = taskOwner.DepartmentID;
                            taskEvaluationReturns.TaskOwnerLocationId = taskOwner.LocationID;
                            taskEvaluationReturns.ExemptFromEvaluation = model.ExemptFromEvaluation;
                        }

                        bool _evaluationReturnIsAdded = await _wspService.ReturnTaskEvaluationAsync(taskEvaluationReturns, true);
                        if (_evaluationReturnIsAdded)
                        {
                            return RedirectToAction("SubmittedEvaluations", new { id = model.TaskFolderID, fn = model.TaskFolderName, sd = model.FolderSubmissionID, ed = model.FromEmployeeID, od = model.TaskOwnerID });
                        }
                        else { model.ViewModelErrorMessage = "Sorry,an error was encountered. Please try again."; }
                    }
                    else { model.ViewModelErrorMessage = "Sorry,an error was encountered. Please try again."; }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ReviewResults(string id, DateTime? sd, DateTime? ed)
        {
            MyReviewResultsViewModel model = new MyReviewResultsViewModel();
            List<TaskEvaluationSummary> summaryList = new List<TaskEvaluationSummary>();
            model.sd = sd ?? DateTime.Now.AddMonths(-3);
            model.ed = ed ?? DateTime.Now;
            if (string.IsNullOrWhiteSpace(id))
            {
                var claims = HttpContext.User.Claims.ToList();
                string current_employeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                model.id = current_employeeId;
            }
            else { model.id = id; }

            summaryList = await _wspService.GetTaskEvaluationSummaryByTaskOwnerIdAsync(model.id, model.sd, model.ed);
            if (summaryList != null && summaryList.Count > 0)
            {
                model.EvaluationSummaryList = summaryList;
            }
            return View(model);
        }

        public async Task<IActionResult> ReviewResultDetails(int id)
        {
            ReviewResultDetailsViewModel model = new ReviewResultDetailsViewModel();
            List<TaskEvaluationDetail> evaluationDetails = new List<TaskEvaluationDetail>();
            if (id < 1) { model.EvaluationDetailList = evaluationDetails; }
            else
            {
                model.TaskEvaluationHeaderID = id;
                var entities = await _wspService.GetTaskEvaluationDetailsAsync(model.TaskEvaluationHeaderID);
                if (entities != null && entities.Count > 0)
                {
                    model.EvaluationDetailList = entities;
                    model.TaskFolderID = entities[0].TaskFolderId;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> TeamReviewResults(string id, DateTime? sd, DateTime? ed)
        {
            MyReviewResultsViewModel model = new MyReviewResultsViewModel();
            List<TaskEvaluationSummary> summaryList = new List<TaskEvaluationSummary>();
            List<EmployeeReportLine> directReports = new List<EmployeeReportLine>();

            model.sd = sd ?? DateTime.Now.AddMonths(-3);
            model.ed = ed ?? DateTime.Now.AddMonths(1);
            model.id = id;
            if (!string.IsNullOrWhiteSpace(model.id))
            {
                summaryList = await _wspService.GetTaskEvaluationSummaryByTaskOwnerIdAsync(model.id, model.sd, model.ed);
                if (summaryList != null && summaryList.Count > 0)
                {
                    model.EvaluationSummaryList = summaryList;
                }
            }

            var claims = HttpContext.User.Claims.ToList();
            string reportsToEmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

            var employee_entities = await _ermService.GetEmployeeReportsByReportsToEmployeeIdAsync(reportsToEmployeeID);
            if (employee_entities != null && employee_entities.Count > 0)
            {
                directReports = employee_entities;
            }
            ViewBag.ReportsList = new SelectList(directReports, "EmployeeID", "EmployeeName", model.id);

            return View(model);
        }

        #endregion

        #region Reports
        [Authorize(Roles = "WSPVWAEER, WSPVWAETK, XYALLACCZ")]
        public async Task<IActionResult> TaskReviewResults(string id, string sn, DateTime? sd = null, DateTime? ed = null, string src = null)
        {
            MyReviewResultsViewModel model = new MyReviewResultsViewModel();
            List<TaskEvaluationSummary> summaryList = new List<TaskEvaluationSummary>();
            List<EmployeeReportLine> directReports = new List<EmployeeReportLine>();

            model.sd = sd ?? DateTime.Now.AddMonths(-3);
            model.ed = ed ?? DateTime.Now.AddMonths(1);
            model.id = id;
            model.sn = sn;
            model.SourcePage = src;

            if (!string.IsNullOrWhiteSpace(model.id))
            {
                summaryList = await _wspService.GetTaskEvaluationSummaryByTaskOwnerIdAsync(model.id, model.sd, model.ed);
                if (summaryList != null && summaryList.Count > 0)
                {
                    model.EvaluationSummaryList = summaryList;
                }
            }
            else if (string.IsNullOrWhiteSpace(model.id) && !string.IsNullOrWhiteSpace(model.sn))
            {
                Employee employee = await _ermService.GetEmployeeByNameAsync(model.sn);
                if (employee != null && !string.IsNullOrWhiteSpace(employee.EmployeeID))
                {
                    model.id = employee.EmployeeID;
                }
                summaryList = await _wspService.GetTaskEvaluationSummaryByTaskOwnerIdAsync(model.id, model.sd, model.ed);
                if (summaryList != null && summaryList.Count > 0)
                {
                    model.EvaluationSummaryList = summaryList;
                }
            }

            return View(model);
        }

        public async Task<IActionResult> TeamProductivityReport(string id = null, DateTime? sd = null, DateTime? ed = null)
        {
            ProductivityReportViewModel model = new ProductivityReportViewModel();
            model.EvaluationScoresList = new List<TaskEvaluationScores>();
            List<EmployeeReportLine> directReports = new List<EmployeeReportLine>();
            model.id = id;
            model.sd = sd ?? DateTime.Now.AddMonths(-3);
            model.ed = ed ?? DateTime.Now.AddMonths(1);

            if (!string.IsNullOrWhiteSpace(model.id))
            {
                TaskEvaluationScores score = await _wspService.GetTaskEvaluationScoresByOwnerId(model.id, model.sd, model.ed);
                if (score != null)
                {
                    model.EvaluationScoresList.Add(score);
                }
            }

            var claims = HttpContext.User.Claims.ToList();
            string reportsToEmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

            var employee_entities = await _ermService.GetEmployeeReportsByReportsToEmployeeIdAsync(reportsToEmployeeID);
            if (employee_entities != null && employee_entities.Count > 0)
            {
                directReports = employee_entities;
            }
            ViewBag.ReportsList = new SelectList(directReports, "EmployeeID", "EmployeeName", model.id);

            return View(model);
        }

        [Authorize(Roles = "WSPVWAEER, WSPVWAETK, XYALLACCZ")]
        public async Task<IActionResult> ProductivityReport(string sn = null, int? ud = null, int? dd = null, int? ld = null, int? gd = null, DateTime? sd = null, DateTime? ed = null)
        {
            ProductivityReportViewModel model = new ProductivityReportViewModel();
            model.EvaluationScoresList = new List<TaskEvaluationScores>();
            List<EmployeeReportLine> directReports = new List<EmployeeReportLine>();
            model.sn = sn;
            model.ud = ud;
            model.dd = dd;
            model.ld = ld;
            model.gd = gd;
            model.sd = sd ?? DateTime.Now.AddMonths(-3);
            model.ed = ed ?? DateTime.Now.AddMonths(1);

            try
            {
                var entities = await _wspService.GetTaskEvaluationScoresAsync(model.sn, model.ud, model.dd, model.ld, model.gd, model.sd, model.ed);
                if (entities != null && entities.Count > 0)
                {
                    model.EvaluationScoresList = entities;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var grouploc_entities = await _globalSettingsService.GetAllLocationGroupsAsync();
            if (grouploc_entities != null && grouploc_entities.Count > 0)
            {
                ViewBag.LocationGroupList = new SelectList(grouploc_entities, "LocationGroupId", "LocationGroupName", gd);
            }

            var loc_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (loc_entities != null && loc_entities.Count > 0)
            {
                ViewBag.LocationList = new SelectList(loc_entities, "LocationID", "LocationName", ld);
            }

            var dept_entities = await _globalSettingsService.GetDepartmentsAsync();
            if (dept_entities != null && dept_entities.Count > 0)
            {
                ViewBag.DepartmentList = new SelectList(dept_entities, "DepartmentID", "DepartmentName", dd);
            }

            var unit_entities = await _globalSettingsService.GetUnitsAsync();
            if (unit_entities != null && unit_entities.Count > 0)
            {
                ViewBag.UnitList = new SelectList(unit_entities, "UnitID", "UnitName", ud);
            }

            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = model.ViewModelErrorMessage + ' ' + TempData["ErrorMessage"].ToString();
            }

            return View(model);
        }

        [Authorize(Roles = "WSPVWAEER, WSPVWAETK, XYALLACCZ")]
        public async Task<IActionResult> FoldersReport(string id = null, int? ud = null, int? dd = null, int? ld = null, int? gd = null, DateTime? sd = null, DateTime? ed = null, int vs = 2, string sn = null)
        {
            EmployeesFoldersReportViewModel model = new EmployeesFoldersReportViewModel();
            model.FoldersList = new List<WorkItemFolder>();
            model.EmployeesList = new List<EmployeeRoll>();
            model.id = id;
            model.sn = sn;
            model.ud = ud;
            model.dd = dd;
            model.ld = ld;
            model.gd = gd;
            model.vs = vs;
            model.sd = sd ?? DateTime.Now.AddMonths(-1);
            model.ed = ed ?? DateTime.Now.AddMonths(1);

            var _folderEntities = await _wspService.GetWorkItemFoldersAsync(model.sd.Value, model.ed.Value, model.vs, model.ld, model.dd, model.ud, model.id, model.sn, model.gd);
            if (_folderEntities != null) { model.FoldersList = _folderEntities; }

            var grouploc_entities = await _globalSettingsService.GetAllLocationGroupsAsync();
            if (grouploc_entities != null && grouploc_entities.Count > 0)
            {
                ViewBag.LocationGroupList = new SelectList(grouploc_entities, "LocationGroupId", "LocationGroupName", gd);
            }

            var loc_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (loc_entities != null && loc_entities.Count > 0)
            {
                ViewBag.LocationList = new SelectList(loc_entities, "LocationID", "LocationName", ld);
            }

            var dept_entities = await _globalSettingsService.GetDepartmentsAsync();
            if (dept_entities != null && dept_entities.Count > 0)
            {
                ViewBag.DepartmentList = new SelectList(dept_entities, "DepartmentID", "DepartmentName", dd);
            }

            var unit_entities = await _globalSettingsService.GetUnitsAsync();
            if (unit_entities != null && unit_entities.Count > 0)
            {
                ViewBag.UnitList = new SelectList(unit_entities, "UnitID", "UnitName", ud);
            }

            var emp_entities = await _ermService.GetEmployeeRollsAsync(null, model.ld, model.dd, model.ud, model.id);
            if (emp_entities != null && emp_entities.Count > 0)
            {
                model.EmployeesList = emp_entities;
                ViewBag.EmployeesList = new SelectList(emp_entities, "EmployeeID", "FullName", id);
            }

            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }
            return View(model);
        }

        public async Task<IActionResult> FolderTaskList(long id, string fn)
        {
            TaskListViewModel model = new TaskListViewModel();
            model.FolderID = id;
            model.FolderTitle = fn;
            if (model.FolderID > 0)
            {
                WorkItemFolder folder = await _wspService.GetWorkItemFolderAsync(model.FolderID);
                if (folder != null)
                {
                    model.FolderIsArchived = folder.IsArchived;
                    model.FolderIsLocked = folder.IsLocked;
                    model.FolderTitle = folder.Title;
                    model.FolderOwnerID = folder.OwnerId;
                    model.FolderOwnerName = folder.OwnerName;
                }
                var entities = await _wspService.GetTasksByFolderIdAsync(model.FolderID);
                if (entities != null) { model.TaskItems = entities; }
            }
            return View(model);
        }

        public async Task<IActionResult> FolderEvaluations(long id, string fn, string od)
        {
            SubmittedEvaluationsViewModel model = new SubmittedEvaluationsViewModel();
            model.FolderID = id;
            model.FolderOwnerID = od;
            model.FolderName = fn;

            if (!string.IsNullOrWhiteSpace(model.FolderOwnerID))
            {
                var taskOwner = await _ermService.GetEmployeeByIdAsync(model.FolderOwnerID);
                if (taskOwner != null)
                {
                    model.FolderOwnerName = taskOwner.FullName;
                    model.FolderOwnerUnitName = taskOwner.UnitName;
                    model.FolderOwnerLocationName = taskOwner.LocationName;
                    model.FolderOwnerDesignation = taskOwner.CurrentDesignation;
                }
            }

            if (model.FolderID > 0)
            {
                var entities = await _wspService.GetTaskItemEvaluationsAsync(model.FolderID);
                if (entities != null && entities.Count > 0)
                {
                    model.TaskItemEvaluations = entities;
                    model.SubmittedToEmployeeName = entities[0].EvaluatorName;

                }
            }
            return View(model);
        }

        #endregion

        #region Download Report Action Methods
        public async Task<IActionResult> DownloadCumulativeProductivityReport(string sn = null, int? ud = null, int? dd = null, int? ld = null, int? gd = null, DateTime? sd = null, DateTime? ed = null)
        {
            ProductivityReportViewModel model = new ProductivityReportViewModel();
            model.EvaluationScoresList = new List<TaskEvaluationScores>();
            List<EmployeeReportLine> directReports = new List<EmployeeReportLine>();
            model.sn = sn;
            model.ud = ud;
            model.dd = dd;
            model.ld = ld;
            model.sd = sd ?? DateTime.Now.AddMonths(-3);
            model.ed = ed ?? DateTime.Now.AddMonths(1);
            string fileName = $"Productivity Report for the Period {model.sd?.ToString("yyyy-MMM-dd")} To {model.ed?.ToString("yyyy-MMM-dd")} {DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            try
            {
                var entities = await _wspService.GetTaskEvaluationScoresAsync(model.sn, model.ud, model.dd, model.ld, model.gd, model.sd, model.ed);
                if (entities != null && entities.Count > 0)
                {
                    model.EvaluationScoresList = entities;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return GenerateCumulativeProductivityReport(fileName, model.EvaluationScoresList);
        }
        public async Task<IActionResult> DownloadTaskEvaluationReport(long id)
        {
            TaskEvaluationReportViewModel model = new TaskEvaluationReportViewModel();
            model.EvaluationDetailsList = new List<TaskEvaluationDetail>();
            model.id = id;
            try
            {
                var entities = await _wspService.GetTaskEvaluationDetailsbyFolderIdAsync(model.id);
                if (entities != null && entities.Count > 0)
                {
                    model.EvaluationDetailsList = entities;
                    model.EmployeeName = entities[0].TaskOwnerName;
                    model.TaskFolderTitle = entities[0].TaskFolderName;
                }
            }
            catch (Exception)
            {
                return null;
            }
            string fileName = $"Task Evaluation Report for {model.EmployeeName}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return GenerateTaskEvaluationReport(fileName, model.EvaluationDetailsList);
        }

        #endregion

        #region Controller Helper Methods
        public string SaveNote(string nm, string msg, long td, long fd, long pd)
        {
            WorkItemNote note = new WorkItemNote()
            {
                NoteTime = DateTime.Now,
                NoteWrittenBy = nm,
                NoteContent = msg,
                TaskItemId = td == 0 ? (long?)null : td,
                WorkItemFolderId = fd == 0 ? (long?)null : fd,
                ProjectId = pd == 0 ? (long?)null : pd,
            };

            if ((note.TaskItemId < 1 && note.WorkItemFolderId < 1 && note.ProjectId < 1) || string.IsNullOrWhiteSpace(nm) || string.IsNullOrWhiteSpace(msg)) { return "parameter"; }
            try
            {
                if (_wspService.AddWorkItemNoteAsync(note).Result)
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

        #region Task Folder Helper Methods
        public string UpdateTaskFolderArchive(long id, bool st)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_wspService.UpdateWorkItemFolderArchiveStatusAsync(id, st, actionBy).Result)
                {
                    return "success";
                }
                else
                {
                    return "method failure";
                }
            }
            catch
            {
                return "service error";
            }
        }
        public string UpdateTaskFolderLock(long id, bool st)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_wspService.UpdateWorkItemFolderLockStatusAsync(id, st, actionBy).Result)
                {
                    return "success";
                }
                else
                {
                    return "method failure";
                }
            }
            catch
            {
                return "service error";
            }
        }
        public string DeleteTaskFolderSubmission(long id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_wspService.DeleteFolderSubmissionAsync(id).Result)
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
        public string ReturnTaskFolder(long id, long sd, string ps = null)
        {
            if (id < 1 || sd < 1) { return "parameter error"; }
            try
            {
                WorkItemSubmissionType purposeOfSubmission = WorkItemSubmissionType.Approval;
                if (!string.IsNullOrWhiteSpace(ps) && ps != "Approval") { purposeOfSubmission = WorkItemSubmissionType.Review; }
                string evaluatorId = string.Empty;
                string evaluatorName = string.Empty;
                string employeeId = string.Empty;
                string employeeName = string.Empty;
                long folderId = id;
                long submissionId = sd;
                FolderSubmission folderSubmission = _wspService.GetFolderSubmissionByIdAsync(sd).Result;
                if (folderSubmission != null)
                {
                    evaluatorId = folderSubmission.ToEmployeeId;
                    evaluatorName = folderSubmission.ToEmployeeName;
                    employeeId = folderSubmission.FromEmployeeId;
                    employeeName = folderSubmission.FromEmployeeName;
                    purposeOfSubmission = folderSubmission.SubmissionType;
                }

                if (purposeOfSubmission == WorkItemSubmissionType.Review)
                {
                    TaskEvaluationHeader t = new TaskEvaluationHeader();
                    List<TaskEvaluationReturns> returnList = new List<TaskEvaluationReturns>();
                    t = _wspService.GetTaskEvaluationHeaderAsync(folderId, evaluatorId).Result;
                    long totalNoOfTasks = t.TotalNumberOfTasks;
                    long taskEvaluationHeaderId = t.Id;
                    long noOfReturnedTasks = _wspService.GetTaskEvaluationReturnsByTaskFolderIdAsync(t.TaskFolderId).Result.Count;
                    long noOfEvaluatedTasks = _wspService.GetEvaluatedTaskItemsCountAsync(folderId, evaluatorId).Result;
                    if (totalNoOfTasks > (noOfEvaluatedTasks + noOfReturnedTasks))
                    {
                        throw new Exception("It appears that there are some tasks that have not been attended to. Please action all tasks before attempting to submit.");
                    }
                }

                if (_wspService.ReturnFolderSubmissionAsync(id, sd).Result)
                {
                    return "success";
                }
                else
                {
                    return "Sorry, the request failed due to server error.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string DeleteFolderSubmission(long id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_wspService.DeleteFolderSubmissionAsync(id).Result)
                {
                    return "success";
                }
                else
                {
                    return "method failure";
                }
            }
            catch
            {
                return "service error";
            }
        }
        public string DeleteAllActionedFolderSubmissions(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_wspService.DeleteFolderSubmissionsByToEmployeeIdAsync(id).Result)
                {
                    return "success";
                }
                else
                {
                    return "method failure";
                }
            }
            catch
            {
                return "service error";
            }
        }

        #endregion

        #region Task Items Helper Methods
        public string DeleteTaskItem(long id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_wspService.DeleteTaskItemAsync(id).Result)
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
        public string UpdateTaskStatus(long id, bool cls)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            WorkItemStatus newStatus = new WorkItemStatus();
            if (cls) { newStatus = WorkItemStatus.Closed; }
            else { newStatus = WorkItemStatus.Open; }
            try
            {
                if (_wspService.UpdateTaskItemOpenCloseStatusAsync(id, newStatus, actionBy).Result)
                {
                    return "success";
                }
                else
                {
                    return "method failure";
                }
            }
            catch
            {
                return "service error";
            }
        }
        public string UpdateTaskItemProgressStatus(long id, int ns, string os)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_wspService.UpdateTaskItemProgressAsync(id, ns, os, actionBy).Result)
                {
                    return "success";
                }
                else
                {
                    return "method failure";
                }
            }
            catch
            {
                return "service error";
            }
        }
        public string ApproveTaskItem(long id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_wspService.UpdateTaskItemApprovalStatusAsync(id, ApprovalStatus.Approved, actionBy).Result)
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
        public string DeclineTaskItem(long id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_wspService.UpdateTaskItemApprovalStatusAsync(id, ApprovalStatus.Declined, actionBy).Result)
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
        public string MoveTaskToFolder(long id, long fd)
        {
            if (id < 1 || fd < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_wspService.UpdateTaskItemFolderIdAsync(id, actionBy, fd).Result)
                {
                    return "success";
                }
                else
                {
                    return "method failure";
                }
            }
            catch
            {
                return "service error";
            }
        }
        public string EvaluateTaskItem(long td, long fd, string ed, long qs, long hd, long dd)
        {
            TaskEvaluationDetail detail = new TaskEvaluationDetail();
            detail.TaskItemId = td;
            detail.TaskFolderId = fd;
            detail.TaskEvaluatorId = ed;
            detail.TaskEvaluatorName = HttpContext.User.Identity.Name;
            detail.QualityScore = qs;
            detail.TaskEvaluationHeaderId = hd;
            detail.TaskEvaluationDetailId = dd;
            try
            {
                if (detail.TaskItemId < 1) { throw new Exception("Required parameter [TaskItemId] is missing."); }
                if (string.IsNullOrWhiteSpace(detail.TaskEvaluatorId))
                {
                    var claims = HttpContext.User.Claims.ToList();
                    string current_employeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    if (string.IsNullOrWhiteSpace(current_employeeId)) { throw new Exception("Sorry, it appears your session has expired. Please login again and try again."); }
                    detail.TaskEvaluatorId = current_employeeId;
                }

                if (detail.TaskEvaluationDetailId > 0)
                {
                    var old_detail = _wspService.GetTaskEvaluationDetailByIdAsync(detail.TaskEvaluationDetailId).Result;
                    if (old_detail != null)
                    {
                        detail = old_detail;
                        detail.QualityScore = qs;
                        detail.CompletionScore = 100;
                        bool IsUpdated = _wspService.UpdateTaskEvaluationDetailAsync(detail).Result;
                        if (IsUpdated) { return "success"; } else { throw new Exception("Sorry, task rating was not updated. Please try again."); }
                    }
                    else
                    {
                        throw new Exception("Sorry no evaluation record was found for this task. Please try again.");
                    }
                }
                else
                {
                    TaskItem task = _wspService.GetTaskItemByIdAsync(detail.TaskItemId).Result;
                    if (task == null || task.Id < 1) { throw new Exception("Ooops! No record was found for this task item. Please try again."); }
                    detail.TaskItemId = task.Id;
                    detail.CompletionScore = 100;
                    detail.EvaluationDate = DateTime.Now;
                    detail.TaskOwnerDeptId = task.DepartmentId ?? 0;
                    detail.TaskOwnerUnitId = task.UnitId ?? 0;
                    detail.TaskOwnerLocationId = task.LocationId ?? 0;
                    detail.TaskOwnerId = task.TaskOwnerId;

                    TaskEvaluationHeader evaluationHeader = _wspService.GetTaskEvaluationHeaderAsync(detail.TaskFolderId, detail.TaskEvaluatorId).Result;
                    if (evaluationHeader == null || evaluationHeader.Id < 1)
                    {
                        evaluationHeader = new TaskEvaluationHeader();
                        evaluationHeader.EvaluatorId = detail.TaskEvaluatorId;
                        evaluationHeader.TaskFolderId = detail.TaskFolderId;
                        evaluationHeader.TaskOwnerDeptId = task.DepartmentId ?? 0;
                        evaluationHeader.TaskOwnerId = task.TaskOwnerId;
                        evaluationHeader.TaskOwnerLocationId = task.LocationId ?? 0;
                        evaluationHeader.TaskOwnerUnitId = task.UnitId ?? 0;
                        long evaluationHeaderId = _wspService.CreateTaskEvaluationHeaderAsync(evaluationHeader).Result;
                        if (evaluationHeaderId > 0) { detail.TaskEvaluationHeaderId = evaluationHeaderId; }
                    }
                    bool IsAdded = _wspService.AddTaskEvaluationDetailAsync(detail).Result;
                    if (IsAdded)
                    {
                        return "success";
                    }
                    else
                    {
                        throw new Exception("Sorry, attempt to rate task failed. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region Download Report Helper Methods
        private FileResult GenerateCumulativeProductivityReport(string fileName, IEnumerable<TaskEvaluationScores> results)
        {
            DataTable dataTable = new DataTable("results");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("#"),
                new DataColumn("Staff Name"),
                new DataColumn("Total"),
                new DataColumn("Completed"),
                new DataColumn("Uncompleted"),
                new DataColumn("Quality Rating"),
                new DataColumn("% Completion"),
            });
            int rowNumber = 0;
            foreach (var result in results)
            {
                rowNumber++;
                dataTable.Rows.Add(
                    rowNumber.ToString(),
                    result.TaskOwnerName,
                    result.TotalNumberOfTasks,
                    result.NoOfCompletedTasks,
                    result.NoOfUncompletedTasks,
                    result.AverageQualityScore,
                    result.AverageCompletionScore
                    );
            }

            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        private FileResult GenerateTaskEvaluationReport(string fileName, IEnumerable<TaskEvaluationDetail> results)
        {
            DataTable dataTable = new DataTable("results");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("#"),
                new DataColumn("No"),
                new DataColumn("Description"),
                new DataColumn("Resolution"),
                new DataColumn("Status"),
                new DataColumn("Quality Rating"),
            });
            int rowNumber = 0;
            foreach (var result in results)
            {
                rowNumber++;
                dataTable.Rows.Add(
                    rowNumber.ToString(),
                    result.TaskItemNo,
                    result.TaskItemDescription,
                    result.TaskItemMoreInfo,
                    result.CompletionScore == 100 ? "Completed" : "Uncompleted",
                    result.QualityScore
                    );
            }

            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        #endregion

        #endregion
    }
}