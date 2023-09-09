using IntranetPortal.Base.Models.WksModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.WksRepositories
{
    public interface IProjectRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(Project project);
        Task<bool> UpdateAsync(Project project);
        Task<IList<Project>> GetByFolderIdAsync(int folderId);
        Task<IList<Project>> GetByIdAsync(int projectId);
        Task<IList<Project>> GetByNumberAsync(string projectNumber);
        Task<IList<Project>> GetByOwnerIdAsync(string ownerId);
        Task<IList<Project>> GetByTitleAsync(string projectTitle);
        Task<IList<Project>> SearchByFolderIdAndTitleAsync(int folderId, string projectTitle);
        Task<IList<Project>> SearchByTitleAsync(string projectTitle);
        Task<bool> UpdateToDeletedAsync(int folderId, string deletedBy);
    }
}
