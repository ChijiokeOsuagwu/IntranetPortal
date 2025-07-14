using IntranetPortal.Base.Models.AtsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AtsRepositories
{
    public interface IAssignmentRepository
    {
        #region Assignment Event Type Action Interfaces
        Task<int> AddAssignmentEventTypeAsync(AssignmentEventType eventType);
        Task<bool> UpdateAssignmentEventTypeAsync(AssignmentEventType eventType);
        Task<bool> DeleteAssignmentEventTypeAsync(int assignmentEventTypeId);
        Task<AssignmentEventType> GetAssignmentEventTypeByIdAsync(int assignmentEventTypeId);
        Task<List<AssignmentEventType>> GetAssignmentEventTypesByDescriptionAsync(string description);
        Task<List<AssignmentEventType>> GetAssignmentEventTypesAsync();
        #endregion

        #region Assignment Role Action Interfaces
        Task<int> AddAssignmentRoleAsync(AssignmentRole role);
        Task<bool> UpdateAssignmentEventTypeAsync(AssignmentRole role);
        Task<bool> DeleteAssignmentRoleAsync(int assignmentRoleId);

        Task<AssignmentRole> GetAssignmentRoleByIdAsync(int assignmentRoleId);
        Task<List<AssignmentRole>> GetAssignmentRolesByDescriptionAsync(string assignmentRoleDescription);
        Task<List<AssignmentRole>> GetAssignmentRolesAsync();
        #endregion

        #region Assignment Action Interfaces
        Task<List<Assignment>> GetAssignmentsByClientIdAsync(string clientId, DateTime? fromDate = null, DateTime? toDate = null);
        #endregion

        #region Assignment Note Action Interfaces
        Task<List<AssignmentNote>> GetAssignmentNotesByAssignmentIdAsync(long assignmentId);
        Task<bool> AddNoteAsync(AssignmentNote n);
        Task<bool> CancelAssignmentNoteAsync(long assignmentNoteId, string cancelledBy);
        Task<bool> DeleteAssignmentNoteAsync(long assignmentNoteId);
        #endregion


        #region Assignment History Action Interfaces
        Task<List<AssignmentHistory>> GetAssignmentHistoryByAssignmentIdAsync(long assignmentId);
        Task<bool> AddAssignmentHistoryAsync(AssignmentHistory assignmentHistory);
        Task<bool> DeleteAssignmentHistoryAsync(long assignmentHistoryId);
        #endregion
    }
}
