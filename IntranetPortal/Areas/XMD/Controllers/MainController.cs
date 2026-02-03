using IntranetPortal.Areas.XMD.Models;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.XMD.Controllers
{
    [Area("XMD")]
    public class MainController : Controller
    {
        private readonly IWspService _workspaceService;
        private readonly IErmService _ermService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IConfiguration _configuration;

        public MainController(IWspService workspaceService, IErmService ermService, IBaseModelService baseModelService,
         IGlobalSettingsService globalSettingsService, IConfiguration configuration)
        {
            _workspaceService = workspaceService;
            _ermService = ermService;
            _baseModelService = baseModelService;
            _globalSettingsService = globalSettingsService;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> XmProjectList(int tp, int? st = null)
        {
            ProjectListViewModel model = new ProjectListViewModel();
            model.tp = tp;
            model.st = st;
            model.ProjectList = await _workspaceService.GetExecutiveManagementProjectsAsync(tp, st);
            return View(model);
        }

        public async Task<IActionResult> ManageXmProject(long id = 0, string src = null)
        {
            ManageProjectViewModel model = new ManageProjectViewModel();
            model.ProjectId = id;

            try
            {
                if (model.ProjectId < 1)
                {
                    string projectNo = await _baseModelService.GenerateAutoNumberAsync("projno");
                    if (!string.IsNullOrWhiteSpace(projectNo)) { model.ProjectCode = $"XP{projectNo}"; }

                    if (string.IsNullOrWhiteSpace(model.ProjectOwnerId))
                    {
                        var claims = HttpContext.User.Claims.ToList();
                        model.ProjectOwnerId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                        if (string.IsNullOrWhiteSpace(model.ProjectOwnerId))
                        {
                            model.ViewModelErrorMessage = "Oops! It appears you session has expired. Please login again and try again.";
                            return View(model);
                        }
                    }
                    Employee employee = await _ermService.GetEmployeeByIdAsync(model.ProjectOwnerId);
                    if (employee == null || string.IsNullOrWhiteSpace(employee.FullName))
                    {
                        model.ViewModelErrorMessage = "No employee record was found for the active user. Please login and try again.";
                        return View(model);
                    }
                    model.ProjectOwnerId = employee.EmployeeID;
                    model.ProjectOwnerName = employee.FullName;
                    model.UnitId = employee.UnitID;
                    model.DepartmentId = employee.DepartmentID;
                    model.LocationId = employee.LocationID;
                    model.ExpectedStartTime = null;
                    model.ExpectedEndTime = null;
                }
                else
                {
                    var project = await _workspaceService.GetProjectAsync(model.ProjectId);
                    if (project != null)
                    {
                        model = model.Convert(project);
                        model.SourcePage = src;
                    }
                }
                var entities = await _workspaceService.GetExecutiveManagementProjectTypesAsync();
                if (entities != null && entities.Count > 0)
                {
                    ViewBag.ProjectTypesList = new SelectList(entities, "ProjectTypeId", "ProjectTypeDescription", model.ProjectTypeId);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageXmProject(ManageProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Project project = model.Convert();
                    if (project.ProjectId > 0)
                    {
                        project.ModifiedBy = HttpContext.User.Identity.Name;
                        project.IsExecutiveManagementProject = true;
                        bool isUpdated = await _workspaceService.UpdateProjectAsync(project);
                        if (isUpdated)
                        {
                            return RedirectToAction("XmProjectList");
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                    else
                    {
                        switch (project.ProgressStatusId)
                        {
                            case 1:
                            case 3:
                                project.ActualStartTime = DateTime.Now;
                                break;
                            case 2:
                                project.ActualStartTime = DateTime.Now;
                                project.ActualEndTime = DateTime.Now;
                                break;
                            default:
                                break;
                        }
                        project.ModifiedBy = HttpContext.User.Identity.Name;
                        project.IsExecutiveManagementProject = true;
                        long newProjectId = await _workspaceService.CreateProjectAsync(project);
                        if (newProjectId > 0)
                        {
                            return RedirectToAction("XmProjectList");
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
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

        public async Task<IActionResult> ProjectTasks(long id, string nm, int? ps = null, int? dm = null, string src = null)
        {
            ProjectTaskListViewModel model = new ProjectTaskListViewModel();
            model.SourcePage = src;
            model.ProjectID = id;
            model.ProjectTitle = nm;
            model.ProgressStatusID = ps;

            Project project = await _workspaceService.GetProjectAsync(model.ProjectID);
            if (project != null)
            {
                model.ProjectNo = project.ProjectCode;
                model.ProjectTitle = project.ProjectTitle;
                model.ProjectOwnerID = project.ProjectOwnerId;
                model.ProjectOwnerName = project.ProjectOwnerName;
                var entities = await _workspaceService.GetDelegatedTaskItemsByProjectNumberAsync(model.ProjectNo);
                if (entities != null) { model.TaskItems = entities; }
            }
            return View(model);
        }

        public async Task<IActionResult> XmDelegateTask(long id = 0, string ed = null, long? pd = null, string src = null, long? sd = null, string pn = null)
        {
            XmDelegateTaskViewModel model = new XmDelegateTaskViewModel();
            long? _projectId = pd;

            model.SourcePage = src;
            model.FolderSubmissionId = sd;
            model.TaskDelegationId = id;
            model.LinkProjectNumber = pn;
            model.TaskOwnerId = ed;

            try
            {
                if (id > 0)
                {
                    DelegatedTaskItem task = await _workspaceService.GetDelegatedTaskItemAsync(id);
                    if (task != null)
                    {
                        model = model.Convert(task);
                        if(pd == null && !string.IsNullOrWhiteSpace(pn))
                        {
                            Project project = await _workspaceService.GetProjectAsync(pn);
                            if (project != null) { _projectId = project.ProjectId; model.LinkProjectNumber = project.ProjectCode; }
                        }
                        else if(pd != null && pd > 0)
                        {
                            Project project = await _workspaceService.GetProjectAsync(pd.Value);
                            if (project != null) { _projectId = project.ProjectId;  model.LinkProjectNumber = project.ProjectCode; }
                        }
                    }
                }
                else
                {
                    model.DelegatedByEmployeeName = HttpContext.User.Identity.Name;
                    model.DelegatedTime = DateTime.Now;
                    Employee employee = new Employee();

                    if (pd > 0 && string.IsNullOrWhiteSpace(pn))
                    {
                        Project project = new Project();
                        project = await _workspaceService.GetProjectAsync(pd.Value);
                        if (project != null) { model.LinkProjectNumber = project.ProjectCode; }
                    }

                    string taskNo = await _baseModelService.GenerateAutoNumberAsync("taskno");
                    if (!string.IsNullOrWhiteSpace(taskNo)) { model.Number = $"X{taskNo}"; }
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
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            model.LinkProjectId = _projectId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XmDelegateTask(XmDelegateTaskViewModel model)
        {
            long? _projectId = model.LinkProjectId;
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
                    bool IsCreated = await _workspaceService.CreateDelegatedTaskItemAsync(task);
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


                        if (model.SourcePage == "pjt")
                        {
                            return RedirectToAction("ProjectTasks", new { id = _projectId });
                        }
                        else if (model.SourcePage == "mtl")
                        {
                            return RedirectToAction("MyTaskList", new { id = model.WorkFolderId, nm = model.WorkFolderName });
                        }
                        else if (model.SourcePage == "sbt")
                        {
                            return RedirectToAction("SubmittedTasks", new { id = model.WorkFolderId, sd = model.FolderSubmissionId, tp = "Approval", od = model.TaskOwnerId });
                        }
                        else
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

        public async Task<IActionResult> XmReassignTask(long id, long td, long pd, string src = null, long? sd = null)
        {
            XmReassignTaskViewModel model = new XmReassignTaskViewModel();
            model.OldTaskDelegationId = id;
            model.TaskItemId = td;
            model.LinkProjectId = pd;
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
                    DelegatedTaskItem d = await _workspaceService.GetDelegatedTaskItemAsync(model.OldTaskDelegationId);
                    if (d != null)
                    {
                        model.ProgressStatusId = d.ProgressStatusId;
                        model.TaskItemDescription = d.Description;
                        model.TaskNumber = d.Number;
                        model.ExpectedDueTime = d.ExpectedDueTime;
                        model.ExpectedStartTime = d.ExpectedStartTime;
                        model.OldTaskDelegationId = d.TaskDelegationId;
                        model.TaskItemId = d.Id;
                        model.LinkProjectCode = d.LinkProjectNumber;
                    }
                    else
                    {
                        TaskItem t = await _workspaceService.GetTaskItemByIdAsync(model.TaskItemId);
                        if (t != null)
                        {
                            model.TaskItemId = t.Id;
                            model.TaskNumber = t.Number;
                            model.TaskItemDescription = t.Description;
                            model.MoreInformation = t.MoreInformation;
                            model.ProgressStatusId = t.ProgressStatusId;
                            model.ExpectedDueTime = t.ExpectedDueTime;
                            model.ExpectedStartTime = t.ExpectedStartTime;
                            model.LinkProjectCode = t.LinkProjectNumber;
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
        public async Task<IActionResult> XmReassignTask(XmReassignTaskViewModel model)
        {
            string _delegatedToEmployeeId = "";
            if (ModelState.IsValid)
            {
                DelegatedTaskItem delegatedTaskItem = await _workspaceService.GetDelegatedTaskItemAsync(model.OldTaskDelegationId);
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
                            delegatedTaskItem.DelegatedToEmployeeId = _delegatedToEmployeeId = employee.PersonID;
                            delegatedTaskItem.TaskOwnerId = employee.EmployeeID;
                            delegatedTaskItem.UnitId = employee.UnitID;
                            delegatedTaskItem.DepartmentId = employee.DepartmentID;
                            delegatedTaskItem.LocationId = employee.LocationID;
                        }
                    }

                    delegatedTaskItem.LastModifiedTime = DateTime.Now;
                    bool IsReassigned = await _workspaceService.ReDelegateTaskItemAsync(delegatedTaskItem, model.OldTaskDelegationId);
                    if (IsReassigned)
                    {
                        //============= Notification Code Starts Here =========================//

                        Employee sender = new Employee();
                        sender = await _ermService.GetEmployeeByIdAsync(model.DelegatedByEmployeeId);
                        Employee receiver = new Employee();
                        receiver = await _ermService.GetEmployeeByIdAsync(_delegatedToEmployeeId);

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

                        if (model.SourcePage == "pjt")
                        {
                            return RedirectToAction("ProjectTasks", new { id = model.LinkProjectId });
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

        public async Task<IActionResult> Notes(string sp, long pd = 0, long td = 0)
        {
            ProjectNotesViewModel model = new ProjectNotesViewModel();
            model.SourcePage = sp;
            model.ProjectID = pd;
            model.TaskItemID = td;

            if (model.ProjectID < 1 && model.TaskItemID < 1) { return View(model); }
            if (model.TaskItemID > 0)
            {
                var taskItemNotes = await _workspaceService.GetTaskItemNotesAsync(model.TaskItemID.Value);
                if (taskItemNotes != null) { model.NoteList = taskItemNotes; }
            }
            else if(model.ProjectID > 0)
            {
                var projectNotes = await _workspaceService.GetProjectNotesAsync(model.ProjectID.Value);
                if (projectNotes != null) { model.NoteList = projectNotes; }
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

        public async Task<IActionResult> history(long pd = 0, long td = 0)
        {
            ProjectActivitiesViewModel model = new ProjectActivitiesViewModel();
            model.ProjectID = pd;
            model.TaskItemID = td;
            if (pd < 1 && td < 1) { return View(model); }

            if (model.TaskItemID > 0)
            {
                var taskActivities = await _workspaceService.GetWorkItemActivitiesByTaskIdAsync(model.TaskItemID);
                if (taskActivities != null) { model.ActivityList = taskActivities; }
            }
            else if (model.ProjectID > 0)
            {
                var projectActivities = await _workspaceService.GetWorkItemActivitiesByProjectIdAsync(model.ProjectID);
                if (projectActivities != null) { model.ActivityList = projectActivities; }
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

            var _folderEntities = await _workspaceService.GetWorkItemFoldersAsync(model.sd.Value, model.ed.Value, model.vs, model.ld, model.dd, model.ud, model.id, model.sn, model.gd);
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

            var emp_entities = await _ermService.GetEmployeeRollsAsync(null, model.ld, model.dd, model.ud, model.id, model.sn);
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
                WorkItemFolder folder = await _workspaceService.GetWorkItemFolderAsync(model.FolderID);
                if (folder != null)
                {
                    model.FolderIsArchived = folder.IsArchived;
                    model.FolderIsLocked = folder.IsLocked;
                    model.FolderTitle = folder.Title;
                    model.FolderOwnerID = folder.OwnerId;
                    model.FolderOwnerName = folder.OwnerName;
                }
                var entities = await _workspaceService.GetTasksByFolderIdAsync(model.FolderID);
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
                var entities = await _workspaceService.GetTaskItemEvaluationsAsync(model.FolderID);
                if (entities != null && entities.Count > 0)
                {
                    model.TaskItemEvaluations = entities;
                    model.SubmittedToEmployeeName = entities[0].EvaluatorName;

                }
            }
            return View(model);
        }






        #region Project Helper Methods
        public string DeleteProject(long id)
        {
            if (id < 1) { return "parameter error"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_workspaceService.DeleteProjectAsync(id).Result)
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

        public string SaveProjectNote(string nm, string msg, long pd = 0, long td = 0)
        {
            WorkItemNote note = new WorkItemNote()
            {
                NoteTime = DateTime.Now,
                NoteWrittenBy = nm,
                NoteContent = msg,
                ProjectId = pd == 0 ? (long?)null : pd,
                TaskItemId = td == 0 ? (long?)null : td,
            };

            if ((note.ProjectId < 1 && note.TaskItemId < 0) || string.IsNullOrWhiteSpace(nm) || string.IsNullOrWhiteSpace(msg)) { return "parameter"; }
            try
            {
                if (_workspaceService.AddWorkItemNoteAsync(note).Result)
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

        #endregion
    }
}
