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
        public async Task<List<Assignment>> GetAssignments(string ClientId, DateTime? EventStartDate = null, DateTime? EventEndDate = null)
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
        public async Task<Assignment> GetAssignment(long AssignmentId)
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

            bool isUpdated = await _assignmentRepository.UpdateAssignmentAsync(assignment);
            if (isUpdated)
            {
                AssignmentHistory history = new AssignmentHistory();
                history.ActivityBy = assignment.CreatedBy;
                history.ActivityDescription = $"Assignment was updated by {assignment.CreatedBy} on {DateTime.Now.ToString("f")}.";
                history.ActivityTime = DateTime.Now;
                history.AssignmentId = assignment.Id.Value;
                await _assignmentRepository.AddAssignmentHistoryAsync(history);
            }
            return isUpdated;
        }

        public async Task<bool> DeleteAssignmentAsync(long assignmentId)
        {
            return await _assignmentRepository.DeleteAssignmentAsync(assignmentId);
        }
        #endregion

        #endregion

        #region Assignment Crew Service Actions
        public async Task<AssignmentCrewMember> GetAssignmentCrewMember(long AssignmentCrewId)
        {
            AssignmentCrewMember crewMember = new AssignmentCrewMember();
            if (AssignmentCrewId > 0)
            {
                var entity = await _assignmentRepository.GetAssignmentCrewMemberbyIdAsync(AssignmentCrewId);
                if (entity != null) { crewMember = entity; }
            }
            return crewMember;
        }
        public async Task<List<AssignmentCrewMember>> GetAssignmentCrewMembers(long AssignmentId)
        {
            List<AssignmentCrewMember> listOfCrewMembers = new List<AssignmentCrewMember>();
            if (AssignmentId > 0)
            {
                var entities = await _assignmentRepository.GetAssignmentCrewMembersbyAssignmentIdAsync(AssignmentId);
                if (entities != null) { listOfCrewMembers = entities; }
            }

            return listOfCrewMembers;
        }
        public async Task<long> AddAssignmentCrewMemberAsync(AssignmentCrewMember crewMember)
        {
            long newId = 0L;
            if (crewMember == null) { throw new ArgumentNullException(nameof(crewMember), "The required parameter [Assignment Crew Member] is missing."); }
            var entities = await _assignmentRepository.GetAssignmentCrewMembersbyCrewMemberIdAsync(crewMember.CrewMemberId);
            if (entities == null || entities.Count < 1)
            {
                newId = await _assignmentRepository.AddAssignmentCrewMemberAsync(crewMember);
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
            if (crewMember != null && crewMember.Id > 1)
            {
                if (await _assignmentRepository.DeleteAssignmentCrewMemberAsync(crewMember.Id.Value))
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
        public async Task<bool> UpdateAssignmentCrewParticipationAsync(long AssignmentCrewId, string AttendanceStatus, string ServiceRating, string Remarks, string UpdatedBy)
        {
            if (AssignmentCrewId < 1) { throw new ArgumentNullException(nameof(AssignmentCrewId), "The required parameter [Assignment Crew ID] is missing."); }

            AssignmentCrewMember crewMember = await _assignmentRepository.GetAssignmentCrewMemberbyIdAsync(AssignmentCrewId);
            if (crewMember == null || crewMember.Id < 1 || string.IsNullOrWhiteSpace(crewMember.CrewMemberName))
            {
                throw new Exception("Sorry, no record was found for this crew membership. Refresh you page to make sure it has not been deleted, then try again.");
            }
            else
            {
                if (await _assignmentRepository.UpdateAssignmentCrewParticipationAsync(AssignmentCrewId, ServiceRating, AttendanceStatus, Remarks, UpdatedBy))
                {
                    AssignmentHistory history = new AssignmentHistory();
                    history.ActivityBy = UpdatedBy;
                    history.ActivityDescription = $"Participation Report for {crewMember.CrewMemberName} was updated by {UpdatedBy} on {DateTime.Now.ToString("f")}. ";
                    history.ActivityTime = DateTime.Now;
                    history.AssignmentId = crewMember.AssignmentId;

                    await _assignmentRepository.AddAssignmentHistoryAsync(history);
                    return true;
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
