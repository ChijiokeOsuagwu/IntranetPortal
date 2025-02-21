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
        public async Task<List<WorkItemFolder>> GetFoldersByOwnerIdAsync(string OwnerId, bool? IsArchived = null)
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

        //============== Work Item Folder Write Service Methods ================================//
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
        public async Task<List<TaskItem>> GetTasksByFolderIdAsync(long FolderId)
        {
            if (FolderId < 1) { throw new ArgumentNullException(nameof(FolderId)); }
            return await _deskspaceRepository.GetTaskItemsByFolderIdAsync(FolderId);
        }
        public async Task<TaskItem> GetTaskById(long TaskId)
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

                if (taskItem.MoreInformation != oldTaskItem.MoreInformation)
                {
                    sb.AppendLine($"Updated More Information from: [{oldTaskItem.MoreInformation}] to [{taskItem.MoreInformation}].");
                }

                if (taskItem.ExpectedStartTime != oldTaskItem.ExpectedStartTime)
                {
                    sb.AppendLine($"Updated Expected Start Date from: [{oldTaskItem.ExpectedStartTime}] to [{taskItem.ExpectedStartTime}].");
                }

                if (taskItem.ExpectedDueTime != oldTaskItem.ExpectedDueTime)
                {
                    sb.AppendLine($"Updated Expected Due Date from: [{oldTaskItem.ExpectedDueTime}] to [{taskItem.ExpectedDueTime}].");
                }

                if (taskItem.LinkProjectNumber != oldTaskItem.LinkProjectNumber)
                {
                    sb.AppendLine($"Updated the associated Project Number from: [{oldTaskItem.LinkProjectNumber}] to [{taskItem.LinkProjectNumber}].");
                }

                if (taskItem.LinkProgramCode != oldTaskItem.LinkProgramCode)
                {
                    sb.AppendLine($"Updated the associated Program Code from: [{oldTaskItem.LinkProgramCode}] to [{taskItem.LinkProgramCode}].");
                }

                if (taskItem.LinkProgramDate != oldTaskItem.LinkProgramDate)
                {
                    sb.AppendLine($"Updated the associated Program Date from: [{oldTaskItem.LinkProgramDate}] to [{taskItem.LinkProgramDate}].");
                }

                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = taskItem.CreatedBy,
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
                WorkItemActivityLog activityLog = new WorkItemActivityLog
                {
                    Time = DateTime.Now,
                    ActivityBy = updatedBy,
                    Description = $"Progress Status was updated from {previousProgressStatus} to {newStatusDescription}.",
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
                    Description = $"Task Status was updated from [{previousStatusDescription}] to [{newStatusDescription}].",
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

        #endregion
    }
}
