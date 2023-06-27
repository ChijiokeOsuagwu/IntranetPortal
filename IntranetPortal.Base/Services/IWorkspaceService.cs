using IntranetPortal.Base.Models.WksModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IWorkspaceService
    {
        #region Workspace Action Interfaces
        Task<bool> CreateMainWorkspaceAsync(string ownerId);
        Task<bool> CreateWorkspaceAsync(Workspace workspace);
        Task<bool> UpdateWorkspaceAsync(Workspace workspace);
        Task<bool> DeleteWorkspaceAsync(Workspace workspace);
        Task<Workspace> GetMainWorkspaceAsync(string ownerId);
        Task<Workspace> GetWorkspaceAsync(int workspaceId);
        Task<List<Workspace>> GetWorkspacesByOwnerIdAsync(string ownerId);
        Task<List<Workspace>> SearchWorkspacesByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle = null);

        #endregion

        #region Project Folder Action Interfaces
        Task<List<ProjectFolder>> GetProjectFoldersByWorkspaceIDAsync(int WorkspaceID, bool? isArchived = null);
       
        Task<List<ProjectFolder>> GetProjectFoldersByOwnerIDAsync(string OwnerID, bool? isArchived = null);

        Task<ProjectFolder> GetProjectFolderAsync(int projectFolderId);

        Task<bool> CreateProjectFolderAsync(ProjectFolder folder);

        Task<bool> UpdateProjectFolderAsync(ProjectFolder folder);

        Task<bool> UpdateDeleteProjectFolderAsync(ProjectFolder folder);

        Task<List<ProjectFolder>> SearchProjectFoldersAsync(int WorkspaceID, string FolderTitle, bool? isArchived = null);
        #endregion

        #region Work Item Action Interfaces
        Task<bool> CreateProjectAsync(WorkItem project);
        Task<bool> UpdateProjectAsync(WorkItem project);
        Task<WorkItem> GetWorkItemByIDAsync(int workItemID);
        Task<List<WorkItem>> GetWorkItemsByFolderIDAsync(int FolderID);
        Task<List<WorkItem>> GetProjectsByOwnerIDAsync(string OwnerID);
        Task<List<WorkItem>> SearchProjectsAsync(string workItemTitle);
        Task<List<WorkItem>> SearchProjectsAsync(int FolderID, string workItemTitle);

        #endregion
    }
}