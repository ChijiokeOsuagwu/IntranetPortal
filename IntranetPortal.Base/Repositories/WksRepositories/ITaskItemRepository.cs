using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.WksModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.WksRepositories
{
    public interface ITaskItemRepository
    {
        IConfiguration _config { get; }

        #region Write Action Methods
        Task<bool> AddAsync(TaskItem taskItem);
        Task<bool> UpdateAsync(TaskItem taskItem);
        Task<bool> DeleteAsync(long taskItemId);
        Task<bool> UpdateTaskCancelAsync(long taskItemId, string modifiedBy, bool cancelTask);
        Task<bool> UpdateTaskStatusAsync(long taskItemId, int newTaskStatus, string modifiedBy);
        Task<bool> UpdateTaskStatusByTaskListIdAsync(int taskListId, int newTaskStatus, string modifiedBy);
        Task<bool> UpdateApprovalStatusAsync(long taskItemId, ApprovalStatus newApprovalStatus, string approvedBy);
        Task<bool> UpdateProgressStatusAsync(long taskItemId, int newProgressStatus, string modifiedBy);
        Task<bool> UpdateTimelineAsync(long taskItemId, string modifiedBy, DateTime? previousStartDate, DateTime? newStartDate, DateTime? previousEndDate, DateTime? newEndDate);
        Task<bool> UpdateProgramLinkAsync(long taskItemId, string programCode, DateTime? programDate, string modifiedBy);
        Task<bool> UpdateProjectLinkAsync(long taskItemId, string projectCode, string modifiedBy);
        Task<bool> UpdateTaskOwnerAsync(long taskItemId, int newTaskListId, string newTaskOwnerId, int newUnitId, int newDeptId, int newLocationId, string modifiedBy);

        #endregion

        Task<TaskItem> GetByIdAsync(long taskItemId);
        Task<TaskItem> GetByNumberAsync(string taskItemNumber);

        //=========== Get By OwnerId and AssignedToId ===================================//
        #region Get By OwnerId and AssignedToId
        Task<List<TaskItem>> GetByOwnerIdOrAssignedToIdAsync(string empId);
        Task<List<TaskItem>> GetByOwnerIdOrAssignedToIdAsync(string empId, int taskListId);
        Task<List<TaskItem>> GetByOwnerIdOrAssignedToIdAsync(string empId, int taskListId, WorkItemStatus taskStatus);
        Task<List<TaskItem>> GetByOwnerIdOrAssignedToIdAsync(string empId, int taskListId, WorkItemStatus taskStatus, WorkItemProgressStatus progressStatus);
        Task<List<TaskItem>> GetByOwnerIdOrAssignedToIdAsync(string empId, WorkItemStatus taskStatus);
        #endregion

        #region Get By Task List ID
        Task<List<TaskItem>> GetByTaskListIdAndDueYearAndDueMonthAsync(int taskListId, int dueYear, int dueMonth);
        Task<List<TaskItem>> GetByTaskListIdAndDueYearAndDueMonthAndProgressStatusAsync(int taskListId, int dueYear, int dueMonth, int progressStatus);
        Task<List<TaskItem>> GetByTaskListIdAndDueYearAsync(int taskListId, int dueYear);
        Task<List<TaskItem>> GetByTaskListIdAndDueYearAndProgressStatusAsync(int taskListId, int dueYear, int progressStatus);
        Task<List<TaskItem>> GetByTaskListIdAsync(int taskListId);
        Task<List<TaskItem>> GetByTaskListIdAndDescriptionAsync(int taskListId, string description);
        Task<List<TaskItem>> GetByTaskListIdAndProgressStatusAsync(int taskListId, int progressStatus);
        #endregion

        #region Task Notes Action Methods
        Task<IList<TaskNote>> GetNotesByTaskItemIdAsync(long taskItemId);
        Task<bool> AddNoteAsync(TaskNote taskNote);
        Task<bool> UpdateToIsCancelledAsync(long taskNoteId);

        #endregion
    }
}