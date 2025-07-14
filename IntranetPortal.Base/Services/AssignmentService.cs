using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Base.Repositories.AtsRepositories;
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
    public class AssignmentService : IAssignmentService
    {
        private readonly IDeskspaceRepository _deskspaceRepository;
        private readonly IUtilityRepository _utilityRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IAssignmentRepository _assignmentRepository;

        public AssignmentService(IDeskspaceRepository deskspaceRepository, IUtilityRepository utilityRepository,
                                    IProgramRepository programRepository, IEmployeesRepository employeesRepository,
                                    IAssignmentRepository assignmentRepository)
        {
            _deskspaceRepository = deskspaceRepository;
            _utilityRepository = utilityRepository;
            _programRepository = programRepository;
            _employeesRepository = employeesRepository;
            _assignmentRepository = assignmentRepository;

        }


        #region Assignments Service Actions
        #region Assignments Read Service Actions
        public async Task<List<Assignment>> GetAssignments(string ClientId, DateTime? EventStartDate = null, DateTime? EventEndDate = null)
        {
            List<Assignment> listOfAssignments = new List<Assignment>();
            var entities = await _assignmentRepository.GetAssignmentsByClientIdAsync(ClientId, EventStartDate, EventEndDate);
            if(entities != null) { listOfAssignments = entities; }
            return listOfAssignments;
        }
        #endregion

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
    }
}
