using IntranetPortal.Base.Models.WksModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.WksRepositories
{
    public interface IWorkItemRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(WorkItem workItem);
        Task<bool> UpdateAsync(WorkItem workItem);
        Task<IList<WorkItem>> GetByFolderIdAsync(int folderId);
        Task<IList<WorkItem>> GetByIdAsync(int workItemId);
        Task<IList<WorkItem>> GetByOwnerIdAsync(string ownerId);
        Task<IList<WorkItem>> GetByTitleAsync(string workItemTitle);
        Task<IList<WorkItem>> SearchByFolderIdAndTitleAsync(int folderId, string workItemTitle);
        Task<IList<WorkItem>> SearchByTitleAsync(string workItemTitle);
        Task<bool> UpdateToDeletedAsync(int folderId, string deletedBy);
    }
}