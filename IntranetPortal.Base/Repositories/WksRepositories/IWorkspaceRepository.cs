using IntranetPortal.Base.Models.WksModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.WksRepositories
{
    public interface IWorkspaceRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(Workspace workspace);
        Task<Workspace> GetByIdAsync(int workspaceId);
        Task<Workspace> GetMainByOwnerIdAsync(string ownerId);
        Task<IList<Workspace>> GetByOwnerIdAsync(string ownerId);
        Task<IList<Workspace>> GetByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle);
        Task<IList<Workspace>> SearchByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle);
        Task<bool> UpdateTitleAsync(int workspaceId, string newTitle);
        Task<bool> UpdateToDeletedAsync(int workspaceId, string deletedBy);
    }
}