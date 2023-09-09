using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.WKS.Models;
using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.WKS.Controllers
{
    [Area("WKS")]
    public class HomeController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IBamsManagerService _bamsManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IWorkspaceService _workspaceService;
        public HomeController(IConfiguration configuration, ISecurityService securityService,
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

        public async Task<IActionResult> MyWorkspace(int? id, string sp = null)
        {
            MyWorkspaceViewModel model = new MyWorkspaceViewModel();
            bool IsArchived = false;
            if (!string.IsNullOrWhiteSpace(sp))
            {
                switch (sp)
                {
                    case "archived":
                        IsArchived = true;
                        break;
                    case "active":
                        IsArchived = false;
                        break;
                    default:
                        IsArchived = false;
                        break;
                }
            }

            Workspace workspace = new Workspace();
            model.OwnerID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            if (id == null || id < 1)
            {
                if (!String.IsNullOrWhiteSpace(model.OwnerID))
                {
                    workspace = await _workspaceService.GetMainWorkspaceAsync(model.OwnerID);
                    if (workspace != null && workspace.ID > 0)
                    {
                        model.OpenWorkspace = workspace;
                        model.WorkspaceID = workspace.ID;
                    }
                    else
                    {
                        bool MainWorkspaceIsCreated = await _workspaceService.CreateMainWorkspaceAsync(model.OwnerID);
                        if (MainWorkspaceIsCreated)
                        {
                            workspace = await _workspaceService.GetMainWorkspaceAsync(model.OwnerID);
                            if (workspace != null && workspace.ID > 0)
                            {
                                model.OpenWorkspace = workspace;
                                model.WorkspaceID = workspace.ID;
                            }
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Home", new { Area = "" });
                }
            }
            else
            {
                model.OpenWorkspace = await _workspaceService.GetWorkspaceAsync(id.Value);
                model.WorkspaceID = id.Value;
            }

            model.WorkspaceList = await _workspaceService.GetWorkspacesByOwnerIdAsync(model.OwnerID);
            model.FolderList = await _workspaceService.GetProjectFoldersByWorkspaceIDAsync(model.WorkspaceID, IsArchived);
            ViewBag.WorkspaceSelectList = new SelectList(model.WorkspaceList, "ID", "Title");
            return View(model);
        }

        public async Task<IActionResult> ExecutiveWorkspace(int? id, string sp = null)
        {
            MyWorkspaceViewModel model = new MyWorkspaceViewModel();
            bool IsArchived = false;
            if (!string.IsNullOrWhiteSpace(sp))
            {
                switch (sp)
                {
                    case "archived":
                        IsArchived = true;
                        break;
                    case "active":
                        IsArchived = false;
                        break;
                    default:
                        IsArchived = false;
                        break;
                }
            }

            Workspace workspace = new Workspace();
            model.OwnerID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            if (id == null || id < 1)
            {
                if (!String.IsNullOrWhiteSpace(model.OwnerID))
                {
                    workspace = await _workspaceService.GetMainWorkspaceAsync(model.OwnerID);
                    if (workspace != null && workspace.ID > 0)
                    {
                        model.OpenWorkspace = workspace;
                        model.WorkspaceID = workspace.ID;
                    }
                    else
                    {
                        bool MainWorkspaceIsCreated = await _workspaceService.CreateMainWorkspaceAsync(model.OwnerID);
                        if (MainWorkspaceIsCreated)
                        {
                            workspace = await _workspaceService.GetMainWorkspaceAsync(model.OwnerID);
                            if (workspace != null && workspace.ID > 0)
                            {
                                model.OpenWorkspace = workspace;
                                model.WorkspaceID = workspace.ID;
                            }
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Home", new { Area = "" });
                }
            }
            else
            {
                model.OpenWorkspace = await _workspaceService.GetWorkspaceAsync(id.Value);
                model.WorkspaceID = id.Value;
            }

            model.WorkspaceList = await _workspaceService.GetWorkspacesByOwnerIdAsync(model.OwnerID);
            model.FolderList = await _workspaceService.GetProjectFoldersByWorkspaceIDAsync(model.WorkspaceID, IsArchived);
            ViewBag.WorkspaceSelectList = new SelectList(model.WorkspaceList, "ID", "Title");
            return View(model);
        }
    }
}