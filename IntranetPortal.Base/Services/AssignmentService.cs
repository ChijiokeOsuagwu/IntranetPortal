using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Base.Repositories.AtsRepositories;
using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.BusinessManagerRepositories;
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
    public class AssignmentService : IAssignmentService
    {
        private readonly IDeskspaceRepository _deskspaceRepository;
        private readonly IUtilityRepository _utilityRepository;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IProgramRepository _programRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IBusinessManagerService _businessManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IWspService _wspService;

        public AssignmentService(IDeskspaceRepository deskspaceRepository, IUtilityRepository utilityRepository,
                                    IProgramRepository programRepository, IEmployeesRepository employeesRepository,
                                    IAssignmentRepository assignmentRepository, IBusinessRepository businessRepository,
                                    IBusinessManagerService businessManagerService, IGlobalSettingsService globalSettingsService,
                                    IBaseModelService baseModelService, IWspService wspService)
        {
            _deskspaceRepository = deskspaceRepository;
            _utilityRepository = utilityRepository;
            _programRepository = programRepository;
            _employeesRepository = employeesRepository;
            _assignmentRepository = assignmentRepository;
            _businessRepository = businessRepository;
            _businessManagerService = businessManagerService;
            _globalSettingsService = globalSettingsService;
            _baseModelService = baseModelService;
            _wspService = wspService;
        }

        #region Assignments Service Actions

        #region Assignments Read Service Actions
        public async Task<List<Assignment>> GetAssignmentsAsync(string ClientId, DateTime? EventStartDate = null, DateTime? EventEndDate = null)
        {
            List<Assignment> listOfAssignments = new List<Assignment>();
            if (!string.IsNullOrWhiteSpace(ClientId))
            {
                var entities = await _assignmentRepository.GetAssignmentsByClientIdAsync(ClientId, EventStartDate, EventEndDate);
                if (entities != null) { listOfAssignments = entities; }
            }
            else
            {
                var entities = await _assignmentRepository.GetAssignmentsByDateRangeAsync(EventStartDate, EventEndDate);
                if (entities != null) { listOfAssignments = entities; }
            }

            return listOfAssignments;
        }
        public async Task<Assignment> GetAssignmentAsync(long AssignmentId)
        {
            return await _assignmentRepository.GetAssignmentByIdAsync(AssignmentId);
        }

        #endregion

        #region Assignment Write Service Actions
        public async Task<bool> CreateNewAssignmentAsync(Assignment assignment)
        {
            bool isCreated = false;
            if (assignment == null) { throw new Exception("Invalid parameter value [assignment]."); }
            assignment.No = await GetNewAssignmentNumberAsync();
            var clientInfo = await _businessRepository.GetCustomerByNameAsync(assignment.ClientName);
            if (clientInfo == null | string.IsNullOrWhiteSpace(clientInfo.BusinessID))
            {
                Business client = new Business();
                client.BusinessID = Guid.NewGuid().ToString();
                client.BusinessNumber = await _businessManagerService.GetNewCodeNumber();
                client.BusinessName = assignment.ClientName;
                client.IsCustomer = true;
                client.CreatedBy = assignment.CreatedBy;
                client.CreatedTime = DateTime.Now;
                if (await _businessRepository.AddAsync(client))
                {
                    assignment.ClientId = client.BusinessID;
                    if (!string.IsNullOrWhiteSpace(assignment.ContactPerson))
                    {
                        BusinessContact contact = new BusinessContact();
                        contact.BusinessID = client.BusinessID;
                        contact.ContactName = assignment.ContactPerson;
                        contact.ContactPhone1 = assignment.ContactPhone;
                        contact.CreatedBy = assignment.CreatedBy;
                        contact.CreatedTime = DateTime.Now;
                        await _businessManagerService.CreateBusinessContactAsync(contact);
                    }
                }
            }
            else { assignment.ClientId = clientInfo.BusinessID; }
            if (!string.IsNullOrWhiteSpace(assignment.EventState))
            {
                State state = await _globalSettingsService.GetStateAsync(assignment.EventState);
                if (state != null)
                {
                    assignment.EventCountry = state.Country;
                }
            }
            if (string.IsNullOrWhiteSpace(assignment.AssignedToName))
            {
                assignment.Id = await _assignmentRepository.AddAssignmentAsync(assignment);
                isCreated = assignment.Id > 0;
                AssignmentHistory history = new AssignmentHistory();
                history.ActivityBy = assignment.AssignedByName;
                history.ActivityDescription = $"Assignment was Created by {assignment.CreatedBy} on {DateTime.Now.ToString("f")}, but has not been assigned to anyone yet.";
                history.ActivityTime = DateTime.Now;
                history.AssignmentId = assignment.Id.Value;
                await _assignmentRepository.AddAssignmentHistoryAsync(history);
            }
            else
            {
                var _assignedToEmployee = await _employeesRepository.GetEmployeeByNameAsync(assignment.AssignedToName);
                if (_assignedToEmployee == null || string.IsNullOrWhiteSpace(_assignedToEmployee.EmployeeID)) { throw new Exception("No employee record was found for the assigned employee. "); }
                assignment.AssignedToId = _assignedToEmployee.EmployeeID;
                assignment.Id = await _assignmentRepository.AddAssignmentAsync(assignment);
                isCreated = assignment.Id > 0;
                if (isCreated)
                {
                    AssignmentHistory history = new AssignmentHistory();
                    history.ActivityBy = assignment.CreatedBy;
                    history.ActivityDescription = $"Assignment was Created by {assignment.CreatedBy} on {DateTime.Now.ToString("f")} and assigned to {assignment.AssignedToName}.";
                    history.ActivityTime = DateTime.Now;
                    history.AssignmentId = assignment.Id.Value;
                    await _assignmentRepository.AddAssignmentHistoryAsync(history);

                    AssignmentCrewMember assignmentCrewMember = new AssignmentCrewMember();
                    assignmentCrewMember.AssignmentId = assignment.Id.Value;
                    assignmentCrewMember.CrewMemberId = _assignedToEmployee.EmployeeID;
                    assignmentCrewMember.CrewMemberName = assignment.AssignedToName;
                    assignmentCrewMember.CrewMemberRole1 = assignment.AssignedToRole;
                    assignmentCrewMember.DepartmentId = _assignedToEmployee.DepartmentID;
                    assignmentCrewMember.LocationId = _assignedToEmployee.LocationID;
                    assignmentCrewMember.UnitId = _assignedToEmployee.UnitID;
                    assignmentCrewMember.IsTeamLead = true;

                    if (await _assignmentRepository.AddAssignmentCrewMemberAsync(assignmentCrewMember) > 0)
                    {
                        TaskItem assignmentTask = new TaskItem();
                        assignmentTask.AssignmentId = assignment.Id;
                        assignmentTask.TaskOwnerId = assignment.AssignedToId;
                        assignmentTask.AssignedByEmployeeId = assignment.AssignedById;
                        assignmentTask.CreatedBy = assignment.CreatedBy;
                        assignmentTask.CreatedTime = assignment.CreatedTime;
                        string taskCodeNumber = await _baseModelService.GenerateAutoNumberAsync("taskno");
                        assignmentTask.Number = $"T{taskCodeNumber}";
                        assignmentTask.Description = $"Coverage of assignment [{assignment.No}] - {assignment.Title}";
                        assignmentTask.ExpectedStartTime = assignment.EventStartTime;
                        assignmentTask.ExpectedDueTime = assignment.ReportDueDate;
                        assignmentTask.AssignedByEmployeeId = assignment.AssignedById;
                        assignmentTask.AssignedToId = _assignedToEmployee.EmployeeID;
                        assignmentTask.UnitId = _assignedToEmployee.UnitID;
                        assignmentTask.DepartmentId = _assignedToEmployee.DepartmentID;
                        assignmentTask.LocationId = _assignedToEmployee.LocationID;

                        assignmentTask.Id = await _wspService.CreateTaskItemAsync(assignmentTask);
                    }
                }
            }
            return isCreated;
        }
        public async Task<bool> EditAssignmentAsync(Assignment assignment)
        {
            if (assignment == null) { throw new Exception("Invalid parameter value [assignment]."); }
            var clientInfo = await _businessRepository.GetCustomerByNameAsync(assignment.ClientName);
            if (clientInfo == null || string.IsNullOrWhiteSpace(clientInfo.BusinessID))
            {
                Business client = new Business();
                client.BusinessID = Guid.NewGuid().ToString();
                client.BusinessNumber = await _businessManagerService.GetNewCodeNumber();
                client.BusinessName = assignment.ClientName;
                client.IsCustomer = true;
                client.CreatedBy = assignment.ModifiedBy;
                client.CreatedTime = DateTime.Now;
                if (await _businessRepository.AddAsync(client))
                {
                    assignment.ClientId = client.BusinessID;
                    if (!string.IsNullOrWhiteSpace(assignment.ContactPerson))
                    {
                        BusinessContact contact = new BusinessContact();
                        contact.BusinessID = client.BusinessID;
                        contact.ContactName = assignment.ContactPerson;
                        contact.ContactPhone1 = assignment.ContactPhone;
                        contact.CreatedBy = assignment.CreatedBy;
                        contact.CreatedTime = DateTime.Now;
                        await _businessManagerService.CreateBusinessContactAsync(contact);
                    }
                }
            }
            else
            {
                assignment.ClientId = clientInfo.BusinessID;
            }

            if (!string.IsNullOrWhiteSpace(assignment.EventState))
            {
                State state = await _globalSettingsService.GetStateAsync(assignment.EventState);
                if (state != null)
                {
                    assignment.EventCountry = state.Country;
                }
            }

            Assignment oldAssignmentEntity = await _assignmentRepository.GetAssignmentByIdAsync(assignment.Id.Value);
            if (oldAssignmentEntity == null)
            {
                throw new Exception("Sorry no record was found for this Assignment in the system.");
            }
            else
            {
                bool isUpdated = await _assignmentRepository.UpdateAssignmentAsync(assignment);
                if (isUpdated)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"{assignment.ModifiedBy} made some changes on {DateTime.Now.ToString("f")}. ");
                    if (oldAssignmentEntity.ApprovalStatus != assignment.ApprovalStatus)
                    {
                        sb.Append($"The Assignment Approval Status was changed from [{oldAssignmentEntity.ApprovalStatus}] to [{assignment.ApprovalStatus}]. ");
                    }
                    if (oldAssignmentEntity.Title != assignment.Title)
                    {
                        sb.Append($"The Assignment Title was changed from [{oldAssignmentEntity.Title}] to [{assignment.Title}]. ");
                    }
                    if (oldAssignmentEntity.EventVenue != assignment.EventVenue)
                    {
                        if (string.IsNullOrWhiteSpace(oldAssignmentEntity.EventVenue))
                        {
                            if (assignment.EventVenue != null)
                            {
                                sb.Append($"Event venue [{assignment.EventVenue}] was added. ");
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(assignment.EventVenue))
                            {
                                sb.Append($"The previous Event Venue of [{oldAssignmentEntity.EventVenue}] was changed to [{assignment.EventVenue}]. ");
                            }
                            else
                            {
                                sb.Append($"The previous Event Venue of [{oldAssignmentEntity.EventVenue}] was deleted.");
                            }
                        }
                    }
                    if (oldAssignmentEntity.Description != assignment.Description)
                    {
                        if (string.IsNullOrWhiteSpace(oldAssignmentEntity.Description))
                        {
                            if (assignment.Description != null)
                            {
                                sb.Append($"the following Event Description: [{assignment.Description}] was added. ");
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(assignment.Description))
                            {
                                sb.Append($"The previous Event Description of [{oldAssignmentEntity.Description}] was changed to [{assignment.Description}]. ");
                            }
                            else
                            {
                                sb.Append($"The previous Event Description of [{oldAssignmentEntity.Description}] was deleted.");
                            }
                        }
                    }
                    if (oldAssignmentEntity.EventStartTime != assignment.EventStartTime)
                    {
                        if (oldAssignmentEntity.EventStartTime == null)
                        {
                            if (assignment.EventStartTime != null)
                            {
                                sb.Append($"the following Event Start Time: [{assignment.EventStartTime}] was added. ");
                            }
                        }
                        else
                        {
                            if (assignment.EventStartTime == null)
                            {
                                sb.Append($"The previous Event Start Time of [{oldAssignmentEntity.EventStartTime}] was changed to [{assignment.EventStartTime}]. ");
                            }
                            else
                            {
                                sb.Append($"The previous Event Start Time of [{oldAssignmentEntity.EventStartTime}] was deleted.");
                            }
                        }
                    }
                    if (oldAssignmentEntity.EventEndTime != assignment.EventEndTime)
                    {
                        if (oldAssignmentEntity.EventEndTime == null)
                        {
                            if (assignment.EventEndTime != null)
                            {
                                sb.Append($"the following Event End Time: [{assignment.EventEndTime}] was added. ");
                            }
                        }
                        else
                        {
                            if (assignment.EventEndTime == null)
                            {
                                sb.Append($"The previous Event End Time of [{oldAssignmentEntity.EventEndTime}] was changed to [{assignment.EventEndTime}]. ");
                            }
                            else
                            {
                                sb.Append($"The previous Event End Time of [{oldAssignmentEntity.EventEndTime}] was deleted.");
                            }
                        }
                    }
                    if (oldAssignmentEntity.ReportDueDate != assignment.ReportDueDate)
                    {
                        if (oldAssignmentEntity.ReportDueDate == null)
                        {
                            if (assignment.ReportDueDate != null)
                            {
                                sb.Append($"the following Report Due Date: [{assignment.ReportDueDate}] was added. ");
                            }
                        }
                        else
                        {
                            if (assignment.ReportDueDate == null)
                            {
                                sb.Append($"The previous Event Report Due Date of [{oldAssignmentEntity.ReportDueDate}] was changed to [{assignment.ReportDueDate}]. ");
                            }
                            else
                            {
                                sb.Append($"The previous Event Report Due Date of [{oldAssignmentEntity.ReportDueDate}] was deleted.");
                            }
                        }
                    }
                    if (oldAssignmentEntity.EventState != assignment.EventState)
                    {
                        if (string.IsNullOrWhiteSpace(oldAssignmentEntity.EventState))
                        {
                            if (assignment.EventState != null)
                            {
                                sb.Append($"the following Event EventState: [{assignment.EventState}] was added. ");
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(assignment.EventState))
                            {
                                sb.Append($"The previous Event EventState of [{oldAssignmentEntity.EventState}] was changed to [{assignment.EventState}]. ");
                            }
                            else
                            {
                                sb.Append($"The previous Event EventState of [{oldAssignmentEntity.EventState}] was deleted.");
                            }
                        }
                    }
                    if (oldAssignmentEntity.StationId != assignment.StationId)
                    {
                        if (oldAssignmentEntity.StationId == null)
                        {
                            if (assignment.StationId != null)
                            {
                                sb.Append($"The Coordinating Station was added. ");
                            }
                        }
                        else
                        {
                            if (assignment.StationId == null)
                            {
                                sb.Append($"The previous Coordinating Station of [{oldAssignmentEntity.StationName}] was changed. ");
                            }
                            else
                            {
                                sb.Append($"The previous Coordinating Station of [{oldAssignmentEntity.StationName}] was deleted.");
                            }
                        }
                    }
                    if (oldAssignmentEntity.ProgressStatus != assignment.ProgressStatus)
                    {
                        if (string.IsNullOrWhiteSpace(oldAssignmentEntity.ProgressStatus))
                        {
                            if (assignment.ProgressStatus != null)
                            {
                                sb.Append($"The following Progress Status: [{assignment.ProgressStatus}] was added. ");
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(assignment.ProgressStatus))
                            {
                                sb.Append($"The previous Progress Status of [{oldAssignmentEntity.ProgressStatus}] was changed to [{assignment.ProgressStatus}]. ");
                            }
                            else
                            {
                                sb.Append($"The previous Progress Status of [{oldAssignmentEntity.ProgressStatus}] was deleted.");
                            }
                        }
                    }
                    if (oldAssignmentEntity.IsPaid != assignment.IsPaid)
                    {
                        sb.Append($"Changed Is Paid to {assignment.IsPaid}. ");
                    }
                    if (oldAssignmentEntity.IsConfirmed != assignment.IsConfirmed)
                    {
                        sb.Append($"Changed Is Confirmed to {assignment.IsConfirmed}. ");
                    }
                    if (oldAssignmentEntity.IsLive != assignment.IsLive)
                    {
                        sb.Append($"Changed Is Live to {assignment.IsLive}. ");
                    }
                    if (oldAssignmentEntity.IsPriority != assignment.IsPriority)
                    {
                        sb.Append($"Changed Is Priority to {assignment.IsPriority}. ");
                    }

                    AssignmentHistory history = new AssignmentHistory();
                    history.ActivityBy = assignment.CreatedBy;
                    history.ActivityDescription = sb.ToString();
                    history.ActivityTime = DateTime.Now;
                    history.AssignmentId = assignment.Id.Value;
                    await _assignmentRepository.AddAssignmentHistoryAsync(history);
                }
                return isUpdated;
            }
        }
        public async Task<bool> DeleteAssignmentAsync(long assignmentId)
        {
            return await _assignmentRepository.DeleteAssignmentAsync(assignmentId);
        }
        #endregion

        #endregion

        #region Assignment Crew Service Actions
        public async Task<AssignmentCrewMember> GetAssignmentCrewMemberAsync(long AssignmentCrewId)
        {
            AssignmentCrewMember crewMember = new AssignmentCrewMember();
            if (AssignmentCrewId > 0)
            {
                var entity = await _assignmentRepository.GetAssignmentCrewMemberbyIdAsync(AssignmentCrewId);
                if (entity != null) { crewMember = entity; }
            }
            return crewMember;
        }
        public async Task<AssignmentCrewMember> GetAssignmentCrewMemberAsync(long AssignmentId, string EmployeeId)
        {
            AssignmentCrewMember crewMember = new AssignmentCrewMember();
            if (AssignmentId > 0 && !string.IsNullOrWhiteSpace(EmployeeId))
            {
                var entity = await _assignmentRepository.GetAssignmentCrewMemberbyAssignmentIdnEmployeeIdAsync(AssignmentId, EmployeeId);
                if (entity != null) { crewMember = entity; }
            }
            return crewMember;
        }
        public async Task<List<AssignmentCrewMember>> GetAssignmentCrewMembersAsync(long AssignmentId)
        {
            List<AssignmentCrewMember> listOfCrewMembers = new List<AssignmentCrewMember>();
            if (AssignmentId > 0)
            {
                var entities = await _assignmentRepository.GetAssignmentCrewMembersbyAssignmentIdAsync(AssignmentId);
                if (entities != null) { listOfCrewMembers = entities; }
            }

            return listOfCrewMembers;
        }
        public async Task<List<Employee>> GetAssignmentEmployeesAsync(long AssignmentId)
        {
            List<Employee> listOfEmployees = new List<Employee>();
            if (AssignmentId > 0)
            {
                var entities = await _assignmentRepository.GetAssignmentEmployeesByAssignmentIdAsync(AssignmentId);
                if (entities != null) { listOfEmployees = entities; }
            }
            return listOfEmployees;
        }



        public async Task<long> AddAssignmentCrewMemberAsync(AssignmentCrewMember crewMember)
        {
            long newId = 0L;
            if (crewMember == null) { throw new ArgumentNullException(nameof(crewMember), "The required parameter [Assignment Crew Member] is missing."); }
            //var entities = await _assignmentRepository.GetAssignmentCrewMembersbyCrewMemberIdAsync(crewMember.CrewMemberId);
            var entity = await _assignmentRepository.GetAssignmentCrewMemberbyAssignmentIdnEmployeeIdAsync(crewMember.AssignmentId, crewMember.CrewMemberId);
            if (entity == null || entity.Id == null || entity.Id == 0)
            {
                newId = await _assignmentRepository.AddAssignmentCrewMemberAsync(crewMember);
                if (newId > 0)
                {
                    Employee employee = await _employeesRepository.GetEmployeeByNameAsync(crewMember.CreatedBy);
                    if (employee != null)
                    {
                        crewMember.AssignedByEmployeeId = employee.EmployeeID;
                        crewMember.AssignedByEmployeeName = employee.FullName;
                    }

                    Assignment assignment = await _assignmentRepository.GetAssignmentByIdAsync(crewMember.AssignmentId);
                    if (assignment != null)
                    {
                        TaskItem assignmentTask = new TaskItem();
                        assignmentTask.AssignmentId = crewMember.AssignmentId;
                        assignmentTask.TaskOwnerId = crewMember.CrewMemberId;
                        assignmentTask.AssignedByEmployeeId = crewMember.AssignedByEmployeeId;
                        assignmentTask.CreatedBy = crewMember.CreatedBy;
                        assignmentTask.CreatedTime = crewMember.CreatedTime;
                        string taskCodeNumber = await _baseModelService.GenerateAutoNumberAsync("taskno");
                        assignmentTask.Number = $"T{taskCodeNumber}";
                        assignmentTask.Description = $"Coverage of the assignment: [{assignment.No}] - {crewMember.AssignmentTitle}].";
                        assignmentTask.ExpectedStartTime = assignment.EventStartTime;
                        assignmentTask.ExpectedDueTime = assignment.ReportDueDate;
                        assignmentTask.AssignedByEmployeeId = assignment.AssignedById;
                        assignmentTask.AssignedToId = crewMember.CrewMemberId;
                        assignmentTask.UnitId = crewMember.UnitId;
                        assignmentTask.DepartmentId = crewMember.DepartmentId;
                        assignmentTask.LocationId = crewMember.LocationId;

                        assignmentTask.Id = await _wspService.CreateTaskItemAsync(assignmentTask);
                    }
                }

                AssignmentHistory history = new AssignmentHistory();
                history.ActivityBy = crewMember.CreatedBy;
                history.ActivityDescription = $"New member [{crewMember.CrewMemberName}] was added to the crew by {crewMember.CreatedBy} on {DateTime.Now.ToString("f")}.";
                history.ActivityTime = DateTime.Now;
                history.AssignmentId = crewMember.AssignmentId;
                await _assignmentRepository.AddAssignmentHistoryAsync(history);
            }
            else
            {
                throw new Exception("Sorry, this name has already been added.");
            }
            return newId;
        }
        public async Task<bool> UpdateAssignmentCrewMemberAsync(AssignmentCrewMember crewMember)
        {
            if (crewMember == null) { throw new ArgumentNullException(nameof(crewMember), "The required parameter [Assignment Crew Member] is missing."); }
            if (crewMember.Id > 0)
            {
                AssignmentCrewMember oldCrewMember = await _assignmentRepository.GetAssignmentCrewMemberbyIdAsync(crewMember.Id.Value);
                if (oldCrewMember == null || oldCrewMember.Id < 1 || string.IsNullOrWhiteSpace(oldCrewMember.CrewMemberName))
                {
                    throw new Exception("Sorry, no record was found for this crew membership. Refresh you page to make sure it has not been deleted, then try again.");
                }
                else
                {
                    if (await _assignmentRepository.UpdateAssignmentCrewMemberAsync(crewMember))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"Some changes were made to the crew membership of {oldCrewMember.CrewMemberName} by {crewMember.ModifiedBy} on {DateTime.Now.ToString("f")}. ");

                        if (oldCrewMember.CrewMemberName != crewMember.CrewMemberName)
                        {
                            sb.Append($"The previous member [{oldCrewMember.CrewMemberName}] was replaced with [{crewMember.CrewMemberName}]. ");
                        }

                        if (oldCrewMember.CrewMemberRole1 != crewMember.CrewMemberRole1)
                        {
                            if (string.IsNullOrWhiteSpace(oldCrewMember.CrewMemberRole1))
                            {
                                if (!string.IsNullOrWhiteSpace(crewMember.CrewMemberRole1))
                                {
                                    sb.Append($"New role of [{crewMember.CrewMemberRole1}] was added. ");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(crewMember.CrewMemberRole1))
                                {
                                    sb.Append($"The previous role of [{oldCrewMember.CrewMemberRole1}] was replaced with [{crewMember.CrewMemberRole3}]. ");
                                }
                                else
                                {
                                    sb.Append($"The previous role of [{oldCrewMember.CrewMemberRole1}] was removed. ");
                                }
                            }
                        }

                        if (oldCrewMember.CrewMemberRole2 != crewMember.CrewMemberRole2)
                        {
                            if (string.IsNullOrWhiteSpace(oldCrewMember.CrewMemberRole2))
                            {
                                if (!string.IsNullOrWhiteSpace(crewMember.CrewMemberRole2))
                                {
                                    sb.Append($"New role of [{crewMember.CrewMemberRole2}] was added. ");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(crewMember.CrewMemberRole2))
                                {
                                    sb.Append($"The previous role of [{oldCrewMember.CrewMemberRole2}] was replaced with [{crewMember.CrewMemberRole3}]. ");
                                }
                                else
                                {
                                    sb.Append($"The previous role of [{oldCrewMember.CrewMemberRole2}] was removed. ");
                                }
                            }
                        }

                        if (oldCrewMember.CrewMemberRole3 != crewMember.CrewMemberRole3)
                        {
                            if (string.IsNullOrWhiteSpace(oldCrewMember.CrewMemberRole3))
                            {
                                if (!string.IsNullOrWhiteSpace(crewMember.CrewMemberRole3))
                                {
                                    sb.Append($"New role of [{crewMember.CrewMemberRole3}] was added. ");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(crewMember.CrewMemberRole3))
                                {
                                    sb.Append($"The previous role of [{oldCrewMember.CrewMemberRole3}] was replaced with [{crewMember.CrewMemberRole3}]. ");
                                }
                                else
                                {
                                    sb.Append($"The previous role of [{oldCrewMember.CrewMemberRole3}] was removed. ");
                                }
                            }
                        }

                        AssignmentHistory history = new AssignmentHistory();
                        history.ActivityBy = crewMember.ModifiedBy;
                        history.ActivityDescription = sb.ToString();
                        history.ActivityTime = DateTime.Now;
                        history.AssignmentId = crewMember.AssignmentId;
                        await _assignmentRepository.AddAssignmentHistoryAsync(history);
                        return true;
                    }
                }
            }
            return false;
        }
        public async Task<bool> RemoveAssignmentCrewMemberAsync(AssignmentCrewMember crewMember)
        {
            if (crewMember == null) { throw new ArgumentNullException(nameof(crewMember), "The required parameter [Assignment Crew Member] is missing."); }
            var entities = await _assignmentRepository.GetAssignmentCrewMembersbyCrewMemberIdAsync(crewMember.CrewMemberId);
            if (crewMember != null && crewMember.Id > 0)
            {
                if (await _assignmentRepository.DeleteAssignmentCrewMemberAsync(crewMember.Id.Value))
                {
                    TaskItem taskItem = await _deskspaceRepository.GetTaskItemByOwnerIdnAssignmentIdAsync(crewMember.CrewMemberId, crewMember.AssignmentId);
                    if (taskItem != null)
                    {
                        if (await _wspService.DeleteTaskItemAsync(taskItem.Id))
                        {
                            AssignmentHistory history = new AssignmentHistory();
                            history.ActivityBy = crewMember.ModifiedBy;
                            history.ActivityDescription = $"{crewMember.CrewMemberName} was removed from the crew by {crewMember.ModifiedBy} on {DateTime.Now.ToString("f.")}.";
                            history.ActivityTime = DateTime.Now;
                            history.AssignmentId = crewMember.AssignmentId;

                            await _assignmentRepository.AddAssignmentHistoryAsync(history);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public async Task<bool> UpdateAssignmentCrewLeadAsync(long AssignmentCrewId, bool IsCrewLead, string updatedBy)
        {
            if (AssignmentCrewId < 1) { throw new ArgumentNullException(nameof(AssignmentCrewId), "The required parameter [Assignment Crew ID] is missing."); }

            AssignmentCrewMember crewMember = await _assignmentRepository.GetAssignmentCrewMemberbyIdAsync(AssignmentCrewId);
            if (crewMember == null || crewMember.Id < 1 || string.IsNullOrWhiteSpace(crewMember.CrewMemberName))
            {
                throw new Exception("Sorry, no record was found for this crew membership. Refresh you page to make sure it has not been deleted, then try again.");
            }
            else
            {
                if (await _assignmentRepository.UpdateAssignmentCrewLeadAsync(AssignmentCrewId, IsCrewLead, updatedBy))
                {
                    AssignmentHistory history = new AssignmentHistory();
                    history.ActivityBy = crewMember.ModifiedBy;
                    history.ActivityTime = DateTime.Now;
                    history.AssignmentId = crewMember.AssignmentId;

                    if (IsCrewLead)
                    {
                        history.ActivityDescription = $"{crewMember.CrewMemberName} was appointed as Team Lead by {updatedBy} on {DateTime.Now.ToString("f.")}.";
                    }
                    else
                    {
                        history.ActivityDescription = $"{crewMember.CrewMemberName} was removed as Team Lead by {updatedBy} on {DateTime.Now.ToString("f.")}.";
                    }
                    await _assignmentRepository.AddAssignmentHistoryAsync(history);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Assignment Crew Report Service Actions
        public async Task<AssignmentCrewReport> GetAssignmentCrewReportAsync(long AssignmentCrewReportId)
        {
            AssignmentCrewReport crewMemberReport = new AssignmentCrewReport();
            if (AssignmentCrewReportId > 0)
            {
                var entity = await _assignmentRepository.GetAssignmentCrewReportByIdAsync(AssignmentCrewReportId);
                if (entity != null) { crewMemberReport = entity; }
            }
            return crewMemberReport;
        }
        public async Task<List<AssignmentCrewReport>> GetAssignmentCrewReportAsync(long AssignmentId, string EmployeeId)
        {
            List<AssignmentCrewReport> crewMemberReports = new List<AssignmentCrewReport>();
            if (AssignmentId > 0 && !string.IsNullOrWhiteSpace(EmployeeId))
            {
                var entities = await _assignmentRepository.GetAssignmentCrewReportsByAssignmentIdnEmployeeIdAsync(AssignmentId, EmployeeId);
                if (entities != null) { crewMemberReports = entities; }
            }
            return crewMemberReports;
        }
        public async Task<List<AssignmentCrewReport>> GetAssignmentCrewReportsAsync(long AssignmentId)
        {
            List<AssignmentCrewReport> listOfCrewReports = new List<AssignmentCrewReport>();
            if (AssignmentId > 0)
            {
                var entities = await _assignmentRepository.GetAssignmentCrewReportsByAssignmentIdAsync(AssignmentId);
                if (entities != null) { listOfCrewReports = entities; }
            }
            return listOfCrewReports;
        }
        public async Task<long> AddAssignmentCrewReportAsync(AssignmentCrewReport crewMemberReport)
        {
            long newId = 0L;
            if (crewMemberReport == null) { throw new ArgumentNullException(nameof(crewMemberReport), "The required parameter [Crew Report] is missing."); }
            var entities = await _assignmentRepository.GetAssignmentCrewReportsByAssignmentIdnEmployeeIdAsync(crewMemberReport.AssignmentId, crewMemberReport.EmployeeId);
            if (entities == null || entities.Count < 1)
            {
                newId = await _assignmentRepository.AddAssignmentCrewReportAsync(crewMemberReport);
                if (newId > 0)
                {
                    await _assignmentRepository.UpdateAssignmentProgressStatusAsync(crewMemberReport.AssignmentId, "Coverage Completed");
                    AssignmentHistory history = new AssignmentHistory();
                    history.ActivityBy = crewMemberReport.ModifiedBy;
                    history.ActivityDescription = $"{crewMemberReport.EmployeeName} submitted a Coverage Report on {DateTime.Now.ToString("f")}.";
                    history.ActivityTime = DateTime.Now;
                    history.AssignmentId = crewMemberReport.AssignmentId;
                    await _assignmentRepository.AddAssignmentHistoryAsync(history);
                }

            }
            else
            {
                throw new Exception("Sorry, this crew member has already submitted a Coverage Report.");
            }
            return newId;
        }
        public async Task<bool> UpdateAssignmentCrewReportAsync(AssignmentCrewReport crewMemberReport)
        {
            if (crewMemberReport == null) { throw new ArgumentNullException(nameof(crewMemberReport), "The required parameter [Crew Member Report] is missing."); }
            if (crewMemberReport.CrewReportId > 0)
            {
                AssignmentCrewReport oldCrewMemberReport = await _assignmentRepository.GetAssignmentCrewReportByIdAsync(crewMemberReport.CrewReportId);
                if (oldCrewMemberReport == null || oldCrewMemberReport.CrewReportId < 1 || string.IsNullOrWhiteSpace(oldCrewMemberReport.EmployeeName))
                {
                    throw new Exception("Sorry, no record was found for this Coverage Report. Refresh you page to make sure it has not been deleted, then try again.");
                }
                else
                {
                    if (await _assignmentRepository.UpdateAssignmentCrewReportAsync(crewMemberReport))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"{crewMemberReport.EmployeeName} made some changes to his/her Coverage Report on {DateTime.Now.ToString("f")}. ");

                        if (oldCrewMemberReport.AttendanceStatus != crewMemberReport.AttendanceStatus)
                        {
                            sb.AppendLine($"The previous Attendance Status [{oldCrewMemberReport.AttendanceStatus}] was changed to [{crewMemberReport.AttendanceStatus}]. ");
                        }
                        if (oldCrewMemberReport.ArrivalType != crewMemberReport.ArrivalType)
                        {
                            if (string.IsNullOrWhiteSpace(oldCrewMemberReport.ArrivalType))
                            {
                                if (!string.IsNullOrWhiteSpace(crewMemberReport.ArrivalType))
                                {
                                    sb.AppendLine($"New Arrival Type [{crewMemberReport.ArrivalType}] was added. ");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(crewMemberReport.ArrivalType))
                                {
                                    sb.AppendLine($"The previous Arrival Type of [{oldCrewMemberReport.ArrivalType}] was changed to [{crewMemberReport.ArrivalType}]. ");
                                }
                                else
                                {
                                    sb.AppendLine($"The previous Arrival Type of [{oldCrewMemberReport.ArrivalType}] was deleted. ");
                                }
                            }
                        }
                        if (oldCrewMemberReport.DepartureType != crewMemberReport.DepartureType)
                        {
                            if (string.IsNullOrWhiteSpace(oldCrewMemberReport.DepartureType))
                            {
                                if (!string.IsNullOrWhiteSpace(crewMemberReport.DepartureType))
                                {
                                    sb.AppendLine($"New Departure Type [{crewMemberReport.DepartureType}] was added. ");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(crewMemberReport.DepartureType))
                                {
                                    sb.AppendLine($"The previous Departure Type of [{oldCrewMemberReport.DepartureType}] was changed to [{crewMemberReport.DepartureType}]. ");
                                }
                                else
                                {
                                    sb.AppendLine($"The previous Departure Type of [{oldCrewMemberReport.DepartureType}] was deleted. ");
                                }
                            }
                        }
                        if (oldCrewMemberReport.ArrivalTime != crewMemberReport.ArrivalTime)
                        {
                            if (oldCrewMemberReport.ArrivalTime == null)
                            {
                                if (crewMemberReport.ArrivalTime != null)
                                {
                                    sb.AppendLine($"Arrival Time of [{crewMemberReport.ArrivalTime?.ToString("f")}] was added. ");
                                }
                            }
                            else
                            {
                                if (crewMemberReport.ArrivalTime != null)
                                {
                                    sb.AppendLine($"The previous Arrival Time of [{oldCrewMemberReport.ArrivalTime?.ToString("f")}] was changed to [{crewMemberReport.ArrivalTime?.ToString("f")}]. ");
                                }
                                else
                                {
                                    sb.AppendLine($"The previous Arrival Time of [{oldCrewMemberReport.ArrivalTime?.ToString("f")}] was deleted.");
                                }
                            }
                        }
                        if (oldCrewMemberReport.DepartureTime != crewMemberReport.DepartureTime)
                        {
                            if (oldCrewMemberReport.DepartureTime == null)
                            {
                                if (crewMemberReport.DepartureTime != null)
                                {
                                    sb.AppendLine($"Departure Time of [{crewMemberReport.DepartureTime?.ToString("f")}] was added. ");
                                }
                            }
                            else
                            {
                                if (crewMemberReport.DepartureTime != null)
                                {
                                    sb.AppendLine($"The previous Departure Time of [{oldCrewMemberReport.DepartureTime?.ToString("f")}] was changed to [{crewMemberReport.DepartureTime?.ToString("f")}]. ");
                                }
                                else
                                {
                                    sb.AppendLine($"The previous Departure Time of [{oldCrewMemberReport.DepartureTime?.ToString("f")}] was deleted.");
                                }
                            }
                        }

                        AssignmentHistory history = new AssignmentHistory();
                        history.ActivityBy = crewMemberReport.ModifiedBy;
                        history.ActivityDescription = sb.ToString();
                        history.ActivityTime = DateTime.Now;
                        history.AssignmentId = crewMemberReport.AssignmentId;
                        await _assignmentRepository.AddAssignmentHistoryAsync(history);
                        return true;
                    }
                }
            }
            return false;
        }
        public async Task<bool> DeleteAssignmentCrewReportAsync(AssignmentCrewReport crewMemberReport)
        {
            if (crewMemberReport == null) { throw new ArgumentNullException(nameof(crewMemberReport), "The required parameter [Assignment Crew Report] is missing."); }
            var oldCrewMemberReport = await _assignmentRepository.GetAssignmentCrewReportByIdAsync(crewMemberReport.CrewReportId);
            if (oldCrewMemberReport != null)
            {

                if (crewMemberReport != null && crewMemberReport.CrewReportId > 1)
                {
                    if (await _assignmentRepository.DeleteAssignmentCrewReportAsync(crewMemberReport.CrewReportId))
                    {
                        AssignmentHistory history = new AssignmentHistory();
                        history.ActivityBy = crewMemberReport.ModifiedBy;
                        history.ActivityDescription = $"{oldCrewMemberReport.EmployeeName} deleted his/her earlier submitted Crew Participation Report on {DateTime.Now.ToString("f.")}.";
                        history.ActivityTime = DateTime.Now;
                        history.AssignmentId = crewMemberReport.AssignmentId;

                        await _assignmentRepository.AddAssignmentHistoryAsync(history);
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Assignment Equipment Service Actions
        public async Task<AssignmentEquipment> GetAssignmentEquipmentAsync(long AssignmentEquipmentId)
        {
            AssignmentEquipment assignmentEquipment = new AssignmentEquipment();
            if (AssignmentEquipmentId > 0)
            {
                var entity = await _assignmentRepository.GetAssignmentEquipmentByIdAsync(AssignmentEquipmentId);
                if (entity != null) { assignmentEquipment = entity; }
            }
            return assignmentEquipment;
        }
        public async Task<List<AssignmentEquipment>> GetAssignmentEquipmentsAsync(long AssignmentId)
        {
            List<AssignmentEquipment> listOfEquipments = new List<AssignmentEquipment>();
            if (AssignmentId > 0)
            {
                var entities = await _assignmentRepository.GetAssignmentEquipmentByAssignmentIdAsync(AssignmentId);
                if (entities != null) { listOfEquipments = entities; }
            }

            return listOfEquipments;
        }
        public async Task<long> AddAssignmentEquipmentAsync(AssignmentEquipment equipment)
        {
            long newId = 0L;
            if (equipment == null) { throw new ArgumentNullException(nameof(equipment), "The required parameter [Assignment Equipment] is missing."); }
            var entities = await _assignmentRepository.GetAssignmentEquipmentByAssetIdAsync(equipment.AssetId);
            if (entities == null || entities.Count < 1)
            {
                newId = await _assignmentRepository.AddAssignmentEquipmentAsync(equipment);
                if (newId > 0)
                {
                    AssignmentHistory history = new AssignmentHistory();
                    history.ActivityBy = equipment.ModifiedBy;
                    history.ActivityDescription = $"{equipment.AssetName} was assigned for this assignment by {equipment.ModifiedBy} on {DateTime.Now.ToString("f")}.";
                    history.ActivityTime = DateTime.Now;
                    history.AssignmentId = equipment.AssignmentId;
                    await _assignmentRepository.AddAssignmentHistoryAsync(history);
                }
                else
                {
                    throw new Exception("Sorry, this operation could not be completed.");
                }
            }
            else
            {
                throw new Exception("Sorry, this equipment has already been assigned for this assignment.");
            }
            return newId;
        }
        public async Task<bool> UpdateAssignmentEquipmentAsync(AssignmentEquipment equipment)
        {
            if (equipment == null) { throw new ArgumentNullException(nameof(equipment), "The required parameter [Assignment Equipment] is missing."); }
            if (equipment.AssignmentEquipmentId > 0)
            {
                AssignmentEquipment oldAssignmentEquipment = await _assignmentRepository.GetAssignmentEquipmentByIdAsync(equipment.AssignmentEquipmentId);
                if (oldAssignmentEquipment == null || oldAssignmentEquipment.AssignmentEquipmentId < 1 || string.IsNullOrWhiteSpace(oldAssignmentEquipment.AssetName))
                {
                    throw new Exception("Sorry, no record was found for this equipment assignment. Refresh you page to make sure it has not been deleted, then try again.");
                }
                else
                {
                    if (await _assignmentRepository.UpdateAssignmentEquipmentAsync(equipment))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"Some changes were made to the equipment assignment of {oldAssignmentEquipment.AssetName} by {equipment.ModifiedBy} on {DateTime.Now.ToString("f")}. ");

                        if (oldAssignmentEquipment.AssetName != equipment.AssetName)
                        {
                            sb.Append($"The previous equipment [{oldAssignmentEquipment.AssetName}] was replaced with [{equipment.AssetName}]. ");
                        }

                        if (oldAssignmentEquipment.AssignedToEmployeeId != equipment.AssignedToEmployeeId)
                        {
                            if (string.IsNullOrWhiteSpace(oldAssignmentEquipment.AssignedToEmployeeId))
                            {
                                if (!string.IsNullOrWhiteSpace(equipment.AssignedToEmployeeId))
                                {
                                    sb.Append($"The equipment was assigned to a new staff [{equipment.AssignedToEmployeeName}]. ");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(equipment.AssignedToEmployeeId))
                                {
                                    sb.Append($"The equipment was re-assigned to another staff [{equipment.AssignedToEmployeeName}]. ");
                                }
                                else
                                {
                                    sb.Append($"The staff to which this equipment was assigned [{oldAssignmentEquipment.AssignedToEmployeeName}] was removed. ");
                                }
                            }
                        }

                        AssignmentHistory history = new AssignmentHistory();
                        history.ActivityBy = equipment.ModifiedBy;
                        history.ActivityDescription = sb.ToString();
                        history.ActivityTime = DateTime.Now;
                        history.AssignmentId = equipment.AssignmentId;
                        await _assignmentRepository.AddAssignmentHistoryAsync(history);
                        return true;
                    }
                }
            }
            return false;
        }
        public async Task<bool> RemoveAssignmentEquipmentAsync(AssignmentEquipment equipment)
        {
            if (equipment == null) { throw new ArgumentNullException(nameof(equipment), "The required parameter [Assignment Equipment] is missing."); }
            var entities = await _assignmentRepository.GetAssignmentEquipmentByIdAsync(equipment.AssignmentEquipmentId);
            if (equipment != null && equipment.AssignmentEquipmentId > 0)
            {
                if (await _assignmentRepository.DeleteAssignmentEquipmentAsync(equipment.AssignmentEquipmentId))
                {
                            AssignmentHistory history = new AssignmentHistory();
                            history.ActivityBy = equipment.ModifiedBy;
                            history.ActivityDescription = $"{equipment.AssetName} was withdrawn from this assignment by {equipment.ModifiedBy} on {DateTime.Now.ToString("f.")}.";
                            history.ActivityTime = DateTime.Now;
                            history.AssignmentId = equipment.AssignmentId;

                            await _assignmentRepository.AddAssignmentHistoryAsync(history);
                            return true;
                }
            }
            return false;
        }

        #endregion

        #region Assignment Editing Report Service Actions
        public async Task<AssignmentEngReport> GetAssignmentEngReportAsync(long AssignmentEngReportId)
        {
            AssignmentEngReport engReport = new AssignmentEngReport();
            if (AssignmentEngReportId > 0)
            {
                var entity = await _assignmentRepository.GetAssignmentEngReportByIdAsync(AssignmentEngReportId);
                if (entity != null) { engReport = entity; }
            }
            return engReport;
        }
        public async Task<List<AssignmentEngReport>> GetAssignmentEngReportAsync(long AssignmentId, string EmployeeId)
        {
            List<AssignmentEngReport> engReports = new List<AssignmentEngReport>();
            if (AssignmentId > 0 && !string.IsNullOrWhiteSpace(EmployeeId))
            {
                var entities = await _assignmentRepository.GetAssignmentEngReportsByAssignmentIdnEmployeeIdAsync(AssignmentId, EmployeeId);
                if (entities != null) { engReports = entities; }
            }
            return engReports;
        }
        public async Task<List<AssignmentEngReport>> GetAssignmentEngReportsAsync(long AssignmentId)
        {
            List<AssignmentEngReport> listOfEngReports = new List<AssignmentEngReport>();
            if (AssignmentId > 0)
            {
                var entities = await _assignmentRepository.GetAssignmentEngReportsByAssignmentIdAsync(AssignmentId);
                if (entities != null) { listOfEngReports = entities; }
            }
            return listOfEngReports;
        }
        public async Task<long> AddAssignmentEngReportAsync(AssignmentEngReport engReport)
        {
            long newId = 0L;
            if (engReport == null) { throw new ArgumentNullException(nameof(engReport), "The required parameter [Crew Report] is missing."); }
            var entities = await _assignmentRepository.GetAssignmentEngReportsByAssignmentIdnEmployeeIdAsync(engReport.AssignmentId, engReport.EmployeeId);
            if (entities == null || entities.Count < 1)
            {
                newId = await _assignmentRepository.AddAssignmentEngReportAsync(engReport);
                if (newId > 0)
                {
                    await _assignmentRepository.UpdateAssignmentProgressStatusAsync(engReport.AssignmentId, "Editing Completed");
                    AssignmentHistory history = new AssignmentHistory();
                    history.ActivityBy = engReport.ModifiedBy;
                    history.ActivityDescription = $"{engReport.EmployeeName} submitted Post Editing Report on {DateTime.Now.ToString("f")}.";
                    history.ActivityTime = DateTime.Now;
                    history.AssignmentId = engReport.AssignmentId;
                    await _assignmentRepository.AddAssignmentHistoryAsync(history);
                }
            }
            else
            {
                throw new Exception("Sorry, this ENG Editor has already submitted a Post Editing Report for this assignment.");
            }
            return newId;
        }
        public async Task<bool> UpdateAssignmentEngReportAsync(AssignmentEngReport engReport)
        {
            if (engReport == null) { throw new ArgumentNullException(nameof(engReport), "The required parameter [ENG Report] is missing."); }
            if (engReport.EngReportId > 0)
            {
                AssignmentEngReport oldEngReport = await _assignmentRepository.GetAssignmentEngReportByIdAsync(engReport.EngReportId);
                if (oldEngReport == null || oldEngReport.EngReportId < 1 || string.IsNullOrWhiteSpace(oldEngReport.EmployeeName))
                {
                    throw new Exception("Sorry, no record was found for this Report. Refresh you page to make sure it has not been deleted, then try again.");
                }
                else
                {
                    switch (engReport.AudioQuality)
                    {
                        case 1:
                            engReport.AudioQualityDescription = "Poor";
                            break;
                        case 2:
                            engReport.AudioQualityDescription = "Fair";
                            break;
                        case 3:
                            engReport.AudioQualityDescription = "Good";
                            break;
                        default:
                            engReport.AudioQualityDescription = "None";
                            break;
                    }
                    switch (engReport.VideoQuality)
                    {
                        case 1:
                            engReport.VideoQualityDescription = "Poor";
                            break;
                        case 2:
                            engReport.VideoQualityDescription = "Fair";
                            break;
                        case 3:
                            engReport.VideoQualityDescription = "Good";
                            break;
                        default:
                            engReport.VideoQualityDescription = "None";
                            break;
                    }
                    if (await _assignmentRepository.UpdateAssignmentEngReportAsync(engReport))
                    {
                        await _assignmentRepository.UpdateAssignmentProgressStatusAsync(engReport.AssignmentId, "Editing Completed");
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"{engReport.EmployeeName} made some changes to his Post Editing Report on {DateTime.Now.ToString("f")}. ");

                        if (oldEngReport.EditingStatus != oldEngReport.EditingStatus)
                        {
                            sb.AppendLine($"The previous Editing Status [{oldEngReport.EditingStatus}] was changed to [{oldEngReport.EditingStatus}]. ");
                        }
                        if (oldEngReport.AudioQuality != engReport.AudioQuality)
                        {
                            if (string.IsNullOrWhiteSpace(oldEngReport.AudioQualityDescription))
                            {
                                if (!string.IsNullOrWhiteSpace(engReport.AudioQualityDescription))
                                {
                                    sb.AppendLine($"New Audio Quality Rating of [{engReport.AudioQualityDescription}] was added. ");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(engReport.AudioQualityDescription))
                                {
                                    sb.AppendLine($"The previous Audio Quality Rating of [{oldEngReport.AudioQualityDescription}] was changed to [{engReport.AudioQualityDescription}]. ");
                                }
                                else
                                {
                                    sb.AppendLine($"The previous Audio Quality Rating of [{oldEngReport.AudioQualityDescription}] was deleted. ");
                                }
                            }
                        }
                        if (oldEngReport.VideoQuality != engReport.VideoQuality)
                        {
                            if (string.IsNullOrWhiteSpace(oldEngReport.VideoQualityDescription))
                            {
                                if (!string.IsNullOrWhiteSpace(engReport.VideoQualityDescription))
                                {
                                    sb.AppendLine($"New Video Quality Rating of [{engReport.VideoQualityDescription}] was added. ");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(engReport.VideoQualityDescription))
                                {
                                    sb.AppendLine($"The previous Video Quality Rating of [{oldEngReport.VideoQualityDescription}] was changed to [{engReport.VideoQualityDescription}]. ");
                                }
                                else
                                {
                                    sb.AppendLine($"The previous Video Quality Rating of [{oldEngReport.VideoQualityDescription}] was deleted. ");
                                }
                            }
                        }
                        if (oldEngReport.ScriptIsAvailable != engReport.ScriptIsAvailable)
                        {
                            sb.AppendLine($"The previous Script Availability value of [{oldEngReport.ScriptIsAvailable}] changed to [{engReport.ScriptIsAvailable}]. ");
                        }
                        if (oldEngReport.MaterialsAreAvailable != engReport.MaterialsAreAvailable)
                        {
                            sb.AppendLine($"The previous Materials Availability value of [{oldEngReport.MaterialsAreAvailable}] changed to [{engReport.MaterialsAreAvailable}]. ");
                        }
                        if (oldEngReport.ReporterIsAvailable != engReport.ReporterIsAvailable)
                        {
                            sb.AppendLine($"The previous Reporter Availability value of [{oldEngReport.ReporterIsAvailable}] changed to [{engReport.ReporterIsAvailable}]. ");
                        }
                        if (oldEngReport.ReporterArrivalTime != engReport.ReporterArrivalTime)
                        {
                            if (oldEngReport.ReporterArrivalTime == null)
                            {
                                if (engReport.ReporterArrivalTime != null)
                                {
                                    sb.AppendLine($"Reporter Arrival Time of [{engReport.ReporterArrivalTime?.ToString("f")}] was added. ");
                                }
                            }
                            else
                            {
                                if (engReport.ReporterArrivalTime != null)
                                {
                                    sb.AppendLine($"The previous Reporter Arrival Time of [{oldEngReport.ReporterArrivalTime?.ToString("f")}] was changed to [{engReport.ReporterArrivalTime?.ToString("f")}]. ");
                                }
                                else
                                {
                                    sb.AppendLine($"The previous Reporter Arrival Time of [{oldEngReport.ReporterArrivalTime?.ToString("f")}] was deleted.");
                                }
                            }
                        }
                        if (oldEngReport.EditingStartTime != engReport.EditingStartTime)
                        {
                            if (oldEngReport.EditingStartTime == null)
                            {
                                if (engReport.EditingStartTime != null)
                                {
                                    sb.AppendLine($"Editing Start Time of [{engReport.EditingStartTime?.ToString("f")}] was added. ");
                                }
                            }
                            else
                            {
                                if (engReport.EditingStartTime != null)
                                {
                                    sb.AppendLine($"The previous Editing Start Time of [{oldEngReport.EditingStartTime?.ToString("f")}] was changed to [{engReport.EditingStartTime?.ToString("f")}]. ");
                                }
                                else
                                {
                                    sb.AppendLine($"The previous Editing Start Time of [{oldEngReport.EditingStartTime?.ToString("f")}] was deleted.");
                                }
                            }
                        }
                        if (oldEngReport.EditingEndTime != engReport.EditingEndTime)
                        {
                            if (oldEngReport.EditingEndTime == null)
                            {
                                if (engReport.EditingEndTime != null)
                                {
                                    sb.AppendLine($"Editing Completed Time of [{engReport.EditingEndTime?.ToString("f")}] was added. ");
                                }
                            }
                            else
                            {
                                if (engReport.EditingEndTime != null)
                                {
                                    sb.AppendLine($"The previous Editing Completed Time of [{oldEngReport.EditingEndTime?.ToString("f")}] was changed to [{engReport.EditingEndTime?.ToString("f")}]. ");
                                }
                                else
                                {
                                    sb.AppendLine($"The previous Editing Completed Time of [{oldEngReport.EditingEndTime?.ToString("f")}] was deleted.");
                                }
                            }
                        }
                        if (oldEngReport.Feedback != engReport.Feedback)
                        {
                            if (string.IsNullOrWhiteSpace(oldEngReport.Feedback))
                            {
                                if (!string.IsNullOrWhiteSpace(engReport.Feedback))
                                {
                                    sb.Append($"New Comment [{engReport.Feedback}] was added. ");
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(engReport.Feedback))
                                {
                                    sb.Append($"The previous Comment of [{oldEngReport.Feedback}] was changed to [{engReport.Feedback}]. ");
                                }
                                else
                                {
                                    sb.Append($"The previous Comment of [{oldEngReport.Feedback}] was deleted. ");
                                }
                            }
                        }

                        AssignmentHistory history = new AssignmentHistory();
                        history.ActivityBy = engReport.ModifiedBy;
                        history.ActivityDescription = sb.ToString();
                        history.ActivityTime = DateTime.Now;
                        history.AssignmentId = engReport.AssignmentId;
                        await _assignmentRepository.AddAssignmentHistoryAsync(history);
                        return true;
                    }
                }
            }
            return false;
        }
        public async Task<bool> DeleteAssignmentEngReportAsync(AssignmentEngReport engReport)
        {
            if (engReport == null) { throw new ArgumentNullException(nameof(engReport), "The required parameter [Assignment Editing Report] is missing."); }
            var oldEngReport = await _assignmentRepository.GetAssignmentEngReportByIdAsync(engReport.EngReportId);
            if (oldEngReport != null)
            {

                if (engReport != null && oldEngReport.EngReportId > 1)
                {
                    if (await _assignmentRepository.DeleteAssignmentEngReportAsync(engReport.EngReportId))
                    {
                        AssignmentHistory history = new AssignmentHistory();
                        history.ActivityBy = engReport.ModifiedBy;
                        history.ActivityDescription = $"{oldEngReport.EmployeeName} deleted his/her earlier submitted Post Editing Report on {DateTime.Now.ToString("f.")}.";
                        history.ActivityTime = DateTime.Now;
                        history.AssignmentId = engReport.AssignmentId;

                        await _assignmentRepository.AddAssignmentHistoryAsync(history);
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Assignment History and Notes Service Actions
        public async Task<List<AssignmentHistory>> GetAssignmentHistoryAsync(long AssignmentId)
        {
            List<AssignmentHistory> assignmentHistory = new List<AssignmentHistory>();
            var entities = await _assignmentRepository.GetAssignmentHistoryByAssignmentIdAsync(AssignmentId);
            if (entities != null && entities.Count > 0) { assignmentHistory = entities.ToList(); }
            return assignmentHistory;
        }

        public async Task<List<AssignmentNote>> GetAssignmentNotesAsync(long AssignmentId)
        {
            List<AssignmentNote> assignmentNotes = new List<AssignmentNote>();
            var entities = await _assignmentRepository.GetAssignmentNotesByAssignmentIdAsync(AssignmentId);
            if (entities != null && entities.Count > 0) { assignmentNotes = entities.ToList(); }
            return assignmentNotes;
        }

        public async Task<bool> AddAssignmentNoteAsync(AssignmentNote note)
        {
            return await _assignmentRepository.AddNoteAsync(note);
        }

        #endregion

        #region Assignment Settings Service Actions

        #region Assignment Event Type Service Actions
        public async Task<int> CreateAssignmentEventTypeAsync(AssignmentEventType assignmentEventType)
        {
            if (assignmentEventType == null) { throw new ArgumentNullException(nameof(assignmentEventType), "The required parameter [Event Type] is missing."); }
            var entities = await _assignmentRepository.GetAssignmentEventTypesByDescriptionAsync(assignmentEventType.Description);
            if (entities != null && entities.Count > 0 && entities[0].Id > 0)
            {
                throw new Exception("Sorry, an Event Type with this Description already exists. Please choose another Description.");
            }
            return await _assignmentRepository.AddAssignmentEventTypeAsync(assignmentEventType);
        }
        public async Task<bool> UpdateAssignmentEventTypeAsync(AssignmentEventType assignmentEventType)
        {
            if (assignmentEventType == null) { throw new ArgumentNullException(nameof(assignmentEventType), "The required parameter [Event Type] is missing."); }
            var entities = await _assignmentRepository.GetAssignmentEventTypesByDescriptionAsync(assignmentEventType.Description);
            if (entities != null && entities.Count > 0 && entities[0].Id > 0)
            {
                foreach (var entity in entities)
                {
                    if (entity.Id != assignmentEventType.Id)
                    {
                        throw new Exception("Title already exists. Please choose another Title.");
                    }
                }
            }
            return await _assignmentRepository.UpdateAssignmentEventTypeAsync(assignmentEventType);
        }
        public async Task<bool> DeleteAssignmentEventTypeAsync(int assignmentEventTypeId)
        {
            return await _assignmentRepository.DeleteAssignmentEventTypeAsync(assignmentEventTypeId);
        }

        public async Task<AssignmentEventType> GetAssignmentEventTypeAsync(int? assignmentEventTypeId = null, string assignmentEventTypeDescription = null)
        {
            AssignmentEventType e = new AssignmentEventType();
            if (assignmentEventTypeId != null)
            {
                var entity = await _assignmentRepository.GetAssignmentEventTypeByIdAsync(assignmentEventTypeId.Value);
                if (entity != null && entity.Id > 0) { e = entity; }
            }
            else if (!string.IsNullOrWhiteSpace(assignmentEventTypeDescription))
            {
                var entities = await _assignmentRepository.GetAssignmentEventTypesByDescriptionAsync(assignmentEventTypeDescription);
                if (entities != null && entities[0].Id > 0) { e = entities[0]; }
            }
            return e;
        }
        public async Task<List<AssignmentEventType>> GetAssignmentEventTypesAsync()
        {
            List<AssignmentEventType> events = new List<AssignmentEventType>();
            var entities = await _assignmentRepository.GetAssignmentEventTypesAsync();
            if (entities != null && entities.Count > 0) { events = entities.ToList(); }
            return entities;
        }

        #endregion

        #region Assignment Role Service Actions
        public async Task<int> CreateAssignmentRoleAsync(AssignmentRole assignmentRole)
        {
            if (assignmentRole == null) { throw new ArgumentNullException(nameof(assignmentRole), "The required parameter [Assignment Role] is missing."); }
            var entities = await _assignmentRepository.GetAssignmentRolesByDescriptionAsync(assignmentRole.Description);
            if (entities != null && entities.Count > 0 && entities[0].Id > 0)
            {
                throw new Exception("Sorry, an Assignment Role with this Description already exists. Please choose another Description.");
            }
            return await _assignmentRepository.AddAssignmentRoleAsync(assignmentRole);
        }
        public async Task<bool> UpdateAssignmentRoleAsync(AssignmentRole assignmentRole)
        {
            if (assignmentRole == null) { throw new ArgumentNullException(nameof(assignmentRole), "The required parameter [Assignment Role] is missing."); }
            var entities = await _assignmentRepository.GetAssignmentEventTypesByDescriptionAsync(assignmentRole.Description);
            if (entities != null && entities.Count > 0 && entities[0].Id > 0)
            {
                foreach (var entity in entities)
                {
                    if (entity.Id != assignmentRole.Id)
                    {
                        throw new Exception("An Assignment Role with this Description already exists. Please choose another Description.");
                    }
                }
            }
            return await _assignmentRepository.UpdateAssignmentEventTypeAsync(assignmentRole);
        }
        public async Task<bool> DeleteAssignmentRoleAsync(int assignmentRoleId)
        {
            return await _assignmentRepository.DeleteAssignmentRoleAsync(assignmentRoleId);
        }

        public async Task<AssignmentRole> GetAssignmentRoleAsync(int? assignmentRoleId = null, string assignmentRoleDescription = null)
        {
            AssignmentRole e = new AssignmentRole();
            if (assignmentRoleId != null)
            {
                var entity = await _assignmentRepository.GetAssignmentRoleByIdAsync(assignmentRoleId.Value);
                if (entity != null && entity.Id > 0) { e = entity; }
            }
            else if (!string.IsNullOrWhiteSpace(assignmentRoleDescription))
            {
                var entities = await _assignmentRepository.GetAssignmentRolesByDescriptionAsync(assignmentRoleDescription);
                if (entities != null && entities[0].Id > 0) { e = entities[0]; }
            }
            return e;
        }
        public async Task<List<AssignmentRole>> GetAssignmentRolesAsync()
        {
            List<AssignmentRole> events = new List<AssignmentRole>();
            var entities = await _assignmentRepository.GetAssignmentRolesAsync();
            if (entities != null && entities.Count > 0) { events = entities.ToList(); }
            return entities;
        }

        #endregion

        public List<string> GetAssignmentProgressStatuses()
        {
            return new List<string>
            {
                "Pending Coverage",
                "Coverage Completed",
                "Pending Editing",
                "Editing Completed",
                "Ready for Broadcast",
                "Used In Broadcast",
                "Suspended",
                "Cancelled",
            };
        }

        #endregion

        #region AssignmentService Helper Methods
        public async Task<string> GetNewAssignmentNumberAsync()
        {
            List<string> _existingNumbers = new List<string>();
            DateTime createdDate = DateTime.Now;
            string yy = createdDate.Year.ToString().Substring(2, 2);
            string mm = createdDate.Month.ToString().PadLeft(2, '0');
            //string dd = day.ToString().PadLeft(2, '0');

            _existingNumbers = await _assignmentRepository.GetAssignmentNumbersByCreatedDateAsync(createdDate);

            if (_existingNumbers == null || _existingNumbers.Count < 1)
            {
                return $"A{yy}{mm}001";
            }

            string _newAssignmentNumber = string.Empty;
            int _nextCount = 1;
            bool _isExisting = true;
            do
            {
                string _nextDigitString = _nextCount.ToString().PadLeft(3, '0');
                _newAssignmentNumber = $"A{yy}{mm}{_nextDigitString}";
                _isExisting = _existingNumbers.Contains(_newAssignmentNumber);
                _nextCount++;
            }
            while (_isExisting);
            return _newAssignmentNumber;
        }
        #endregion
    }
}
