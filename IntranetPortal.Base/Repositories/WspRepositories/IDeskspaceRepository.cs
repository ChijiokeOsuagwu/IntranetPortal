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

        #region Work Item Folder Write Action methods
        //===== Work Item Folder Write Action Methods =====//
        Task<long> AddWorkItemFolderAsync(WorkItemFolder w);
        Task<bool> DeleteWorkItemFoldersAsync(long workItemFolderId);
        Task<bool> UpdateWorkItemFolderLockStatusAsync(long workItemFolderId, bool isLocked);
        Task<bool> UpdateWorkItemFolderArchiveAsync(long workItemFolderId, bool isArchived);
        Task<bool> UpdateWorkItemFolderAsync(WorkItemFolder w);
        #endregion

        #region Work Item Folder Read Action Methods
        //===== Work Item Folder Write Action Methods =====//
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdAndFolderTitleAsync(string ownerId, string folderTitle);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchivedStatusnCreaatedYearnCreatedMonthAsync(string ownerId, bool isArchived, int createdYear, int createdMonth);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchiveStatusAsync(string ownerId, bool isArchived);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchiveStatusnCreatedYearAsync(string ownerId, bool isArchived, int createdYear);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnCreatedYearAsync(string ownerId, int createdYear);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnCreatedYearnCreatedMonthAsync(string ownerId, int createdYear, int createdMonth);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchiveStatusnArchivedDateAsync(string ownerId, bool isArchived, DateTime? fromDate = null, DateTime? toDate = null);

        Task<WorkItemFolder> GetWorkItemFolderByIdAsync(long workItemFolderId);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdAsync(string ownerId);
        Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchivednDatesAsync(string ownerId, bool IsArchived, DateTime? startDate = null, DateTime? endDate = null);
        #endregion

        #endregion

        #region Folder Submission Action Methods
        Task<bool> AddFolderSubmissionAsync(FolderSubmission submission);
        Task<bool> UpdateFolderSubmissionAsync(long folderSubmissionId);
        Task<bool> DeleteFolderSubmissionAsync(long submissionId);
        Task<FolderSubmission> GetFolderSubmissionByIdAsync(long submissionId);
        Task<List<FolderSubmission>> GetFolderSubmissionsByFolderIdAsync(long folderId);

        //==== Filter with To_Employee_Id ====//
        Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAsync(string toEmployeeId);
        Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAndFolderIdAsync(string toEmployeeId, long folderId);
        Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAndSubmittedYearAsync(string toEmployeeId, int submittedYear);
        Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAndSubmittedYearAndSubmittedMonthAsync(string toEmployeeId, int submittedYear, int submittedMonth);

        //===== Filter with From_Employee_Name =====//
        Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAndFromEmployeeNameAsync(string toEmployeeId, string fromEmployeeName);
        Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAsync(string toEmployeeId, string fromEmployeeName, int submittedYear);
        Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAndSubmittedMonthAsync(string toEmployeeId, string fromEmployeeName, int submittedYear, int submittedMonth);
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
        Task<long> GetTaskItemsCountByFolderIdAsync(long folderId);
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
        Task<bool> UpdateTaskItemResolutionAsync(long taskId, string taskResolution, string updatedBy);
        Task<bool> UpdateTaskItemFolderIdAsync(long taskItemId, string modifiedBy, long? taskFolderId = null);
        Task<bool> UpdateTaskItemFolderIdForPendingTaskItemsAsync(string taskOwnerId, long taskFolderId);
        Task<bool> UpdateTaskItemProgressStatusAsync(long taskItemId, int newProgressStatus, string modifiedBy);
        Task<bool> UpdateTaskItemStageAsync(long taskItemId, int newStage, string modifiedBy);
        Task<bool> UpdateTaskItemOpenCloseStatusAsync(long taskItemId, bool closeTask, string modifiedBy);
        Task<bool> UpdateTaskItemCompletionStatusAsync(long taskItemId, bool isCompleted, string modifiedBy);
        Task<bool> UpdateTaskItemOpenCloseStatusByFolderIdAsync(long taskFolderId, bool closeTask, string modifiedBy);
        Task<bool> UpdateTaskItemApprovalStatusAsync(long taskItemId, ApprovalStatus newApprovalStatus, string approvedBy);
        Task<bool> UpdateTaskItemTimelineAsync(long taskItemId, string modifiedBy, DateTime? previousStartDate, DateTime? newStartDate, DateTime? previousEndDate, DateTime? newEndDate);
        Task<bool> UpdateTaskItemProgramLinkAsync(long taskItemId, string programCode, DateTime? programDate, string modifiedBy);
        Task<bool> UpdateTaskItemProjectLinkAsync(long taskItemId, string projectCode, string modifiedBy);
        Task<bool> UpdateTaskItemOwnerAsync(long taskItemId, long newTaskFolderId, string newTaskOwnerId, int newUnitId, int newDeptId, int newLocationId, string modifiedBy);
        Task<bool> UpdateTaskItemActualStartDateAsync(long taskItemId, DateTime? newStartDate = null, string modifiedBy = null);
        Task<bool> UpdateTaskItemActualDueDateAsync(long taskItemId, DateTime? newDueDate = null, string modifiedBy = null);
        #endregion

        #region Task Item Timeline Change Repository
        Task<bool> AddTaskTimelineChangeAsync(TaskTimelineChange taskTimelineChange);
        Task<List<TaskTimelineChange>> GetTaskTimelineByTaskItemIdAsync(long taskItemId);
        Task<TaskTimelineChange> GetTaskTimelineBytimelineIdAsync(long timelineChangeId);
        #endregion

        #region Task Item Evaluation Repository

        //==== Evaluation Scores ======//
        Task<TaskEvaluationScores> GetTaskEvaluationHeaderScoresByTaskFolderIdAndEvaluatorIdAsync(long taskFolderId, string evaluatorId);

        //==== Task Item Evaluation 
        Task<List<TaskItemEvaluation>> GetTaskItemEvaluationsByTaskFolderIdAsync(long taskFolderId);
        Task<List<TaskItemEvaluation>> GetTaskItemEvaluationsByTaskFolderIdAndEvaluatorIdAsync(long taskFolderId, string evaluatorId);

        #region Task Evaluation Header Write Action Methods
        Task<long> AddTaskEvaluationHeaderAsync(TaskEvaluationHeader taskEvaluationHeader);
        Task<bool> UpdateTaskEvaluationHeaderAsync(TaskEvaluationHeader taskEvaluationHeader);
        #endregion

        #region Task Evaluation Header Read by Task OwnerID Action Methods
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByTaskOwnerIdAsync(string taskOwnerId, DateTime? fromDate = null, DateTime? toDate = null);
        #endregion

        #region Task Evaluation Header Read by Task List ID Action Methods
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByTaskFolderIdAndEvaluatorIdAsync(long taskFolderId, string taskEvaluatorId);
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByIdAsync(long taskEvaluationHeaderId);
        #endregion

        #region Task Evaluation Header Read by Reports To Employee ID Action Methods
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByReportsToEmployeeIdAsync(string reportsToEmployeeId, DateTime? fromDate = null, DateTime? toDate = null);
        #endregion

        #region Task Evaluation Header Read by Location, Department and Unit Action Methods
        //======== Get By Unit ID, Department ID, Location ID =========//
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByUnitIdAsync(int taskOwnerUnitId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByUnitIdAndLocationIdAsync(int taskOwnerUnitId, int taskOwnerLocationId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByDepartmentIdAsync(int taskOwnerDepartmentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByDepartmentIdAndLocationIdAsync(int taskOwnerDepartmentId, int taskOwnerLocationId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByLocationIdAsync(int taskOwnerLocationId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersAsync(DateTime? fromDate = null, DateTime? toDate = null);

        #endregion

        #region Task Evaluation Detail Read Action Methods
        Task<TaskEvaluationDetail> GetTaskEvaluationDetailByTaskEvaluationDetailIdAsync(long taskEvaluationDetailId);
        Task<List<TaskEvaluationDetail>> GetTaskEvaluationDetailsByTaskEvaluationHeaderIdAndTaskItemIdAsync(long taskEvaluationHeaderId, long taskItemId);
        Task<List<TaskEvaluationDetail>> GetTaskEvaluationDetailsByTaskEvaluationHeaderIdAsync(long taskEvaluationHeaderId);
        Task<long> GetTaskEvaluationItemsCountByFolderIdnEvaluatorIdAsync(long taskFolderId, string evaluatorId);

        Task<bool> AddTaskEvaluationDetailsAsync(TaskEvaluationDetail taskEvaluationDetail);
        Task<bool> UpdateTaskEvaluationDetailsAsync(TaskEvaluationDetail taskEvaluationDetail);
        #endregion

        #region Task Evaluation Return Action Methods
        Task<bool> AddTaskEvaluationReturnAsync(TaskEvaluationReturns taskEvaluationReturn);
        Task<bool> DeleteTaskEvaluationReturnAsync(long taskItemId, string returnedBy);
        Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskItemIdAsync(long taskItemId);
        Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskOwnerIdAsync(string taskOwnerId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskFolderIdAsync(long taskFolderId);
        Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskFolderIdAsync(long taskFolderId, bool isExempted = false);

        Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskEvaluationHeaderIdAsync(long taskEvaluationHeaderId);
        Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskEvaluationHeaderIdAsync(long taskEvaluationHeaderId, bool isExempted = false);
        #endregion

        #region Task Evaluation Summary Read Action Methods
        //===== By Task Owner ID =====//
        Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByTaskOwnerIdAsync(string taskOwnerId, DateTime? fromDate = null, DateTime? toDate = null);
        //===== By Unit ID ====//
        Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByUnitIdAsync(int taskOwnerUnitId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByUnitIdAndLocationIdAsync(int taskOwnerUnitId, int taskOwnerLocationId, DateTime? fromDate = null, DateTime? toDate = null);
        //===== By Department ID ====//
        Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByDepartmentIdAsync(int taskOwnerDepartmentId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByDepartmentIdAndLocationIdAsync(int taskOwnerDepartmentId, int taskOwnerLocationId, DateTime? fromDate = null, DateTime? toDate = null);
        //===== By Location ID ====//
        Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByLocationIdAsync(int taskOwnerLocationId, DateTime? fromDate = null, DateTime? toDate = null);

        Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryAsync(DateTime? fromDate = null, DateTime? toDate = null);
        #endregion

        #region Task Evaluation Summary Write Action Methods
        Task<long> AddTaskEvaluationSummaryAsync(TaskEvaluationSummary s);
        Task<bool> UpdateTaskEvaluationSummaryAsync(TaskEvaluationSummary s);
        #endregion

        #region Task Evaluation Scores Read Action Methods
        Task<TaskEvaluationScores> GetTaskEvaluationScoresByTaskOwnerIdAsync(string taskOwnerId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<TaskEvaluationScores> GetTaskEvaluationScoresByTaskOwnerNameAsync(string taskOwnerName, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationScores>> GetTaskEvaluationScoresByUnitIdAsync(int unitId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationScores>> GetTaskEvaluationScoresByUnitIdnLocationIdAsync(int locationId, int unitId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationScores>> GetTaskEvaluationScoresByDepartmentIdnLocationIdAsync(int locationId, int deptId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationScores>> GetTaskEvaluationScoresByDepartmentIdAsync(int deptId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<TaskEvaluationScores>> GetTaskEvaluationScoresByLocationIdAsync(int locationId, DateTime? fromDate = null, DateTime? toDate = null);
        #endregion

        #endregion

        #endregion

        #region Work Item Return Reasons Action Methods
        Task<List<WorkItemReturnReason>> GetWorkItemReturnReasonsAsync();
        #endregion
    }
}