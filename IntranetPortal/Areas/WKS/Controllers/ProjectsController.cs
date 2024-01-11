using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.WKS.Models;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.WKS
{
    [Area("WKS")]
    public class ProjectsController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IBamsManagerService _bamsManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IWorkspaceService _workspaceService;
        private readonly IErmService _ermService;
        public ProjectsController(IConfiguration configuration, ISecurityService securityService,
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

        public async Task<IActionResult> MyProjects(string src, int? id = null, string sp = null)
        {
            ProjectListViewModel model = new ProjectListViewModel();
            model.Source = src;
            try
            {
                model.OwnerID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                if (id != null && id > 0)
                {
                    model.id = id.Value;
                    ProjectFolder folder = await _workspaceService.GetProjectFolderAsync(id.Value);
                    if(folder != null)
                    {
                        model.FolderName = folder.Title;
                    }

                    if (!string.IsNullOrWhiteSpace(sp))
                    {
                        var entities = await _workspaceService.SearchProjectsAsync(id.Value, sp);
                        model.ProjectList = entities;
                    }
                    else
                    {
                        var entities = await _workspaceService.GetProjectsByFolderIDAsync(id.Value);
                        model.ProjectList = entities;
                    }
                }
                else
                {
                    var entities = await _workspaceService.GetProjectsByOwnerIDAsync(model.OwnerID);
                    model.ProjectList = entities;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            //var folderList = await _workspaceService.GetProjectFoldersByOwnerIDAsync(model.OwnerID);
            //ViewBag.FolderSelectList = new SelectList(folderList, "ID", "Title");
            return View(model);
        }

        public async Task<IActionResult> Manage(int? id = null, int? fd = null)
        {
            ProjectViewModel model = new ProjectViewModel();
            model.OwnerID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;

            if (id == null || id < 1)
            {
                model.ID = 0;
                DateTime currentDate = DateTime.Now;
                model.ExpectedStartTime = DateTime.Today;
                model.ExpectedDueTime = DateTime.Today;
                if (fd != null && fd.Value > 0)
                {
                    ProjectFolder folder = await _workspaceService.GetProjectFolderAsync(fd.Value);
                    if (folder != null)
                    {
                        model.FolderID = fd.Value;
                        model.FolderTitle = folder.Title;
                        model.WorkspaceID = folder.WorkspaceID;
                    }
                }

                string newProjectNo = await _baseModelService.GenerateAutoNumberAsync("projno");
                if (!string.IsNullOrWhiteSpace(newProjectNo))
                {
                    model.Code = newProjectNo;
                }
            }
            else
            {
                model.ID = id.Value;
                Project project = await _workspaceService.GetProjectByIDAsync(id.Value);
                model = model.ExtractViewModel(project);
            }

            var folderList = await _workspaceService.GetProjectFoldersByOwnerIDAsync(model.OwnerID);
            ViewBag.FolderSelectList = new SelectList(folderList, "ID", "Title");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(ProjectViewModel model)
        {
            Project project = new Project();
            if (ModelState.IsValid)
            {
                try
                {
                    project = model.ConvertToProject();
                    string actionBy = HttpContext.User.Identity.Name;
                    if (!string.IsNullOrWhiteSpace(model.AssignedToName))
                    {
                        Employee emp = new Employee();
                        emp = await _ermService.GetEmployeeByNameAsync(model.AssignedToName);
                        project.AssignedToID = emp.EmployeeID;
                        project.UnitID = emp.UnitID;
                        project.DepartmentID = emp.DepartmentID;
                        project.LocationID = emp.LocationID;
                    }

                    if (project.ID > 0)
                    {
                        project.LastModifiedBy = actionBy;
                        project.LastModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()}";
                        bool IsUpdated = await _workspaceService.UpdateProjectAsync(project);
                        if (IsUpdated)
                        {
                            //model.ViewModelSuccessMessage = "New Project was added successfully!";
                            //model.OperationIsSuccessful = true;
                            return RedirectToAction("MyProjects", new { id = project.FolderID });
                        }
                    }
                    else
                    {
                        project.CreatedBy = actionBy;
                        project.CreatedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()}";
                        bool IsAdded = await _workspaceService.CreateProjectAsync(project);
                        if (IsAdded)
                        {
                            //model.ViewModelSuccessMessage = "New Project was added successfully!";
                            //model.OperationIsSuccessful = true;
                            return RedirectToAction("MyProjects", new { id = project.FolderID });
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }

            var folderList = await _workspaceService.GetProjectFoldersByOwnerIDAsync(model.OwnerID);
            ViewBag.FolderSelectList = new SelectList(folderList, "ID", "Title");

            return View(model);
        }

        public async Task<IActionResult> ProjectInfo(int id)
        {
            ProjectViewModel model = new ProjectViewModel();
            try
            {
                Project project = await _workspaceService.GetProjectByIDAsync(id);
                model = model.ExtractViewModel(project);
            }
            catch(Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> MoreInfo(int id)
        {
            ProjectMoreInfoViewModel model = new ProjectMoreInfoViewModel();

            if (id > 0)
            {
                Project project = await _workspaceService.GetProjectByIDAsync(id);
                model.ProjectID = project.ID;
                model.FolderID = project.FolderID;
                model.Deliverables = project.Deliverables;
                model.Instructions = project.Instructions;
            }
            else
            {
                model.ViewModelErrorMessage = "Sorry, no record was found for the selected Project.";
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MoreInfo(ProjectMoreInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string modifiedBy = HttpContext.User.Identity.Name;
                    bool IsUpdated = await _workspaceService.UpdateProjectMoreInfoAsync(model.ProjectID, model.Instructions, model.Deliverables, modifiedBy);
                        if (IsUpdated)
                        {
                            return RedirectToAction("MyProjects", new { id = model.FolderID });
                        }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

    }
}