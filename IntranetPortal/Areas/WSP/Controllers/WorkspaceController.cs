using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IntranetPortal.Areas.WSP.Models;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IntranetPortal.Areas.WSP.Controllers
{
    [Area("WSP")]
    public class WorkspaceController : Controller
    {
        private readonly IWspService _wspService;
        private readonly IErmService _ermService;
        private readonly IBaseModelService _baseModelService;
        public WorkspaceController(IWspService wspService, IErmService ermService, IBaseModelService baseModelService)
        {
            _wspService = wspService;
            _ermService = ermService;
            _baseModelService = baseModelService;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region Task Folders Controller Action Methods
        public async Task<IActionResult> MyTaskFolders(string id = null, int y = 0, int? m = null)
        {
            TaskFoldersViewModel model = new TaskFoldersViewModel();
            string ownerName = HttpContext.User.Identity.Name;
            model.Id = id;
            model.Mn = m;
            try
            {
                if (y == 0) { model.Yr = DateTime.Today.Year; } else { model.Yr = y; }

                if (string.IsNullOrWhiteSpace(model.Id))
                {
                    var claims = HttpContext.User.Claims.ToList();
                    model.Id = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                }

                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    var entities = await _wspService.SearchWorkItemFoldersAsync(model.Id, false, model.Yr, model.Mn);
                    if (entities != null && entities.Count > 0)
                    {
                        model.TaskFolders = entities;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }
        public async Task<IActionResult> TeamTaskFolders(string id = null, int? y = null, int? m = null)
        {
            List<EmployeeReportLine> directReports = new List<EmployeeReportLine>();
            TaskFoldersViewModel model = new TaskFoldersViewModel
            {
                TaskFolders = new List<WorkItemFolder>(),
                Id = id,
                Mn = m
            };

            if (y == null) { model.Yr = DateTime.Now.Year; }
            else { model.Yr = y.Value; }

            try
            {
                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    var entities = await _wspService.SearchWorkItemFoldersAsync(model.Id, null, model.Yr, model.Mn);
                    if (entities != null && entities.Count > 0)
                    {
                        model.TaskFolders = entities;
                    }
                }

                var claims = HttpContext.User.Claims.ToList();
                string reportsToEmployeeID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                var employee_entities = await _ermService.GetEmployeeReportsByReportsToEmployeeIdAsync(reportsToEmployeeID);
                if (employee_entities != null && employee_entities.Count > 0)
                {
                    directReports = employee_entities;
                }

                ViewBag.ReportsList = new SelectList(directReports, "EmployeeID", "EmployeeName", model.Id);
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }
        public async Task<IActionResult> MyArchivedTaskFolders(string id = null, int? y = null, int? m = null)
        {
            TaskFoldersViewModel model = new TaskFoldersViewModel();
            string ownerName = HttpContext.User.Identity.Name;
            model.Id = id;
            if (string.IsNullOrWhiteSpace(id))
            {
                var claims = HttpContext.User.Claims.ToList();
                model.Id = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(model.Id))
            {
                var entities = await _wspService.SearchWorkItemFoldersAsync(model.Id, true, y, m);
                if (entities != null && entities.Count > 0)
                {
                    model.TaskFolders = entities;
                }
            }

            if (y == null) { model.Yr = DateTime.Now.Year; }
            else { model.Yr = y.Value; }

            if (m == null) { model.Mn = DateTime.Now.Month; }
            else { model.Mn = m.Value; }

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
                    model.PeriodEndDate = DateTime.Today;
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
        public async Task<IActionResult> Notes(string sp, long? td, long? fd, long? pd )
        {
            WorkItemNotesViewModel model = new WorkItemNotesViewModel();
            model.SourcePage = sp;
            model.TaskID = td;
            model.FolderID = fd;
            model.ProjectID = pd;
            if (td < 1 && fd < 1 && pd < 1) { return View(model); }
            else
            {
                if(model.TaskID > 0)
                {
                    var taskNotes = await _wspService.GetTaskItemNotesAsync(model.TaskID.Value);
                    if (taskNotes != null) { model.NoteList = taskNotes; }
                }
                else if(model.TaskID < 1 && model.FolderID > 0)
                {
                        var folderNotes = await _wspService.GetWorkItemFolderNotesAsync(model.FolderID.Value);
                        if (folderNotes != null) { model.NoteList = folderNotes; }
                }
                else if(model.ProjectID > 0)
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
        public async Task<IActionResult> Activities(long id, string tp)
        {
            WorkItemActivitiesViewModel model = new WorkItemActivitiesViewModel();
            if (id < 1) { return View(model); }

            if (tp == "t")
            {
                model.TaskID = id;
                var entities = await _wspService.GetWorkItemActivitiesByTaskIdAsync(id);
                if (entities != null) { model.ActivityList = entities; }
            }
            else if (tp == "f")
            {
                model.FolderID = id;
                var entities = await _wspService.GetWorkItemActivitiesByFolderIdAsync(id);
                if (entities != null) { model.ActivityList = entities; }
            }
            else if (tp == "p")
            {
                model.ProjectID = id;
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
            model.FolderOwnerName = HttpContext.User.Identity.Name;
            var claims = HttpContext.User.Claims.ToList();
            model.FolderOwnerID = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

            if (id < 1)
            {
                model.FolderID = 0;
                model.FolderTitle = "Pending Tasks";
                model.IsPendingTasks = true;
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
                }
                var entities = await _wspService.GetTasksByFolderIdAsync(model.FolderID);
                if (entities != null) { model.TaskItems = entities; }
            }
            return View(model);
        }

        public async Task<IActionResult> ManageTask(long id, string od = null, long? fd = null)
        {
            ManageTaskViewModel model = new ManageTaskViewModel();
            model.Id = id;
            model.WorkFolderId = fd;
            model.TaskOwnerId = od;
            Employee employee = new Employee();
            try
            {
                if (model.Id < 1)
                {
                    string taskNo = await _baseModelService.GenerateAutoNumberAsync("taskno");
                    if (!string.IsNullOrWhiteSpace(taskNo)) { model.Number = $"T{taskNo}"; }
                    if (string.IsNullOrWhiteSpace(model.TaskOwnerId))
                    {
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
                    var task = await _wspService.GetTaskById(model.Id);
                    if(task != null)
                    {
                        model = model.Convert(task);
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
                TaskItem task = new TaskItem();
                try
                {
                    task = model.Convert();

                    if (task.Id > 0)
                    {
                        task.LastModifiedBy =  HttpContext.User.Identity.Name;
                        task.LastModifiedTime = DateTime.UtcNow;
                        bool isUpdated = await _wspService.UpdateTaskItemAsync(task);
                        if (isUpdated)
                        {
                            return RedirectToAction("MyTaskList", new { id = model.WorkFolderId, nm=model.WorkFolderName });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                    else
                    {
                        task.CreatedBy = HttpContext.User.Identity.Name;
                        task.CreatedTime = DateTime.UtcNow;
                        long newTaskId = await _wspService.CreateTaskItemAsync(task);
                        if (newTaskId > 0)
                        {
                            return RedirectToAction("MyTaskList", new { id = model.WorkFolderId, nm = model.WorkFolderName });
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

        //public string ReturnTaskFolder(int id, long sd)
        //{
        //    if (id < 1 || sd < 1) { return "parameter error"; }
        //    try
        //    {
        //        if (_wspService.ReturnTaskListSubmissionAsync(id, sd).Result)
        //        {
        //            return "success";
        //        }
        //        else
        //        {
        //            return "method failure";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //        //return " service error";
        //    }
        //}
        #endregion

        #region Task Items Action Methods
        //public string UpdateTaskStatus(int id, bool cls)
        //{
        //    if (id < 1) { return "parameter error"; }
        //    string actionBy = HttpContext.User.Identity.Name;
        //    WorkItemStatus newStatus = new WorkItemStatus();
        //    if (cls) { newStatus = WorkItemStatus.Closed; }
        //    else { newStatus = WorkItemStatus.Open; }
        //    try
        //    {
        //        if (_workspaceService.UpdateTaskStatusAsync(id, newStatus, actionBy).Result)
        //        {
        //            return "success";
        //        }
        //        else
        //        {
        //            return "method failure";
        //        }
        //    }
        //    catch
        //    {
        //        return "service error";
        //    }
        //}

        //public string CancelTaskItem(int id, bool ccl)
        //{
        //    if (id < 1) { return "parameter error"; }
        //    string actionBy = HttpContext.User.Identity.Name;
        //    try
        //    {
        //        if (_workspaceService.UpdateTaskCancelStatusAsync(id, actionBy, ccl).Result)
        //        {
        //            return "success";
        //        }
        //        else
        //        {
        //            return "method failure";
        //        }
        //    }
        //    catch
        //    {
        //        return " service error";
        //    }
        //}

        //public string DeleteTask(int id)
        //{
        //    if (id < 1) { return "parameter error"; }
        //    string actionBy = HttpContext.User.Identity.Name;
        //    try
        //    {
        //        if (_workspaceService.DeleteTaskItemAsync(id).Result)
        //        {
        //            return "success";
        //        }
        //        else
        //        {
        //            return "method failure";
        //        }
        //    }
        //    catch
        //    {
        //        return "service error";
        //    }
        //}

        //public string ApproveTaskItem(int id)
        //{
        //    if (id < 1) { return "parameter error"; }
        //    string actionBy = HttpContext.User.Identity.Name;
        //    try
        //    {
        //        if (_workspaceService.UpdateTaskApprovalStatusAsync(id, ApprovalStatus.Approved, actionBy).Result)
        //        {
        //            return "success";
        //        }
        //        else
        //        {
        //            return "method failure";
        //        }
        //    }
        //    catch
        //    {
        //        return " service error";
        //    }
        //}

        //public string DeclineTaskItem(int id)
        //{
        //    if (id < 1) { return "parameter error"; }
        //    string actionBy = HttpContext.User.Identity.Name;
        //    try
        //    {
        //        if (_workspaceService.UpdateTaskApprovalStatusAsync(id, ApprovalStatus.Declined, actionBy).Result)
        //        {
        //            return "success";
        //        }
        //        else
        //        {
        //            return "method failure";
        //        }
        //    }
        //    catch
        //    {
        //        return " service error";
        //    }
        //}
        #endregion
        #endregion
    }
}