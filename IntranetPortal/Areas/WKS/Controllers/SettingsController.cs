using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.WKS.Models;
using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.WKS.Controllers
{
    [Area("WKS")]
    public class SettingsController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IBamsManagerService _bamsManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IWorkspaceService _workspaceService;

        public SettingsController(IConfiguration configuration, ISecurityService securityService,
                        IBaseModelService baseModelService, IBamsManagerService bamsManagerService,
                        IGlobalSettingsService globalSettingsService, IWorkspaceService workspaceService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _bamsManagerService = bamsManagerService;
            _globalSettingsService = globalSettingsService;
            _workspaceService = workspaceService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Workspace Controller Actions

        public async Task<IActionResult> WorkspaceList(string sp = null)
        {
            WorkspaceListViewModel model = new WorkspaceListViewModel();
            try
            {
                model.OwnerID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;

                if (!string.IsNullOrWhiteSpace(sp))
                {
                    var entities = await _workspaceService.SearchWorkspacesByOwnerIdAndTitleAsync(model.OwnerID, sp);
                    model.WorkspaceList = entities;
                }
                else
                {
                    var entities = await _workspaceService.GetWorkspacesByOwnerIdAsync(model.OwnerID);
                    model.WorkspaceList = entities;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageWorkspace(int id = 0)
        {
            WorkspaceViewModel model = new WorkspaceViewModel();
            if (id == 0)
            {
                ViewBag.FormLabel = "New Workspace";
                model.ID = 0;
            }
            else
            {
                ViewBag.FormLabel = "Edit Workspace";
                Workspace workspace = await _workspaceService.GetWorkspaceAsync(id);
                model.ID = workspace.ID;
                model.OwnerID = workspace.OwnerID;
                model.Title = workspace.Title;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageWorkspace(WorkspaceViewModel model)
        {
            Workspace workspace = new Workspace();
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.ID == 0)
                    {
                        ViewBag.FormLabel = "New Workspace";
                        workspace.OwnerID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                        workspace.CreatedBy = HttpContext.User.Identity.Name;
                        if (string.IsNullOrWhiteSpace(workspace.OwnerID))
                        {
                            var entities = await _securityService.GetEmployeeUsersByNameAsync(workspace.CreatedBy);
                            if (entities != null && entities.Count > 0)
                            {
                                workspace.OwnerID = entities.FirstOrDefault().UserID;
                            }
                        }
                        workspace.CreatedTime = DateTime.UtcNow;
                        workspace.Title = model.Title;
                        workspace.IsMain = false;
                        if (!string.IsNullOrWhiteSpace(workspace.Title))
                        {
                            if (await _workspaceService.CreateWorkspaceAsync(workspace))
                            {
                                model.OperationIsSuccessful = true;
                                model.OperationIsCompleted = true;
                                model.ViewModelSuccessMessage = "New Workspace Created successfully!";
                            }
                            else
                            {
                                model.OperationIsCompleted = true;
                                model.ViewModelErrorMessage = "Sorry, an error was encountered. Creating new Workspace failed.";
                            }
                        }
                    }
                    else
                    {
                        ViewBag.FormLabel = "Edit Workspace";
                        workspace.ID = model.ID.Value;
                        workspace.OwnerID = model.OwnerID;
                        workspace.Title = model.Title;
                        if (await _workspaceService.UpdateWorkspaceAsync(workspace))
                        {
                            model.OperationIsSuccessful = true;
                            model.OperationIsCompleted = true;
                            model.ViewModelSuccessMessage = "Workspace was updated successfully!";
                        }
                        else
                        {
                            model.OperationIsCompleted = true;
                            model.ViewModelErrorMessage = "Sorry, an error was encountered. Updating Workspace failed.";
                        }
                    }
                }
                catch (Exception e)
                {
                    model.ViewModelErrorMessage = e.Message;
                    model.OperationIsSuccessful = false;
                    model.OperationIsCompleted = false;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteWorkspace(int id = 0)
        {
            WorkspaceViewModel model = new WorkspaceViewModel();
            Workspace workspace = await _workspaceService.GetWorkspaceAsync(id);
            if (workspace != null && !string.IsNullOrWhiteSpace(workspace.Title))
            {
                model.ID = workspace.ID;
                model.OwnerID = workspace.OwnerID;
                model.Title = workspace.Title;
                if (workspace.IsMain)
                {
                    model.OperationIsCompleted = true;
                    model.ViewModelWarningMessage = "This is a system default Workspace. Delete is not permitted.";
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWorkspace(WorkspaceViewModel model)
        {
            if (ModelState.IsValid)
            {
                Workspace workspace = new Workspace();
                try
                {
                    workspace.DeletedBy = HttpContext.User.Identity.Name;
                    workspace.Title = model.Title;
                    workspace.ID = model.ID.Value;

                    if (await _workspaceService.DeleteWorkspaceAsync(workspace))
                    {
                        return RedirectToAction("WorkspaceList");
                    }
                    else
                    {
                        model.OperationIsCompleted = true;
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Deleting Workspace failed.";
                    }
                }
                catch (Exception e)
                {
                    model.ViewModelErrorMessage = e.Message;
                    model.OperationIsSuccessful = false;
                    model.OperationIsCompleted = false;
                }
            }
            return View(model);
        }

        #endregion

        #region Project Folders Controller Actions
        public async Task<IActionResult> Folders(int? id = null, string sp = null)
        {
            ProjectFolderListViewModel model = new ProjectFolderListViewModel();
            try
            {
                model.OwnerID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                if (id != null && id > 0)
                {
                    model.id = id.Value;
                    if (!string.IsNullOrWhiteSpace(sp))
                    {
                        var entities = await _workspaceService.SearchProjectFoldersAsync(id.Value, sp);
                        model.ProjectFolderList = entities;
                    }
                    else
                    {
                        var entities = await _workspaceService.GetProjectFoldersByWorkspaceIDAsync(id.Value);
                        model.ProjectFolderList = entities;
                    }
                }
                else
                {
                    var entities = await _workspaceService.GetProjectFoldersByOwnerIDAsync(model.OwnerID);
                    model.ProjectFolderList = entities;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var workspaceList = await _workspaceService.GetWorkspacesByOwnerIdAsync(model.OwnerID);
            ViewBag.WorkspaceSelectList = new SelectList(workspaceList, "ID", "Title");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageFolder(int id = 0)
        {
            ProjectFolderViewModel model = new ProjectFolderViewModel();
            if (id == 0)
            {
                ViewBag.FormLabel = "New Folder";
                model.FolderID = 0;
                model.OwnerID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;

            }
            else
            {
                ViewBag.FormLabel = "Edit Folder";
                ProjectFolder folder = await _workspaceService.GetProjectFolderAsync(id);
                model.FolderID = folder.ID;
                model.OwnerID = folder.OwnerID;
                model.FolderTitle = folder.Title;
                model.FolderDescription = folder.Description;
                model.WorkspaceID = folder.WorkspaceID;
                model.IsArchived = folder.IsArchived;
            }
            List<Workspace> workspaces = await _workspaceService.GetWorkspacesByOwnerIdAsync(model.OwnerID);
            ViewBag.WorkspaceList = new SelectList(workspaces, "ID", "Title");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageFolder(ProjectFolderViewModel model)
        {
            ProjectFolder folder = new ProjectFolder();
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.FolderID == 0)
                    {
                        ViewBag.FormLabel = "New Folder";
                        if (string.IsNullOrWhiteSpace(model.OwnerID))
                        {
                            folder.OwnerID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                        }
                        else { folder.OwnerID = model.OwnerID; }
                        folder.CreatedBy = HttpContext.User.Identity.Name;
                        if (string.IsNullOrWhiteSpace(folder.OwnerID))
                        {
                            var entities = await _securityService.GetEmployeeUsersByNameAsync(folder.CreatedBy);
                            if (entities != null && entities.Count > 0)
                            {
                                folder.OwnerID = entities.FirstOrDefault().UserID;
                            }
                        }
                        folder.CreatedTime = DateTime.UtcNow;
                        folder.Title = model.FolderTitle;
                        folder.Description = model.FolderDescription;
                        folder.IsArchived = model.IsArchived;
                        folder.WorkspaceID = model.WorkspaceID.Value;
                        if (!string.IsNullOrWhiteSpace(folder.Title))
                        {
                            if (await _workspaceService.CreateProjectFolderAsync(folder))
                            {
                                model.OperationIsSuccessful = true;
                                model.OperationIsCompleted = true;
                                model.ViewModelSuccessMessage = "New Folder Created successfully!";
                            }
                            else
                            {
                                model.OperationIsCompleted = true;
                                model.ViewModelErrorMessage = "Sorry, an error was encountered. Creating new Folder failed.";
                            }
                        }
                    }
                    else
                    {
                        ViewBag.FormLabel = "Edit Folder";
                        folder.ID = model.FolderID.Value;
                        folder.OwnerID = model.OwnerID;
                        folder.Title = model.FolderTitle;
                        folder.Description = model.FolderDescription;
                        folder.IsArchived = model.IsArchived;
                        folder.WorkspaceID = model.WorkspaceID.Value;
                        if (await _workspaceService.UpdateProjectFolderAsync(folder))
                        {
                            model.OperationIsSuccessful = true;
                            model.OperationIsCompleted = true;
                            model.ViewModelSuccessMessage = "Folder was updated successfully!";
                        }
                        else
                        {
                            model.OperationIsCompleted = true;
                            model.ViewModelErrorMessage = "Sorry, an error was encountered. Updating Folder failed.";
                        }
                    }
                }
                catch (Exception e)
                {
                    model.ViewModelErrorMessage = e.Message;
                    model.OperationIsSuccessful = false;
                    model.OperationIsCompleted = false;
                }
            }
            List<Workspace> workspaces = await _workspaceService.GetWorkspacesByOwnerIdAsync(model.OwnerID);
            ViewBag.WorkspaceList = new SelectList(workspaces, "ID", "Title");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFolder(int id = 0)
        {
            ProjectFolderViewModel model = new ProjectFolderViewModel();
            ProjectFolder folder = await _workspaceService.GetProjectFolderAsync(id);
            model.FolderID = folder.ID;
            model.OwnerID = folder.OwnerID;
            model.FolderTitle = folder.Title;
            model.FolderDescription = folder.Description;
            model.WorkspaceID = folder.WorkspaceID;
            model.IsArchived = folder.IsArchived;
            model.WorkspaceTitle = folder.WorkspaceTitle;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFolder(ProjectFolderViewModel model)
        {
            ProjectFolder folder = new ProjectFolder();
            try
            {
                folder.DeletedBy = HttpContext.User.Identity.Name;
                folder.ID = model.FolderID.Value;
                folder.OwnerID = model.OwnerID;
                folder.Title = model.FolderTitle;
                folder.Description = model.FolderDescription;
                folder.WorkspaceID = model.WorkspaceID.Value;
                folder.IsArchived = model.IsArchived;

                if (await _workspaceService.UpdateDeleteProjectFolderAsync(folder))
                {
                    return RedirectToAction("Folders");
                }
                else
                {
                    model.OperationIsCompleted = true;
                    model.ViewModelErrorMessage = "Sorry, an error was encountered. Deleting Folder failed.";
                }
            }
            catch (Exception e)
            {
                model.ViewModelErrorMessage = e.Message;
                model.OperationIsSuccessful = false;
                model.OperationIsCompleted = false;
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> FolderDetails(int id = 0)
        {
            ProjectFolderViewModel model = new ProjectFolderViewModel();
            ProjectFolder folder = await _workspaceService.GetProjectFolderAsync(id);
            model.FolderID = folder.ID;
            model.OwnerID = folder.OwnerID;
            model.FolderTitle = folder.Title;
            model.FolderDescription = folder.Description;
            model.WorkspaceID = folder.WorkspaceID;
            model.IsArchived = folder.IsArchived;
            model.WorkspaceTitle = folder.WorkspaceTitle;

            return View(model);
        }

        #endregion
    }
}