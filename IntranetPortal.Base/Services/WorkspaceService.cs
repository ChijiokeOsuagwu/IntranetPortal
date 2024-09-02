using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.ErmRepositories;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using IntranetPortal.Base.Repositories.WksRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IProjectFolderRepository _projectFolderRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUtilityRepository _utilityRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ITaskListRepository _taskListRepository;
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly ITaskTimelineRepository _taskTimelineRepository;
        private readonly ITaskSubmissionRepository _taskSubmissionRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ITaskEvaluationHeaderRepository _taskEvaluationHeaderRepository;
        private readonly ITaskEvaluationDetailRepository _taskEvaluationDetailRepository;
        private readonly ITaskItemEvaluationRepository _taskItemEvaluationRepository;

        public WorkspaceService(IWorkspaceRepository workspaceRepository, IProjectFolderRepository projectFolderRepository,
                                IProjectRepository projectRepository, IUtilityRepository utilityRepository,
                                ITaskListRepository taskListRepository, ITaskItemRepository taskItemRepository,
                                ITaskTimelineRepository taskTimelineRepository, ITaskSubmissionRepository taskSubmissionRepository,
                                IProgramRepository programRepository, ITaskEvaluationHeaderRepository taskEvaluationHeaderRepository,
                                ITaskEvaluationDetailRepository taskEvaluationDetailRepository, ITaskItemEvaluationRepository taskItemEvaluationRepository,
                                IEmployeesRepository employeesRepository)
        {
            _workspaceRepository = workspaceRepository;
            _projectFolderRepository = projectFolderRepository;
            _projectRepository = projectRepository;
            _utilityRepository = utilityRepository;
            _taskListRepository = taskListRepository;
            _taskItemRepository = taskItemRepository;
            _taskTimelineRepository = taskTimelineRepository;
            _taskSubmissionRepository = taskSubmissionRepository;
            _programRepository = programRepository;
            _taskEvaluationHeaderRepository = taskEvaluationHeaderRepository;
            _taskEvaluationDetailRepository = taskEvaluationDetailRepository;
            _taskItemEvaluationRepository = taskItemEvaluationRepository;
            _employeesRepository = employeesRepository;
        }

        #region Workspace Service Actions
        public async Task<bool> CreateWorkspaceAsync(Workspace workspace)
        {
            if (workspace == null) { throw new ArgumentNullException(nameof(workspace), "The required parameter [Workspace] is missing."); }
            try
            {
                var entities = await _workspaceRepository.GetByOwnerIdAndTitleAsync(workspace.OwnerID, workspace.Title);
                if (entities != null && entities.Count > 0 && entities[0].ID > 0)
                {
                    throw new Exception("Please choose another Title. You already have a Workspace in the system with the same title.");
                }
                return await _workspaceRepository.AddAsync(workspace);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateWorkspaceAsync(Workspace workspace)
        {
            if (workspace == null) { throw new ArgumentNullException(nameof(workspace), "The required parameter [Workspace] is missing."); }
            try
            {
                var entities = await _workspaceRepository.GetByOwnerIdAndTitleAsync(workspace.OwnerID, workspace.Title);
                if (entities != null && entities.Count > 0 && entities[0].ID > 0)
                {
                    throw new Exception("Please choose another Title. You already have a Workspace in the system with the same title.");
                }
                return await _workspaceRepository.UpdateTitleAsync(workspace.ID, workspace.Title);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CreateMainWorkspaceAsync(string ownerId)
        {
            if (String.IsNullOrWhiteSpace(ownerId)) { throw new ArgumentNullException(nameof(ownerId), "The required parameter [OwnerId] is missing."); }
            Workspace workspace = new Workspace
            {
                CreatedBy = "System Service",
                CreatedTime = DateTime.UtcNow,
                IsDeleted = false,
                IsMain = true,
                OwnerID = ownerId,
                Title = "Main Workspace",
                ID = 0
            };
            try
            {
                var entities = await _workspaceRepository.GetByOwnerIdAndTitleAsync(workspace.OwnerID, workspace.Title);
                if (entities != null && entities.Count > 0 && entities[0].ID > 0)
                {
                    throw new Exception("Please choose another Title. You already have a Workspace in the system with the same title.");
                }
                bool WorkspaceIsCreated = await _workspaceRepository.AddAsync(workspace);
                if (WorkspaceIsCreated)
                {
                    var new_entities = await _workspaceRepository.GetByOwnerIdAndTitleAsync(workspace.OwnerID, workspace.Title);
                    if (new_entities != null && new_entities.Count > 0 && new_entities[0].ID > 0)
                    {
                        Workspace mainWorkspace = new_entities.FirstOrDefault();
                        ProjectFolder projectFolder = new ProjectFolder
                        {
                            ID = 0,
                            Title = "General Folder",
                            Description = "General Folder",
                            WorkspaceID = mainWorkspace.ID,
                            CreatedBy = "System Service",
                            CreatedTime = DateTime.UtcNow,
                            IsArchived = false,
                            OwnerID = ownerId,
                        };
                        return await _projectFolderRepository.AddAsync(projectFolder);
                    }
                    else { return true; }
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteWorkspaceAsync(Workspace workspace)
        {
            if (workspace == null) { throw new ArgumentNullException(nameof(workspace), "The required parameter [Workspace] is missing."); }
            try
            {
                return await _workspaceRepository.UpdateToDeletedAsync(workspace.ID, workspace.Title);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Workspace> GetMainWorkspaceAsync(string ownerId)
        {
            Workspace workspace = new Workspace();
            try
            {
                var entity = await _workspaceRepository.GetMainByOwnerIdAsync(ownerId);
                if (entity != null && entity.ID > 0) { workspace = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workspace;
        }

        public async Task<Workspace> GetWorkspaceAsync(int workspaceId)
        {
            Workspace workspace = new Workspace();
            try
            {
                var entity = await _workspaceRepository.GetByIdAsync(workspaceId);
                if (entity != null && entity.ID > 0) { workspace = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workspace;
        }

        public async Task<List<Workspace>> GetWorkspacesByOwnerIdAsync(string ownerId)
        {
            List<Workspace> workspaces = new List<Workspace>();
            try
            {
                var entities = await _workspaceRepository.GetByOwnerIdAsync(ownerId);
                if (entities != null && entities.Count > 0) { workspaces = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workspaces;
        }

        public async Task<List<Workspace>> SearchWorkspacesByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle = null)
        {
            List<Workspace> workspaces = new List<Workspace>();
            try
            {
                if (!string.IsNullOrWhiteSpace(workspaceTitle))
                {
                    var entities = await _workspaceRepository.SearchByOwnerIdAndTitleAsync(ownerId, workspaceTitle);
                    if (entities != null && entities.Count > 0) { workspaces = entities.ToList(); }
                }
                else
                {
                    var entities = await _workspaceRepository.GetByOwnerIdAsync(ownerId);
                    if (entities != null && entities.Count > 0) { workspaces = entities.ToList(); }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return workspaces;
        }

        #endregion

        #region ProjectFolder Service Action
        public async Task<List<ProjectFolder>> GetProjectFoldersByWorkspaceIDAsync(int WorkspaceID, bool? isArchived = null)
        {
            List<ProjectFolder> folders = new List<ProjectFolder>();
            try
            {
                if (isArchived != null)
                {
                    var entities = await _projectFolderRepository.GetByWorkspaceIdAsync(WorkspaceID, isArchived.Value);
                    if (entities != null && entities.Count > 0) { folders = entities.ToList(); }
                }
                else
                {
                    var entities = await _projectFolderRepository.GetByWorkspaceIdAsync(WorkspaceID);
                    if (entities != null && entities.Count > 0) { folders = entities.ToList(); }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return folders;
        }

        public async Task<List<ProjectFolder>> GetProjectFoldersByOwnerIDAsync(string OwnerID, bool? isArchived = null)
        {
            List<ProjectFolder> folders = new List<ProjectFolder>();
            try
            {
                if (isArchived != null)
                {
                    var entities = await _projectFolderRepository.GetByOwnerIdAsync(OwnerID, isArchived.Value);
                    if (entities != null && entities.Count > 0) { folders = entities.ToList(); }
                }
                else
                {
                    var entities = await _projectFolderRepository.GetByOwnerIdAsync(OwnerID);
                    if (entities != null && entities.Count > 0) { folders = entities.ToList(); }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return folders;
        }

        public async Task<ProjectFolder> GetProjectFolderAsync(int projectFolderId)
        {
            ProjectFolder projectFolder = new ProjectFolder();
            try
            {
                var entity = await _projectFolderRepository.GetByIdAsync(projectFolderId);
                if (entity != null && entity.ID > 0) { projectFolder = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return projectFolder;
        }

        public async Task<bool> CreateProjectFolderAsync(ProjectFolder folder)
        {
            if (folder == null) { throw new ArgumentNullException(nameof(folder), "The required parameter [Project Folder] is missing."); }
            try
            {
                var entities = await _projectFolderRepository.GetByTitleAndOwnerIdAsync(folder.Title, folder.OwnerID);
                if (entities != null && entities.Count > 0 && entities[0].ID > 0)
                {
                    throw new Exception("Please choose another Title. You already have a Folder in the system with the same title.");
                }
                return await _projectFolderRepository.AddAsync(folder);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateProjectFolderAsync(ProjectFolder folder)
        {
            if (folder == null) { throw new ArgumentNullException(nameof(folder), "The required parameter [Project Folder] is missing."); }
            try
            {
                var entities = await _projectFolderRepository.GetByTitleAndOwnerIdAsync(folder.Title, folder.OwnerID);
                if (entities != null && entities.Count > 0 && entities[0].ID != folder.ID)
                {
                    throw new Exception("Please choose another Title. You already have a Folder in the system with the same title.");
                }
                return await _projectFolderRepository.UpdateAsync(folder);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateDeleteProjectFolderAsync(ProjectFolder folder)
        {
            if (folder == null) { throw new ArgumentNullException(nameof(folder), "The required parameter [Project Folder] is missing."); }
            try
            {
                return await _projectFolderRepository.UpdateToDeletedAsync(folder.ID, folder.DeletedBy);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<ProjectFolder>> SearchProjectFoldersAsync(int WorkspaceID, string FolderTitle, bool? isArchived = null)
        {
            List<ProjectFolder> folders = new List<ProjectFolder>();
            try
            {
                if (isArchived != null)
                {
                    var entities = await _projectFolderRepository.SearchByWorkspaceIdAndTitleAsync(WorkspaceID, FolderTitle, isArchived.Value);
                    if (entities != null && entities.Count > 0) { folders = entities.ToList(); }
                }
                else
                {
                    var entities = await _projectFolderRepository.SearchByWorkspaceIdAndTitleAsync(WorkspaceID, FolderTitle);
                    if (entities != null && entities.Count > 0) { folders = entities.ToList(); }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return folders;
        }
        #endregion

        #region Project Service Action
        public async Task<bool> CreateProjectAsync(Project project)
        {
            if (project == null) { throw new ArgumentNullException(nameof(project), "The required parameter [Project] is missing."); }
            try
            {
                var entities = await _projectRepository.GetByTitleAsync(project.Title);
                if (entities != null && entities.Count > 0 && entities[0].ID > 0)
                {
                    throw new Exception("Please choose another Title. There is already a record in the system with the same title.");
                }
                bool IsAdded = await _projectRepository.AddAsync(project);
                if (IsAdded)
                {
                    await _utilityRepository.IncrementAutoNumberAsync("projno");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateProjectAsync(Project project)
        {
            if (project == null) { throw new ArgumentNullException(nameof(project), "Required parameter is missing."); }
            try
            {
                var entities = await _projectRepository.GetByTitleAsync(project.Title);
                if (entities != null && entities.Count > 0 && entities[0].ID > 0 && entities[0].ID != project.ID)
                {
                    throw new Exception("Please choose another Title. There is already an item in the system with the same title.");
                }
                return await _projectRepository.UpdateAsync(project);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateProjectMoreInfoAsync(int projectId, string instructions, string deliverables, string modifiedBy)
        {
            return await _projectRepository.UpdateMoreProjectInfoAsync(projectId, instructions, deliverables, modifiedBy);
        }

        public async Task<Project> GetProjectByIDAsync(int projectID)
        {
            Project project = new Project();
            try
            {
                var entities = await _projectRepository.GetByIdAsync(projectID);
                if (entities != null && entities.Count > 0) { project = entities.FirstOrDefault(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return project;
        }

        public async Task<Project> GetProjectByCodeAsync(string projectCode)
        {
            Project project = new Project();
            try
            {
                var entities = await _projectRepository.GetByNumberAsync(projectCode);
                if (entities != null && entities.Count > 0) { project = entities.FirstOrDefault(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return project;
        }
        public async Task<List<Project>> GetProjectsByFolderIDAsync(int FolderID)
        {
            List<Project> projects = new List<Project>();
            try
            {
                var entities = await _projectRepository.GetByFolderIdAsync(FolderID);
                if (entities != null && entities.Count > 0) { projects = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return projects;
        }
        public async Task<List<Project>> GetProjectsByOwnerIDAsync(string OwnerID)
        {
            List<Project> projects = new List<Project>();
            try
            {
                var entities = await _projectRepository.GetByOwnerIdAsync(OwnerID);
                if (entities != null && entities.Count > 0) { projects = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return projects;
        }
        public async Task<List<Project>> SearchProjectsAsync(string projectTitle)
        {
            List<Project> projects = new List<Project>();
            try
            {
                var entities = await _projectRepository.SearchByTitleAsync(projectTitle);
                if (entities != null && entities.Count > 0) { projects = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return projects;
        }
        public async Task<List<Project>> SearchProjectsAsync(int FolderID, string projectTitle)
        {
            List<Project> projects = new List<Project>();
            try
            {
                var entities = await _projectRepository.SearchByFolderIdAndTitleAsync(FolderID, projectTitle);
                if (entities != null && entities.Count > 0) { projects = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return projects;
        }
        #endregion

        #region TaskList Service Action

        //============== Task List Read Service Methods ================================//

        public async Task<List<TaskList>> GetTaskListsByOwnerIdAsync(string OwnerId, bool? isArchived = null)
        {
            List<TaskList> taskLists = new List<TaskList>();
            try
            {
                if (isArchived != null)
                {
                    var entities = await _taskListRepository.GetByOwnerIdAsync(OwnerId, isArchived.Value);
                    if (entities != null && entities.Count > 0) { taskLists = entities.ToList(); }
                }
                else
                {
                    var entities = await _taskListRepository.GetByOwnerIdAsync(OwnerId);
                    if (entities != null && entities.Count > 0) { taskLists = entities.ToList(); }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return taskLists;
        }

        public async Task<TaskList> GetTaskListAsync(int taskListId)
        {
            TaskList taskList = new TaskList();
            try
            {
                var entity = await _taskListRepository.GetByIdAsync(taskListId);
                if (entity != null && entity.Id > 0) { taskList = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return taskList;
        }

        public async Task<List<TaskList>> GetActiveTaskListsAsync(string OwnerId)
        {
            List<TaskList> taskLists = new List<TaskList>();
            var entities = await _taskListRepository.GetByOwnerIdAsync(OwnerId, false);
            if(entities != null && entities.Count > 0)
            {
                taskLists = entities.ToList();
            }
            return taskLists;
        }

        public async Task<List<TaskList>> SearchTaskListAsync(string OwnerId, bool? IsArchived = null, int? createdYear = null, int? createdMonth = null)
        {
            List<TaskList> taskLists = new List<TaskList>();
            if (IsArchived != null)
            {
                if (createdYear != null && createdYear.Value > 0)
                {
                    if (createdMonth != null && createdMonth.Value > 0)
                    {
                        var entities = await _taskListRepository.GetByOwnerIdAsync(OwnerId, IsArchived.Value, createdYear.Value, createdMonth.Value);
                        taskLists = entities.ToList();
                    }
                    else
                    {
                        var entities = await _taskListRepository.GetByOwnerIdAsync(OwnerId, IsArchived.Value, createdYear.Value);
                        taskLists = entities.ToList();
                    }
                }
                else
                {
                    var entities = await _taskListRepository.GetByOwnerIdAsync(OwnerId, IsArchived.Value);
                    taskLists = entities.ToList();
                }
            }
            else
            {
                if (createdYear != null && createdYear.Value > 0)
                {
                    if (createdMonth != null && createdMonth.Value > 0)
                    {
                        var entities = await _taskListRepository.GetByOwnerIdAsync(OwnerId, createdYear.Value, createdMonth.Value);
                        taskLists = entities.ToList();
                    }
                    else
                    {
                        var entities = await _taskListRepository.GetByOwnerIdAsync(OwnerId, createdYear.Value);
                        taskLists = entities.ToList();
                    }
                }
                else
                {
                    var entities = await _taskListRepository.GetByOwnerIdAsync(OwnerId);
                    taskLists = entities.ToList();
                }
            }
            return taskLists;
        }


        //============== Task List Write Service Methods ================================//
        public async Task<bool> CreateTaskListAsync(TaskList taskList)
        {
            if (taskList == null) { throw new ArgumentNullException(nameof(taskList), "The required parameter [TaskList] is missing."); }
            try
            {
                var entities = await _taskListRepository.GetByOwnerIdAndNameAsync(taskList.OwnerId, taskList.Name);
                if (entities != null && entities.Count > 0 && entities[0].Id > 0)
                {
                    throw new Exception("Please choose another Name. You already have a Task Folder in the system with the same name.");
                }
                return await _taskListRepository.AddAsync(taskList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateTaskListAsync(TaskList taskList)
        {
            if (taskList == null) { throw new ArgumentNullException(nameof(taskList), "The required parameter [Task List] is missing."); }
            try
            {
                var entities = await _taskListRepository.GetByOwnerIdAndNameAsync(taskList.OwnerId, taskList.Name);
                if (entities != null && entities.Count > 0 && entities[0].Id != taskList.Id)
                {
                    throw new Exception("Please choose another Name. You already have another Task List in the system with the same name.");
                }
                return await _taskListRepository.UpdateAsync(taskList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateTaskListArchiveAsync(int taskListId, bool isArchived)
        {
            if (taskListId < 1) { throw new ArgumentNullException(nameof(taskListId), "The required parameter [Task List ID] is missing."); }
            try
            {
                return await _taskListRepository.UpdateArchiveAsync(taskListId, isArchived);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateTaskListLockAsync(int taskListId, bool isLocked)
        {
            if (taskListId < 1) { throw new ArgumentNullException(nameof(taskListId), "The required parameter [Task List ID] is missing."); }
            return await _taskListRepository.UpdateLockAsync(taskListId, isLocked);
        }

        public async Task<bool> UpdateDeleteTaskListAsync(TaskList taskList)
        {
            if (taskList == null) { throw new ArgumentNullException(nameof(taskList), "The required parameter [Task List] is missing."); }
            try
            {
                return await _taskListRepository.UpdateToDeletedAsync(taskList.Id, taskList.DeletedBy);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteTaskListAsync(int taskListId)
        {
            if (taskListId < 1) { throw new ArgumentNullException(nameof(taskListId), "The required parameter [Task List ID] is missing."); }
            try
            {
                return await _taskListRepository.DeleteAsync(taskListId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ReturnTaskListSubmissionAsync(int taskListId, long submissionId)
        {
            bool taskListIsUnlocked = false;
            bool submissionStatusUpdated = false;
            if (submissionId < 1) { throw new ArgumentNullException(nameof(submissionId), "The required parameter [Task List Submission ID] is missing."); }
            if (taskListId < 1) { throw new ArgumentNullException(nameof(taskListId), "The required parameter [Task List ID] is missing."); }

            TaskSubmission taskSubmission = new TaskSubmission();
            taskSubmission = await _taskSubmissionRepository.GetByIdAsync(submissionId);
            if (taskSubmission == null) { throw new Exception("The submission record was not found."); }
            else
            {
                if (taskSubmission.SubmissionType == WorkItemSubmissionType.Approval)
                {
                    taskListIsUnlocked = await _taskListRepository.UpdateLockAsync(taskListId, false);
                    if (taskListIsUnlocked)
                    {
                        submissionStatusUpdated = await _taskSubmissionRepository.UpdateAsync(submissionId);
                        if (submissionStatusUpdated)
                        {
                            TaskListActivityHistory taskListHistory = new TaskListActivityHistory
                            {
                                ActivityTime = DateTime.UtcNow,
                                ActivityBy = taskSubmission.ToEmployeeName,
                                ActivityDescription = $"Task List that was submitted for {taskSubmission.SubmissionType.ToString()} by {taskSubmission.FromEmployeeName}] was returned by {taskSubmission.ToEmployeeName} on [{DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT. ",
                                TaskListId = taskSubmission.TaskListId,
                                EntityId = (object)taskSubmission.TaskListId,
                            };
                            await _utilityRepository.InsertTaskListActivityHistoryAsync(taskListHistory);
                        }
                    }
                    return taskListIsUnlocked;
                }
                else if (taskSubmission.SubmissionType == WorkItemSubmissionType.Evaluation)
                {
                    //====== Code for Update the Evaluation Records goes here. ========//
                    bool tasksClosed = false;
                    tasksClosed = await _taskItemRepository.UpdateTaskStatusByTaskListIdAsync(taskSubmission.TaskListId, (int)WorkItemStatus.Closed, taskSubmission.ToEmployeeName);
                    if (!tasksClosed) { return false; }
                    else
                    {
                        bool IsUpdated = false;
                        bool taskListArchived = false;
                        taskListArchived = await _taskListRepository.UpdateArchiveAsync(taskSubmission.TaskListId, true);
                        if (!taskListArchived) { return false; }
                        else
                        {
                            TaskEvaluationHeader header = new TaskEvaluationHeader();
                            var entity = await _taskEvaluationHeaderRepository.GetScoresByTaskListIdAndEvaluatorIdAsync(taskSubmission.TaskListId, taskSubmission.ToEmployeeId);
                            if (entity == null) { return false; }
                            else
                            {
                                header = entity;
                                header.AveragePercentCompletion = (header.TotalPercentCompletion / header.TotalNumberOfTasks);
                                header.AverageQualityRating = (header.TotalQualityRating / header.TotalNumberOfTasks);
                                header.NoOfUncompletedTasks = header.TotalNumberOfTasks - header.NoOfCompletedTasks;

                                var existing_entity = await _taskEvaluationHeaderRepository.GetByIdAsync(header.Id);
                                if (existing_entity == null || existing_entity.Count == 0) { return false; }
                                else
                                {
                                    TaskEvaluationHeader existingHeader = new TaskEvaluationHeader();
                                    existingHeader = existing_entity.FirstOrDefault();
                                    header.EvaluationDate = existingHeader.EvaluationDate;
                                    header.EvaluatorId = existingHeader.EvaluatorId;
                                    header.TaskOwnerDeptId = existingHeader.TaskOwnerDeptId;
                                    header.TaskOwnerUnitId = existingHeader.TaskOwnerUnitId;
                                    header.TaskOwnerLocationId = existingHeader.TaskOwnerLocationId;

                                    IsUpdated = await _taskEvaluationHeaderRepository.UpdateAsync(header);

                                    if (!IsUpdated) { return false; }
                                    else
                                    {
                                        submissionStatusUpdated = await _taskSubmissionRepository.UpdateAsync(submissionId);
                                        if (submissionStatusUpdated)
                                        {
                                            TaskListActivityHistory taskListHistory = new TaskListActivityHistory
                                            {
                                                ActivityTime = DateTime.UtcNow,
                                                ActivityBy = taskSubmission.ToEmployeeName,
                                                ActivityDescription = $"Task List that was submitted for {taskSubmission.SubmissionType.ToString()} by {taskSubmission.FromEmployeeName}] was returned by {taskSubmission.ToEmployeeName} on [{DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT. ",
                                                TaskListId = taskSubmission.TaskListId,
                                                EntityId = (object)taskSubmission.TaskListId,
                                            };
                                            await _utilityRepository.InsertTaskListActivityHistoryAsync(taskListHistory);
                                        }
                                    }
                                }

                            }
                           
                        }
                        return IsUpdated;
                    }
                }
                else { throw new Exception("Error: Unknown submission type."); }
            }
        }

        //============== Task List Note Service Methods =============================//
        public async Task<List<TaskListNote>> GetTaskListNotesAsync(int taskListId)
        {
            List<TaskListNote> taskListNotes = new List<TaskListNote>();
            try
            {
                var entities = await _taskListRepository.GetNotesByTaskListIdAsync(taskListId);
                if (entities != null && entities.Count > 0) { taskListNotes = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return taskListNotes;
        }
        public async Task<bool> AddTaskListNoteAsync(TaskListNote taskListNote)
        {
            if (taskListNote == null) { throw new ArgumentNullException(nameof(taskListNote), "The required parameter [Task List Note] is missing."); }
            return await _taskListRepository.AddNoteAsync(taskListNote);
        }


        #endregion

        #region Task Items Service Methods

        //===================== Task Items Read Service Methods =========================================================================================================================//
        public async Task<List<TaskItem>> GetTaskItemsByTaskListAsync(int taskListId, int? dueYear = null, int? dueMonth = null, WorkItemProgressStatus? progressStatus = null)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
            try
            {
                if (progressStatus != null)
                {
                    if (dueYear != null && dueYear.Value > 1900)
                    {
                        if (dueMonth != null && dueMonth.Value > 0)
                        {
                            var month_year_entities = await _taskItemRepository.GetByTaskListIdAndDueYearAndDueMonthAndProgressStatusAsync(taskListId, dueYear.Value, dueMonth.Value, (int)progressStatus.Value);
                            if (month_year_entities != null && month_year_entities.Count > 0) { taskItems = month_year_entities.ToList(); }
                        }
                        else
                        {
                            var year_entities = await _taskItemRepository.GetByTaskListIdAndDueYearAndProgressStatusAsync(taskListId, dueYear.Value, (int)progressStatus.Value);
                            if (year_entities != null && year_entities.Count > 0) { taskItems = year_entities.ToList(); }
                        }
                    }
                    else
                    {
                        dueYear = DateTime.Now.Year;
                        if (dueMonth != null && dueMonth.Value > 0)
                        {
                            var month_year_entities = await _taskItemRepository.GetByTaskListIdAndDueYearAndDueMonthAndProgressStatusAsync(taskListId, dueYear.Value, dueMonth.Value, (int)progressStatus.Value);
                            if (month_year_entities != null && month_year_entities.Count > 0) { taskItems = month_year_entities.ToList(); }
                        }
                        else
                        {
                            var year_entities = await _taskItemRepository.GetByTaskListIdAndDueYearAndProgressStatusAsync(taskListId, dueYear.Value, (int)progressStatus.Value);
                            if (year_entities != null && year_entities.Count > 0) { taskItems = year_entities.ToList(); }
                        }
                    }
                }
                else
                {
                    if (dueYear != null && dueYear.Value > 1900)
                    {
                        if (dueMonth != null && dueMonth.Value > 0)
                        {
                            var month_year_entities = await _taskItemRepository.GetByTaskListIdAndDueYearAndDueMonthAsync(taskListId, dueYear.Value, dueMonth.Value);
                            if (month_year_entities != null && month_year_entities.Count > 0) { taskItems = month_year_entities.ToList(); }
                        }
                        else
                        {
                            var year_entities = await _taskItemRepository.GetByTaskListIdAndDueYearAsync(taskListId, dueYear.Value);
                            if (year_entities != null && year_entities.Count > 0) { taskItems = year_entities.ToList(); }
                        }
                    }
                    else
                    {
                        dueYear = DateTime.Now.Year;
                        if (dueMonth != null && dueMonth.Value > 0)
                        {
                            var month_year_entities = await _taskItemRepository.GetByTaskListIdAndDueYearAndDueMonthAsync(taskListId, dueYear.Value, dueMonth.Value);
                            if (month_year_entities != null && month_year_entities.Count > 0) { taskItems = month_year_entities.ToList(); }
                        }
                        else
                        {
                            var year_entities = await _taskItemRepository.GetByTaskListIdAndDueYearAsync(taskListId, dueYear.Value);
                            if (year_entities != null && year_entities.Count > 0) { taskItems = year_entities.ToList(); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return taskItems;
        }

        public async Task<TaskItem> GetTaskItemAsync(long taskItemId)
        {
            TaskItem taskItem = new TaskItem();
            var entity = await _taskItemRepository.GetByIdAsync(taskItemId);
            if (entity != null) { taskItem = entity; }
            return taskItem;
        }

        public async Task<TaskItem> GetTaskItemAsync(string taskItemNo)
        {
            TaskItem taskItem = new TaskItem();
            var entity = await _taskItemRepository.GetByNumberAsync(taskItemNo);
            if (entity != null) { taskItem = entity; }
            return taskItem;
        }

        //===================== Task Items Write Service Methods =========================================================================================================================//
        public async Task<bool> CreateTaskItemAsync(TaskItem taskItem)
        {
            if (taskItem == null) { throw new ArgumentNullException(nameof(taskItem), "The required parameter [TaskItem] is missing."); }
            try
            {
                var entities = await _taskItemRepository.GetByTaskListIdAndDescriptionAsync(taskItem.TaskListId, taskItem.Description);
                if (entities != null && entities.Count > 0 && entities[0].Id > 0)
                {
                    throw new Exception("Please enter a different Task Description. You already have a Task in the same Task Folder with the same Description.");
                }
                bool IsAdded = await _taskItemRepository.AddAsync(taskItem);
                if (IsAdded)
                {
                    await _utilityRepository.IncrementAutoNumberAsync("taskno");
                    TaskItemActivityHistory taskHistory = new TaskItemActivityHistory
                    {
                        ActivityTime = DateTime.UtcNow,
                        ActivityBy = taskItem.CreatedBy,
                        ActivityDescription = $"Created with Task Number: [{taskItem.Number}] and Description as: [{taskItem.Description}].",
                        TaskItemId = taskItem.Id,
                        EntityId = (object)taskItem.Id,
                    };
                    await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }

        public async Task<bool> UpdateTaskItemAsync(TaskItem taskItem)
        {
            if (taskItem == null) { throw new ArgumentNullException(nameof(taskItem), "The required parameter [TaskItem] is missing."); }
            try
            {
                TaskItem oldTaskItem = await _taskItemRepository.GetByIdAsync(taskItem.Id);
                if ((taskItem.Description == oldTaskItem.Description) && (taskItem.Deliverable == oldTaskItem.Deliverable))
                {
                    throw new Exception("This update is unnecessary as no changes were made to the original task details.");
                }

                var entities = await _taskItemRepository.GetByTaskListIdAndDescriptionAsync(taskItem.TaskListId, taskItem.Description);
                if (entities != null && entities.Count > 0 && entities[0].Id > 0 && entities[0].Id != taskItem.Id)
                {
                    throw new Exception("Sorry, you may want to choose another Description. Because you already have a Task in the same Task List with this same Description.");
                }
                bool IsUpdated = await _taskItemRepository.UpdateAsync(taskItem);
                if (IsUpdated)
                {
                    TaskItemActivityHistory taskHistory = new TaskItemActivityHistory();
                    taskHistory.ActivityTime = DateTime.UtcNow;
                    taskHistory.ActivityBy = taskItem.CreatedBy;
                    taskHistory.TaskItemId = taskItem.Id;
                    taskHistory.EntityId = (object)taskItem.Id;

                    if ((taskItem.Description != oldTaskItem.Description) && (taskItem.Deliverable != oldTaskItem.Deliverable))
                    {
                        taskHistory.ActivityDescription = $"Updated Task Description from: [{oldTaskItem.Description}] to [{taskItem.Description}], and Deliverables from: [{oldTaskItem.Deliverable}] to [{taskItem.Deliverable}].";
                    }
                    else if ((taskItem.Description != oldTaskItem.Description) && (taskItem.Deliverable == oldTaskItem.Deliverable))
                    {
                        taskHistory.ActivityDescription = $"Updated Task Description from: [{oldTaskItem.Description}] to [{taskItem.Description}].";
                    }
                    else if ((taskItem.Description == oldTaskItem.Description) && (taskItem.Deliverable != oldTaskItem.Deliverable))
                    {
                        taskHistory.ActivityDescription = $"Updated Task Delivarables from: [{oldTaskItem.Deliverable}] to [{taskItem.Deliverable}].";
                    }
                    else
                    {
                        taskHistory.ActivityDescription = $"Updated Task details but nothing was changed.";
                    }

                    await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }

        public async Task<bool> DeleteTaskItemAsync(long taskItemId)
        {
            if (taskItemId < 1) { throw new ArgumentNullException(nameof(taskItemId), "The required parameter [Task Item ID] is missing."); }
            try
            {
                return await _taskItemRepository.DeleteAsync(taskItemId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateTaskCancelStatusAsync(long taskItemId, string updatedBy, bool cancelTask)
        {
            try
            {
                bool IsUpdated = await _taskItemRepository.UpdateTaskCancelAsync(taskItemId, updatedBy, cancelTask);
                if (IsUpdated)
                {
                    string actionDescription = string.Empty;
                    if (cancelTask)
                    {
                        actionDescription = $"Task was cancelled by {updatedBy} on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT.";
                    }
                    else
                    {
                        actionDescription = $"Task which was previously cancelled was reversed by {updatedBy} on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT.";
                    }

                    TaskItemActivityHistory taskHistory = new TaskItemActivityHistory
                    {
                        ActivityTime = DateTime.UtcNow,
                        ActivityBy = updatedBy,
                        ActivityDescription = actionDescription,
                        TaskItemId = taskItemId,
                        EntityId = (object)taskItemId,
                    };
                    await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }

        public async Task<bool> UpdateTaskProgressAsync(long taskItemId, int newProgressStatusId, string previousProgressStatus, string updatedBy)
        {
            if (taskItemId < 1 || newProgressStatusId < 0 || string.IsNullOrWhiteSpace(previousProgressStatus) || string.IsNullOrWhiteSpace(updatedBy))
            {
                throw new Exception("Missing Parameters Error. Sorry, the request could not be processed because some key parameters needed to process it are missing.");
            }

            try
            {
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
                bool IsUpdated = await _taskItemRepository.UpdateProgressStatusAsync(taskItemId, newProgressStatusId, updatedBy);
                if (IsUpdated)
                {
                    TaskItemActivityHistory taskHistory = new TaskItemActivityHistory
                    {
                        ActivityTime = DateTime.UtcNow,
                        ActivityBy = updatedBy,
                        ActivityDescription = $"Progress Status was updated from {previousProgressStatus} to {newStatusDescription}.",
                        TaskItemId = taskItemId,
                        EntityId = (object)taskItemId,
                    };
                    await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }

        public async Task<bool> UpdateTaskStatusAsync(long taskItemId, WorkItemStatus newStatus, string updatedBy)
        {
            try
            {
                string newStatusDescription = string.Empty;
                string previousStatusDescription = string.Empty;

                switch (newStatus)
                {
                    case WorkItemStatus.Closed:
                        newStatusDescription = "Closed";
                        previousStatusDescription = "Open";
                        break;
                    case WorkItemStatus.Open:
                        newStatusDescription = "Open";
                        previousStatusDescription = "Closed";
                        break;
                    default:
                        break;
                }
                bool IsUpdated = await _taskItemRepository.UpdateTaskStatusAsync(taskItemId, (int)newStatus, updatedBy);
                if (IsUpdated)
                {
                    TaskItemActivityHistory taskHistory = new TaskItemActivityHistory
                    {
                        ActivityTime = DateTime.UtcNow,
                        ActivityBy = updatedBy,
                        ActivityDescription = $"Task Status was updated from [{previousStatusDescription}] to [{newStatusDescription}].",
                        TaskItemId = taskItemId,
                        EntityId = (object)taskItemId,
                    };
                    await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }

        public async Task<bool> UpdateTaskStatusByTaskListIdAsync(int taskListId, WorkItemStatus newStatus, string updatedBy)
        {
            try
            {
                string newStatusDescription = string.Empty;
                string previousStatusDescription = string.Empty;

                switch (newStatus)
                {
                    case WorkItemStatus.Closed:
                        newStatusDescription = "Closed";
                        previousStatusDescription = "Open";
                        break;
                    case WorkItemStatus.Open:
                        newStatusDescription = "Open";
                        previousStatusDescription = "Closed";
                        break;
                    default:
                        break;
                }
                bool IsUpdated = await _taskItemRepository.UpdateTaskStatusByTaskListIdAsync(taskListId, (int)newStatus, updatedBy);
                if (IsUpdated)
                {
                    List<TaskItem> taskItems = new List<TaskItem>();
                    taskItems = await _taskItemRepository.GetByTaskListIdAsync(taskListId);
                    foreach (var item in taskItems)
                    {
                        TaskItemActivityHistory taskHistory = new TaskItemActivityHistory
                        {
                            ActivityTime = DateTime.UtcNow,
                            ActivityBy = updatedBy,
                            ActivityDescription = $"Task Status was updated from [{previousStatusDescription}] to [{newStatusDescription}].",
                            TaskItemId = item.Id,
                            EntityId = (object)item.Id,
                        };
                        await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }

        public async Task<bool> UpdateTaskApprovalStatusAsync(long taskItemId, ApprovalStatus newApprovalStatus, string updatedBy)
        {
            try
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
                bool IsUpdated = await _taskItemRepository.UpdateApprovalStatusAsync(taskItemId, newApprovalStatus, updatedBy);
                if (IsUpdated)
                {
                    TaskItemActivityHistory taskHistory = new TaskItemActivityHistory
                    {
                        ActivityTime = DateTime.UtcNow,
                        ActivityBy = updatedBy,
                        ActivityDescription = $"Task Approval Status was updated to [{newStatusDescription}] by [{updatedBy}] on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT.",
                        TaskItemId = taskItemId,
                        EntityId = (object)taskItemId,
                    };
                    await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }

        public async Task<bool> LinkTaskToProgramAsync(long taskItemId, string programCode, DateTime? programDate, string updatedBy)
        {
            if (taskItemId < 1 || string.IsNullOrWhiteSpace(programCode) || string.IsNullOrWhiteSpace(updatedBy))
            {
                throw new Exception("Missing Parameters Error. Sorry, the request could not be processed because some key parameters needed to process it are missing.");
            }

            try
            {
                bool IsUpdated = await _taskItemRepository.UpdateProgramLinkAsync(taskItemId, programCode, programDate, updatedBy);
                if (IsUpdated)
                {
                    Programme programme = new Programme();
                    programme = await _programRepository.GetByCodeAsync(programCode);

                    TaskItemActivityHistory taskHistory = new TaskItemActivityHistory
                    {
                        ActivityTime = DateTime.UtcNow,
                        ActivityBy = updatedBy,
                        ActivityDescription = $"Task was successfully linked to the program [{programme.Title}] by [{updatedBy}] on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT.",
                        TaskItemId = taskItemId,
                        EntityId = (object)taskItemId,
                    };
                    await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }

        public async Task<bool> LinkTaskToProjectAsync(long taskItemId, string projectCode, string updatedBy, string projectTitle)
        {
            if (taskItemId < 1 || string.IsNullOrWhiteSpace(projectCode) || string.IsNullOrWhiteSpace(updatedBy))
            {
                throw new Exception("Missing Parameters Error. Sorry, the request could not be processed because some key parameters needed to process it are missing.");
            }

            try
            {
                bool IsUpdated = await _taskItemRepository.UpdateProjectLinkAsync(taskItemId, projectCode, updatedBy);
                if (IsUpdated)
                {
                    TaskItemActivityHistory taskHistory = new TaskItemActivityHistory
                    {
                        ActivityTime = DateTime.UtcNow,
                        ActivityBy = updatedBy,
                        ActivityDescription = $"Task was successfully linked to the project [{projectTitle}] by [{updatedBy}] on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT.",
                        TaskItemId = taskItemId,
                        EntityId = (object)taskItemId,
                    };
                    await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return false;
        }

        public async Task<bool> ReassignTaskItemAsync(long taskItemId, int newTaskListId, string newTaskOwnerId, string oldTaskOwnerName, string reassignedBy)
        {
            Employee newTaskOwner = new Employee();
            int? newUnitId = null;
            int? newDeptId = null;
            int? newLocationId = null;
            string newTaskOwnerName = string.Empty;

            newTaskOwner = await _employeesRepository.GetEmployeeByIdAsync(newTaskOwnerId);
            if(newTaskOwner != null)
            {
                newUnitId = newTaskOwner.UnitID;
                newDeptId = newTaskOwner.DepartmentID;
                newLocationId = newTaskOwner.LocationID;
                newTaskOwnerName = newTaskOwner.FullName;
            }

                bool IsUpdated = await _taskItemRepository.UpdateTaskOwnerAsync(taskItemId,newTaskListId,newTaskOwnerId, newUnitId.Value,newDeptId.Value,newLocationId.Value,reassignedBy);
                if (IsUpdated)
                {
                        TaskItemActivityHistory taskHistory = new TaskItemActivityHistory
                        {
                            ActivityTime = DateTime.UtcNow,
                            ActivityBy = reassignedBy,
                            ActivityDescription = $"Task was rea-assigned from [{oldTaskOwnerName}] to [{newTaskOwnerName}].",
                            TaskItemId = taskItemId,
                            EntityId = (object)taskItemId,
                        };
                        await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                    return true;
                }

            return false;
        }


        //================== Task Item Notes =====================================//
        public async Task<List<TaskNote>> GetTaskNotesAsync(long taskItemId)
        {
            List<TaskNote> taskItemNotes = new List<TaskNote>();
            try
            {
                var entities = await _taskItemRepository.GetNotesByTaskItemIdAsync(taskItemId);
                if (entities != null && entities.Count > 0) { taskItemNotes = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return taskItemNotes;
        }

        public async Task<bool> AddTaskItemNoteAsync(TaskNote taskNote)
        {
            if (taskNote == null) { throw new ArgumentNullException(nameof(taskNote), "The required parameter [Task List Note] is missing."); }
            return await _taskItemRepository.AddNoteAsync(taskNote);
        }

        #endregion

        #region Task Item Timeline Changes Service Methods

        public async Task<bool> UpdateTaskTimelineAsync(TaskTimelineChange taskTimelineChange)
        {
            if (taskTimelineChange == null)
            {
                throw new Exception("Missing Parameters Error. Sorry, the request could not be processed because some key parameters needed to process it are missing.");
            }

            bool IsUpdated = await _taskItemRepository.UpdateTimelineAsync(taskTimelineChange.TaskItemId, taskTimelineChange.ModifiedBy, taskTimelineChange.PreviousStartDate, taskTimelineChange.NewStartDate, taskTimelineChange.PreviousEndDate, taskTimelineChange.NewEndDate);
            if (IsUpdated)
            {
                bool IsAdded = await _taskTimelineRepository.AddAsync(taskTimelineChange);
                if (IsAdded)
                {
                    TaskItemActivityHistory taskHistory = new TaskItemActivityHistory
                    {
                        ActivityTime = DateTime.UtcNow,
                        ActivityBy = taskTimelineChange.ModifiedBy,
                        ActivityDescription = $"Task rescheduled as follows: Expected Start Date was changed from [{taskTimelineChange.PreviousStartDate}] to [{taskTimelineChange.NewStartDate}]  while Expected Due Date was changed from [{taskTimelineChange.PreviousEndDate}] to [{taskTimelineChange.NewEndDate}]. And as a result this task's Approval Status was reverted to [Pending Approval]. ",
                        TaskItemId = taskTimelineChange.TaskItemId,
                        EntityId = (object)taskTimelineChange.TaskItemId,
                    };
                    await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                    return true;
                }
            }
            return false;
        }

        public async Task<List<TaskTimelineChange>> GetTaskTimelineChangesByTaskItemIdAsync(long taskItemId)
        {
            List<TaskTimelineChange> taskTimelineChanges = new List<TaskTimelineChange>();
            var entities = await _taskTimelineRepository.GetByTaskItemIdAsync(taskItemId);
            if (entities != null) { taskTimelineChanges = entities.ToList(); }
            return taskTimelineChanges;
        }
        #endregion

        #region Task Submission Service Methods
        public async Task<bool> AddTaskListSubmissionAsync(TaskSubmission taskSubmission)
        {
            bool IsAdded = false;
            if (taskSubmission == null) { throw new ArgumentNullException(nameof(taskSubmission), "The required parameter [Task Submission] is missing."); }
            List<TaskSubmission> existingSubmissions = await _taskSubmissionRepository.GetByToEmployeeIdAndTaskListIdAsync(taskSubmission.ToEmployeeId, taskSubmission.TaskListId);
            if (existingSubmissions != null && existingSubmissions.Count > 0)
            {
                foreach (var submission in existingSubmissions)
                {
                    if (submission.SubmissionType == taskSubmission.SubmissionType && submission.IsActioned == false)
                    {
                        throw new Exception($"Double submission. You already submitted this task list for {taskSubmission.SubmissionType.ToString()}.");
                    }
                }
            }

            IsAdded = await _taskSubmissionRepository.AddAsync(taskSubmission);
            if (IsAdded)
            {
                TaskListActivityHistory taskListHistory = new TaskListActivityHistory
                {
                    ActivityTime = DateTime.UtcNow,
                    ActivityBy = taskSubmission.FromEmployeeName,
                    ActivityDescription = $"Task List was submitted to {taskSubmission.ToEmployeeName} for {taskSubmission.SubmissionType.ToString()} by {taskSubmission.FromEmployeeName}] on [{taskSubmission.DateSubmitted.Value.ToLongDateString()} at exactly {taskSubmission.DateSubmitted.Value.ToLongTimeString()} GMT. ",
                    TaskListId = taskSubmission.TaskListId,
                    EntityId = (object)taskSubmission.TaskListId,
                };
                await _utilityRepository.InsertTaskListActivityHistoryAsync(taskListHistory);
            }
            return IsAdded;
        }

        public async Task<bool> DeleteTaskListSubmissionAsync(long taskListSubmissionId)
        {
            return await _taskSubmissionRepository.DeleteAsync(taskListSubmissionId);
        }

        public async Task<List<TaskSubmission>> SearchTaskSubmissionsAsync(string toEmployeeId, int? submittedYear = null, int? submittedMonth = null, string fromEmployeeName = null)
        {
            List<TaskSubmission> taskSubmissions = new List<TaskSubmission>();
            try
            {
                if (!string.IsNullOrWhiteSpace(fromEmployeeName))
                {
                    if (submittedYear != null && submittedYear.Value > 1900)
                    {
                        if (submittedMonth != null && submittedMonth.Value > 0)
                        {
                            var month_year_entities = await _taskSubmissionRepository.GetByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAndSubmittedMonthAsync(toEmployeeId, fromEmployeeName, submittedYear.Value, submittedMonth.Value);
                            if (month_year_entities != null && month_year_entities.Count > 0) { taskSubmissions = month_year_entities.ToList(); }
                        }
                        else
                        {
                            var year_entities = await _taskSubmissionRepository.GetByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAsync(toEmployeeId, fromEmployeeName, submittedYear.Value);
                            if (year_entities != null && year_entities.Count > 0) { taskSubmissions = year_entities.ToList(); }
                        }
                    }
                    else
                    {
                        submittedYear = DateTime.Now.Year;
                        if (submittedMonth != null && submittedMonth.Value > 0)
                        {
                            var month_year_entities = await _taskSubmissionRepository.GetByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAndSubmittedMonthAsync(toEmployeeId, fromEmployeeName, submittedYear.Value, submittedMonth.Value);
                            if (month_year_entities != null && month_year_entities.Count > 0) { taskSubmissions = month_year_entities.ToList(); }
                        }
                        else
                        {
                            var year_entities = await _taskSubmissionRepository.GetByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAsync(toEmployeeId, fromEmployeeName, submittedYear.Value);
                            if (year_entities != null && year_entities.Count > 0) { taskSubmissions = year_entities.ToList(); }
                        }
                    }
                }
                else
                {
                    if (submittedYear != null && submittedYear.Value > 1900)
                    {
                        if (submittedMonth != null && submittedMonth.Value > 0)
                        {
                            var month_year_entities = await _taskSubmissionRepository.GetByToEmployeeIdAndSubmittedYearAndSubmittedMonthAsync(toEmployeeId, submittedYear.Value, submittedMonth.Value);
                            if (month_year_entities != null && month_year_entities.Count > 0) { taskSubmissions = month_year_entities.ToList(); }
                        }
                        else
                        {
                            var year_entities = await _taskSubmissionRepository.GetByToEmployeeIdAndSubmittedYearAsync(toEmployeeId, submittedYear.Value);
                            if (year_entities != null && year_entities.Count > 0) { taskSubmissions = year_entities.ToList(); }
                        }
                    }
                    else
                    {
                        submittedYear = DateTime.Now.Year;
                        if (submittedMonth != null && submittedMonth.Value > 0)
                        {
                            var month_year_entities = await _taskSubmissionRepository.GetByToEmployeeIdAndSubmittedYearAndSubmittedMonthAsync(toEmployeeId, submittedYear.Value, submittedMonth.Value);
                            if (month_year_entities != null && month_year_entities.Count > 0) { taskSubmissions = month_year_entities.ToList(); }
                        }
                        else
                        {
                            var year_entities = await _taskSubmissionRepository.GetByToEmployeeIdAndSubmittedYearAsync(toEmployeeId, submittedYear.Value);
                            if (year_entities != null && year_entities.Count > 0) { taskSubmissions = year_entities.ToList(); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return taskSubmissions;
        }

        #endregion

        #region Task Evaluation Header Service Action Methods

        public async Task<TaskEvaluationHeader> GetTaskEvaluationHeaderAsync(int taskEvaluationHeaderId)
        {
            TaskEvaluationHeader hdr = new TaskEvaluationHeader();
            var entities = await _taskEvaluationHeaderRepository.GetByIdAsync(taskEvaluationHeaderId);
            if (entities != null && entities.Count > 0)
            {
                hdr = entities.FirstOrDefault();
            }
            return hdr;
        }

        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByTaskOwnerIdAsync(string taskOwnerId, int? evaluatedYear = null, int? evaluatedMonth = null)
        {
            List<TaskEvaluationHeader> headers = new List<TaskEvaluationHeader>();
            if (string.IsNullOrWhiteSpace(taskOwnerId)) { return headers; }
            if(evaluatedYear == null || evaluatedYear < 2000) { evaluatedYear = DateTime.Now.Year; }

            if(evaluatedMonth != null && evaluatedMonth > 0)
            {
                var entities = await _taskEvaluationHeaderRepository.GetByTaskOwnerIdAndDueYearAndDueMonthAsync(taskOwnerId, evaluatedYear.Value, evaluatedMonth.Value);
                if (entities != null && entities.Count > 0)
                {
                    headers = entities;
                }
            }
            else
            {
                var entities = await _taskEvaluationHeaderRepository.GetByTaskOwnerIdAndDueYearAsync(taskOwnerId, evaluatedYear.Value);
                if (entities != null && entities.Count > 0)
                {
                    headers = entities;
                }
            }
            return headers;
        }

        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByReportsToEmployeeIdAsync(string reportsToEmployeeId, int? evaluatedYear = null, int? evaluatedMonth = null)
        {
            List<TaskEvaluationHeader> headers = new List<TaskEvaluationHeader>();
            if (string.IsNullOrWhiteSpace(reportsToEmployeeId)) { return headers; }
            if (evaluatedYear == null || evaluatedYear < 2000) { evaluatedYear = DateTime.Now.Year; }

            if (evaluatedMonth != null && evaluatedMonth > 0)
            {
                var entities = await _taskEvaluationHeaderRepository.GetByReportsToEmployeeIdAndDueYearAndDueMonthAsync(reportsToEmployeeId, evaluatedYear.Value, evaluatedMonth.Value);
                if (entities != null && entities.Count > 0)
                {
                    headers = entities;
                }
            }
            else
            {
                var entities = await _taskEvaluationHeaderRepository.GetByReportsToEmployeeIdAndDueYearAsync(reportsToEmployeeId, evaluatedYear.Value);
                if (entities != null && entities.Count > 0)
                {
                    headers = entities;
                }
            }
            return headers;
        }

        public async Task<TaskEvaluationHeader> GetTaskEvaluationHeaderAsync(int taskListId, string evaluatorId)
        {
            TaskEvaluationHeader hdr = new TaskEvaluationHeader();
            var entities = await _taskEvaluationHeaderRepository.GetByTaskListIdAndEvaluatorIdAsync(taskListId, evaluatorId);
            if (entities != null && entities.Count > 0)
            {
                hdr = entities.FirstOrDefault();
            }
            return hdr;
        }

        public async Task<List<TaskEvaluationHeader>> SearchTaskEvaluationHeaderAsync(int startYear, int endYear, int? startMonth = null, int? endMonth = null, int? unitId = null, int? deptId = null, int? locationId = null)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaderList = new List<TaskEvaluationHeader>();
            if(locationId != null && locationId > 0)
            {
                if(unitId != null && unitId > 0)
                {
                    if(startMonth != null && startMonth > 0)
                    {
                        if(endMonth == null || endMonth < 1) { endMonth = 12; }
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetByUnitIdAndLocationIdAndEvaluationYearAndEvaluationMonthAsync(unitId.Value, locationId.Value, startYear, endYear, startMonth.Value, endMonth.Value);
                    }
                    else
                    {
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetByUnitIdAndLocationIdAndEvaluationYearAsync(unitId.Value, locationId.Value, startYear, endYear);
                    }
                }
                else if(deptId != null && deptId > 0)
                {
                    if (startMonth != null && startMonth > 0)
                    {
                        if (endMonth == null || endMonth < 1) { endMonth = 12; }
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetByDepartmentIdAndLocationIdAndEvaluationYearAndEvaluationMonthAsync(deptId.Value, locationId.Value, startYear, endYear, startMonth.Value, endMonth.Value);
                    }
                    else
                    {
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetByDepartmentIdAndLocationIdAndEvaluationYearAsync(deptId.Value, locationId.Value, startYear, endYear);
                    }
                }
                else
                {
                    if (startMonth != null && startMonth > 0)
                    {
                        if (endMonth == null || endMonth < 1) { endMonth = 12; }
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetByLocationIdAndEvaluationYearAndEvaluationMonthAsync(locationId.Value, startYear, endYear, startMonth.Value, endMonth.Value);
                    }
                    else
                    {
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetByLocationIdAndEvaluationYearAsync(locationId.Value, startYear, endYear);
                    }
                }
            }
            else
            {
                if (unitId != null && unitId > 0)
                {
                    if (startMonth != null && startMonth > 0)
                    {
                        if (endMonth == null || endMonth < 1) { endMonth = 12; }
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetByUnitIdAndEvaluationYearAndEvaluationMonthAsync(unitId.Value, startYear, endYear, startMonth.Value, endMonth.Value);
                    }
                    else
                    {
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetByUnitIdAndEvaluationYearAsync(unitId.Value, startYear, endYear);
                    }
                }
                else if (deptId != null && deptId > 0)
                {
                    if (startMonth != null && startMonth > 0)
                    {
                        if (endMonth == null || endMonth < 1) { endMonth = 12; }
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetByDepartmentIdAndEvaluationYearAndEvaluationMonthAsync(deptId.Value, startYear, endYear, startMonth.Value, endMonth.Value);
                    }
                    else
                    {
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetByDepartmentIdAndEvaluationYearAsync(deptId.Value, startYear, endYear);
                    }
                }
                else
                {
                    if (startMonth != null && startMonth > 0)
                    {
                        if (endMonth == null || endMonth < 1) { endMonth = 12; }
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetByEvaluationYearAndEvaluationMonthAsync(startYear, endYear, startMonth.Value, endMonth.Value);
                    }
                    else
                    {
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetByEvaluationYearAsync(startYear, endYear);
                    }
                }
            }
            return taskEvaluationHeaderList;
        }

        public async Task<List<TaskEvaluationHeaderSummary>> GetTaskEvaluationHeaderSummaryAsync(int startYear, int endYear, int? startMonth = null, int? endMonth = null, int? unitId = null, int? deptId = null, int? locationId = null)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaderList = new List<TaskEvaluationHeaderSummary>();
            if (locationId != null && locationId > 0)
            {
                if (unitId != null && unitId > 0)
                {
                    if (startMonth != null && startMonth > 0)
                    {
                        if (endMonth == null || endMonth < 1) { endMonth = 12; }
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetSummaryByUnitIdAndLocationIdAndEvaluationYearAndEvaluationMonthAsync(unitId.Value, locationId.Value, startYear, endYear, startMonth.Value, endMonth.Value);
                    }
                    else
                    {
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetSummaryByUnitIdAndLocationIdAndEvaluationYearAsync(unitId.Value, locationId.Value, startYear, endYear);
                    }
                }
                else if (deptId != null && deptId > 0)
                {
                    if (startMonth != null && startMonth > 0)
                    {
                        if (endMonth == null || endMonth < 1) { endMonth = 12; }
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetSummaryByDepartmentIdAndLocationIdAndEvaluationYearAndEvaluationMonthAsync(deptId.Value, locationId.Value, startYear, endYear, startMonth.Value, endMonth.Value);
                    }
                    else
                    {
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetSummaryByDepartmentIdAndLocationIdAndEvaluationYearAsync(deptId.Value, locationId.Value, startYear, endYear);
                    }
                }
                else
                {
                    if (startMonth != null && startMonth > 0)
                    {
                        if (endMonth == null || endMonth < 1) { endMonth = 12; }
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetSummaryByLocationIdAndEvaluationYearAndEvaluationMonthAsync(locationId.Value, startYear, endYear, startMonth.Value, endMonth.Value);
                    }
                    else
                    {
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetSummaryByLocationIdAndEvaluationYearAsync(locationId.Value, startYear, endYear);
                    }
                }
            }
            else
            {
                if (unitId != null && unitId > 0)
                {
                    if (startMonth != null && startMonth > 0)
                    {
                        if (endMonth == null || endMonth < 1) { endMonth = 12; }
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetSummaryByUnitIdAndEvaluationYearAndEvaluationMonthAsync(unitId.Value, startYear, endYear, startMonth.Value, endMonth.Value);
                    }
                    else
                    {
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetSummaryByUnitIdAndEvaluationYearAsync(unitId.Value, startYear, endYear);
                    }
                }
                else if (deptId != null && deptId > 0)
                {
                    if (startMonth != null && startMonth > 0)
                    {
                        if (endMonth == null || endMonth < 1) { endMonth = 12; }
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetSummaryByDepartmentIdAndEvaluationYearAndEvaluationMonthAsync(deptId.Value, startYear, endYear, startMonth.Value, endMonth.Value);
                    }
                    else
                    {
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetSummaryByDepartmentIdAndEvaluationYearAsync(deptId.Value, startYear, endYear);
                    }
                }
                else
                {
                    if (startMonth != null && startMonth > 0)
                    {
                        if (endMonth == null || endMonth < 1) { endMonth = 12; }
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetSummaryByEvaluationYearAndEvaluationMonthAsync(startYear, endYear, startMonth.Value, endMonth.Value);
                    }
                    else
                    {
                        taskEvaluationHeaderList = await _taskEvaluationHeaderRepository.GetSummaryByEvaluationYearAsync(startYear, endYear);
                    }
                }
            }
            return taskEvaluationHeaderList;
        }


        public async Task<int> CreateTaskEvaluationHeaderAsync(TaskEvaluationHeader header)
        {
            if (header == null) { throw new ArgumentNullException(nameof(header), "The required parameter [Task Evaluation Header] is missing."); }
            int headerId = 0;

            var entities = await _taskEvaluationHeaderRepository.GetByTaskListIdAndEvaluatorIdAsync(header.TaskListId, header.EvaluatorId);
            if (entities != null && entities.Count > 0 && entities[0].Id > 0)
            {
                headerId = entities[0].Id;
            }
            else
            {
                headerId = await _taskEvaluationHeaderRepository.AddAsync(header);
            }
            return headerId;
        }

        public async Task<bool> UpdateTaskEvaluationHeaderAsync(TaskEvaluationHeader header)
        {
            if (header == null) { throw new ArgumentNullException(nameof(header), "The required parameter [Task Evaluation Header] is missing."); }

            return await _taskEvaluationHeaderRepository.UpdateAsync(header);
        }

        #endregion

        #region Task Evaluation Detail Service Action Methods
        public async Task<TaskEvaluationDetail> GetTaskEvaluationDetailAsync(int taskEvaluationHeaderId, long taskItemId)
        {
            TaskEvaluationDetail detail = new TaskEvaluationDetail();
            var entities = await _taskEvaluationDetailRepository.GetByTaskEvaluationHeaderIdAndTaskItemIdAsync(taskEvaluationHeaderId, taskItemId);
            if (entities != null && entities.Count > 0)
            {
                detail = entities.FirstOrDefault();
            }
            return detail;
        }

        public async Task<List<TaskEvaluationDetail>> GetTaskEvaluationDetailsAsync(int taskEvaluationHeaderId)
        {
            List<TaskEvaluationDetail> details = new List<TaskEvaluationDetail>();
            var entities = await _taskEvaluationDetailRepository.GetByTaskEvaluationHeaderIdAsync(taskEvaluationHeaderId);
            if (entities != null && entities.Count > 0)
            {
                details = entities;
            }
            return details;
        }

        public async Task<bool> AddTaskEvaluationDetailAsync(TaskEvaluationDetail detail)
        {
            if (detail == null) { throw new ArgumentNullException(nameof(detail), "The required parameter [Task Evaluation Detail] is missing."); }
            bool IsAdded = false;

            var entities = await _taskEvaluationDetailRepository.GetByTaskEvaluationHeaderIdAndTaskItemIdAsync(detail.TaskEvaluationHeaderId, detail.TaskItemId);
            if (entities != null && entities.Count > 0 && entities[0].TaskEvaluationDetailId > 0)
            {
                IsAdded = await _taskEvaluationDetailRepository.UpdateAsync(detail);
                if (IsAdded)
                {
                    TaskItemActivityHistory taskHistory = new TaskItemActivityHistory
                    {
                        ActivityTime = detail.EvaluationDate,
                        ActivityBy = detail.EvaluatorName,
                        ActivityDescription = $"Task was re-evaluated by [{detail.EvaluatorName}] on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT.",
                        TaskItemId = detail.TaskItemId,
                        EntityId = (object)detail.TaskItemId,
                    };
                    await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                }
            }
            else
            {
                IsAdded = await _taskEvaluationDetailRepository.AddAsync(detail);
                if (IsAdded)
                {
                    TaskItemActivityHistory taskHistory = new TaskItemActivityHistory
                    {
                        ActivityTime = detail.EvaluationDate,
                        ActivityBy = detail.EvaluatorName,
                        ActivityDescription = $"Task evaluated by [{detail.EvaluatorName}] on {DateTime.UtcNow.ToLongDateString()} at exactly {DateTime.UtcNow.ToLongTimeString()} GMT.",
                        TaskItemId = detail.TaskItemId,
                        EntityId = (object)detail.TaskItemId,
                    };
                    await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskHistory);
                }
            }
            return IsAdded;
        }

        public async Task<bool> UpdateTaskEvaluationDetailAsync(TaskEvaluationDetail detail)
        {
            if (detail == null) { throw new ArgumentNullException(nameof(detail), "The required parameter [Task Evaluation Detail] is missing."); }

            return await _taskEvaluationDetailRepository.UpdateAsync(detail);
        }

        #endregion

        #region Task Item Evaluation Service Action Methods
        public async Task<List<TaskItemEvaluation>> GetTaskItemEvaluationsAsync(int taskListId, string evaluatorId)
        {
            List<TaskItemEvaluation> itemEvaluations = new List<TaskItemEvaluation>();
            var entities = await _taskItemEvaluationRepository.GetByTaskListIdAndEvaluatorIdAsync(taskListId, evaluatorId);
            if (entities != null && entities.Count > 0)
            {
                itemEvaluations = entities;
            }
            return itemEvaluations;
        }
        #endregion
    }
}
