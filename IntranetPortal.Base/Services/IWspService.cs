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
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdAsync(string OwnerId, bool? IsArchived = null);
        Task<WorkItemFolder> GetWorkItemFolderAsync(long WorkItemFolderId);
        Task<List<WorkItemFolder>> GetWorkItemFoldersArchivedAsync(string OwnerId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdAsync(string OwnerId, bool IsArchived, DateTime? fromDate = null, DateTime? toDate = null);



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
        Task<long> GetTaskItemsCountByFolderIdAsync(long FolderId);
        Task<List<TaskItem>> GetTasksByFolderIdAsync(long FolderId);
        Task<TaskItem> GetTaskItemByIdAsync(long TaskId);
        #endregion

        #region Pending Task Items Service Methods
        Task<List<TaskItem>> GetTasksPendingAsync(string OwnerId);
        Task<long> GetTasksPendingCountAsync(string OwnerId);
        #endregion

        #region Task Item Write Service Methods
        Task<long> CreateTaskItemAsync(TaskItem taskItem);
        Task<bool> UpdateTaskItemAsync(TaskItem taskItem);
        Task<bool> DeleteTaskItemAsync(long taskItemId);
        Task<bool> UpdateTaskItemResolutionAsync(long taskItemId, string taskResolution, string updatedBy);
        Task<bool> UpdateTaskItemFolderIdAsync(long taskItemId, string updatedBy, long? taskFolderId = null);
        Task<bool> UpdateTaskItemProgressAsync(long taskItemId, int newProgressStatusId, string previousProgressStatus, string updatedBy);
        Task<bool> UpdateTaskItemOpenCloseStatusAsync(long taskItemId, WorkItemStatus newStatus, string updatedBy);
        Task<bool> UpdateTaskItemOpenCloseStatusByFolderIdAsync(long taskFolderId, WorkItemStatus newStatus, string updatedBy);
        Task<bool> UpdateTaskItemApprovalStatusAsync(long taskItemId, ApprovalStatus newApprovalStatus, string updatedBy);
        Task<bool> LinkTaskItemToProgramAsync(long taskItemId, string programCode, DateTime? programDate, string updatedBy);
        Task<bool> LinkTaskItemToProjectAsync(long taskItemId, string projectCode, string updatedBy, string projectTitle);
        Task<bool> ReassignTaskItemAsync(long taskItemId, int newTaskListId, string newTaskOwnerId, string oldTaskOwnerName, string reassignedBy);
        #endregion

        #region Task Item Timeline Service Methods
        Task<bool> UpdateTaskTimelineAsync(TaskTimelineChange taskTimelineChange);
        Task<List<TaskTimelineChange>> GetTaskTimelineChangesByTaskItemIdAsync(long taskItemId);
        #endregion
        #endregion

        #region Folder Submission Service Methods
        Task<bool> AddFolderSubmissionAsync(FolderSubmission submission);
        Task<bool> DeleteFolderSubmissionAsync(long folderSubmissionId);
        Task<bool> ReturnFolderSubmissionAsync(long folderId, long submissionId);
        Task<FolderSubmission> GetFolderSubmissionByIdAsync(long folderSubmissionId);
        Task<List<FolderSubmission>> GetFolderSubmissionsByFolderIdAsync(long folderId);
        Task<List<FolderSubmission>> SearchFolderSubmissionsAsync(string toEmployeeId, int? submittedYear = null, int? submittedMonth = null, string fromEmployeeName = null);
        #endregion

        #region Task Evaluation Header Service Action Interfaces
        #region Task Evaluation Header Read Interfaces
        Task<TaskEvaluationHeader> GetTaskEvaluationHeaderAsync(long taskEvaluationHeaderId);
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByTaskOwnerIdAsync(string taskOwnerId, DateTime? FromDate = null, DateTime? ToDate = null);
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByReportsToEmployeeIdAsync(string reportsToEmployeeId, DateTime? FromDate = null, DateTime? ToDate = null);
        Task<TaskEvaluationHeader> GetTaskEvaluationHeaderAsync(long taskFolderId, string evaluatorId);
        Task<List<TaskEvaluationHeader>> SearchTaskEvaluationHeaderAsync(DateTime FromDate, DateTime? ToDate = null, int? unitId = null, int? deptId = null, int? locationId = null);
        #endregion

        #region Task Evaluation Header Write Interfaces
        Task<long> CreateTaskEvaluationHeaderAsync(TaskEvaluationHeader header);
        Task<bool> UpdateTaskEvaluationHeaderAsync(TaskEvaluationHeader header);
        #endregion

        #endregion

        #region Task Evaluation Summary Service Interfaces
        Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryAsync(DateTime? FromDate = null, DateTime? ToDate = null, int? unitId = null, int? deptId = null, int? locationId = null);
        Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByTaskOwnerIdAsync(string TaskOwnerId, DateTime? FromDate = null, DateTime? ToDate = null);
        #endregion

        #region Evaluation Details Service Interfaces
        Task<TaskEvaluationDetail> GetTaskEvaluationDetailByIdAsync(long taskEvaluationDetailId);
        Task<TaskEvaluationDetail> GetTaskEvaluationDetailAsync(long taskEvaluationHeaderId, long taskItemId);
        Task<List<TaskEvaluationDetail>> GetTaskEvaluationDetailsAsync(long taskEvaluationHeaderId);
        Task<long> GetEvaluatedTaskItemsCountAsync(long taskFolderId, string evaluatorId);

        Task<bool> AddTaskEvaluationDetailAsync(TaskEvaluationDetail detail);
        Task<bool> UpdateTaskEvaluationDetailAsync(TaskEvaluationDetail detail);
        #endregion

        #region Evaluation Returns Service Interfaces
        Task<bool> ReturnTaskEvaluationAsync(TaskEvaluationReturns evaluationReturn);
        Task<bool> DeleteTaskEvaluationReturnAsync(long taskItemId, string returnedBy);
        Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsAsync(string taskOwnerId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskItemIdAsync(long taskItemId);
        Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskFolderIdAsync(long taskFolderId, bool? IsExemptedFromEvaluation = null);

        Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskEvaluationHeaderIdAsync(long taskEvaluationHeaderId, bool? IsExemptedFromEvaluation = null);
        #endregion

        #region Task Item Evaluation Service Interfaces
        Task<List<TaskItemEvaluation>> GetTaskItemEvaluationsAsync(long taskFolderId, string evaluatorId = null);
        #endregion

        #region Task Item Evaluation Scores Service Interfaces
        Task<TaskEvaluationScores> GetTaskEvaluationScoresByOwnerId(string TaskOwnerId, DateTime? StartDate = null, DateTime? EndDate = null);
        Task<List<TaskEvaluationScores>> GetTaskEvaluationScoresAsync(string TaskOwnerName = null, int? UnitId = null, int? DepartmentId = null, int? LocationId = null, DateTime? StartDate = null, DateTime? EndDate = null);
        #endregion



        #region Work Item Return Reasons Service Interfaces
        Task<List<WorkItemReturnReason>> GetWorkItemReturnReasonsAsync();
        #endregion
    }
}