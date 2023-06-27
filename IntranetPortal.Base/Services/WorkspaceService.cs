using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.WksRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IProjectFolderRepository _projectFolderRepository;
        private readonly IWorkItemRepository _workItemRepository;
        private readonly IUtilityRepository _utilityRepository;

        public WorkspaceService(IWorkspaceRepository workspaceRepository, IProjectFolderRepository projectFolderRepository,
                                IWorkItemRepository workItemRepository, IUtilityRepository utilityRepository)
        {
            _workspaceRepository = workspaceRepository;
            _projectFolderRepository = projectFolderRepository;
            _workItemRepository = workItemRepository;
            _utilityRepository = utilityRepository;
        }

        #region Workspace Service Actions
        public async Task<bool> CreateWorkspaceAsync(Workspace workspace)
        {
            if (workspace == null) { throw new ArgumentNullException(nameof(workspace), "The required parameter [Workspace] is missing."); }
            try
            {
                var entities = await _workspaceRepository.GetByOwnerIdAndTitleAsync(workspace.OwnerID, workspace.Title);
                if (entities != null && entities.Count > 0 && entities[0].ID > 0)
                {
                    throw new Exception("Please choose another Title. You already have a Workspace in the system with the same title.");
                }
                return await _workspaceRepository.AddAsync(workspace);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateWorkspaceAsync(Workspace workspace)
        {
            if (workspace == null) { throw new ArgumentNullException(nameof(workspace), "The required parameter [Workspace] is missing."); }
            try
            {
                var entities = await _workspaceRepository.GetByOwnerIdAndTitleAsync(workspace.OwnerID, workspace.Title);
                if (entities != null && entities.Count > 0 && entities[0].ID > 0)
                {
                    throw new Exception("Please choose another Title. You already have a Workspace in the system with the same title.");
                }
                return await _workspaceRepository.UpdateTitleAsync(workspace.ID, workspace.Title);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CreateMainWorkspaceAsync(string ownerId)
        {
            if (String.IsNullOrWhiteSpace(ownerId)) { throw new ArgumentNullException(nameof(ownerId), "The required parameter [OwnerId] is missing."); }
            Workspace workspace = new Workspace
            {
                CreatedBy = "System Service",
                CreatedTime = DateTime.UtcNow,
                IsDeleted = false,
                IsMain = true,
                OwnerID = ownerId,
                Title = "Main Workspace",
                ID = 0
            };
            try
            {
                var entities = await _workspaceRepository.GetByOwnerIdAndTitleAsync(workspace.OwnerID, workspace.Title);
                if (entities != null && entities.Count > 0 && entities[0].ID > 0)
                {
                    throw new Exception("Please choose another Title. You already have a Workspace in the system with the same title.");
                }
                bool WorkspaceIsCreated = await _workspaceRepository.AddAsync(workspace);
                if (WorkspaceIsCreated)
                {
                    var new_entities = await _workspaceRepository.GetByOwnerIdAndTitleAsync(workspace.OwnerID, workspace.Title);
                    if (new_entities != null && new_entities.Count > 0 && new_entities[0].ID > 0)
                    {
                        Workspace mainWorkspace = new_entities.FirstOrDefault();
                        ProjectFolder projectFolder = new ProjectFolder
                        {
                            ID = 0,
                            Title = "General Folder",
                            Description = "General Folder",
                            WorkspaceID = mainWorkspace.ID,
                            CreatedBy = "System Service",
                            CreatedTime = DateTime.UtcNow,
                            IsArchived = false,
                            OwnerID = ownerId,
                        };
                        return await _projectFolderRepository.AddAsync(projectFolder);
                    }
                    else { return true; }
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteWorkspaceAsync(Workspace workspace)
        {
            if (workspace == null) { throw new ArgumentNullException(nameof(workspace), "The required parameter [Workspace] is missing."); }
            try
            {
                return await _workspaceRepository.UpdateToDeletedAsync(workspace.ID, workspace.Title);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Workspace> GetMainWorkspaceAsync(string ownerId)
        {
            Workspace workspace = new Workspace();
            try
            {
                var entity = await _workspaceRepository.GetMainByOwnerIdAsync(ownerId);
                if (entity != null && entity.ID > 0) { workspace = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workspace;
        }

        public async Task<Workspace> GetWorkspaceAsync(int workspaceId)
        {
            Workspace workspace = new Workspace();
            try
            {
                var entity = await _workspaceRepository.GetByIdAsync(workspaceId);
                if (entity != null && entity.ID > 0) { workspace = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workspace;
        }

        public async Task<List<Workspace>> GetWorkspacesByOwnerIdAsync(string ownerId)
        {
            List<Workspace> workspaces = new List<Workspace>();
            try
            {
                var entities = await _workspaceRepository.GetByOwnerIdAsync(ownerId);
                if (entities != null && entities.Count > 0) { workspaces = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workspaces;
        }

        public async Task<List<Workspace>> SearchWorkspacesByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle = null)
        {
            List<Workspace> workspaces = new List<Workspace>();
            try
            {
                if (!string.IsNullOrWhiteSpace(workspaceTitle))
                {
                    var entities = await _workspaceRepository.SearchByOwnerIdAndTitleAsync(ownerId, workspaceTitle);
                    if (entities != null && entities.Count > 0) { workspaces = entities.ToList(); }
                }
                else
                {
                    var entities = await _workspaceRepository.GetByOwnerIdAsync(ownerId);
                    if (entities != null && entities.Count > 0) { workspaces = entities.ToList(); }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workspaces;
        }

        #endregion

        #region ProjectFolder Service Action
        public async Task<List<ProjectFolder>> GetProjectFoldersByWorkspaceIDAsync(int WorkspaceID, bool? isArchived = null)
        {
            List<ProjectFolder> folders = new List<ProjectFolder>();
            try
            {
                if(isArchived != null)
                {
                    var entities = await _projectFolderRepository.GetByWorkspaceIdAsync(WorkspaceID, isArchived.Value);
                    if (entities != null && entities.Count > 0) { folders = entities.ToList(); }
                }
                else
                {
                    var entities = await _projectFolderRepository.GetByWorkspaceIdAsync(WorkspaceID);
                    if (entities != null && entities.Count > 0) { folders = entities.ToList(); }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return folders;
        }

        public async Task<List<ProjectFolder>> GetProjectFoldersByOwnerIDAsync(string OwnerID, bool? isArchived = null)
        {
            List<ProjectFolder> folders = new List<ProjectFolder>();
            try
            {
                if (isArchived != null)
                {
                    var entities = await _projectFolderRepository.GetByOwnerIdAsync(OwnerID, isArchived.Value);
                    if (entities != null && entities.Count > 0) { folders = entities.ToList(); }
                }
                else
                {
                    var entities = await _projectFolderRepository.GetByOwnerIdAsync(OwnerID);
                    if (entities != null && entities.Count > 0) { folders = entities.ToList(); }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return folders;
        }

        public async Task<ProjectFolder> GetProjectFolderAsync(int projectFolderId)
        {
            ProjectFolder projectFolder = new ProjectFolder();
            try
            {
                var entity = await _projectFolderRepository.GetByIdAsync(projectFolderId);
                if (entity != null && entity.ID > 0) { projectFolder = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return projectFolder;
        }

        public async Task<bool> CreateProjectFolderAsync(ProjectFolder folder)
        {
            if (folder == null) { throw new ArgumentNullException(nameof(folder), "The required parameter [Project Folder] is missing."); }
            try
            {
                var entities = await _projectFolderRepository.GetByTitleAndOwnerIdAsync(folder.Title, folder.OwnerID);
                if (entities != null && entities.Count > 0 && entities[0].ID > 0)
                {
                    throw new Exception("Please choose another Title. You already have a Folder in the system with the same title.");
                }
                return await _projectFolderRepository.AddAsync(folder); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateProjectFolderAsync(ProjectFolder folder)
        {
            if (folder == null) { throw new ArgumentNullException(nameof(folder), "The required parameter [Project Folder] is missing."); }
            try
            {
                var entities = await _projectFolderRepository.GetByTitleAndOwnerIdAsync(folder.Title, folder.OwnerID);
                if (entities != null && entities.Count > 0 && entities[0].ID != folder.ID)
                {
                    throw new Exception("Please choose another Title. You already have a Folder in the system with the same title.");
                }
                return await _projectFolderRepository.UpdateAsync(folder);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateDeleteProjectFolderAsync(ProjectFolder folder)
        {
            if (folder == null) { throw new ArgumentNullException(nameof(folder), "The required parameter [Project Folder] is missing."); }
            try
            {
                return await _projectFolderRepository.UpdateToDeletedAsync(folder.ID,folder.DeletedBy);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<ProjectFolder>> SearchProjectFoldersAsync(int WorkspaceID, string FolderTitle, bool? isArchived = null)
        {
            List<ProjectFolder> folders = new List<ProjectFolder>();
            try
            {
                if (isArchived != null)
                {
                    var entities = await _projectFolderRepository.SearchByWorkspaceIdAndTitleAsync(WorkspaceID, FolderTitle, isArchived.Value);
                    if (entities != null && entities.Count > 0) { folders = entities.ToList(); }
                }
                else
                {
                    var entities = await _projectFolderRepository.SearchByWorkspaceIdAndTitleAsync(WorkspaceID, FolderTitle);
                    if (entities != null && entities.Count > 0) { folders = entities.ToList(); }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return folders;
        }
        #endregion

        #region Work Item Service Action
        public async Task<bool> CreateProjectAsync(WorkItem project)
        {
            if (project == null) { throw new ArgumentNullException(nameof(project), "The required parameter [WorkItem] is missing."); }
            try
            {
                var entities = await _workItemRepository.GetByTitleAsync(project.Title);
                if (entities != null && entities.Count > 0 && entities[0].ID > 0)
                {
                    throw new Exception("Please choose another Title. There is already a record in the system with the same title.");
                }
                bool IsAdded = await _workItemRepository.AddAsync(project);
                if (IsAdded)
                {
                    await _utilityRepository.IncrementAutoNumberAsync("projno");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
 
        public async Task<bool> UpdateProjectAsync(WorkItem project)
        {
            if (project == null) { throw new ArgumentNullException(nameof(project), "Required parameter is missing."); }
            try
            {
                var entities = await _workItemRepository.GetByTitleAsync(project.Title);
                if (entities != null && entities.Count > 0 && entities[0].ID > 0 && entities[0].ID != project.ID)
                {
                    throw new Exception("Please choose another Title. There is already an item in the system with the same title.");
                }
                return await _workItemRepository.UpdateAsync(project);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WorkItem> GetWorkItemByIDAsync(int workItemID)
        {
            WorkItem workItem = new WorkItem();
            try
            {
                var entities = await _workItemRepository.GetByIdAsync(workItemID);
                if (entities != null && entities.Count > 0) { workItem = entities.FirstOrDefault(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workItem;
        }
        public async Task<List<WorkItem>> GetWorkItemsByFolderIDAsync(int FolderID)
        {
            List<WorkItem> workItems = new List<WorkItem>();
            try
            {
                    var entities = await _workItemRepository.GetByFolderIdAsync(FolderID);
                    if (entities != null && entities.Count > 0) { workItems = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workItems;
        }
        public async Task<List<WorkItem>> GetProjectsByOwnerIDAsync(string OwnerID)
        {
            List<WorkItem> workItems = new List<WorkItem>();
            try
            {
                var entities = await _workItemRepository.GetByOwnerIdAsync(OwnerID);
                if (entities != null && entities.Count > 0) { workItems = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workItems;
        }
        public async Task<List<WorkItem>> SearchProjectsAsync(string workItemTitle)
        {
            List<WorkItem> workItems = new List<WorkItem>();
            try
            {
                var entities = await _workItemRepository.SearchByTitleAsync(workItemTitle);
                if (entities != null && entities.Count > 0) { workItems = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workItems;
        }
        public async Task<List<WorkItem>> SearchProjectsAsync(int FolderID, string workItemTitle)
        {
            List<WorkItem> workItems = new List<WorkItem>();
            try
            {
                    var entities = await _workItemRepository.SearchByFolderIdAndTitleAsync(FolderID, workItemTitle);
                    if (entities != null && entities.Count > 0) { workItems = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workItems;
        }
        #endregion
    }
}
