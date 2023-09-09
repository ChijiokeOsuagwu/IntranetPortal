using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.WksModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IWorkspaceService
    {
        #region Workspace Action Interfaces
        Task<bool> CreateMainWorkspaceAsync(string ownerId);
        Task<bool> CreateWorkspaceAsync(Workspace workspace);
        Task<bool> UpdateWorkspaceAsync(Workspace workspace);
        Task<bool> DeleteWorkspaceAsync(Workspace workspace);
        Task<Workspace> GetMainWorkspaceAsync(string ownerId);
        Task<Workspace> GetWorkspaceAsync(int workspaceId);
        Task<List<Workspace>> GetWorkspacesByOwnerIdAsync(string ownerId);
        Task<List<Workspace>> SearchWorkspacesByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle = null);

        #endregion

        #region Project Folder Action Interfaces
        Task<List<ProjectFolder>> GetProjectFoldersByWorkspaceIDAsync(int WorkspaceID, bool? isArchived = null);
       
        Task<List<ProjectFolder>> GetProjectFoldersByOwnerIDAsync(string OwnerID, bool? isArchived = null);

        Task<ProjectFolder> GetProjectFolderAsync(int projectFolderId);

        Task<bool> CreateProjectFolderAsync(ProjectFolder folder);

        Task<bool> UpdateProjectFolderAsync(ProjectFolder folder);

        Task<bool> UpdateDeleteProjectFolderAsync(ProjectFolder folder);

        Task<List<ProjectFolder>> SearchProjectFoldersAsync(int WorkspaceID, string FolderTitle, bool? isArchived = null);
        #endregion

        #region Project Action Interfaces
        Task<bool> CreateProjectAsync(Project project);
        Task<bool> UpdateProjectAsync(Project project);
        Task<Project> GetWorkItemByIDAsync(int projectID);
        Task<Project> GetWorkItemByCodeAsync(string projectCode);
        Task<List<Project>> GetWorkItemsByFolderIDAsync(int FolderID);
        Task<List<Project>> GetProjectsByOwnerIDAsync(string OwnerID);
        Task<List<Project>> SearchProjectsAsync(string projectTitle);
        Task<List<Project>> SearchProjectsAsync(int FolderID, string projectTitle);

        #endregion

        #region Task List Service Interfaces

        //=============== Task List Read Service Methods =================//
        Task<List<TaskList>> GetTaskListsByOwnerIdAsync(string OwnerId, bool? isArchived = null);
        Task<List<TaskList>> SearchTaskListAsync(string OwnerId, bool? IsArchived = null, int? createdYear = null, int? createdMonth = null);
        Task<TaskList> GetTaskListAsync(int taskListId);
        Task<List<TaskList>> GetActiveTaskListsAsync(string OwnerId);

        //=============== Task List Note Service Methods =================//
        Task<List<TaskListNote>> GetTaskListNotesAsync(int taskListId);
        Task<bool> AddTaskListNoteAsync(TaskListNote taskListNote);


        //=============== Task List Write Service Methods =================//
        Task<bool> CreateTaskListAsync(TaskList taskList);
        Task<bool> UpdateTaskListAsync(TaskList taskList);
        Task<bool> UpdateTaskListArchiveAsync(int taskListId, bool isArchived);
        Task<bool> UpdateTaskListLockAsync(int taskListId, bool isLocked);
        Task<bool> UpdateDeleteTaskListAsync(TaskList taskList);
        Task<bool> DeleteTaskListAsync(int taskListId);
        Task<bool> ReturnTaskListSubmissionAsync(int taskListId, long submissionId);

        #endregion

        #region Task Items Service Interfaces

        //======= Task Items Write Service Interfaces ==================//
        Task<bool> CreateTaskItemAsync(TaskItem taskItem);
        Task<bool> UpdateTaskItemAsync(TaskItem taskItem);
        Task<bool> DeleteTaskItemAsync(long taskItemId);

        Task<bool> UpdateTaskCancelStatusAsync(long taskItemId, string updatedBy, bool cancelTask);
        Task<bool> UpdateTaskProgressAsync(long taskItemId, int newProgressStatusId, string previousProgressStatus, string updatedBy);
        Task<bool> UpdateTaskStatusAsync(long taskItemId, WorkItemStatus newTaskStatus, string updatedBy);
        Task<bool> UpdateTaskStatusByTaskListIdAsync(int taskListId, WorkItemStatus newStatus, string updatedBy);
        Task<bool> UpdateTaskTimelineAsync(TaskTimelineChange taskTimelineChange);
        Task<bool> UpdateTaskApprovalStatusAsync(long taskItemId, ApprovalStatus newApprovalStatus, string updatedBy);
        Task<bool> ReassignTaskItemAsync(long taskItemId, int newTaskListId, string newTaskOwnerId, string oldTaskOwnerName, string reassignedBy);

        Task<bool> LinkTaskToProgramAsync(long taskItemId, string programCode, DateTime? programDate, string updatedBy);
        Task<bool> LinkTaskToProjectAsync(long taskItemId, string projectCode, string updatedBy, string projectTitle);
        Task<bool> AddTaskItemNoteAsync(TaskNote taskNote);

        //======= Task Items Read Service Interfaces ==================//
        Task<TaskItem> GetTaskItemAsync(long taskItemId);
        Task<TaskItem> GetTaskItemAsync(string taskItemNo);
        Task<List<TaskItem>> GetTaskItemsByTaskListAsync(int taskListId, int? dueYear = null, int? dueMonth = null, WorkItemProgressStatus? progressStatus = null);
        
        //================= Task Item Note Service Methods ====================================//
        Task<List<TaskNote>> GetTaskNotesAsync(long taskItemId);
        
        //================= Task Item TimelineChanges Service Methods ===========================//
        Task<List<TaskTimelineChange>> GetTaskTimelineChangesByTaskItemIdAsync(long taskItemId);

        #endregion

        #region Task List Submission Service Methods
        Task<List<TaskSubmission>> SearchTaskSubmissionsAsync(string toEmployeeId, int? submittedYear = null, int? submittedMonth = null, string fromEmployeeName = null);
        Task<bool> DeleteTaskListSubmissionAsync(long taskListSubmissionId);
        Task<bool> AddTaskListSubmissionAsync(TaskSubmission taskSubmission);

        #endregion

        #region Task Evaluation Header Service Methods
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByTaskOwnerIdAsync(string taskOwnerId, int? evaluatedYear = null, int? evaluatedMonth = null);
        Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByReportsToEmployeeIdAsync(string reportsToEmployeeId, int? evaluatedYear = null, int? evaluatedMonth = null);
        Task<TaskEvaluationHeader> GetTaskEvaluationHeaderAsync(int taskListId, string evaluatorId);
        Task<TaskEvaluationHeader> GetTaskEvaluationHeaderAsync(int taskEvaluationHeaderId);
        Task<List<TaskEvaluationHeader>> SearchTaskEvaluationHeaderAsync(int startYear, int endYear, int? startMonth = null, int? endMonth = null, int? unitId = null, int? deptId = null, int? locationId = null);
        Task<List<TaskEvaluationHeaderSummary>> GetTaskEvaluationHeaderSummaryAsync(int startYear, int endYear, int? startMonth = null, int? endMonth = null, int? unitId = null, int? deptId = null, int? locationId = null);

        Task<int> CreateTaskEvaluationHeaderAsync(TaskEvaluationHeader header);
        #endregion

        #region Task Evaluation Detail Service Methods
        Task<TaskEvaluationDetail> GetTaskEvaluationDetailAsync(int taskEvaluationHeaderId, long taskItemId);
        Task<List<TaskEvaluationDetail>> GetTaskEvaluationDetailsAsync(int taskEvaluationHeaderId);
        Task<bool> AddTaskEvaluationDetailAsync(TaskEvaluationDetail detail);
        Task<bool> UpdateTaskEvaluationDetailAsync(TaskEvaluationDetail detail);
        #endregion

        #region Task Item Evaluation Service Methods
        Task<List<TaskItemEvaluation>> GetTaskItemEvaluationsAsync(int taskListId, string evaluatorId);

        #endregion
    }
}