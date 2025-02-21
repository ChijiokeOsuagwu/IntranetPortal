using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.WspModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.WspRepositories
{
    public interface IDeskspaceRepository
    {
        IConfiguration _config { get; }

        #region Workspaces Action Methods
        //===== Workspaces Write Action Methods =======//
        Task<long> AddWorkspaceAsync(Workspace workspace);
        Task<bool> DeleteWorkspaceAsync(long workspaceId);
        Task<bool> UpdateWorkspaceAsync(long workspaceId, string newTitle);

        //====== Workspaces Read Action Methods =====//
        Task<Workspace> GetWorkspaceByIdAsync(long workspaceId);
        Task<List<Workspace>> GetWorkspacesByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle);
        Task<List<Workspace>> GetWorkspacesByOwnerIdAsync(string ownerId);
        Task<Workspace> GetMainWorkspaceByOwnerIdAsync(string ownerId);
        Task<List<Workspace>> SearchWorkspacesByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle);
        #endregion

        #region Work Item Folders Action Methods
        //===== Work Item Folder Read Action Methods =====//
        Task<long> AddWorkItemFolderAsync(WorkItemFolder w);
        Task<bool> DeleteWorkItemFoldersAsync(long workItemFolderId);
        Task<bool> UpdateWorkItemFolderLockStatusAsync(long workItemFolderId, bool isLocked);
        Task<bool> UpdateWorkItemFolderArchiveAsync(long workItemFolderId, bool isArchived);
        Task<bool> UpdateWorkItemFolderAsync(WorkItemFolder w);

        //===== Work Item Folder Write Action Methods =====//
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdAndFolderTitleAsync(string ownerId, string folderTitle);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchivedStatusnCreaatedYearnCreatedMonthAsync(string ownerId, bool isArchived, int createdYear, int createdMonth);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchiveStatusAsync(string ownerId, bool isArchived);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchiveStatusnCreatedYearAsync(string ownerId, bool isArchived, int createdYear);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnCreatedYearAsync(string ownerId, int createdYear);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnCreatedYearnCreatedMonthAsync(string ownerId, int createdYear, int createdMonth);
        Task<WorkItemFolder> GetWorkItemFolderByIdAsync(long workItemFolderId);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdAsync(string ownerId);

        #endregion

        #region Work Item Note Action Methods
        Task<bool> AddNoteAsync(WorkItemNote n);
        Task<bool> DeleteWorkItemNoteAsync(long workItemNoteId);
        Task<List<WorkItemNote>> GetWorkItemNotesByFolderIdAsync(long folderId);
        Task<List<WorkItemNote>> GetWorkItemNotesByProjectIdAsync(long projectId);
        Task<List<WorkItemNote>> GetWorkItemNotesByTaskIdAsync(long taskId);
        Task<bool> UpdateWorkItemNoteToIsCancelledAsync(long workItemNoteId, string cancelledBy);
        #endregion

        #region Work Item Activity Log Action Methods
        Task<List<WorkItemActivityLog>> GetWorkItemActivityLogByFolderIdAsync(long folderId);
        Task<List<WorkItemActivityLog>> GetWorkItemActivityLogByTaskIdAsync(long taskId);
        Task<List<WorkItemActivityLog>> GetWorkItemActivityLogByProjectIdAsync(long projectId);
        Task<bool> AddWorkItemActivityLogAsync(WorkItemActivityLog log);
        Task<bool> DeleteWorkItemActivityLogAsync(long activityLogId);
        #endregion


        #region Task Items Repository

        #region Task Items Read Action Methods
        Task<TaskItem> GetTaskItemByIdAsync(long taskItemId);
        Task<List<TaskItem>> GetTaskItemsByFolderIdAsync(long folderId);
        Task<List<TaskItem>> GetTaskItemsByOwnerIdnDescriptionnFolderIdAsync(string ownerId, string taskDescription, long? folderId);

        //========== Pending Task Items ================//
        Task<List<TaskItem>> GetTaskItemsPendingByOwnerIdAsync(string ownerId);
        Task<long> GetTaskItemsPendingCountByOwnerIdAsync(string ownerId);
        #endregion

        #region Task Item Write Action Methods
        Task<long> AddTaskItemAsync(TaskItem task);
        Task<bool> UpdateTaskItemAsync(TaskItem task);
        Task<bool> DeleteTaskItemAsync(long taskItemId);

        Task<bool> UpdateTaskItemProgressStatusAsync(long taskItemId, int newProgressStatus, string modifiedBy);
        Task<bool> UpdateTaskItemStageAsync(long taskItemId, int newStage, string modifiedBy);
        Task<bool> UpdateTaskItemOpenCloseStatusAsync(long taskItemId, bool closeTask, string modifiedBy);
        Task<bool> UpdateTaskItemOpenCloseStatusByFolderIdAsync(long taskFolderId, bool closeTask, string modifiedBy);
        Task<bool> UpdateTaskItemApprovalStatusAsync(long taskItemId, ApprovalStatus newApprovalStatus, string approvedBy);
        Task<bool> UpdateTaskItemTimelineAsync(long taskItemId, string modifiedBy, DateTime? previousStartDate, DateTime? newStartDate, DateTime? previousEndDate, DateTime? newEndDate);
        Task<bool> UpdateTaskItemProgramLinkAsync(long taskItemId, string programCode, DateTime? programDate, string modifiedBy);
        Task<bool> UpdateTaskItemProjectLinkAsync(long taskItemId, string projectCode, string modifiedBy);
        Task<bool> UpdateTaskItemOwnerAsync(long taskItemId, long newTaskFolderId, string newTaskOwnerId, int newUnitId, int newDeptId, int newLocationId, string modifiedBy);


        #endregion

        #endregion

    }
}