using IntranetPortal.Base.Models.AtsModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IAssignmentService
    {
        #region Assignment Settings Service Interfaces
        #region Assignment Event Type Action Interfaces
        Task<int> CreateAssignmentEventTypeAsync(AssignmentEventType assignmentEventType);
        Task<bool> DeleteAssignmentEventTypeAsync(int assignmentEventTypeId);
        Task<AssignmentEventType> GetAssignmentEventTypeAsync(int? assignmentEventTypeId = null, string assignmentEventTypeDescription = null);
        Task<List<AssignmentEventType>> GetAssignmentEventTypesAsync();
        Task<bool> UpdateAssignmentEventTypeAsync(AssignmentEventType assignmentEventType);
        #endregion

        #region Assignment Role Action Interfaces
        Task<int> CreateAssignmentRoleAsync(AssignmentRole assignmentRole);
        Task<bool> DeleteAssignmentRoleAsync(int assignmentRoleId);
        Task<AssignmentRole> GetAssignmentRoleAsync(int? assignmentRoleId = null, string assignmentRoleDescription = null);
        Task<List<AssignmentRole>> GetAssignmentRolesAsync();
        Task<bool> UpdateAssignmentRoleAsync(AssignmentRole assignmentRole);
        #endregion

        #region Assignment History and Notes Service Interfaces
        Task<List<AssignmentHistory>> GetAssignmentHistoryAsync(long AssignmentId);
        Task<List<AssignmentNote>> GetAssignmentNotesAsync(long AssignmentId);
        #endregion

        #endregion

        #region Assignment Service Interfaces
        #region Assignment Read Action Interfaces
        Task<List<Assignment>> GetAssignments(string ClientId, DateTime? EventStartDate = null, DateTime? EventEndDate = null);
        #endregion
        #endregion
    }
}