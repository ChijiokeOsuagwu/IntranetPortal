using IntranetPortal.Base.Models.WksModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.WksRepositories
{
    public interface IProjectFolderRepository
    {
        IConfiguration _config { get; }

        #region Project Folder Read Action Methods
        Task<ProjectFolder> GetByIdAsync(int projectFolderId);
        Task<IList<ProjectFolder>> GetByWorkspaceIdAsync(int workspaceId);
        Task<IList<ProjectFolder>> GetByOwnerIdAsync(string ownerId);
        Task<IList<ProjectFolder>> GetByOwnerIdAsync(string ownerId, bool isArchived);
        Task<IList<ProjectFolder>> GetByWorkspaceIdAsync(int workspaceId, bool isArchived);
        Task<IList<ProjectFolder>> GetMainFoldersByOwnerIdAsync(string ownerId);
        Task<IList<ProjectFolder>> GetMainFoldersByOwnerIdAsync(string ownerId, bool isArchived);
        Task<List<ProjectFolder>> GetByTitleAndOwnerIdAsync(string folderTitle, string ownerId);
        Task<IList<ProjectFolder>> SearchByWorkspaceIdAndTitleAsync(int workspaceId, string folderTitle);
        Task<IList<ProjectFolder>> SearchByWorkspaceIdAndTitleAsync(int workspaceId, string folderTitle, bool isArchived);
        #endregion

        #region Project Folder Write Action Methods
        Task<bool> AddAsync(ProjectFolder projectFolder);
        Task<bool> UpdateAsync(ProjectFolder projectFolder);
        Task<bool> UpdateToDeletedAsync(int projectFolderId, string deletedBy);
        #endregion
    }
}