using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClosedXML.Excel;
using IntranetPortal.Areas.WKS.Models;
using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.WKS.Controllers
{
    [Area("WKS")]
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IBamsManagerService _bamsManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IErmService _ermService;
        public TasksController(IConfiguration configuration, ISecurityService securityService,
                        IBaseModelService baseModelService, IBamsManagerService bamsManagerService,
                        IGlobalSettingsService globalSettingsService, IWorkspaceService workspaceService,
                        IErmService ermService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _bamsManagerService = bamsManagerService;
            _globalSettingsService = globalSettingsService;
            _workspaceService = workspaceService;
            _ermService = ermService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        #region Task List Controller Action Methods
        public async Task<IActionResult> TaskList(string id = null, int? cy = null, int? cm = null)
        {
            TaskListViewModel model = new TaskListViewModel();
            string ownerName = HttpContext.User.Identity.Name;
            model.id = id;

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    var claims = HttpContext.User.Claims.ToList();
                    model.id = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                }

                if (!string.IsNullOrWhiteSpace(model.id))
                {
                    var entities = await _workspaceService.SearchTaskListAsync(model.id, false, cy, cm);
                    if (entities != null && entities.Count > 0)
                    {
                        model.TaskLists = entities;
                    }
                }

                if (cy == null) { model.cy = DateTime.Now.Year; }
                else { model.cy = cy.Value; }

                if (cm == null) { model.cm = 0; }
                else { model.cm = cm.Value; }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> TeamTaskList(string id = null, int? cy = null, int? cm = null)
        {
            List<EmployeeReportLine> directReports = new List<EmployeeReportLine>();
            TaskListViewModel model = new TaskListViewModel();
            model.TaskLists = new List<TaskList>();
            model.id = id;
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var entities = await _workspaceService.SearchTaskListAsync(id, null, cy, cm);
                    if (entities != null && entities.Count > 0)
                    {
                        model.TaskLists = entities;
                    }
                }

                var claims = HttpContext.User.Claims.ToList();
                string reportsToEmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                var employee_entities = await _ermService.GetEmployeeReportsByReportsToEmployeeIdAsync(reportsToEmployeeID);
                if (employee_entities != null && employee_entities.Count > 0)
                {
                    directReports = employee_entities;
                }

                ViewBag.ReportsList = new SelectList(directReports, "EmployeeID", "EmployeeName", id);

                if (cy == null) { model.cy = DateTime.Now.Year; }
                else { model.cy = cy.Value; }

                if (cm == null) { model.cm = 0; }
                else { model.cm = cm.Value; }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> ArchivedTaskList(string id = null, int? cy = null, int? cm = null)
        {
            TaskListViewModel model = new TaskListViewModel();
            string ownerName = HttpContext.User.Identity.Name;
            model.id = id;
            if (string.IsNullOrWhiteSpace(id))
            {
                var claims = HttpContext.User.Claims.ToList();
                model.id = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(model.id))
            {
                var entities = await _workspaceService.SearchTaskListAsync(model.id, true, cy, cm);
                if (entities != null && entities.Count > 0)
                {
                    model.TaskLists = entities;
                }
            }

            if (cy == null) { model.cy = DateTime.Now.Year; }
            else { model.cy = cy.Value; }

            if (cm == null) { model.cm = DateTime.Now.Month; }
            else { model.cm = cm.Value; }

            return View(model);
        }

        public async Task<IActionResult> ManageTaskList(int? id = null, string od = null, string src = null)
        {
            ManageTaskListViewModel model = new ManageTaskListViewModel();
            model.Source = src;
            try
            {
                if (id == null || id < 1)
                {
                    model.IsNew = true;
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
                    var entity = await _workspaceService.GetTaskListAsync(id.Value);
                    if (entity != null)
                    {
                        model.Description = entity.Description;
                        model.Id = entity.Id;
                        model.IsArchived = entity.IsArchived;
                        model.IsDeleted = entity.IsDeleted;
                        model.Name = entity.Name;
                        model.OwnerId = entity.OwnerId;
                        model.OwnerName = entity.OwnerName;
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
        public async Task<IActionResult> ManageTaskList(ManageTaskListViewModel model)
        {
            if (ModelState.IsValid)
            {
                TaskList taskList = new TaskList();
                try
                {
                    if (model.IsNew)
                    {
                        if (string.IsNullOrWhiteSpace(model.OwnerId))
                        {
                            var claims = HttpContext.User.Claims.ToList();
                            taskList.OwnerId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                        }
                        else { taskList.OwnerId = model.OwnerId; }
                        taskList.CreatedBy = HttpContext.User.Identity.Name;
                        taskList.CreatedTime = DateTime.UtcNow;
                        taskList.Name = model.Name;
                        taskList.Description = model.Description;
                        taskList.IsArchived = false;
                        bool IsAdded = await _workspaceService.CreateTaskListAsync(taskList);
                        if (IsAdded)
                        {
                            TaskListActivityHistory taskListHistory = new TaskListActivityHistory
                            {
                                ActivityTime = DateTime.UtcNow,
                                ActivityBy = taskList.CreatedBy,
                                ActivityDescription = $"New task List was created by {taskList.CreatedBy} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} GMT.",
                                TaskListId = taskList.Id,
                                EntityId = (object)taskList.Id,
                            };
                            await _baseModelService.AddEntityActivityHistoryAsync(taskListHistory, EntityType.TaskList);
                            if (model.Source == "ttl")
                            {
                                return RedirectToAction("TeamTaskList", new { id = taskList.OwnerId });
                            }
                            else
                            {
                                return RedirectToAction("TaskList", new { id = taskList.OwnerId });
                            }
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "Sorry, the operation could not be completed. Please try again.";
                        }
                    }
                    else
                    {
                        taskList.Description = model.Description;
                        taskList.Id = model.Id;
                        taskList.IsArchived = model.IsArchived;
                        taskList.Name = model.Name;
                        taskList.OwnerId = model.OwnerId;
                        string ModifiedBy = HttpContext.User.Identity.Name;
                        bool IsUpdated = await _workspaceService.UpdateTaskListAsync(taskList);
                        if (IsUpdated)
                        {
                            TaskListActivityHistory taskListHistory = new TaskListActivityHistory
                            {
                                ActivityTime = DateTime.UtcNow,
                                ActivityBy = ModifiedBy,
                                ActivityDescription = $"Task List was updated by {ModifiedBy} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} GMT.",
                                TaskListId = taskList.Id,
                                EntityId = (object)taskList.Id,
                            };
                            await _baseModelService.AddEntityActivityHistoryAsync(taskListHistory, EntityType.TaskList);
                            if (model.Source == "ttl")
                            {
                                return RedirectToAction("TeamTaskList", new { id = taskList.OwnerId });
                            }
                            else
                            {
                                return RedirectToAction("TaskList", new { id = taskList.OwnerId });
                            }
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "Sorry, the operation could not be completed. Please try again.";
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

        public async Task<IActionResult> TaskListInfo(int? id)
        {
            ManageTaskListViewModel model = new ManageTaskListViewModel();
            try
            {
                var entity = await _workspaceService.GetTaskListAsync(id.Value);
                if (entity != null)
                {
                    model.Description = entity.Description;
                    model.Id = entity.Id;
                    model.IsArchived = entity.IsArchived;
                    model.IsDeleted = entity.IsDeleted;
                    model.Name = entity.Name;
                    model.OwnerId = entity.OwnerId;
                    model.OwnerName = entity.OwnerName;
                    model.CreatedBy = entity.CreatedBy;
                    model.CreatedTime = entity.CreatedTime;

                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteTaskList(int? id)
        {
            ManageTaskListViewModel model = new ManageTaskListViewModel();
            try
            {
                var entity = await _workspaceService.GetTaskListAsync(id.Value);
                if (entity != null)
                {
                    model.Description = entity.Description;
                    model.Id = entity.Id;
                    model.IsArchived = entity.IsArchived;
                    model.IsDeleted = entity.IsDeleted;
                    model.Name = entity.Name;
                    model.OwnerId = entity.OwnerId;
                    model.OwnerName = entity.OwnerName;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTaskList(ManageTaskListViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool IsDeleted = await _workspaceService.DeleteTaskListAsync(model.Id);
                    if (IsDeleted)
                    {
                        return RedirectToAction("TaskList", new { id = model.OwnerId });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, the operation could not be completed. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> TaskListNotes(int id, string src)
        {
            TaskListNotesViewModel model = new TaskListNotesViewModel();
            model.Source = src;
            model.TaskListID = id;

            var entities = await _workspaceService.GetTaskListNotesAsync(model.TaskListID);
            if (entities != null && entities.Count > 0)
            {
                model.TaskListNotes = entities;
            }
            return View(model);
        }

        #endregion

        #region Task Items Controller Action Methods
        public async Task<IActionResult> Taskboard(int id, int? ps = null, int? dy = null, int? dm = null, string src = null)
        {
            TaskBoardViewModel model = new TaskBoardViewModel();
            model.Source = src;
            model.id = id;
            model.ps = ps;
            if (dy == null) { model.dy = DateTime.Now.Year; } else { model.dy = dy; }
            model.dm = dm;
            string ownerName = HttpContext.User.Identity.Name;

            if (id > 0)
            {
                TaskList taskList = new TaskList();
                taskList = await _workspaceService.GetTaskListAsync(id);
                if (taskList != null)
                {
                    model.TaskListIsLocked = taskList.IsLocked;
                    model.TaskListIsArchived = taskList.IsArchived;
                    model.TaskListOwnerID = taskList.OwnerId;
                }

                var entities = await _workspaceService.GetTaskItemsByTaskListAsync(model.id, model.dy, model.dm, (WorkItemProgressStatus?)model.ps);
                if (entities != null && entities.Count > 0)
                {
                    model.TaskItems = entities;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> TeamTaskboard(int id, int? ps = null, int? dy = null, int? dm = null, string src = null)
        {
            TaskBoardViewModel model = new TaskBoardViewModel();
            List<TaskItem> TaskItemList = new List<TaskItem>();
            model.id = id;
            model.ps = ps;
            model.Source = src;

            try
            {
                if (dy == null) { model.dy = DateTime.Now.Year; } else { model.dy = dy; }
                model.dm = dm;

                if (id > 0)
                {
                    TaskList taskList = new TaskList();
                    taskList = await _workspaceService.GetTaskListAsync(id);
                    if (taskList != null)
                    {
                        model.TaskListIsLocked = taskList.IsLocked;
                        model.TaskListIsArchived = taskList.IsArchived;
                        model.TaskListOwnerID = taskList.OwnerId;
                    }

                    var entities = await _workspaceService.GetTaskItemsByTaskListAsync(model.id, model.dy, model.dm, (WorkItemProgressStatus?)model.ps);
                    if (entities != null && entities.Count > 0)
                    {
                        model.TaskItems = entities;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> NewTaskItem(int tl, string src, long? sd = null, long? md = null)
        {
            ManageTaskItemViewModel model = new ManageTaskItemViewModel();
            model.Source = src;
            model.ExpectedStartTime = DateTime.Now;
            model.ExpectedDueTime = DateTime.Now.AddDays(7.00);
            if (!string.IsNullOrWhiteSpace(src)) { model.Source = src; } else { model.Source = "txb"; }
            model.SubmissionID = sd;
            try
            {
                string taskNo = await _baseModelService.GenerateAutoNumberAsync("taskno");
                if (!string.IsNullOrWhiteSpace(taskNo)) { model.Number = $"T{taskNo}"; }

                if (tl > 0)
                {
                    TaskList taskList = new TaskList();
                    taskList = await _workspaceService.GetTaskListAsync(tl);
                    model.TaskListId = tl;
                    if (!string.IsNullOrWhiteSpace(taskList.OwnerId))
                    {
                        model.TaskOwnerId = taskList.OwnerId;
                        model.TaskOwnerName = taskList.OwnerName;
                    }
                    else
                    {
                        var claims = HttpContext.User.Claims.ToList();
                        model.TaskOwnerId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                        model.AssignedToId = model.TaskOwnerId;
                    }
                }
                else { return RedirectToAction("TaskList"); }
                if (md != null && md.Value > 0) { model.MasterTaskId = md.Value; }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewTaskItem(ManageTaskItemViewModel model)
        {
            TaskItem taskItem = new TaskItem();
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(model.TaskOwnerId))
                    {
                        var claims = HttpContext.User.Claims.ToList();
                        taskItem.TaskOwnerId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    }
                    else { taskItem.TaskOwnerId = model.TaskOwnerId; }

                    taskItem.CreatedBy = HttpContext.User.Identity.Name;
                    taskItem.CreatedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT";
                    taskItem.TaskListId = model.TaskListId;
                    taskItem.Number = model.Number;
                    taskItem.Description = model.Description;
                    taskItem.Deliverable = model.Deliverable;
                    taskItem.ExpectedStartTime = model.ExpectedStartTime;
                    taskItem.ExpectedDueTime = model.ExpectedDueTime;
                    taskItem.MasterTaskId = model.MasterTaskId;
                    taskItem.TaskStatus = WorkItemStatus.Open;
                    taskItem.ProgressStatus = WorkItemProgressStatus.NotStarted;
                    taskItem.TaskApprovalStatus = ApprovalStatus.Pending;
                    taskItem.AssignedToId = taskItem.TaskOwnerId;

                    Employee employee = new Employee();
                    employee = await _ermService.GetEmployeeByIdAsync(taskItem.TaskOwnerId);
                    if (employee != null && !string.IsNullOrWhiteSpace(employee.FullName))
                    {
                        taskItem.UnitId = employee.UnitID;
                        taskItem.DepartmentId = employee.DepartmentID;
                        taskItem.LocationId = employee.LocationID;
                    }

                    bool IsAdded = await _workspaceService.CreateTaskItemAsync(taskItem);
                    if (IsAdded)
                    {
                        if (model.Source == "sbt")
                        {
                            TaskItem createdTask = await _workspaceService.GetTaskItemAsync(model.Number);
                            if (createdTask.Id > 0) { await _workspaceService.UpdateTaskApprovalStatusAsync(createdTask.Id, ApprovalStatus.Approved, taskItem.CreatedBy); }
                            return RedirectToAction("SubmittedTasks", new { id = model.TaskListId, sd = model.SubmissionID });
                        }
                        else if (model.Source == "ttb")
                        {
                            TaskItem createdTask = await _workspaceService.GetTaskItemAsync(model.Number);
                            if (createdTask.Id > 0) { await _workspaceService.UpdateTaskApprovalStatusAsync(createdTask.Id, ApprovalStatus.Approved, taskItem.CreatedBy); }
                            return RedirectToAction("TeamTaskboard", new { id = model.TaskListId, sd = model.SubmissionID });
                        }
                        else
                        {
                            return RedirectToAction("Taskboard", new { id = taskItem.TaskListId });
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, the operation could not be completed. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public IActionResult UpdateProgress(long id, int tl, string ps, string src)
        {
            UpdateProgressViewModel model = new UpdateProgressViewModel();
            model.TaskItemID = id;
            model.TaskListID = tl;
            model.Source = src;
            switch (ps)
            {
                case "NotStarted":
                    model.CurrentProgressStatusDescription = "Not Started";
                    model.CurrentProgressStatusID = 0;
                    break;
                case "InProgress":
                    model.CurrentProgressStatusDescription = "In Progress";
                    model.CurrentProgressStatusID = 1;
                    break;
                case "Completed":
                    model.CurrentProgressStatusDescription = "Completed";
                    model.CurrentProgressStatusID = 2;
                    break;
                case "OnHold":
                    model.CurrentProgressStatusDescription = "On Hold";
                    model.CurrentProgressStatusID = 3;
                    break;
                default:
                    break;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProgress(UpdateProgressViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool IsUpdated = false;
                    string UpdatedBy = HttpContext.User.Identity.Name;
                    IsUpdated = await _workspaceService.UpdateTaskProgressAsync(model.TaskItemID, model.NewProgressStatusID.Value, model.CurrentProgressStatusDescription, UpdatedBy);
                    if (IsUpdated)
                    {
                        if (model.Source == "ttb")
                        {
                            return RedirectToAction("TeamTaskboard", new { id = model.TaskListID });
                        }
                        else if (model.Source == "txb")
                        {
                            return RedirectToAction("Taskboard", new { id = model.TaskListID });
                        }
                        else if (model.Source == "sbt")
                        {
                            return RedirectToAction("SubmittedTasks", new { id = model.TaskListID });
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

        public async Task<IActionResult> EditTaskItem(long id, string src, long? sd = null)
        {
            ManageTaskItemViewModel model = new ManageTaskItemViewModel();
            model.SubmissionID = sd;
            model.Source = src;
            try
            {
                TaskItem taskItem = new TaskItem();
                model.Id = id;
                taskItem = await _workspaceService.GetTaskItemAsync(id);
                if (taskItem != null)
                {
                    model.ExpectedDueTime = taskItem.ExpectedDueTime;
                    model.ExpectedStartTime = taskItem.ExpectedStartTime;
                    model.Deliverable = taskItem.Deliverable;
                    model.Description = taskItem.Description;
                    model.Number = taskItem.Number;
                    model.TaskOwnerId = taskItem.TaskOwnerId;
                    model.TaskListId = taskItem.TaskListId;
                    model.Id = taskItem.Id;
                    model.MasterTaskId = taskItem.MasterTaskId;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditTaskItem(ManageTaskItemViewModel model)
        {
            TaskItem taskItem = new TaskItem();
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(model.TaskOwnerId))
                    {
                        var claims = HttpContext.User.Claims.ToList();
                        taskItem.TaskOwnerId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    }
                    else { taskItem.TaskOwnerId = model.TaskOwnerId; }

                    taskItem.LastModifiedBy = HttpContext.User.Identity.Name;
                    taskItem.LastModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT";
                    taskItem.Id = model.Id.Value;
                    taskItem.TaskListId = model.TaskListId;
                    taskItem.Number = model.Number;
                    taskItem.Description = model.Description;
                    taskItem.Deliverable = model.Deliverable;
                    taskItem.ExpectedStartTime = model.ExpectedStartTime;
                    taskItem.ExpectedDueTime = model.ExpectedDueTime;
                    taskItem.MasterTaskId = model.MasterTaskId;

                    bool IsUpdated = await _workspaceService.UpdateTaskItemAsync(taskItem);
                    if (IsUpdated)
                    {
                        if (model.Source == "sbt")
                        {
                            if (model.Id > 0) { await _workspaceService.UpdateTaskApprovalStatusAsync(model.Id.Value, ApprovalStatus.Approved, taskItem.LastModifiedBy); }
                            return RedirectToAction("SubmittedTasks", new { id = model.TaskListId, sd = model.SubmissionID });
                        }
                        else if (model.Source == "ttb")
                        {
                            if (model.Id > 0) { await _workspaceService.UpdateTaskApprovalStatusAsync(model.Id.Value, ApprovalStatus.Approved, taskItem.LastModifiedBy); }
                            return RedirectToAction("TeamTaskboard", new { id = model.TaskListId, sd = model.SubmissionID });
                        }
                        else
                        {
                            return RedirectToAction("Taskboard", new { id = taskItem.TaskListId });
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, the operation could not be completed. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> TaskInfo(long id)
        {
            ManageTaskItemViewModel model = new ManageTaskItemViewModel();
            try
            {
                TaskItem taskItem = new TaskItem();
                model.Id = id;
                taskItem = await _workspaceService.GetTaskItemAsync(id);
                if (taskItem != null)
                {
                    model.ExpectedDueTime = taskItem.ExpectedDueTime;
                    model.ExpectedStartTime = taskItem.ExpectedStartTime;
                    model.Deliverable = taskItem.Deliverable;
                    model.Description = taskItem.Description;
                    model.Number = taskItem.Number;
                    model.TaskOwnerId = taskItem.TaskOwnerId;
                    model.TaskOwnerName = taskItem.TaskOwnerName;
                    model.AssignedToName = taskItem.AssignedToName;
                    model.TaskListId = taskItem.TaskListId;
                    model.TaskListName = taskItem.TaskListName;
                    model.Id = taskItem.Id;
                    model.MasterTaskId = taskItem.MasterTaskId;
                    model.MasterTaskDescription = taskItem.MasterTaskDescription;
                    model.UnitName = taskItem.UnitName;
                    model.DepartmentName = taskItem.DepartmentName;
                    model.LocationName = taskItem.LocationName;
                    model.CreatedBy = taskItem.CreatedBy;
                    model.CreatedTime = taskItem.CreatedTime;
                    model.TaskStatusDescription = taskItem.TaskStatusDescription;
                    model.ApprovalStatusDescription = taskItem.ApprovalStatusDescription;
                    model.ProgressStatusDescription = taskItem.ProgressStatusDescription;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> TaskItemHistory(long id, int tl)
        {
            TaskItemHistoryViewModel model = new TaskItemHistoryViewModel();
            List<TaskItemActivityHistory> activitylist = new List<TaskItemActivityHistory>();
            model.TaskItemID = id;
            model.TaskListID = tl;

            var entities = await _baseModelService.GetTaskItemActivityHistory(model.TaskItemID);
            if (entities != null && entities.Count > 0)
            {
                model.ActivityList = entities;
            }
            return View(model);
        }

        public async Task<IActionResult> UpdateTimeline(long id, int tl, string src = null)
        {
            UpdateTimelineViewModel model = new UpdateTimelineViewModel();
            model.Source = src;
            model.TaskItemID = id;
            model.TaskListID = tl;
            var entity = await _workspaceService.GetTaskItemAsync(id);
            model.CurrentEndDate = entity.ExpectedDueTime;
            if (entity.ExpectedDueTime != null) { model.CurrentEndDateDescription = entity.ExpectedDueTime.Value.ToString("yyyy-MMM-dd"); }
            model.CurrentStartDate = entity.ExpectedStartTime;
            if (entity.ExpectedStartTime != null) { model.CurrentStartDateDescription = entity.ExpectedStartTime.Value.ToString("yyyy-MMM-dd"); }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTimeline(UpdateTimelineViewModel model)
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
                    taskTimelineChange.TaskListId = model.TaskListID;
                    if (model.NewEndDate != null && model.CurrentEndDate != null)
                    {
                        TimeSpan timeSpan = model.NewEndDate.Value.Subtract(model.CurrentEndDate.Value);
                        if (timeSpan != null)
                        {
                            taskTimelineChange.DifferentInDays = timeSpan.Days;
                        }
                    }
                    taskTimelineChange.ModifiedBy = HttpContext.User.Identity.Name;
                    bool IsUpdated = false;
                    IsUpdated = await _workspaceService.UpdateTaskTimelineAsync(taskTimelineChange);
                    if (IsUpdated)
                    {
                        //model.OperationIsSuccessful = true;
                        //model.ViewModelSuccessMessage = "Task rescheduled successfully! ";
                        if (model.Source == "ttb")
                        {
                            return RedirectToAction("TeamTaskboard", new { id = model.TaskListID });
                        }
                        else if (model.Source == "txb")
                        {
                            return RedirectToAction("Taskboard", new { id = model.TaskListID });
                        }
                        else if (model.Source == "sbt")
                        {
                            return RedirectToAction("SubmittedTasks", new { id = model.TaskListID });
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

        public async Task<IActionResult> TaskItemNotes(long id, int tl, string src)
        {
            TaskItemNotesViewModel model = new TaskItemNotesViewModel();
            model.TaskItemID = id;
            model.TaskListID = tl;
            model.Source = src;

            var entities = await _workspaceService.GetTaskNotesAsync(model.TaskItemID);
            if (entities != null && entities.Count > 0)
            {
                model.TaskItemNotes = entities;
            }
            return View(model);
        }

        public async Task<IActionResult> ScheduleHistory(long id, int tl)
        {
            ScheduleHistoryViewModel model = new ScheduleHistoryViewModel();
            model.TaskItemID = id;
            model.TaskListID = tl;

            var entities = await _workspaceService.GetTaskTimelineChangesByTaskItemIdAsync(model.TaskItemID);
            if (entities != null && entities.Count > 0)
            {
                model.TaskTimelineChanges = entities;
            }
            return View(model);
        }

        public IActionResult AddTaskNote(int tl, string tp, string src, long? id = null, string od = null)
        {
            AddTaskNoteViewModel model = new AddTaskNoteViewModel();
            model.Source = src;
            model.OwnerID = od;

            if (!string.IsNullOrWhiteSpace(tp))
            {
                if (tp == "t")
                {
                    model.IsTaskNote = true;
                    model.TaskItemID = id.Value;
                    model.TaskListID = tl;
                }
                else if (tp == "l")
                {
                    model.IsTaskNote = false;
                    model.TaskListID = (int)id;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddTaskNote(AddTaskNoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.IsTaskNote)
                    {
                        TaskNote taskNote = new TaskNote();
                        taskNote.TaskItemId = model.TaskItemID;
                        taskNote.NoteDescription = model.NoteText;
                        taskNote.NoteTime = DateTime.UtcNow;
                        taskNote.NoteWrittenBy = HttpContext.User.Identity.Name;
                        bool NoteIsAdded = false;
                        NoteIsAdded = await _workspaceService.AddTaskItemNoteAsync(taskNote);
                        if (NoteIsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Note added successfully!";
                        }
                    }
                    else
                    {
                        TaskListNote taskListNote = new TaskListNote();
                        taskListNote.TaskListId = model.TaskListID;
                        taskListNote.NoteDescription = model.NoteText;
                        taskListNote.NoteTime = DateTime.UtcNow;
                        taskListNote.NoteWrittenBy = HttpContext.User.Identity.Name;
                        bool ListNoteIsAdded = false;
                        ListNoteIsAdded = await _workspaceService.AddTaskListNoteAsync(taskListNote);
                        if (ListNoteIsAdded)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Note added successfully!";
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
                model.ViewModelErrorMessage = "Sorry key parameters are missing. The request could not be processed.";
            }
            return View(model);
        }

        public async Task<IActionResult> LinkProgram(long id, int tl, string src)
        {
            LinkProgramViewModel model = new LinkProgramViewModel();
            model.Source = src;
            model.TaskItemID = id;
            model.TaskListID = tl;

            var program_entities = await _globalSettingsService.GetProgramsAsync();
            if (program_entities != null)
            {
                ViewBag.ProgrammeList = new SelectList(program_entities, "Code", "Title");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LinkProgram(LinkProgramViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string updatedBy = HttpContext.User.Identity.Name;
                    bool IsAdded = false;
                    IsAdded = await _workspaceService.LinkTaskToProgramAsync(model.TaskItemID, model.ProgramCode, model.ProgramTime, updatedBy);
                    if (IsAdded)
                    {
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "Programme linked successfully!";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Sorry key parameters are missing. The request could not be processed.";
            }

            var program_entities = await _globalSettingsService.GetProgramsAsync();
            if (program_entities != null)
            {
                ViewBag.ProgrammeList = new SelectList(program_entities, "Code", "Title");
            }
            return View(model);
        }

        public IActionResult LinkProject(long id, int tl, string src)
        {
            LinkProjectViewModel model = new LinkProjectViewModel();
            model.Source = src;
            model.TaskItemID = id;
            model.TaskListID = tl;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LinkProject(LinkProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                Project project = new Project();
                try
                {
                    if (!string.IsNullOrWhiteSpace(model.ProjectNumber))
                    {
                        var entity = await _workspaceService.GetWorkItemByCodeAsync(model.ProjectNumber);
                        if (entity != null && !string.IsNullOrWhiteSpace(entity.Title))
                        {
                            project = entity;
                            string updatedBy = HttpContext.User.Identity.Name;
                            bool IsAdded = false;
                            IsAdded = await _workspaceService.LinkTaskToProjectAsync(model.TaskItemID, model.ProjectNumber, updatedBy, project.Title);
                            if (IsAdded)
                            {
                                model.OperationIsSuccessful = true;
                                model.ViewModelSuccessMessage = "Project linked successfully!";
                            }
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"No record was found for the project with Number: [{model.ProjectNumber}]. Please check that the Project Number you entered is correct.";
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
                model.ViewModelErrorMessage = "Sorry key parameters are missing. The request could not be processed.";
            }

            return View(model);
        }

        public async Task<IActionResult> SelectTaskFolder(long id, string od, string src)
        {
            SelectTaskFolderViewModel model = new SelectTaskFolderViewModel();
            List<TaskList> activeTaskLists = new List<TaskList>();
            model.TaskItemID = id;
            model.TaskOwnerID = od;
            model.Source = src;
            var entities = await _workspaceService.GetActiveTaskListsAsync(od);
            if (entities != null && entities.Count > 0)
            {
                activeTaskLists = entities;
            }
            model.ActiveTaskLists = activeTaskLists;
            return View(model);
        }

        public async Task<IActionResult> ReCreateTaskItem(long id, int tl, string src)
        {
            ManageTaskItemViewModel model = new ManageTaskItemViewModel();
            model.Source = src;
            model.TaskListId = tl;

            try
            {
                string taskNo = await _baseModelService.GenerateAutoNumberAsync("taskno");
                if (!string.IsNullOrWhiteSpace(taskNo)) { model.Number = $"T{taskNo}"; }

                TaskItem taskItem = new TaskItem();
                //model.Id = id;
                taskItem = await _workspaceService.GetTaskItemAsync(id);
                if (taskItem != null)
                {
                    model.ExpectedDueTime = taskItem.ExpectedDueTime;
                    model.ExpectedStartTime = taskItem.ExpectedStartTime;
                    model.Deliverable = taskItem.Deliverable;
                    model.Description = taskItem.Description;
                    //model.Number = taskItem.Number;
                    model.TaskOwnerId = taskItem.TaskOwnerId;
                    //model.TaskListId = taskItem.TaskListId;
                    //model.Id = taskItem.Id;
                    model.MasterTaskId = taskItem.MasterTaskId;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ReCreateTaskItem(ManageTaskItemViewModel model)
        {
            TaskItem taskItem = new TaskItem();
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(model.TaskOwnerId))
                    {
                        var claims = HttpContext.User.Claims.ToList();
                        taskItem.TaskOwnerId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                    }
                    else { taskItem.TaskOwnerId = model.TaskOwnerId; }

                    taskItem.CreatedBy = HttpContext.User.Identity.Name;
                    taskItem.CreatedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT";
                    taskItem.TaskListId = model.TaskListId;
                    taskItem.Number = model.Number;
                    taskItem.Description = model.Description;
                    taskItem.Deliverable = model.Deliverable;
                    taskItem.ExpectedStartTime = model.ExpectedStartTime;
                    taskItem.ExpectedDueTime = model.ExpectedDueTime;
                    taskItem.MasterTaskId = model.MasterTaskId;
                    taskItem.TaskStatus = WorkItemStatus.Open;
                    taskItem.ProgressStatus = WorkItemProgressStatus.NotStarted;
                    taskItem.TaskApprovalStatus = ApprovalStatus.Pending;
                    taskItem.AssignedToId = taskItem.TaskOwnerId;

                    Employee employee = new Employee();
                    employee = await _ermService.GetEmployeeByIdAsync(taskItem.TaskOwnerId);
                    if (employee != null && !string.IsNullOrWhiteSpace(employee.FullName))
                    {
                        taskItem.UnitId = employee.UnitID;
                        taskItem.DepartmentId = employee.DepartmentID;
                        taskItem.LocationId = employee.LocationID;
                    }

                    bool IsAdded = await _workspaceService.CreateTaskItemAsync(taskItem);
                    if (IsAdded)
                    {
                        if (model.Source == "sbt")
                        {
                            TaskItem createdTask = await _workspaceService.GetTaskItemAsync(model.Number);
                            if (createdTask.Id > 0) { await _workspaceService.UpdateTaskApprovalStatusAsync(createdTask.Id, ApprovalStatus.Approved, taskItem.CreatedBy); }
                            return RedirectToAction("SubmittedTasks", new { id = model.TaskListId, sd = model.SubmissionID });
                        }
                        else if (model.Source == "ttb")
                        {
                            TaskItem createdTask = await _workspaceService.GetTaskItemAsync(model.Number);
                            if (createdTask.Id > 0) { await _workspaceService.UpdateTaskApprovalStatusAsync(createdTask.Id, ApprovalStatus.Approved, taskItem.CreatedBy); }
                            return RedirectToAction("TeamTaskboard", new { id = model.TaskListId, sd = model.SubmissionID });
                        }
                        else
                        {
                            return RedirectToAction("Taskboard", new { id = taskItem.TaskListId });
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, the operation could not be completed. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> SelectNewTaskOwner(long id, int tl, string od, string src)
        {
            ReassignTaskViewModel model = new ReassignTaskViewModel();
            List<EmployeeReportLine> directReports = new List<EmployeeReportLine>();
            model.TaskItemID = id;
            model.OldTaskListID = tl;
            model.OldTaskOwnerID = od;
            model.Source = src;

            Employee previousTaskOwner = await _ermService.GetEmployeeByIdAsync(od);
            if (previousTaskOwner != null && !string.IsNullOrWhiteSpace(previousTaskOwner.FullName))
            {
                model.OldTaskOwnerName = previousTaskOwner.FullName;
            }

            var claims = HttpContext.User.Claims.ToList();
            string reportsToEmployeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

            var employee_entities = await _ermService.GetEmployeeReportsByReportsToEmployeeIdAsync(reportsToEmployeeId);
            if (employee_entities != null && employee_entities.Count > 0)
            {
                directReports = employee_entities;
            }

            ViewBag.ReportsList = new SelectList(directReports, "EmployeeID", "EmployeeName", od);
            return View(model);
        }

        [HttpPost]
        public IActionResult SelectNewTaskOwner(ReassignTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["OldTaskOwnerID"] = model.OldTaskOwnerID;
                TempData["OldTaskOwnerName"] = model.OldTaskOwnerName;
                TempData["Source"] = model.Source;
                TempData["OldTaskListID"] = model.OldTaskListID;
                TempData["NewTaskOwnerID"] = model.NewTaskOwnerID;
            }
            return RedirectToAction("SelectNewTaskList", new { id = model.TaskItemID });
        }

        public async Task<IActionResult> SelectNewTaskList(long id)
        {
            ReassignTaskViewModel model = new ReassignTaskViewModel();
            List<TaskList> activeTaskLists = new List<TaskList>();

            if (TempData.ContainsKey("OldTaskOwnerID"))
                model.OldTaskOwnerID = TempData["OldTaskOwnerID"].ToString();

            if (TempData.ContainsKey("OldTaskOwnerName"))
                model.OldTaskOwnerName = TempData["OldTaskOwnerName"].ToString();

            if (TempData.ContainsKey("Source"))
                model.Source = TempData["Source"].ToString();

            if (TempData.ContainsKey("OldTaskListID"))
                model.OldTaskListID = (int)TempData["OldTaskListID"];

            if (TempData.ContainsKey("NewTaskOwnerID"))
                model.NewTaskOwnerID = TempData["NewTaskOwnerID"].ToString();

            TempData.Keep();

            var entities = await _workspaceService.GetActiveTaskListsAsync(model.NewTaskOwnerID);
            if (entities != null && entities.Count > 0)
            {
                activeTaskLists = entities;
            }

            ViewBag.ActiveTaskLists = activeTaskLists;
            return View(model);
        }

        public async Task<IActionResult> ReassignTaskItem(long id, int tl, string od, string src)
        {
            ReassignTaskViewModel model = new ReassignTaskViewModel();
            List<EmployeeReportLine> directReports = new List<EmployeeReportLine>();
            model.TaskItemID = id;
            model.NewTaskListID = tl;
            model.NewTaskOwnerID = od;
            model.Source = src;

            string reassignedBy = HttpContext.User.Identity.Name;

            try
            {

                if (TempData.ContainsKey("OldTaskOwnerID"))
                    model.OldTaskOwnerID = TempData["OldTaskOwnerID"].ToString();

                if (TempData.ContainsKey("OldTaskOwnerName"))
                    model.OldTaskOwnerName = TempData["OldTaskOwnerName"].ToString();

                if (TempData.ContainsKey("Source"))
                    model.Source = TempData["Source"].ToString();

                if (TempData.ContainsKey("OldTaskListID"))
                    model.OldTaskListID = (int)TempData["OldTaskListID"];

                if (TempData.ContainsKey("NewTaskOwnerID"))
                    model.NewTaskOwnerID = TempData["NewTaskOwnerID"].ToString();

                bool IsSuccessful = await _workspaceService.ReassignTaskItemAsync(id, tl, od, model.OldTaskOwnerName, reassignedBy);

                if (IsSuccessful)
                {
                    if (model.Source == "ttb")
                    {
                        return RedirectToAction("TeamTaskboard", new { id = model.OldTaskListID });
                    }
                    else if (model.Source == "txb")
                    {
                        return RedirectToAction("Taskboard", new { id = model.OldTaskListID });
                    }
                    else if (model.Source == "sbt")
                    {
                        return RedirectToAction("SubmittedTasks", new { id = model.OldTaskListID, tp = "Approval" });
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
            return View(model);
        }


        #endregion

        #region Task Submissions Controller Action Methods

        public IActionResult SubmitTaskList(int id)
        {
            SubmitTaskListViewModel model = new SubmitTaskListViewModel();
            model.TaskListID = id;
            model.FromEmployeeName = HttpContext.User.Identity.Name;
            var claims = HttpContext.User.Claims.ToList();
            model.FromEmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitTaskList(SubmitTaskListViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var employee = await _ermService.GetEmployeeByNameAsync(model.ToEmployeeName);
                    if (employee != null) { model.ToEmployeeID = employee.EmployeeID; }
                    TaskSubmission taskSubmission = new TaskSubmission();
                    taskSubmission.IsActioned = false;
                    taskSubmission.Comment = model.Comment;
                    taskSubmission.DateActioned = null;
                    taskSubmission.DateSubmitted = DateTime.UtcNow;
                    taskSubmission.FromEmployeeId = model.FromEmployeeID;
                    taskSubmission.FromEmployeeName = model.FromEmployeeName;
                    taskSubmission.SubmissionType = (WorkItemSubmissionType)model.SubmissionTypeID;
                    taskSubmission.TaskListId = model.TaskListID;
                    taskSubmission.ToEmployeeId = model.ToEmployeeID;
                    taskSubmission.ToEmployeeName = model.ToEmployeeName;

                    bool IsAdded = false;
                    IsAdded = await _workspaceService.AddTaskListSubmissionAsync(taskSubmission);
                    if (IsAdded)
                    {
                        await _workspaceService.UpdateTaskListLockAsync(model.TaskListID, true);
                        ///model.OperationIsSuccessful = true;
                        //model.ViewModelSuccessMessage = "Task List submitted successfully!";
                        return RedirectToAction("TaskList", new { id = model.FromEmployeeID });
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> SubmittedToMe(SubmittedToMeViewModel model)
        {
            if (model == null) { model = new SubmittedToMeViewModel(); }
            var claims = HttpContext.User.Claims.ToList();
            model.EmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var entities = await _workspaceService.SearchTaskSubmissionsAsync(model.EmployeeID, model.SubmittedYear, model.SubmittedMonth, model.FromEmployeeName);
            if (entities != null) { model.TaskSubmissionList = entities.ToList(); }
            if (model.SubmittedYear < 1900) { model.SubmittedYear = DateTime.Now.Year; }
            return View(model);
        }

        public async Task<IActionResult> SubmittedTasks(int id, long sd, string ed, string tp, string od)
        {
            SubmittedTasksViewModel model = new SubmittedTasksViewModel();
            model.TaskListID = id;
            model.TaskSubmissionID = sd;
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
                        var entities = await _workspaceService.GetTaskItemsByTaskListAsync(model.TaskListID, null, null, null);
                        if (entities != null && entities.Count > 0)
                        {
                            model.TaskItems = entities;
                        }
                    }
                }
                else if (tp == "Evaluation")
                {
                    model.PurposeOfSubmission = WorkItemSubmissionType.Evaluation;
                    TaskEvaluationHeader evaluationHeader = await _workspaceService.GetTaskEvaluationHeaderAsync(id, ed);
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

                            evaluationHeader.TaskListId = id;
                            evaluationHeader.EvaluatorId = ed;
                            evaluationHeader.TaskOwnerId = od;
                            evaluationHeader.TaskOwnerUnitId = taskOwner.UnitID ?? 0;
                            evaluationHeader.TaskOwnerDeptId = taskOwner.DepartmentID ?? 0;
                            evaluationHeader.TaskOwnerLocationId = taskOwner.LocationID ?? 0;
                            evaluationHeader.EvaluationDate = DateTime.UtcNow;
                            model.TaskEvaluationHeaderID = await _workspaceService.CreateTaskEvaluationHeaderAsync(evaluationHeader);
                        }

                    }
                }
            }
            else
            {
                if (id > 0)
                {
                    var entities = await _workspaceService.GetTaskItemsByTaskListAsync(model.TaskListID, null, null, null);
                    if (entities != null && entities.Count > 0)
                    {
                        model.TaskItems = entities;
                    }
                }
            }

            return View(model);
        }

        public async Task<IActionResult> SubmittedEvaluations(int id, long sd, string ed, string od)
        {
            SubmittedEvaluationsViewModel model = new SubmittedEvaluationsViewModel();
            model.TaskListID = id;
            model.TaskSubmissionID = sd;
            model.TaskOwnerID = od;
            model.SubmittedToEmployeeName = HttpContext.User.Identity.Name;

            if (!string.IsNullOrWhiteSpace(ed))
            {
                model.SubmittedToEmployeeID = ed;
                model.EvaluatorID = ed;
            }
            else
            {
                var claims = HttpContext.User.Claims.ToList();
                string current_employeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                model.EvaluatorID = current_employeeId;
                model.SubmittedToEmployeeID = current_employeeId;
            }

            model.PurposeOfSubmission = WorkItemSubmissionType.Evaluation;
            TaskEvaluationHeader evaluationHeader = await _workspaceService.GetTaskEvaluationHeaderAsync(id, ed);
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
                    evaluationHeader.TaskListId = id;
                    evaluationHeader.EvaluatorId = ed;
                    evaluationHeader.TaskOwnerId = od;
                    evaluationHeader.TaskOwnerUnitId = taskOwner.UnitID ?? 0;
                    evaluationHeader.TaskOwnerDeptId = taskOwner.DepartmentID ?? 0;
                    evaluationHeader.TaskOwnerLocationId = taskOwner.LocationID ?? 0;
                    evaluationHeader.EvaluationDate = DateTime.UtcNow;
                    model.TaskEvaluationHeaderID = await _workspaceService.CreateTaskEvaluationHeaderAsync(evaluationHeader);
                }
            }

            if (id > 0)
            {
                var entities = await _workspaceService.GetTaskItemEvaluationsAsync(model.TaskListID, model.EvaluatorID);
                if (entities != null && entities.Count > 0)
                {
                    model.TaskItemEvaluations = entities;
                }
            }
            return View(model);
        }


        #endregion

        #region Task Evaluation Controller Action Methods
        public async Task<IActionResult> EvaluateTaskItem(long id, int hd, string src)
        {
            EvaluateTaskItemViewModel model = new EvaluateTaskItemViewModel();
            model.TaskItemID = id;
            model.TaskEvaluationHeaderID = hd;
            model.Source = src;
            model.EvaluatorName = HttpContext.User.Identity.Name;
            try
            {
                if (id > 0 && hd > 0)
                {
                    TaskEvaluationHeader header = new TaskEvaluationHeader();
                    header = await _workspaceService.GetTaskEvaluationHeaderAsync(hd);
                    if (header != null && !string.IsNullOrWhiteSpace(header.TaskOwnerId))
                    {
                        model.TaskOwnerID = header.TaskOwnerId;
                        model.EvaluatorID = header.EvaluatorId;
                    }

                    TaskEvaluationDetail detail = new TaskEvaluationDetail();
                    detail = await _workspaceService.GetTaskEvaluationDetailAsync(hd, id);
                    if (detail != null && detail.TaskListId > 0)
                    {
                        model.TaskEvaluationDetailID = detail.TaskEvaluationDetailId;
                        model.TaskListID = detail.TaskListId;
                        model.TaskItemNo = detail.TaskItemNo;
                        model.TaskItemDescription = detail.TaskItemDescription;
                        model.TaskItemDeliverable = detail.TaskItemDeliverable;
                        model.TaskOwnerName = detail.TaskOwnerName;
                        model.PercentageCompletion = detail.PercentageCompletion;
                        model.QualityRating = detail.QualityRating;
                        model.EvaluatorsComment = detail.EvaluatorsComment;
                    }
                    else
                    {
                        TaskItem taskItem = new TaskItem();
                        taskItem = await _workspaceService.GetTaskItemAsync(id);
                        if (taskItem != null && !string.IsNullOrWhiteSpace(taskItem.Description))
                        {
                            //model.TaskItemID = taskItem.Id;
                            model.TaskOwnerID = taskItem.TaskOwnerId;
                            model.TaskListID = taskItem.TaskListId;
                            model.TaskItemNo = taskItem.Number;
                            model.TaskItemDescription = taskItem.Description;
                            model.TaskItemDeliverable = taskItem.Deliverable;
                            model.TaskOwnerName = taskItem.TaskOwnerName;
                            model.PercentageCompletion = 100;
                            model.QualityRating = 5;
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
        public async Task<IActionResult> EvaluateTaskItem(EvaluateTaskItemViewModel model)
        {
            TaskEvaluationDetail detail = new TaskEvaluationDetail();
            if (ModelState.IsValid)
            {
                try
                {
                    detail = model.ConvertToTaskEvaluationDetail();
                    detail.EvaluatorName = HttpContext.User.Identity.Name;
                    detail.EvaluationDate = DateTime.UtcNow;
                    var claims = HttpContext.User.Claims.ToList();
                    string EvaluatorID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();


                    bool IsAdded = await _workspaceService.AddTaskEvaluationDetailAsync(detail);
                    if (IsAdded)
                    {
                        if (model.Source == "sbt")
                        {
                            return RedirectToAction("SubmittedTasks", new { id = model.TaskListID, tp = "Evaluation", ed = EvaluatorID, od = model.TaskOwnerID });
                        }
                        else if (model.Source == "sbv")
                        {
                            return RedirectToAction("SubmittedEvaluations", new { id = model.TaskListID, ed = EvaluatorID, od = model.TaskOwnerID });
                        }
                        else if (model.Source == "txb")
                        {
                            return RedirectToAction("Taskboard", new { id = model.TaskListID });
                        }
                        else
                        {
                            return RedirectToAction("SubmittedToMe");
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, the operation could not be completed. Please try again.";
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

        #region Task Evaluation Results
        public async Task<IActionResult> EvaluationLists(int? yy, int? mm)
        {
            EvaluationListsViewModel model = new EvaluationListsViewModel();
            List<TaskEvaluationHeader> headerList = new List<TaskEvaluationHeader>();

            if (yy == null || yy < 2000)
            {
                model.yy = DateTime.Now.Year;
            }

            if (mm == null || mm < 1 || mm > 12)
            {
                model.mm = 0;
            }

            var claims = HttpContext.User.Claims.ToList();
            string current_employeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            model.TaskOwnerID = current_employeeId;

            headerList = await _workspaceService.GetTaskEvaluationHeadersByTaskOwnerIdAsync(model.TaskOwnerID, model.yy, model.mm);
            if (headerList != null && headerList.Count > 0)
            {
                model.EvaluationHeaderList = headerList;
            }

            return View(model);
        }

        public async Task<IActionResult> TeamEvaluationLists(int? yy = null, int? mm = null, string md = null)
        {
            EvaluationListsViewModel model = new EvaluationListsViewModel();
            List<TaskEvaluationHeader> headerList = new List<TaskEvaluationHeader>();
            List<EmployeeReportLine> directReports = new List<EmployeeReportLine>();

            try
            {
                if (yy == null || yy < 2000)
                {
                    model.yy = DateTime.Now.Year;
                }

                if (mm == null || mm < 1 || mm > 12)
                {
                    model.mm = 0;
                }
                var claims = HttpContext.User.Claims.ToList();
                string reportsToEmployeeId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                var employee_entities = await _ermService.GetEmployeeReportsByReportsToEmployeeIdAsync(reportsToEmployeeId);
                if (employee_entities != null && employee_entities.Count > 0)
                {
                    directReports = employee_entities;
                }

                ViewBag.ReportsList = new SelectList(directReports, "EmployeeID", "EmployeeName", md);

                if (!string.IsNullOrWhiteSpace(md))
                {
                    model.TaskOwnerID = md;
                    headerList = await _workspaceService.GetTaskEvaluationHeadersByTaskOwnerIdAsync(model.TaskOwnerID, model.yy, model.mm);
                    if (headerList != null && headerList.Count > 0)
                    {
                        model.EvaluationHeaderList = headerList;
                    }
                }
                else
                {
                    headerList = await _workspaceService.GetTaskEvaluationHeadersByReportsToEmployeeIdAsync(reportsToEmployeeId, model.yy, model.mm);
                    if (headerList != null && headerList.Count > 0)
                    {
                        model.EvaluationHeaderList = headerList;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> EvaluationTasks(int id)
        {
            EvaluationTasksViewModel model = new EvaluationTasksViewModel();
            List<TaskEvaluationDetail> evaluationDetails = new List<TaskEvaluationDetail>();
            if (id < 1) { model.EvaluationDetailList = evaluationDetails; }
            else
            {
                model.EvaluationHeaderID = id;
                var entities = await _workspaceService.GetTaskEvaluationDetailsAsync(model.EvaluationHeaderID);
                if (entities != null && entities.Count > 0)
                {
                    model.EvaluationDetailList = entities;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> TeamEvaluationTasks(int id, string md)
        {
            EvaluationTasksViewModel model = new EvaluationTasksViewModel();
            model.md = md;
            List<TaskEvaluationDetail> evaluationDetails = new List<TaskEvaluationDetail>();
            if (id < 1) { model.EvaluationDetailList = evaluationDetails; }
            else
            {
                model.EvaluationHeaderID = id;
                var entities = await _workspaceService.GetTaskEvaluationDetailsAsync(model.EvaluationHeaderID);
                if (entities != null && entities.Count > 0)
                {
                    model.EvaluationDetailList = entities;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "WKSVWAEER, XYALLACCZ")]
        public async Task<IActionResult> SearchEvaluationLists(int? yy = null, int? mm = null, string sn = null, string tp = null)
        {
            SearchEvaluationListsViewModel model = new SearchEvaluationListsViewModel();
            List<TaskEvaluationHeader> headerList = new List<TaskEvaluationHeader>();
            List<TaskList> taskLists = new List<TaskList>();
            List<EmployeeReportLine> directReports = new List<EmployeeReportLine>();
            Employee employee = new Employee();
            model.tp = tp;
            try
            {
                if (yy == null || yy < 2000)
                {
                    model.yy = DateTime.Now.Year;
                }

                if (mm == null || mm < 1 || mm > 12)
                {
                    model.mm = 0;
                }

                if (string.IsNullOrWhiteSpace(sn))
                {
                    model.EvaluationHeaderList = headerList;
                    model.TaskLists = taskLists;
                }
                else
                {
                    var entity = await _ermService.GetEmployeeByNameAsync(sn);
                    if (entity == null || string.IsNullOrWhiteSpace(tp))
                    {
                        model.EvaluationHeaderList = headerList;
                        model.TaskLists = taskLists;
                    }
                    else
                    {
                        employee = entity;
                        model.TaskOwnerID = employee.EmployeeID;
                        if (tp == "evl")
                        {
                            headerList = await _workspaceService.GetTaskEvaluationHeadersByTaskOwnerIdAsync(model.TaskOwnerID, model.yy, model.mm);
                            if (headerList != null && headerList.Count > 0)
                            {
                                model.EvaluationHeaderList = headerList;
                            }
                        }
                        else if (tp == "tsk")
                        {
                            taskLists = await _workspaceService.GetTaskListsByOwnerIdAsync(model.TaskOwnerID);
                            if (taskLists != null && taskLists.Count > 0)
                            {
                                model.TaskLists = taskLists;
                            }
                        }
                        else
                        {
                            model.EvaluationHeaderList = headerList;
                            model.TaskLists = taskLists;
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

        [Authorize(Roles = "WKSVWAETK, XYALLACCZ")]
        public async Task<IActionResult> SearchEvaluationTasks(int id, string md)
        {
            EvaluationTasksViewModel model = new EvaluationTasksViewModel();
            model.md = md;
            List<TaskEvaluationDetail> evaluationDetails = new List<TaskEvaluationDetail>();
            if (id < 1) { model.EvaluationDetailList = evaluationDetails; }
            else
            {
                model.EvaluationHeaderID = id;
                var entities = await _workspaceService.GetTaskEvaluationDetailsAsync(model.EvaluationHeaderID);
                if (entities != null && entities.Count > 0)
                {
                    model.EvaluationDetailList = entities;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> SearchTaskboard(int id, int? ps = null, int? dy = null, int? dm = null)
        {
            TaskBoardViewModel model = new TaskBoardViewModel();
            List<TaskItem> TaskItemList = new List<TaskItem>();
            model.id = id;
            model.ps = ps;

            try
            {
                if (dy == null) { model.dy = DateTime.Now.Year; } else { model.dy = dy; }
                model.dm = dm;

                if (id > 0)
                {
                    TaskList taskList = new TaskList();
                    taskList = await _workspaceService.GetTaskListAsync(id);
                    if (taskList != null) { model.TaskListIsLocked = taskList.IsLocked; }

                    var entities = await _workspaceService.GetTaskItemsByTaskListAsync(model.id, model.dy, model.dm, (WorkItemProgressStatus?)model.ps);
                    if (entities != null && entities.Count > 0)
                    {
                        model.TaskItems = entities;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        #endregion

        #region Reports

        public async Task<IActionResult> ProductivityReport()
        {
            ProductivityReportViewModel model = new ProductivityReportViewModel();
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaderSummaryList = new List<TaskEvaluationHeaderSummary>();

            model.EvaluationHeaderSummaryList = taskEvaluationHeaderSummaryList;
            model.FromYear = DateTime.Now.Year;
            model.ToYear = DateTime.Now.Year;

            var location_entities = await _globalSettingsService.GetAllLocationsAsync();
            if(location_entities != null)
            {
                ViewBag.LocationList = new SelectList(location_entities, "LocationID", "LocationName", model.LocationID);
            }

            var department_entities = await _globalSettingsService.GetDepartmentsAsync();
            if(department_entities != null)
            {
                ViewBag.DepartmentList = new SelectList(department_entities, "DepartmentID", "DepartmentName", model.DepartmentID);
            }

            var unit_entities = await _globalSettingsService.GetUnitsAsync();
            if(unit_entities != null)
            {
                ViewBag.UnitList = new SelectList(unit_entities, "UnitID", "UnitName", model.UnitID);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductivityReport(ProductivityReportViewModel model = null)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaderSummaryList = new List<TaskEvaluationHeaderSummary>();
            model.ReportTitle = "Staff Productivity Summary Report";
            string fromMonthName = string.Empty;
            string toMonthName = string.Empty;
            if(model.FromMonth != null && model.FromMonth > 0 )
            {
                DateTime fromDate = new DateTime(2020, model.FromMonth.Value, 1);
                fromMonthName = fromDate.ToString("MMMM");
            }
            else { fromMonthName = "January"; }

            if (model.ToMonth != null && model.ToMonth > 0)
            {
                DateTime toDate = new DateTime(2020, model.ToMonth.Value, 1);
                toMonthName = toDate.ToString("MMMM");
            }
            else { toMonthName = "December"; }

            model.ReportStartDate = $"{fromMonthName}, {model.FromYear}";
            model.ReportEndDate = $"{toMonthName}, {model.ToYear}";

            if (ModelState.IsValid)
            {
                taskEvaluationHeaderSummaryList = await _workspaceService.GetTaskEvaluationHeaderSummaryAsync(model.FromYear, model.ToYear, model.FromMonth, model.ToMonth, model.UnitID, model.DepartmentID, model.LocationID);
            }
            model.EvaluationHeaderSummaryList = taskEvaluationHeaderSummaryList;
            var location_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (location_entities != null)
            {
                ViewBag.LocationList = new SelectList(location_entities, "LocationID", "LocationName", model.LocationID);
            }

            var department_entities = await _globalSettingsService.GetDepartmentsAsync();
            if (department_entities != null)
            {
                ViewBag.DepartmentList = new SelectList(department_entities, "DepartmentID", "DepartmentName", model.DepartmentID);
            }

            var unit_entities = await _globalSettingsService.GetUnitsAsync();
            if (unit_entities != null)
            {
                ViewBag.UnitList = new SelectList(unit_entities, "UnitID", "UnitName", model.UnitID);
            }

            return View(model);
        }

        public async Task<IActionResult> ExportProductivityReportToExcel(int fy, int ty, int? fm, int? tm, int? ud, int? dd, int? ld)
        {
            List<TaskEvaluationHeaderSummary> reportItems = new List<TaskEvaluationHeaderSummary>();
            string ReportTitle = "Staff Productivity Summary Report";
            string fileName = $"productivity_report_{DateTime.Now.Ticks}.xlsx";
            string fromMonthName = string.Empty;
            string toMonthName = string.Empty;
            if (fm != null && fm > 0)
            {
                DateTime fromDate = new DateTime(2020, fm.Value, 1);
                fromMonthName = fromDate.ToString("MMMM");
            }
            else { fromMonthName = "January"; }

            if (tm != null && tm > 0)
            {
                DateTime toDate = new DateTime(2020, tm.Value, 1);
                toMonthName = toDate.ToString("MMMM");
            }
            else { toMonthName = "December"; }

            string ReportStartDate = $"{fromMonthName}, {fy}";
            string ReportEndDate = $"{toMonthName}, {ty}";

            reportItems = await _workspaceService.GetTaskEvaluationHeaderSummaryAsync(fy, ty, fm, tm, ud, dd, ld);
             
            using(var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                //First Excel Worksheet Row for the Report Title
                worksheet.Cell(1, 1).Value = ReportTitle;

                //Second Excel Worksheet Row for the Report Periods
                worksheet.Cell(2, 1).Value = $"From: {ReportStartDate}";
                worksheet.Cell(2, 3).Value = $"To: {ReportEndDate}";

                //Third Excel Worksheet Row for the Column Headers
                worksheet.Cell(4, 1).Value = "Staff Name";
                worksheet.Cell(4, 2).Value = "Location";
                worksheet.Cell(4, 3).Value = "Department";
                worksheet.Cell(4, 4).Value = "Unit";
                worksheet.Cell(4, 5).Value = "Total";
                worksheet.Cell(4, 6).Value = "Completed";
                worksheet.Cell(4, 7).Value = "Uncompleted";
                worksheet.Cell(4, 8).Value = "% Completion";
                worksheet.Cell(4, 9).Value = "Average Rating";

                //Excel Worksheet Rows for the Report Data
                var currentRow = 4;
                foreach (var report in reportItems)
                {
                    currentRow++;
                    string rating = string.Empty;
                    switch (Convert.ToInt64(report.AverageQualityRating))
                    {
                        case 1:
                            rating = "Very Poor";
                            break;
                        case 2:
                            rating = "Poor";
                            break;
                        case 3:
                            rating = "Good";
                            break;
                        case 4:
                            rating = "Very Good";
                            break;
                        case 5:
                            rating = "Excellent";
                            break;
                        default:
                            break;
                    }

                    worksheet.Cell(currentRow, 1).Value = report.TaskOwnerName;
                    worksheet.Cell(currentRow, 2).Value = report.TaskOwnerLocationName;
                    worksheet.Cell(currentRow, 3).Value = report.TaskOwnerDepartmentName;
                    worksheet.Cell(currentRow, 4).Value = report.TaskOwnerUnitName;
                    worksheet.Cell(currentRow, 5).Value = report.TotalNoOfTasks;
                    worksheet.Cell(currentRow, 6).Value = report.TotalNoOfCompletedTasks;
                    worksheet.Cell(currentRow, 7).Value = report.TotalNoOfUncompletedTasks;
                    worksheet.Cell(currentRow, 8).Value = $"{Convert.ToInt64(report.AveragePercentCompletion)}%";
                    worksheet.Cell(currentRow, 9).Value = rating;
                }

                using(var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        #endregion

        #region Helper Action Methods
        public string UpdateTaskListArchive(int id, bool st)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_workspaceService.UpdateTaskListArchiveAsync(id, st).Result)
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

        public string UpdateTaskStatus(int id, bool cls)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            WorkItemStatus newStatus = new WorkItemStatus();
            if (cls) { newStatus = WorkItemStatus.Closed; }
            else { newStatus = WorkItemStatus.Open; }
            try
            {
                if (_workspaceService.UpdateTaskStatusAsync(id, newStatus, actionBy).Result)
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

        public string CancelTaskItem(int id, bool ccl)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_workspaceService.UpdateTaskCancelStatusAsync(id, actionBy, ccl).Result)
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
                return " service error";
            }
        }

        public string DeleteTask(int id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_workspaceService.DeleteTaskItemAsync(id).Result)
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

        public string ApproveTaskItem(int id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_workspaceService.UpdateTaskApprovalStatusAsync(id, ApprovalStatus.Approved, actionBy).Result)
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
                return " service error";
            }
        }

        public string DeclineTaskItem(int id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_workspaceService.UpdateTaskApprovalStatusAsync(id, ApprovalStatus.Declined, actionBy).Result)
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
                return " service error";
            }
        }

        public string ReturnTaskList(int id, long sd)
        {
            if (id < 1 || sd < 1) { return "parameter error"; }
            try
            {
                if (_workspaceService.ReturnTaskListSubmissionAsync(id, sd).Result)
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
                //return " service error";
            }
        }

        public string DeleteTaskListSubmission(long id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_workspaceService.DeleteTaskListSubmissionAsync(id).Result)
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
    }
}