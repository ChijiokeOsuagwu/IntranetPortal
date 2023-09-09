using IntranetPortal.Base.Models.WksModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.WksRepositories
{
    public interface ITaskListRepository
    {
        IConfiguration _config { get; }

        //================== Task List Read Action Methods =================================//
        Task<TaskList> GetByIdAsync(int taskListId);
        Task<IList<TaskList>> GetByOwnerIdAsync(string ownerId);
        Task<IList<TaskList>> GetByOwnerIdAsync(string ownerId, bool isArchived);
        Task<IList<TaskList>> GetByOwnerIdAsync(string ownerId, bool isArchived, int createdYear);
        Task<IList<TaskList>> GetByOwnerIdAsync(string ownerId, int createdYear);
        Task<IList<TaskList>> GetByOwnerIdAsync(string ownerId, bool isArchived, int createdYear, int createdMonth);
        Task<IList<TaskList>> GetByOwnerIdAsync(string ownerId, int createdYear, int createdMonth);
        Task<IList<TaskList>> GetByOwnerIdAndNameAsync(string ownerId, string taskListName);

        //================== Task List Write Action Methods ================================//
        Task<bool> AddAsync(TaskList taskList);
        Task<bool> UpdateAsync(TaskList taskList);
        Task<bool> UpdateArchiveAsync(int taskListId, bool isArchived);
        Task<bool> UpdateLockAsync(int taskListId, bool isLocked);
        Task<bool> UpdateToDeletedAsync(int taskListId, string deletedBy);
        Task<bool> DeleteAsync(int taskListId);

        //============= Task List Note Action Methods =================================//
        Task<IList<TaskListNote>> GetNotesByTaskListIdAsync(int taskListId);
        Task<bool> AddNoteAsync(TaskListNote taskListNote);
        Task<bool> UpdateToIsCancelledAsync(long taskListNoteId);
    }
}