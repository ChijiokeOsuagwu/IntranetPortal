using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.WspModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
   public interface IWspService
    {
        #region Workspace Service Methods
        Task<long> CreateMainWorkspaceAsync(string ownerId);
        Task<long> CreateWorkspaceAsync(Workspace workspace);
        Task<bool> UpdateWorkspaceAsync(Workspace workspace);
        Task<bool> DeleteWorkspaceAsync(long workspaceId);
        Task<Workspace> GetMainWorkspaceAsync(string ownerId);
        Task<Workspace> GetWorkspaceAsync(int workspaceId);
        Task<List<Workspace>> GetWorkspacesByOwnerIdAsync(string ownerId);
        Task<List<Workspace>> SearchWorkspacesByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle = null);

        #endregion

        #region Work Item Folder Service Methods
        #region Work Item Folder Write Service Methods
        Task<long> CreateWorkItemFolderAsync(WorkItemFolder folder);
        Task<bool> DeleteWorkItemFolderAsync(long FolderId);
        Task<bool> UpdateWorkItemFolderArchiveStatusAsync(long folderId, bool IsArchived, string UpdatedBy);
        Task<bool> UpdateWorkItemFolderAsync(WorkItemFolder folder);
        Task<bool> UpdateWorkItemFolderLockStatusAsync(long FolderId, bool IsLocked, string LockedBy);

    #endregion
        #region Work Item Folder Read Service Methods
        Task<List<WorkItemFolder>> GetActiveWorkItemFoldersAsync(string OwnerId);
        Task<List<WorkItemFolder>> GetFoldersByOwnerIdAsync(string OwnerId, bool? IsArchived = null);
        Task<WorkItemFolder> GetWorkItemFolderAsync(long WorkItemFolderId);
        Task<List<WorkItemFolder>> SearchWorkItemFoldersAsync(string OwnerId, bool? IsArchived = null, int? createdYear = null, int? createdMonth = null);
        #endregion
        #endregion

        #region Work Item Note
        Task<bool> AddWorkItemNoteAsync(WorkItemNote workItemNote);
        Task<List<WorkItemNote>> GetWorkItemFolderNotesAsync(long FolderId);
        Task<List<WorkItemNote>> GetTaskItemNotesAsync(long TaskId);
        #endregion

        #region Work Item Activities
        Task<List<WorkItemActivityLog>> GetWorkItemActivitiesByFolderIdAsync(long FolderId);
        Task<List<WorkItemActivityLog>> GetWorkItemActivitiesByTaskIdAsync(long TaskId);
        #endregion

        #region Task Item Service Methods
        #region Task Items Read Service Methods
        Task<List<TaskItem>> GetTasksByFolderIdAsync(long FolderId);
        Task<TaskItem> GetTaskById(long TaskId);
        #endregion

        #region Pending Task Items Service Methods
        Task<List<TaskItem>> GetTasksPendingAsync(string OwnerId);
        Task<long> GetTasksPendingCountAsync(string OwnerId);
        #endregion

        #region Task Item Write Service Methods
        Task<long> CreateTaskItemAsync(TaskItem taskItem);
        Task<bool> UpdateTaskItemAsync(TaskItem taskItem);
        Task<bool> DeleteTaskItemAsync(long taskItemId);
        Task<bool> UpdateTaskItemProgressAsync(long taskItemId, int newProgressStatusId, string previousProgressStatus, string updatedBy);
        Task<bool> UpdateTaskItemOpenCloseStatusAsync(long taskItemId, WorkItemStatus newStatus, string updatedBy);
        Task<bool> UpdateTaskItemOpenCloseStatusByFolderIdAsync(long taskFolderId, WorkItemStatus newStatus, string updatedBy);
        Task<bool> UpdateTaskItemApprovalStatusAsync(long taskItemId, ApprovalStatus newApprovalStatus, string updatedBy);
        Task<bool> LinkTaskItemToProgramAsync(long taskItemId, string programCode, DateTime? programDate, string updatedBy);
        Task<bool> LinkTaskItemToProjectAsync(long taskItemId, string projectCode, string updatedBy, string projectTitle);
        Task<bool> ReassignTaskItemAsync(long taskItemId, int newTaskListId, string newTaskOwnerId, string oldTaskOwnerName, string reassignedBy);
        #endregion
        #endregion
    }
}