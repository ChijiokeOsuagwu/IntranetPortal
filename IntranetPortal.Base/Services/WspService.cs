using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.ErmRepositories;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using IntranetPortal.Base.Repositories.WspRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class WspService : IWspService
    {
        private readonly IDeskspaceRepository _deskspaceRepository;
        private readonly IUtilityRepository _utilityRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IEmployeesRepository _employeesRepository;

        public WspService(IDeskspaceRepository deskspaceRepository, IUtilityRepository utilityRepository,
            IProgramRepository programRepository, IEmployeesRepository employeesRepository)
        {
            _deskspaceRepository = deskspaceRepository;
            _utilityRepository = utilityRepository;
            _programRepository = programRepository;
            _employeesRepository = employeesRepository;
        }

        #region Workspace Service Actions
        public async Task<long> CreateWorkspaceAsync(Workspace workspace)
        {
            if (workspace == null) { throw new ArgumentNullException(nameof(workspace), "The required parameter [Workspace] is missing."); }
            var entities = await _deskspaceRepository.GetWorkspacesByOwnerIdAndTitleAsync(workspace.OwnerID, workspace.Title);
            if (entities != null && entities.Count > 0 && entities[0].Id > 0)
            {
                throw new Exception("Title already exists. Please choose another Title.");
            }
            return await _deskspaceRepository.AddWorkspaceAsync(workspace);
        }
        public async Task<bool> UpdateWorkspaceAsync(Workspace workspace)
        {
            if (workspace == null) { throw new ArgumentNullException(nameof(workspace), "The required parameter [Workspace] is missing."); }
            var entities = await _deskspaceRepository.GetWorkspacesByOwnerIdAndTitleAsync(workspace.OwnerID, workspace.Title);
            if (entities != null && entities.Count > 0 && entities[0].Id > 0)
            {
                throw new Exception("Title already exists. Please choose another Title.");
            }
            return await _deskspaceRepository.UpdateWorkspaceAsync(workspace.Id, workspace.Title);
        }
        public async Task<long> CreateMainWorkspaceAsync(string ownerId)
        {
            if (string.IsNullOrWhiteSpace(ownerId)) { throw new ArgumentNullException(nameof(ownerId), "The required parameter [OwnerId] is missing."); }
            Workspace workspace = new Workspace
            {
                CreatedBy = "System Service",
                CreatedTime = DateTime.UtcNow,
                IsMain = true,
                OwnerID = ownerId,
                Title = "Main Workspace",
                Id = 0
            };

            var entities = await _deskspaceRepository.GetWorkspacesByOwnerIdAndTitleAsync(workspace.OwnerID, workspace.Title);
            if (entities != null && entities.Count > 0 && entities[0].Id > 0)
            {
                throw new Exception("Title already exists. Please choose another Title.");
            }

            long _newWorkspaceId = await _deskspaceRepository.AddWorkspaceAsync(workspace);
            if (_newWorkspaceId > 0)
            {
                var new_entity = await _deskspaceRepository.GetWorkspaceByIdAsync(_newWorkspaceId);
                if (new_entity != null)
                {
                    Workspace mainWorkspace = new_entity;
                    WorkItemFolder workFolder = new WorkItemFolder
                    {
                        Id = 0,
                        Title = "Default Task Folder",
                        WorkspaceId = mainWorkspace.Id,
                        CreatedBy = "System Service",
                        CreatedTime = DateTime.UtcNow,
                        IsArchived = false,
                        OwnerId = ownerId,
                    };
                    return await _deskspaceRepository.AddWorkItemFolderAsync(workFolder);
                }
                else
                {
                    await _deskspaceRepository.DeleteWorkspaceAsync(_newWorkspaceId);
                    return -1;
                }
            }
            else { return -1; }
        }
        public async Task<bool> DeleteWorkspaceAsync(long workspaceId)
        {
            return await _deskspaceRepository.DeleteWorkspaceAsync(workspaceId);
        }
        public async Task<Workspace> GetMainWorkspaceAsync(string ownerId)
        {
            Workspace workspace = new Workspace();
            var entity = await _deskspaceRepository.GetMainWorkspaceByOwnerIdAsync(ownerId);
            if (entity != null && entity.Id > 0) { workspace = entity; }
            return workspace;
        }
        public async Task<Workspace> GetWorkspaceAsync(int workspaceId)
        {
            Workspace workspace = new Workspace();
            var entity = await _deskspaceRepository.GetWorkspaceByIdAsync(workspaceId);
            if (entity != null && entity.Id > 0) { workspace = entity; }
            return workspace;
        }
        public async Task<List<Workspace>> GetWorkspacesByOwnerIdAsync(string ownerId)
        {
            List<Workspace> workspaces = new List<Workspace>();
            var entities = await _deskspaceRepository.GetWorkspacesByOwnerIdAsync(ownerId);
            if (entities != null && entities.Count > 0) { workspaces = entities.ToList(); }
            return workspaces;
        }
        public async Task<List<Workspace>> SearchWorkspacesByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle = null)
        {
            List<Workspace> workspaces = new List<Workspace>();

            if (!string.IsNullOrWhiteSpace(workspaceTitle))
            {
                var entities = await _deskspaceRepository.SearchWorkspacesByOwnerIdAndTitleAsync(ownerId, workspaceTitle);
                if (entities != null && entities.Count > 0) { workspaces = entities.ToList(); }
            }
            else
            {
                var entities = await _deskspaceRepository.GetWorkspacesByOwnerIdAsync(ownerId);
                if (entities != null && entities.Count > 0) { workspaces = entities.ToList(); }
            }
            return workspaces;
        }
        #endregion

        #region Work Item Folders Service Action

        //======= Work Item Folders Read Service Methods ============//
        #region Work Item Folders Read Service Methods
        public async Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdAsync(string OwnerId, bool? IsArchived = null)
        {
            List<WorkItemFolder> folderLists = new List<WorkItemFolder>();
            if (IsArchived != null)
            {
                var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdnArchiveStatusAsync(OwnerId, IsArchived.Value);
                if (entities != null && entities.Count > 0) { folderLists = entities.ToList(); }
            }
            else
            {
                var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdAsync(OwnerId);
                if (entities != null && entities.Count > 0) { folderLists = entities.ToList(); }
            }
            return folderLists;
        }
        public async Task<WorkItemFolder> GetWorkItemFolderAsync(long WorkItemFolderId)
        {
            WorkItemFolder taskList = new WorkItemFolder();
            var entity = await _deskspaceRepository.GetWorkItemFolderByIdAsync(WorkItemFolderId);
            if (entity != null && entity.Id > 0) { taskList = entity; }
            return taskList;
        }
        public async Task<List<WorkItemFolder>> GetActiveWorkItemFoldersAsync(string OwnerId)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdnArchiveStatusAsync(OwnerId, false);
            if (entities != null && entities.Count > 0)
            {
                folderList = entities.ToList();
            }
            return folderList;
        }
        public async Task<List<WorkItemFolder>> GetWorkItemFoldersArchivedAsync(string OwnerId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdnArchiveStatusnArchivedDateAsync(OwnerId, true, fromDate, toDate);
            folderList = entities.ToList();
            return folderList;
        }
        public async Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdAsync(string OwnerId, bool IsArchived, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdnArchivednDatesAsync(OwnerId, IsArchived, fromDate, toDate);
            folderList = entities.ToList();
            return folderList;
        }


        public async Task<List<WorkItemFolder>> SearchWorkItemFoldersAsync(string OwnerId, bool? IsArchived = null, int? createdYear = null, int? createdMonth = null)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            if (IsArchived != null)
            {
                if (createdYear != null && createdYear.Value > 0)
                {
                    if (createdMonth != null && createdMonth.Value > 0)
                    {
                        var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdnArchivedStatusnCreaatedYearnCreatedMonthAsync(OwnerId, IsArchived.Value, createdYear.Value, createdMonth.Value);
                        folderList = entities.ToList();
                    }
                    else
                    {
                        var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdnArchiveStatusnCreatedYearAsync(OwnerId, IsArchived.Value, createdYear.Value);
                        folderList = entities.ToList();
                    }
                }
                else
                {
                    var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdnArchiveStatusAsync(OwnerId, IsArchived.Value);
                    folderList = entities.ToList();
                }
            }
            else
            {
                if (createdYear != null && createdYear.Value > 0)
                {
                    if (createdMonth != null && createdMonth.Value > 0)
                    {
                        var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdnCreatedYearnCreatedMonthAsync(OwnerId, createdYear.Value, createdMonth.Value);
                        folderList = entities.ToList();
                    }
                    else
                    {
                        var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdnCreatedYearAsync(OwnerId, createdYear.Value);
                        folderList = entities.ToList();
                    }
                }
                else
                {
                    var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdAsync(OwnerId);
                    folderList = entities.ToList();
                }
            }
            return folderList;
        }
        #endregion
        //============== Work Item Folder Write Service Methods ================================//
        #region Work Item Folders Write Service Methods
        public async Task<long> CreateWorkItemFolderAsync(WorkItemFolder folder)
        {
            if (folder == null) { throw new ArgumentNullException(nameof(folder), "The required parameter [Folder] is missing."); }
            var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdAndFolderTitleAsync(folder.OwnerId, folder.Title);
            if (entities != null && entities.Count > 0 && entities[0].Id > 0)
            {
                throw new Exception("Name already exists. Please choose another Name.");
            }
            long _newFolderId = await _deskspaceRepository.AddWorkItemFolderAsync(folder);
            if (_newFolderId > 0)
            {
                bool _importAllPendingTasks = await _deskspaceRepository.UpdateTaskItemFolderIdForPendingTaskItemsAsync(folder.OwnerId, _newFolderId);
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = folder.CreatedBy,
                    Description = $"New Task Folder was created by {folder.CreatedBy} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} WAT.",
                    WorkItemFolderId = _newFolderId,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = null
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return _newFolderId;
        }
        public async Task<bool> UpdateWorkItemFolderAsync(WorkItemFolder folder)
        {
            if (folder == null) { throw new ArgumentNullException(nameof(folder), "The required parameter [Folder] is missing."); }
            var entities = await _deskspaceRepository.GetWorkItemFoldersByOwnerIdAndFolderTitleAsync(folder.OwnerId, folder.Title);
            if (entities != null && entities.Count > 0 && entities[0].Id != folder.Id)
            {
                throw new Exception("Please choose another Name. You already have another Task List in the system with the same name.");
            }
            bool IsUpdated = await _deskspaceRepository.UpdateWorkItemFolderAsync(folder);
            if (IsUpdated)
            {
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = folder.UpdatedBy,
                    Description = $"Task Folder was updated by {folder.UpdatedBy} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} WAT.",
                    WorkItemFolderId = folder.Id,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = null
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsUpdated;
        }
        public async Task<bool> UpdateWorkItemFolderArchiveStatusAsync(long folderId, bool IsArchived, string UpdatedBy)
        {
            if (folderId < 1) { throw new ArgumentNullException(nameof(folderId), "The required parameter [Folder ID] is missing."); }
            bool IsUpdated = await _deskspaceRepository.UpdateWorkItemFolderArchiveAsync(folderId, IsArchived);
            if (IsUpdated)
            {
                string actionDescription = "";
                if (IsArchived) { actionDescription = "archived"; } else { actionDescription = "de-archived"; }
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = UpdatedBy,
                    Description = $"Task Folder was {actionDescription} by {UpdatedBy} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} WAT.",
                    WorkItemFolderId = folderId,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = null
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsUpdated;
        }
        public async Task<bool> UpdateWorkItemFolderLockStatusAsync(long FolderId, bool IsLocked, string LockedBy)
        {
            if (FolderId < 1) { throw new ArgumentNullException(nameof(FolderId), "The required parameter [Folder ID] is missing."); }
            bool IsUpdated = await _deskspaceRepository.UpdateWorkItemFolderLockStatusAsync(FolderId, IsLocked);
            if (IsUpdated)
            {
                string actionDescription = "";
                if (IsLocked) { actionDescription = "locked"; } else { actionDescription = "unlocked"; }
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = LockedBy,
                    Description = $"Task Folder was {actionDescription} by {LockedBy} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} WAT.",
                    WorkItemFolderId = FolderId,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = null
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsUpdated;
        }
        public async Task<bool> DeleteWorkItemFolderAsync(long FolderId)
        {
            if (FolderId < 1) { throw new ArgumentNullException(nameof(FolderId), "The required parameter [Folder ID] is missing."); }
            return await _deskspaceRepository.DeleteWorkItemFoldersAsync(FolderId);
        }
        #endregion

        #region Folder Submission Service Methods
        public async Task<bool> AddFolderSubmissionAsync(FolderSubmission submission)
        {
            bool IsAdded = false;
            if (submission == null) { throw new ArgumentNullException(nameof(submission)); }
            List<FolderSubmission> existingSubmissions = await _deskspaceRepository.GetFolderSubmissionsByToEmployeeIdAndFolderIdAsync(submission.ToEmployeeId, submission.FolderId);
            if (existingSubmissions != null && existingSubmissions.Count > 0)
            {
                foreach (var s in existingSubmissions)
                {
                    if (s.SubmissionType == submission.SubmissionType && s.IsActioned == false)
                    {
                        throw new Exception($"Double submission. You already submitted this task list for {submission.SubmissionType.ToString()}.");
                    }
                }
            }

            IsAdded = await _deskspaceRepository.AddFolderSubmissionAsync(submission);
            if (IsAdded)
            {
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = submission.FromEmployeeName,
                    Description = $"Folder was submitted to {submission.ToEmployeeName} for {submission.SubmissionType.ToString()} by {submission.FromEmployeeName}] on [{submission.DateSubmitted.Value.ToLongDateString()} at exactly {submission.DateSubmitted.Value.ToLongTimeString()} GMT. ",
                    WorkItemFolderId = submission.FolderId,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = null
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsAdded;
        }
        public async Task<bool> DeleteFolderSubmissionAsync(long folderSubmissionId)
        {
            return await _deskspaceRepository.DeleteFolderSubmissionAsync(folderSubmissionId);
        }
        public async Task<FolderSubmission> GetFolderSubmissionByIdAsync(long folderSubmissionId)
        {
            FolderSubmission submission = new FolderSubmission();
            if (folderSubmissionId > 0)
            {
                var entity = await _deskspaceRepository.GetFolderSubmissionByIdAsync(folderSubmissionId);
                if (entity != null) { submission = entity; }
            }
            return submission;
        }
        public async Task<List<FolderSubmission>> GetFolderSubmissionsByFolderIdAsync(long folderId)
        {
            List<FolderSubmission> submissions = new List<FolderSubmission>();
            if (folderId > 0)
            {
                var entities = await _deskspaceRepository.GetFolderSubmissionsByFolderIdAsync(folderId);
                if (entities != null) { submissions = entities.ToList(); }
            }
            return submissions;
        }
        public async Task<List<FolderSubmission>> SearchFolderSubmissionsAsync(string toEmployeeId, int? submittedYear = null, int? submittedMonth = null, string fromEmployeeName = null)
        {
            List<FolderSubmission> submissions = new List<FolderSubmission>();

            if (!string.IsNullOrWhiteSpace(fromEmployeeName))
            {
                if (submittedYear != null && submittedYear.Value > 2024)
                {
                    if (submittedMonth != null && submittedMonth.Value > 0)
                    {
                        var month_year_entities = await _deskspaceRepository.GetFolderSubmissionsByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAndSubmittedMonthAsync(toEmployeeId, fromEmployeeName, submittedYear.Value, submittedMonth.Value);
                        if (month_year_entities != null && month_year_entities.Count > 0) { submissions = month_year_entities.ToList(); }
                    }
                    else
                    {
                        var year_entities = await _deskspaceRepository.GetFolderSubmissionsByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAsync(toEmployeeId, fromEmployeeName, submittedYear.Value);
                        if (year_entities != null && year_entities.Count > 0) { submissions = year_entities.ToList(); }
                    }
                }
                else
                {
                    submittedYear = DateTime.Now.Year;
                    if (submittedMonth != null && submittedMonth.Value > 0)
                    {
                        var month_year_entities = await _deskspaceRepository.GetFolderSubmissionsByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAndSubmittedMonthAsync(toEmployeeId, fromEmployeeName, submittedYear.Value, submittedMonth.Value);
                        if (month_year_entities != null && month_year_entities.Count > 0) { submissions = month_year_entities.ToList(); }
                    }
                    else
                    {
                        var year_entities = await _deskspaceRepository.GetFolderSubmissionsByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAsync(toEmployeeId, fromEmployeeName, submittedYear.Value);
                        if (year_entities != null && year_entities.Count > 0) { submissions = year_entities.ToList(); }
                    }
                }
            }
            else
            {
                if (submittedYear != null && submittedYear.Value > 1900)
                {
                    if (submittedMonth != null && submittedMonth.Value > 0)
                    {
                        var month_year_entities = await _deskspaceRepository.GetFolderSubmissionsByToEmployeeIdAndSubmittedYearAndSubmittedMonthAsync(toEmployeeId, submittedYear.Value, submittedMonth.Value);
                        if (month_year_entities != null && month_year_entities.Count > 0) { submissions = month_year_entities.ToList(); }
                    }
                    else
                    {
                        var year_entities = await _deskspaceRepository.GetFolderSubmissionsByToEmployeeIdAndSubmittedYearAsync(toEmployeeId, submittedYear.Value);
                        if (year_entities != null && year_entities.Count > 0) { submissions = year_entities.ToList(); }
                    }
                }
                else
                {
                    submittedYear = DateTime.Now.Year;
                    if (submittedMonth != null && submittedMonth.Value > 0)
                    {
                        var month_year_entities = await _deskspaceRepository.GetFolderSubmissionsByToEmployeeIdAndSubmittedYearAndSubmittedMonthAsync(toEmployeeId, submittedYear.Value, submittedMonth.Value);
                        if (month_year_entities != null && month_year_entities.Count > 0) { submissions = month_year_entities.ToList(); }
                    }
                    else
                    {
                        var year_entities = await _deskspaceRepository.GetFolderSubmissionsByToEmployeeIdAndSubmittedYearAsync(toEmployeeId, submittedYear.Value);
                        if (year_entities != null && year_entities.Count > 0) { submissions = year_entities.ToList(); }
                    }
                }
            }
            return submissions;
        }
        public async Task<bool> ReturnFolderSubmissionAsync(long folderId, long submissionId)
        {
            if (submissionId < 1) { throw new ArgumentNullException(nameof(submissionId), "The required parameter [Submission ID] is missing."); }
            if (folderId < 1) { throw new ArgumentNullException(nameof(folderId), "The required parameter [Folder ID] is missing."); }

            FolderSubmission folderSubmission = await _deskspaceRepository.GetFolderSubmissionByIdAsync(submissionId);
            if (folderSubmission == null) { throw new Exception("No record was found for this submission."); }
            else
            {
                if (folderSubmission.SubmissionType == WorkItemSubmissionType.Approval)
                {
                    bool folderIsUnlocked = await _deskspaceRepository.UpdateWorkItemFolderLockStatusAsync(folderId, false);
                    if (folderIsUnlocked)
                    {
                        bool submissionStatusUpdated = await _deskspaceRepository.UpdateFolderSubmissionAsync(submissionId);
                        if (submissionStatusUpdated)
                        {
                            WorkItemActivityLog activityLog = new WorkItemActivityLog
                            {
                                Time = DateTime.Now,
                                ActivityBy = folderSubmission.ToEmployeeName,
                                Description = $"Folder that was submitted for {folderSubmission.SubmissionType.ToString()} by {folderSubmission.FromEmployeeName}] was returned by {folderSubmission.ToEmployeeName} on [{DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} WAT. ",
                                WorkItemFolderId = folderId,
                                Id = 0,
                                ProjectId = null,
                                TaskItemId = null
                            };
                            await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
                        }
                    }
                    return folderIsUnlocked;
                }
                else if (folderSubmission.SubmissionType == WorkItemSubmissionType.Review)
                {
                    //====== Code for Update the Evaluation Records goes here. ========//
                    bool folderIsClosed = await _deskspaceRepository.UpdateTaskItemOpenCloseStatusByFolderIdAsync(folderSubmission.FolderId, true, folderSubmission.ToEmployeeName);
                    if (!folderIsClosed) { return false; }
                    else
                    {
                        bool folderIsArchived = await _deskspaceRepository.UpdateWorkItemFolderArchiveAsync(folderSubmission.FolderId, true);
                        if (!folderIsArchived) { return false; }
                        else
                        {
                            TaskEvaluationSummary summary = new TaskEvaluationSummary();
                            TaskEvaluationHeader header = new TaskEvaluationHeader();
                            var entities = await _deskspaceRepository.GetTaskEvaluationHeadersByTaskFolderIdAndEvaluatorIdAsync(folderSubmission.FolderId, folderSubmission.ToEmployeeId);
                            if (entities != null && entities.Count > 0)
                            {
                                header = entities.FirstOrDefault();
                                summary.TaskEvaluationHeaderId = header.Id;
                                summary.TaskOwnerId = header.TaskOwnerId;
                                summary.EvaluationDate = DateTime.Now;
                                summary.TaskEvaluatorId = header.EvaluatorId;
                                summary.TaskFolderId = header.TaskFolderId;
                                summary.TaskOwnerDepartmentId = header.TaskOwnerDeptId;
                                summary.TaskOwnerUnitId = header.TaskOwnerUnitId;
                                summary.TaskOwnerLocationId = header.TaskOwnerLocationId;
                            }
                            int noOfReturnedTasks = 0;
                            var returnedTasks = await _deskspaceRepository.GetTaskEvaluationReturnsByTaskFolderIdAsync(summary.TaskFolderId, false);
                            if (returnedTasks != null && returnedTasks.Count > 0) { noOfReturnedTasks = returnedTasks.Count; }

                            var entity = await _deskspaceRepository.GetTaskEvaluationHeaderScoresByTaskFolderIdAndEvaluatorIdAsync(folderSubmission.FolderId, folderSubmission.ToEmployeeId);
                            if (entity == null) { return false; }
                            else
                            {
                                summary.TotalNoOfTasks = entity.TotalNumberOfTasks;
                                summary.TotalNoOfCompletedTasks = entity.NoOfCompletedTasks;
                                summary.TotalNoOfUncompletedTasks = entity.TotalNumberOfTasks - entity.NoOfCompletedTasks;
                                summary.TotalCompletionScore = entity.TotalCompletionScore;
                                summary.TotalQualityScore = entity.TotalQualityScore;
                                summary.AverageCompletionScore = decimal.Divide(entity.TotalCompletionScore, entity.TotalNumberOfTasks); //(entity.TotalCompletionScore / entity.TotalNumberOfTasks);
                                summary.AverageQualityScore = decimal.Divide(entity.TotalQualityScore, entity.TotalNumberOfTasks); //(entity.TotalQualityScore / entity.TotalNumberOfTasks);

                                summary.Id = await _deskspaceRepository.AddTaskEvaluationSummaryAsync(summary);
                                if (summary.Id < 1) { return false; }
                                else
                                {
                                    bool submissionStatusUpdated = await _deskspaceRepository.UpdateFolderSubmissionAsync(submissionId);
                                    if (submissionStatusUpdated)
                                    {
                                        WorkItemActivityLog activityLog = new WorkItemActivityLog
                                        {
                                            Time = DateTime.Now,
                                            ActivityBy = folderSubmission.ToEmployeeName,
                                            Description = $"Folder that was submitted for {folderSubmission.SubmissionType.ToString()} by {folderSubmission.FromEmployeeName}] was returned by {folderSubmission.ToEmployeeName} on [{DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} WAT. ",
                                            WorkItemFolderId = folderId,
                                            Id = 0,
                                            ProjectId = null,
                                            TaskItemId = null
                                        };
                                        await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
                else { throw new Exception("Error: Unknown submission type."); }
            }
        }

        #endregion


        #endregion

        #region Work Item Notes
        public async Task<bool> AddWorkItemNoteAsync(WorkItemNote workItemNote)
        {
            if (workItemNote == null) { throw new ArgumentNullException(nameof(workItemNote), "The required parameter [Note] is missing."); }
            return await _deskspaceRepository.AddNoteAsync(workItemNote);
        }
        public async Task<List<WorkItemNote>> GetWorkItemFolderNotesAsync(long FolderId)
        {
            List<WorkItemNote> folderNotes = new List<WorkItemNote>();
            var entities = await _deskspaceRepository.GetWorkItemNotesByFolderIdAsync(FolderId);
            if (entities != null && entities.Count > 0) { folderNotes = entities.ToList(); }
            return folderNotes;
        }

        public async Task<List<WorkItemNote>> GetTaskItemNotesAsync(long TaskId)
        {
            List<WorkItemNote> taskNotes = new List<WorkItemNote>();
            var entities = await _deskspaceRepository.GetWorkItemNotesByTaskIdAsync(TaskId);
            if (entities != null && entities.Count > 0) { taskNotes = entities.ToList(); }
            return taskNotes;
        }

        #endregion

        #region Work Item Activity Log Service Actions
        public async Task<List<WorkItemActivityLog>> GetWorkItemActivitiesByFolderIdAsync(long FolderId)
        {
            List<WorkItemActivityLog> activityLogs = new List<WorkItemActivityLog>();
            var entities = await _deskspaceRepository.GetWorkItemActivityLogByFolderIdAsync(FolderId);
            if (entities != null && entities.Count > 0) { activityLogs = entities.ToList(); }
            return activityLogs;
        }

        public async Task<List<WorkItemActivityLog>> GetWorkItemActivitiesByTaskIdAsync(long TaskId)
        {
            List<WorkItemActivityLog> activityLogs = new List<WorkItemActivityLog>();
            var entities = await _deskspaceRepository.GetWorkItemActivityLogByTaskIdAsync(TaskId);
            if (entities != null && entities.Count > 0) { activityLogs = entities.ToList(); }
            return activityLogs;
        }
        #endregion

        #region Task Items Service Methods

        #region Task Items Read Methods
        public async Task<long> GetTaskItemsCountByFolderIdAsync(long FolderId)
        {
            if (FolderId < 1) { throw new ArgumentNullException(nameof(FolderId)); }
            return await _deskspaceRepository.GetTaskItemsCountByFolderIdAsync(FolderId);
        }
        public async Task<List<TaskItem>> GetTasksByFolderIdAsync(long FolderId)
        {
            if (FolderId < 1) { throw new ArgumentNullException(nameof(FolderId)); }
            return await _deskspaceRepository.GetTaskItemsByFolderIdAsync(FolderId);
        }
        public async Task<TaskItem> GetTaskItemByIdAsync(long TaskId)
        {
            if (TaskId < 1) { throw new ArgumentNullException(nameof(TaskId)); }
            return await _deskspaceRepository.GetTaskItemByIdAsync(TaskId);
        }
        #endregion

        #region Pending Task Items
        public async Task<List<TaskItem>> GetTasksPendingAsync(string OwnerId)
        {
            if (string.IsNullOrWhiteSpace(OwnerId)) { throw new ArgumentNullException(nameof(OwnerId)); }
            return await _deskspaceRepository.GetTaskItemsPendingByOwnerIdAsync(OwnerId);
        }
        public async Task<long> GetTasksPendingCountAsync(string OwnerId)
        {
            if (string.IsNullOrWhiteSpace(OwnerId)) { throw new ArgumentNullException(nameof(OwnerId)); }
            return await _deskspaceRepository.GetTaskItemsPendingCountByOwnerIdAsync(OwnerId);
        }
        #endregion

        #region Task Items Write Methods
        public async Task<long> CreateTaskItemAsync(TaskItem taskItem)
        {
            if (taskItem == null) { throw new ArgumentNullException(nameof(taskItem), "The required parameter [TaskItem] is missing."); }

            var entities = await _deskspaceRepository.GetTaskItemsByOwnerIdnDescriptionnFolderIdAsync(taskItem.TaskOwnerId, taskItem.Description, taskItem.WorkFolderId);
            if (entities != null && entities.Count > 0 && entities[0].Id > 0)
            {
                throw new Exception("Please enter a different Task Description. You already have a Task in the same Task Folder with the same Description.");
            }
            long newTaskId = await _deskspaceRepository.AddTaskItemAsync(taskItem);
            if (newTaskId > 0)
            {
                await _utilityRepository.IncrementAutoNumberAsync("taskno");
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = taskItem.CreatedBy,
                    Description = $"New Task created by {taskItem.CreatedBy} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} WAT.",
                    WorkItemFolderId = null,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = newTaskId
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return newTaskId;
        }
        public async Task<bool> UpdateTaskItemAsync(TaskItem taskItem)
        {
            if (taskItem == null) { throw new ArgumentNullException(nameof(taskItem), "The required parameter [TaskItem] is missing."); }

            TaskItem oldTaskItem = await _deskspaceRepository.GetTaskItemByIdAsync(taskItem.Id);
            if ((taskItem.Description == oldTaskItem.Description) && (taskItem.MoreInformation == oldTaskItem.MoreInformation))
            {
                throw new Exception("This update is unnecessary as no changes were made to the original task details.");
            }

            var entities = await _deskspaceRepository.GetTaskItemsByOwnerIdnDescriptionnFolderIdAsync(taskItem.TaskOwnerId, taskItem.Description, taskItem.WorkFolderId);
            if (entities != null && entities.Count > 0 && entities[0].Id > 0 && entities[0].Id != taskItem.Id)
            {
                throw new Exception("Sorry, you may want to choose another Description. Because you already have a Task in the same Task List with this same Description.");
            }
            bool IsUpdated = await _deskspaceRepository.UpdateTaskItemAsync(taskItem);
            if (IsUpdated)
            {
                StringBuilder sb = new StringBuilder();

                if (taskItem.Description != oldTaskItem.Description)
                {
                    sb.AppendLine($"Updated Description from: [{oldTaskItem.Description}] to [{taskItem.Description}].");
                }

                if (!(string.IsNullOrWhiteSpace(taskItem.MoreInformation) && string.IsNullOrWhiteSpace(oldTaskItem.MoreInformation)))
                {
                    if (taskItem.MoreInformation != oldTaskItem.MoreInformation)
                    {
                        sb.AppendLine($"Updated More Information from: [{oldTaskItem.MoreInformation}] to [{taskItem.MoreInformation}].");
                    }
                }

                if (!(taskItem.ExpectedStartTime == null && oldTaskItem.ExpectedStartTime == null))
                {
                    if (taskItem.ExpectedStartTime != oldTaskItem.ExpectedStartTime)
                    {
                        sb.AppendLine($"Updated Expected Start Date from: [{oldTaskItem.ExpectedStartTime}] to [{taskItem.ExpectedStartTime}].");
                    }
                }

                if (!(taskItem.ExpectedDueTime == null && oldTaskItem.ExpectedDueTime == null))
                {
                    if (taskItem.ExpectedDueTime != oldTaskItem.ExpectedDueTime)
                    {
                        sb.AppendLine($"Updated Expected Due Date from: [{oldTaskItem.ExpectedDueTime}] to [{taskItem.ExpectedDueTime}].");
                    }
                }

                if (!(string.IsNullOrWhiteSpace(taskItem.LinkProjectNumber) && string.IsNullOrWhiteSpace(oldTaskItem.LinkProjectNumber)))
                {
                    if (taskItem.LinkProjectNumber != oldTaskItem.LinkProjectNumber)
                    {
                        sb.AppendLine($"Updated the associated Project Number from: [{oldTaskItem.LinkProjectNumber}] to [{taskItem.LinkProjectNumber}].");
                    }
                }

                if (!(string.IsNullOrWhiteSpace(taskItem.LinkProgramCode) && string.IsNullOrWhiteSpace(oldTaskItem.LinkProgramCode)))
                {
                    if (taskItem.LinkProgramCode != oldTaskItem.LinkProgramCode)
                    {
                        sb.AppendLine($"Updated the associated Program Code from: [{oldTaskItem.LinkProgramCode}] to [{taskItem.LinkProgramCode}].");
                    }
                }

                if (!(taskItem.LinkProgramDate == null && oldTaskItem.LinkProgramDate == null))
                {
                    if (taskItem.LinkProgramDate != oldTaskItem.LinkProgramDate)
                    {
                        sb.AppendLine($"Updated the associated Program Date from: [{oldTaskItem.LinkProgramDate}] to [{taskItem.LinkProgramDate}].");
                    }
                }

                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = taskItem.LastModifiedBy,
                    Description = sb.ToString(),
                    WorkItemFolderId = null,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = taskItem.Id,
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsUpdated;
        }
        public async Task<bool> DeleteTaskItemAsync(long taskItemId)
        {
            if (taskItemId < 1) { throw new ArgumentNullException(nameof(taskItemId), "The required parameter [Task Item ID] is missing."); }
            return await _deskspaceRepository.DeleteTaskItemAsync(taskItemId);
        }
        public async Task<bool> UpdateTaskItemResolutionAsync(long taskItemId, string taskResolution, string updatedBy)
        {
            if (taskItemId < 1 || string.IsNullOrWhiteSpace(updatedBy))
            {
                throw new Exception("Missing Parameters Error. Sorry, the request could not be processed because some key parameters needed to process it are missing.");
            }

            bool IsUpdated = await _deskspaceRepository.UpdateTaskItemResolutionAsync(taskItemId, taskResolution, updatedBy);
            if (IsUpdated)
            {
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = updatedBy,
                    Description = $"Task Item Resolution was updated by {updatedBy} on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} WAT.",
                    WorkItemFolderId = null,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = taskItemId,
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsUpdated;
        }
        public async Task<bool> UpdateTaskItemProgressAsync(long taskItemId, int newProgressStatusId, string previousProgressStatus, string updatedBy)
        {
            if (taskItemId < 1 || newProgressStatusId < 0 || string.IsNullOrWhiteSpace(previousProgressStatus) || string.IsNullOrWhiteSpace(updatedBy))
            {
                throw new Exception("Missing Parameters Error. Sorry, the request could not be processed because some key parameters needed to process it are missing.");
            }

            WorkItemProgressStatus newStatus = (WorkItemProgressStatus)newProgressStatusId;
            string newStatusDescription = string.Empty;
            switch (newStatus)
            {
                case WorkItemProgressStatus.NotStarted:
                    newStatusDescription = "Not Started";
                    break;
                case WorkItemProgressStatus.InProgress:
                    newStatusDescription = "In Progress";
                    break;
                case WorkItemProgressStatus.Completed:
                    newStatusDescription = "Completed";
                    break;
                case WorkItemProgressStatus.OnHold:
                    newStatusDescription = "On Hold";
                    break;
                default:
                    break;
            }
            bool IsUpdated = await _deskspaceRepository.UpdateTaskItemProgressStatusAsync(taskItemId, newProgressStatusId, updatedBy);
            if (IsUpdated)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Progress Status was updated from [{previousProgressStatus}] to [{newStatusDescription}] by {updatedBy}.");

                if (newStatus == WorkItemProgressStatus.NotStarted)
                {
                    if (await _deskspaceRepository.UpdateTaskItemActualStartDateAsync(taskItemId, null, updatedBy))
                    {
                        sb.AppendLine($"Actual Start Date was reset to [N/A] by {updatedBy}.");
                    }
                }

                if (newStatus == WorkItemProgressStatus.InProgress)
                {
                    if (await _deskspaceRepository.UpdateTaskItemActualStartDateAsync(taskItemId, DateTime.Now, updatedBy))
                    {
                        sb.AppendLine($"Actual Start Date was updated to [{DateTime.Now.ToString("dd-MMM-yyyy")}] by {updatedBy}.");
                    }
                }

                if (newStatus == WorkItemProgressStatus.Completed)
                {
                    TaskItem item = await _deskspaceRepository.GetTaskItemByIdAsync(taskItemId);
                    if (item != null && item.ActualStartTime == null)
                    {
                        if (await _deskspaceRepository.UpdateTaskItemActualStartDateAsync(taskItemId, DateTime.Now, updatedBy))
                        {
                            sb.AppendLine($"Actual Start Date was updated to [{DateTime.Now.ToString("dd-MMM-yyyy")}] by {updatedBy}.");
                        }
                    }

                    if (await _deskspaceRepository.UpdateTaskItemActualDueDateAsync(taskItemId, DateTime.Now, updatedBy))
                    {
                        sb.AppendLine($"Actual Due Date was updated to [{DateTime.Now.ToString("dd-MMM-yyyy")}] by {updatedBy}.");
                    }
                }

                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = updatedBy,
                    Description = sb.ToString(),
                    WorkItemFolderId = null,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = taskItemId,
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsUpdated;
        }
        public async Task<bool> UpdateTaskItemOpenCloseStatusAsync(long taskItemId, WorkItemStatus newStatus, string updatedBy)
        {
            string newStatusDescription = string.Empty;
            string previousStatusDescription = string.Empty;
            bool isClosed = false;
            switch (newStatus)
            {
                case WorkItemStatus.Closed:
                    isClosed = true;
                    newStatusDescription = "Closed";
                    previousStatusDescription = "Open";
                    break;
                case WorkItemStatus.Open:
                    isClosed = false;
                    newStatusDescription = "Open";
                    previousStatusDescription = "Closed";
                    break;
                default:
                    break;
            }
            bool IsUpdated = await _deskspaceRepository.UpdateTaskItemOpenCloseStatusAsync(taskItemId, isClosed, updatedBy);
            if (IsUpdated)
            {
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = updatedBy,
                    Description = $"Task Status was updated from [{previousStatusDescription}] to [{newStatusDescription}] by {updatedBy}.",
                    WorkItemFolderId = null,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = taskItemId,
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsUpdated;
        }
        public async Task<bool> UpdateTaskItemOpenCloseStatusByFolderIdAsync(long taskFolderId, WorkItemStatus newStatus, string updatedBy)
        {
            string newStatusDescription = string.Empty;
            string previousStatusDescription = string.Empty;
            bool IsClosed = false;
            switch (newStatus)
            {
                case WorkItemStatus.Closed:
                    IsClosed = true;
                    newStatusDescription = "Closed";
                    previousStatusDescription = "Open";
                    break;
                case WorkItemStatus.Open:
                    IsClosed = false;
                    newStatusDescription = "Open";
                    previousStatusDescription = "Closed";
                    break;
                default:
                    break;
            }
            bool IsUpdated = await _deskspaceRepository.UpdateTaskItemOpenCloseStatusByFolderIdAsync(taskFolderId, IsClosed, updatedBy);
            if (IsUpdated)
            {
                List<TaskItem> taskItems = await _deskspaceRepository.GetTaskItemsByFolderIdAsync(taskFolderId);
                foreach (var item in taskItems)
                {
                    WorkItemActivityLog activityLog = new WorkItemActivityLog
                    {
                        Time = DateTime.Now,
                        ActivityBy = updatedBy,
                        Description = $"Task Status was updated from [{previousStatusDescription}] to [{newStatusDescription}].",
                        WorkItemFolderId = null,
                        Id = 0,
                        ProjectId = null,
                        TaskItemId = item.Id,
                    };
                    await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
                }
            }
            return IsUpdated;
        }
        public async Task<bool> UpdateTaskItemApprovalStatusAsync(long taskItemId, ApprovalStatus newApprovalStatus, string updatedBy)
        {
            string newStatusDescription = string.Empty;
            string previousStatusDescription = string.Empty;
            switch (newApprovalStatus)
            {
                case ApprovalStatus.Approved:
                    newStatusDescription = "Approved";
                    break;
                case ApprovalStatus.Declined:
                    newStatusDescription = "Declined";
                    break;
                case ApprovalStatus.Pending:
                    newStatusDescription = "Pending";
                    break;
                default:
                    break;
            }
            bool IsUpdated = await _deskspaceRepository.UpdateTaskItemApprovalStatusAsync(taskItemId, newApprovalStatus, updatedBy);
            if (IsUpdated)
            {
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = updatedBy,
                    Description = $"Task Approval Status was updated to [{newStatusDescription}] by [{updatedBy}] on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT.",
                    WorkItemFolderId = null,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = taskItemId,
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsUpdated;
        }
        public async Task<bool> UpdateTaskItemFolderIdAsync(long taskItemId, string updatedBy, long? taskFolderId = null)
        {
            if (taskItemId < 1 || string.IsNullOrWhiteSpace(updatedBy))
            {
                throw new Exception("Missing Parameters Error. Sorry, the request could not be processed because some key parameters needed to process it are missing.");
            }

            bool IsUpdated = await _deskspaceRepository.UpdateTaskItemFolderIdAsync(taskItemId, updatedBy, taskFolderId);
            if (IsUpdated)
            {
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = updatedBy,
                    Description = $"Task Item FolderId was updated by {updatedBy} on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} WAT.",
                    WorkItemFolderId = null,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = taskItemId,
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsUpdated;
        }
        public async Task<bool> LinkTaskItemToProgramAsync(long taskItemId, string programCode, DateTime? programDate, string updatedBy)
        {
            if (taskItemId < 1 || string.IsNullOrWhiteSpace(programCode) || string.IsNullOrWhiteSpace(updatedBy))
            {
                throw new Exception("Missing Parameters Error. Sorry, the request could not be processed because some key parameters needed to process it are missing.");
            }

            bool IsUpdated = await _deskspaceRepository.UpdateTaskItemProgramLinkAsync(taskItemId, programCode, programDate, updatedBy);
            if (IsUpdated)
            {
                Programme programme = new Programme();
                programme = await _programRepository.GetByCodeAsync(programCode);
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = updatedBy,
                    Description = $"Task was successfully linked to the program [{programme.Title}] by [{updatedBy}] on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT.",
                    WorkItemFolderId = null,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = taskItemId,
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsUpdated;
        }
        public async Task<bool> LinkTaskItemToProjectAsync(long taskItemId, string projectCode, string updatedBy, string projectTitle)
        {
            if (taskItemId < 1 || string.IsNullOrWhiteSpace(projectCode) || string.IsNullOrWhiteSpace(updatedBy))
            {
                throw new Exception("Missing Parameters Error. Sorry, the request could not be processed because some key parameters needed to process it are missing.");
            }
            bool IsUpdated = await _deskspaceRepository.UpdateTaskItemProjectLinkAsync(taskItemId, projectCode, updatedBy);
            if (IsUpdated)
            {
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = updatedBy,
                    Description = $"Task was successfully linked to the project [{projectTitle}] by [{updatedBy}] on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} WAT.",
                    WorkItemFolderId = null,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = taskItemId,
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsUpdated;
        }
        public async Task<bool> ReassignTaskItemAsync(long taskItemId, int newTaskListId, string newTaskOwnerId, string oldTaskOwnerName, string reassignedBy)
        {
            Employee newTaskOwner = new Employee();
            int? newUnitId = null;
            int? newDeptId = null;
            int? newLocationId = null;
            string newTaskOwnerName = string.Empty;

            newTaskOwner = await _employeesRepository.GetEmployeeByIdAsync(newTaskOwnerId);
            if (newTaskOwner != null)
            {
                newUnitId = newTaskOwner.UnitID;
                newDeptId = newTaskOwner.DepartmentID;
                newLocationId = newTaskOwner.LocationID;
                newTaskOwnerName = newTaskOwner.FullName;
            }

            bool IsUpdated = await _deskspaceRepository.UpdateTaskItemOwnerAsync(taskItemId, newTaskListId, newTaskOwnerId, newUnitId.Value, newDeptId.Value, newLocationId.Value, reassignedBy);
            if (IsUpdated)
            {
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = reassignedBy,
                    Description = $"Task was re-assigned from [{oldTaskOwnerName}] to [{newTaskOwnerName}] by {reassignedBy} on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} WATT.",
                    WorkItemFolderId = null,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = taskItemId,
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsUpdated;
        }
        #endregion

        #region Task Item Timeline Changes Service Methods
        public async Task<bool> UpdateTaskTimelineAsync(TaskTimelineChange taskTimelineChange)
        {
            if (taskTimelineChange == null) { throw new ArgumentNullException(nameof(taskTimelineChange)); }

            bool IsUpdated = await _deskspaceRepository.UpdateTaskItemTimelineAsync(taskTimelineChange.TaskItemId, taskTimelineChange.ModifiedBy, taskTimelineChange.PreviousStartDate, taskTimelineChange.NewStartDate, taskTimelineChange.PreviousEndDate, taskTimelineChange.NewEndDate);
            if (IsUpdated)
            {
                bool IsAdded = await _deskspaceRepository.AddTaskTimelineChangeAsync(taskTimelineChange);
                if (IsAdded)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"Task was rescheduled as follows: Expected Start Date was changed from [{ taskTimelineChange.PreviousStartDate}] to [{ taskTimelineChange.NewStartDate}], ");
                    sb.Append($"while Expected Due Date was changed from [{ taskTimelineChange.PreviousEndDate}] to [{ taskTimelineChange.NewEndDate}]. ");
                    sb.Append("And as a result the previous approval was revoked. ");

                    WorkItemActivityLog activityLog = new WorkItemActivityLog
                    {
                        Time = DateTime.Now,
                        ActivityBy = taskTimelineChange.ModifiedBy,
                        Description = sb.ToString(),
                        WorkItemFolderId = null,
                        Id = 0,
                        ProjectId = null,
                        TaskItemId = taskTimelineChange.TaskItemId
                    };
                    await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
                }
            }
            return IsUpdated;
        }
        public async Task<List<TaskTimelineChange>> GetTaskTimelineChangesByTaskItemIdAsync(long taskItemId)
        {
            List<TaskTimelineChange> taskTimelineChanges = new List<TaskTimelineChange>();
            if (taskItemId > 0)
            {
                var entities = await _deskspaceRepository.GetTaskTimelineByTaskItemIdAsync(taskItemId);
                if (entities != null) { taskTimelineChanges = entities.ToList(); }
            }
            return taskTimelineChanges;
        }
        #endregion

        #endregion

        #region Task Evaluation Header Service Action Methods
        public async Task<TaskEvaluationHeader> GetTaskEvaluationHeaderAsync(long taskEvaluationHeaderId)
        {
            TaskEvaluationHeader hdr = new TaskEvaluationHeader();
            var entities = await _deskspaceRepository.GetTaskEvaluationHeadersByIdAsync(taskEvaluationHeaderId);
            if (entities != null && entities.Count > 0)
            {
                hdr = entities.FirstOrDefault();
            }
            return hdr;
        }
        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByTaskOwnerIdAsync(string taskOwnerId, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            List<TaskEvaluationHeader> headers = new List<TaskEvaluationHeader>();
            if (string.IsNullOrWhiteSpace(taskOwnerId)) { return headers; }
            var entities = await _deskspaceRepository.GetTaskEvaluationHeadersByTaskOwnerIdAsync(taskOwnerId, FromDate, ToDate);
            if (entities != null && entities.Count > 0)
            {
                headers = entities;
            }
            return headers;
        }
        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByReportsToEmployeeIdAsync(string reportsToEmployeeId, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            List<TaskEvaluationHeader> headers = new List<TaskEvaluationHeader>();
            if (string.IsNullOrWhiteSpace(reportsToEmployeeId)) { return headers; }
            var entities = await _deskspaceRepository.GetTaskEvaluationHeadersByReportsToEmployeeIdAsync(reportsToEmployeeId, FromDate, ToDate);
            if (entities != null && entities.Count > 0)
            {
                headers = entities;
            }
            return headers;
        }

        public async Task<TaskEvaluationHeader> GetTaskEvaluationHeaderAsync(long taskFolderId, string evaluatorId)
        {
            TaskEvaluationHeader hdr = new TaskEvaluationHeader();
            var entities = await _deskspaceRepository.GetTaskEvaluationHeadersByTaskFolderIdAndEvaluatorIdAsync(taskFolderId, evaluatorId);
            if (entities != null && entities.Count > 0)
            {
                hdr = entities.FirstOrDefault();
            }
            return hdr;
        }
        public async Task<List<TaskEvaluationHeader>> SearchTaskEvaluationHeaderAsync(DateTime FromDate, DateTime? ToDate, int? unitId = null, int? deptId = null, int? locationId = null)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaderList = new List<TaskEvaluationHeader>();
            if (locationId != null && locationId > 0)
            {
                if (unitId != null && unitId > 0)
                {
                    if (ToDate == null || ToDate < FromDate) { ToDate = DateTime.Now; }
                    taskEvaluationHeaderList = await _deskspaceRepository.GetTaskEvaluationHeadersByUnitIdAndLocationIdAsync(unitId.Value, locationId.Value, FromDate, ToDate);
                }
                else if (deptId != null && deptId > 0)
                {
                    if (ToDate == null || ToDate < FromDate) { ToDate = DateTime.Now; }
                    taskEvaluationHeaderList = await _deskspaceRepository.GetTaskEvaluationHeadersByDepartmentIdAndLocationIdAsync(deptId.Value, locationId.Value, FromDate, ToDate);
                }
                else
                {
                    if (ToDate == null || ToDate < FromDate) { ToDate = DateTime.Now; }
                    taskEvaluationHeaderList = await _deskspaceRepository.GetTaskEvaluationHeadersByLocationIdAsync(locationId.Value, FromDate, ToDate);
                }
            }
            else
            {
                if (unitId != null && unitId > 0)
                {
                    if (ToDate == null || ToDate < FromDate) { ToDate = DateTime.Now; }
                    taskEvaluationHeaderList = await _deskspaceRepository.GetTaskEvaluationHeadersByUnitIdAsync(unitId.Value, FromDate, ToDate);
                }
                else if (deptId != null && deptId > 0)
                {
                    if (ToDate == null || ToDate < FromDate) { ToDate = DateTime.Now; }
                    taskEvaluationHeaderList = await _deskspaceRepository.GetTaskEvaluationHeadersByDepartmentIdAsync(deptId.Value, FromDate, ToDate);
                }
                else
                {
                    if (ToDate == null || ToDate < FromDate) { ToDate = DateTime.Now; }
                    taskEvaluationHeaderList = await _deskspaceRepository.GetTaskEvaluationHeadersAsync(FromDate, ToDate);
                }
            }
            return taskEvaluationHeaderList;
        }
        public async Task<long> CreateTaskEvaluationHeaderAsync(TaskEvaluationHeader header)
        {
            if (header == null) { throw new ArgumentNullException(nameof(header), "The required parameter [Task Evaluation Header] is missing."); }
            long headerId = 0;

            var entities = await _deskspaceRepository.GetTaskEvaluationHeadersByTaskFolderIdAndEvaluatorIdAsync(header.TaskFolderId, header.EvaluatorId);
            if (entities != null && entities.Count > 0 && entities[0].Id > 0)
            {
                headerId = entities[0].Id;
            }
            else
            {
                headerId = await _deskspaceRepository.AddTaskEvaluationHeaderAsync(header);
            }
            return headerId;
        }
        public async Task<bool> UpdateTaskEvaluationHeaderAsync(TaskEvaluationHeader header)
        {
            if (header == null) { throw new ArgumentNullException(nameof(header), "The required parameter [Task Evaluation Header] is missing."); }
            return await _deskspaceRepository.UpdateTaskEvaluationHeaderAsync(header);
        }
        #endregion

        #region Task Evaluation Detail Service Action Methods
        public async Task<TaskEvaluationDetail> GetTaskEvaluationDetailAsync(long taskEvaluationHeaderId, long taskItemId)
        {
            TaskEvaluationDetail detail = new TaskEvaluationDetail();
            var entities = await _deskspaceRepository.GetTaskEvaluationDetailsByTaskEvaluationHeaderIdAndTaskItemIdAsync(taskEvaluationHeaderId, taskItemId);
            if (entities != null && entities.Count > 0)
            {
                detail = entities.FirstOrDefault();
            }
            return detail;
        }
        public async Task<TaskEvaluationDetail> GetTaskEvaluationDetailByIdAsync(long taskEvaluationDetailId)
        {
            TaskEvaluationDetail detail = new TaskEvaluationDetail();
            return await _deskspaceRepository.GetTaskEvaluationDetailByTaskEvaluationDetailIdAsync(taskEvaluationDetailId);
        }
        public async Task<List<TaskEvaluationDetail>> GetTaskEvaluationDetailsAsync(long taskEvaluationHeaderId)
        {
            List<TaskEvaluationDetail> details = new List<TaskEvaluationDetail>();
            var entities = await _deskspaceRepository.GetTaskEvaluationDetailsByTaskEvaluationHeaderIdAsync(taskEvaluationHeaderId);
            if (entities != null && entities.Count > 0)
            {
                details = entities;
            }
            return details;
        }
        public async Task<bool> AddTaskEvaluationDetailAsync(TaskEvaluationDetail detail)
        {
            if (detail == null) { throw new ArgumentNullException(nameof(detail), "The required parameter [Task Evaluation Detail] is missing."); }
            var entities = await _deskspaceRepository.GetTaskEvaluationDetailsByTaskEvaluationHeaderIdAndTaskItemIdAsync(detail.TaskEvaluationHeaderId, detail.TaskItemId);
            if (entities != null && entities.Count > 0 && entities[0].TaskEvaluationDetailId > 0)
            {
                bool IsUpdated = await _deskspaceRepository.UpdateTaskEvaluationDetailsAsync(detail);
                if (IsUpdated)
                {
                    if (detail.CompletionScore == 100)
                    {
                        await _deskspaceRepository.UpdateTaskItemCompletionStatusAsync(detail.TaskItemId, true, detail.TaskEvaluatorName);
                    }
                    WorkItemActivityLog activityLog = new WorkItemActivityLog
                    {
                        Time = DateTime.Now,
                        ActivityBy = detail.TaskEvaluatorName,
                        Description = $"Task was re-evaluated by [{detail.TaskEvaluatorName}] on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} WAT.",
                        WorkItemFolderId = null,
                        Id = 0,
                        ProjectId = null,
                        TaskItemId = detail.TaskItemId
                    };
                    await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
                    return true;
                }
            }
            else
            {
                bool IsAdded = await _deskspaceRepository.AddTaskEvaluationDetailsAsync(detail);
                if (IsAdded)
                {
                    if (detail.CompletionScore == 100)
                    {
                        await _deskspaceRepository.UpdateTaskItemCompletionStatusAsync(detail.TaskItemId, true, detail.TaskEvaluatorName);
                    }

                    WorkItemActivityLog activityLog = new WorkItemActivityLog
                    {
                        Time = DateTime.Now,
                        ActivityBy = detail.TaskEvaluatorName,
                        Description = $"Task was evaluated by [{detail.TaskEvaluatorName}] on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} WAT.",
                        WorkItemFolderId = null,
                        Id = 0,
                        ProjectId = null,
                        TaskItemId = detail.TaskItemId
                    };
                    await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> UpdateTaskEvaluationDetailAsync(TaskEvaluationDetail detail)
        {
            if (detail == null) { throw new ArgumentNullException(nameof(detail), "The required parameter [Task Evaluation Detail] is missing."); }

            return await _deskspaceRepository.UpdateTaskEvaluationDetailsAsync(detail);
        }
        public async Task<long> GetEvaluatedTaskItemsCountAsync(long taskFolderId, string evaluatorId)
        {
            return await _deskspaceRepository.GetTaskEvaluationItemsCountByFolderIdnEvaluatorIdAsync(taskFolderId, evaluatorId);
        }

        #endregion

        #region Task Evaluation Summary Service Action Methods
        public async Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryAsync(DateTime? FromDate = null, DateTime? ToDate = null, int? unitId = null, int? deptId = null, int? locationId = null)
        {
            List<TaskEvaluationSummary> taskEvaluationSummaryList = new List<TaskEvaluationSummary>();
            DateTime _fromDate = FromDate ?? DateTime.Now.AddMonths(-3);
            DateTime _toDate = ToDate ?? DateTime.Now;
            if (locationId != null && locationId > 0)
            {
                if (unitId != null && unitId > 0)
                {
                    taskEvaluationSummaryList = await _deskspaceRepository.GetTaskEvaluationSummaryByUnitIdAndLocationIdAsync(unitId.Value, locationId.Value, _fromDate, _toDate);
                }
                else if (deptId != null && deptId > 0)
                {
                    taskEvaluationSummaryList = await _deskspaceRepository.GetTaskEvaluationSummaryByDepartmentIdAndLocationIdAsync(deptId.Value, locationId.Value, _fromDate, _toDate);
                }
                else
                {
                    taskEvaluationSummaryList = await _deskspaceRepository.GetTaskEvaluationSummaryByLocationIdAsync(locationId.Value, _fromDate, _toDate);
                }
            }
            else
            {
                if (unitId != null && unitId > 0)
                {
                    taskEvaluationSummaryList = await _deskspaceRepository.GetTaskEvaluationSummaryByUnitIdAsync(unitId.Value, _fromDate, _toDate);
                }
                else if (deptId != null && deptId > 0)
                {
                    taskEvaluationSummaryList = await _deskspaceRepository.GetTaskEvaluationSummaryByDepartmentIdAsync(deptId.Value, _fromDate, _toDate);
                }
                else
                {
                    taskEvaluationSummaryList = await _deskspaceRepository.GetTaskEvaluationSummaryAsync(_fromDate, _toDate);
                }
            }
            return taskEvaluationSummaryList;
        }
        public async Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByTaskOwnerIdAsync(string TaskOwnerId, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            return await _deskspaceRepository.GetTaskEvaluationSummaryByTaskOwnerIdAsync(TaskOwnerId, FromDate, ToDate);
        }
        #endregion

        #region Task Evaluation Returns
        public async Task<bool> ReturnTaskEvaluationAsync(TaskEvaluationReturns evaluationReturn)
        {
            if (evaluationReturn == null) { throw new ArgumentNullException(nameof(evaluationReturn), "The required parameter [Task Evaluation Return] is missing."); }
            bool IsAdded = await _deskspaceRepository.AddTaskEvaluationReturnAsync(evaluationReturn);
            if (IsAdded)
            {
                bool FolderIdIsUpdated = await _deskspaceRepository.UpdateTaskItemFolderIdAsync(evaluationReturn.TaskItemId, evaluationReturn.ReturnedBy, null);
                if (FolderIdIsUpdated)
                {
                    WorkItemActivityLog activityLog = new WorkItemActivityLog
                    {
                        Time = DateTime.Now,
                        ActivityBy = evaluationReturn.ReturnedBy,
                        Description = $"Task failed evaluation and was returned by [{evaluationReturn.ReturnedBy}] on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} WAT.",
                        WorkItemFolderId = null,
                        Id = 0,
                        ProjectId = null,
                        TaskItemId = evaluationReturn.TaskItemId
                    };
                    await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
                    return true;
                }
                else
                {
                    await _deskspaceRepository.DeleteTaskEvaluationReturnAsync(evaluationReturn.TaskItemId, evaluationReturn.ReturnedBy);
                }
            }
            return IsAdded;
        }
        public async Task<bool> DeleteTaskEvaluationReturnAsync(long taskItemId, string returnedBy)
        {
            if (taskItemId < 1) { throw new ArgumentNullException(nameof(taskItemId), "The required parameter [Task Item ID] is missing."); }
            if (string.IsNullOrWhiteSpace(returnedBy)) { throw new ArgumentNullException(nameof(returnedBy), "The required parameter [Returned By] is missing."); }
            bool IsDeleted = await _deskspaceRepository.DeleteTaskEvaluationReturnAsync(taskItemId, returnedBy);
            if (IsDeleted)
            {
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = returnedBy,
                    Description = $"Task Evaluation return was reversed and deleted by [{returnedBy}] on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} WAT.",
                    WorkItemFolderId = null,
                    Id = 0,
                    ProjectId = null,
                    TaskItemId = taskItemId
                };
                await _deskspaceRepository.AddWorkItemActivityLogAsync(activityLog);
            }
            return IsDeleted;
        }
        public async Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsAsync(string taskOwnerId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationReturns> evaluationReturns = new List<TaskEvaluationReturns>();
            var entities = await _deskspaceRepository.GetTaskEvaluationReturnsByTaskOwnerIdAsync(taskOwnerId, fromDate, toDate);
            if (entities != null && entities.Count > 0) { evaluationReturns = entities; }
            return evaluationReturns;
        }
        public async Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskItemIdAsync(long taskItemId)
        {
            List<TaskEvaluationReturns> evaluationReturns = new List<TaskEvaluationReturns>();
            var entities = await _deskspaceRepository.GetTaskEvaluationReturnsByTaskItemIdAsync(taskItemId);
            if (entities != null && entities.Count > 0) { evaluationReturns = entities; }
            return evaluationReturns;
        }
        public async Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskFolderIdAsync(long taskFolderId, bool? IsExemptedFromEvaluation = null)
        {
            List<TaskEvaluationReturns> evaluationReturns = new List<TaskEvaluationReturns>();
            if (IsExemptedFromEvaluation == null)
            {
                var entities = await _deskspaceRepository.GetTaskEvaluationReturnsByTaskFolderIdAsync(taskFolderId);
                if (entities != null && entities.Count > 0) { evaluationReturns = entities; }
            }
            else
            {
                var entities = await _deskspaceRepository.GetTaskEvaluationReturnsByTaskFolderIdAsync(taskFolderId, IsExemptedFromEvaluation.Value);
                if (entities != null && entities.Count > 0) { evaluationReturns = entities; }
            }
            return evaluationReturns;
        }
        public async Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskEvaluationHeaderIdAsync(long taskEvaluationHeaderId, bool? IsExemptedFromEvaluation = null)
        {
            List<TaskEvaluationReturns> evaluationReturns = new List<TaskEvaluationReturns>();
            if (IsExemptedFromEvaluation == null)
            {
                var entities = await _deskspaceRepository.GetTaskEvaluationReturnsByTaskEvaluationHeaderIdAsync(taskEvaluationHeaderId);
                if (entities != null && entities.Count > 0) { evaluationReturns = entities; }
            }
            else
            {
                var entities = await _deskspaceRepository.GetTaskEvaluationReturnsByTaskEvaluationHeaderIdAsync(taskEvaluationHeaderId, IsExemptedFromEvaluation.Value);
                if (entities != null && entities.Count > 0) { evaluationReturns = entities; }
            }
            return evaluationReturns;
        }
        #endregion

        #region Task Item Evaluation Service Action Methods
        public async Task<List<TaskItemEvaluation>> GetTaskItemEvaluationsAsync(long taskFolderId, string evaluatorId = null)
        {
            List<TaskItemEvaluation> itemEvaluations = new List<TaskItemEvaluation>();
            if (!string.IsNullOrWhiteSpace(evaluatorId))
            {
                var entities = await _deskspaceRepository.GetTaskItemEvaluationsByTaskFolderIdAndEvaluatorIdAsync(taskFolderId, evaluatorId);
                if (entities != null && entities.Count > 0)
                {
                    itemEvaluations = entities;
                }
            }
            else
            {
                var entities = await _deskspaceRepository.GetTaskItemEvaluationsByTaskFolderIdAsync(taskFolderId);
                if (entities != null && entities.Count > 0)
                {
                    itemEvaluations = entities;
                }
            }
            return itemEvaluations;
        }
        #endregion

        #region Task Evaluation Scores Service Methods
        public async Task<TaskEvaluationScores> GetTaskEvaluationScoresByOwnerId(string TaskOwnerId, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return await _deskspaceRepository.GetTaskEvaluationScoresByTaskOwnerIdAsync(TaskOwnerId, StartDate, EndDate);
        }

        public async Task<List<TaskEvaluationScores>> GetTaskEvaluationScoresAsync(string TaskOwnerName = null, int? UnitId = null, int? DepartmentId = null, int? LocationId = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            List<TaskEvaluationScores> _scoresList = new List<TaskEvaluationScores>();
            if (!string.IsNullOrWhiteSpace(TaskOwnerName))
            {
                var entity = await _deskspaceRepository.GetTaskEvaluationScoresByTaskOwnerNameAsync(TaskOwnerName, StartDate, EndDate);
                if (entity != null)
                {
                    _scoresList.Add(entity);
                }
            }
            else
            {
                if (UnitId != null)
                {
                    if (LocationId != null)
                    {
                        _scoresList = await _deskspaceRepository.GetTaskEvaluationScoresByUnitIdnLocationIdAsync(LocationId.Value, UnitId.Value, StartDate, EndDate);
                    }
                    else
                    {
                        _scoresList = await _deskspaceRepository.GetTaskEvaluationScoresByUnitIdAsync(UnitId.Value, StartDate, EndDate);
                    }
                }
                else if (DepartmentId != null)
                {
                    if (LocationId != null)
                    {
                        _scoresList = await _deskspaceRepository.GetTaskEvaluationScoresByDepartmentIdnLocationIdAsync(LocationId.Value, DepartmentId.Value, StartDate, EndDate);
                    }
                    else
                    {
                        _scoresList = await _deskspaceRepository.GetTaskEvaluationScoresByDepartmentIdAsync(DepartmentId.Value, StartDate, EndDate);
                    }
                }
                else if (LocationId != null)
                {
                    _scoresList = await _deskspaceRepository.GetTaskEvaluationScoresByLocationIdAsync(UnitId.Value, StartDate, EndDate);
                }
            }
            return _scoresList;
        }





        #endregion

        #region Work Item Return Reasons Service Methods
        public async Task<List<WorkItemReturnReason>> GetWorkItemReturnReasonsAsync()
        {
            List<WorkItemReturnReason> _returnReasons = new List<WorkItemReturnReason>();
            var entities = await _deskspaceRepository.GetWorkItemReturnReasonsAsync();
            if (entities != null && entities.Count > 0) { _returnReasons = entities; }
            return _returnReasons;
        }
        #endregion
    }
}
