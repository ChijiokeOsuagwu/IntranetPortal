using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
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

        #region Assignment Read Action Interfaces
        Task<List<string>> GetAssignmentNumbersByCreatedDateAsync(DateTime createdDate);
        Task<Assignment> GetAssignmentByIdAsync(long assignmentId);
        Task<List<Assignment>> GetAssignmentsByDateRangeAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<Assignment>> GetAssignmentsByClientIdAsync(string clientId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<Assignment>> GetAssignmentsByClientNameAsync(string clientName, DateTime? fromDate = null, DateTime? toDate = null);
        #endregion

        #region Assignment Write Action Interfaces
        Task<long> AddAssignmentAsync(Assignment assignment);
        Task<bool> UpdateAssignmentAsync(Assignment assignment);
        Task<bool> DeleteAssignmentAsync(long assignmentId);
        Task<bool> UpdateAssignmentProgressStatusAsync(long assignmentId, string progressStatus);
        #endregion

        #region Assignment Crew Members Action Interfaces
        Task<AssignmentCrewMember> GetAssignmentCrewMemberbyIdAsync(long assignmentCrewId);
        Task<AssignmentCrewMember> GetAssignmentCrewMemberbyAssignmentIdnEmployeeIdAsync(long assignmentId, string employeeId);
        Task<List<AssignmentCrewMember>> GetAssignmentCrewMembersbyAssignmentIdAsync(long assignmentId);
        Task<List<AssignmentCrewMember>> GetAssignmentCrewMembersbyCrewMemberIdAsync(string employeeId);
        Task<List<Employee>> GetAssignmentEmployeesByAssignmentIdAsync(long assignmentId);

        Task<long> AddAssignmentCrewMemberAsync(AssignmentCrewMember assignmentCrewMember);
        Task<bool> UpdateAssignmentCrewMemberAsync(AssignmentCrewMember assignmentCrewMember);
        Task<bool> UpdateAssignmentCrewLeadAsync(long assignmentCrewId, bool isLead, string updatedBy);
        Task<bool> DeleteAssignmentCrewMemberAsync(long assignmentCrewId);
        #endregion

        #region Assignment Crew Report Action Interfaces
        Task<AssignmentCrewReport> GetAssignmentCrewReportByIdAsync(long assignmentCrewReportId);
        Task<List<AssignmentCrewReport>> GetAssignmentCrewReportsByAssignmentIdAsync(long assignmentId);
        Task<List<AssignmentCrewReport>> GetAssignmentCrewReportsByAssignmentIdnEmployeeIdAsync(long assignmentId, string employeeId);
        Task<long> AddAssignmentCrewReportAsync(AssignmentCrewReport assignmentCrewReport);
        Task<bool> UpdateAssignmentCrewReportAsync(AssignmentCrewReport assignmentCrewReport);
        Task<bool> DeleteAssignmentCrewReportAsync(long assignmentCrewReportId);
        #endregion

        #region Assignment Equipment Action Interfaces
        Task<AssignmentEquipment> GetAssignmentEquipmentByIdAsync(long assignmentEquipmentId);
        Task<List<AssignmentEquipment>> GetAssignmentEquipmentByAssignmentIdnAssetTypeNameAsync(long assignmentId, string assetTypeName);
        Task<List<AssignmentEquipment>> GetAssignmentEquipmentByAssignmentIdnAssetClassIdAsync(long assignmentId, int assetClassId);
        Task<List<AssignmentEquipment>> GetAssignmentEquipmentByAssignmentIdAsync(long assignmentId);
        Task<List<AssignmentEquipment>> GetAssignmentEquipmentByAssetIdAsync(string assetId);

        Task<long> AddAssignmentEquipmentAsync(AssignmentEquipment assignmentEquipment);
        Task<bool> UpdateAssignmentEquipmentAsync(AssignmentEquipment assignmentEquipment);
        Task<bool> DeleteAssignmentEquipmentAsync(long assignmentEquipmentId);

        #endregion

        #region Assignment Editing Report Action Interfaces
        Task<AssignmentEngReport> GetAssignmentEngReportByIdAsync(long assignmentEngReportId);
        Task<List<AssignmentEngReport>> GetAssignmentEngReportsByAssignmentIdAsync(long assignmentId);
        Task<List<AssignmentEngReport>> GetAssignmentEngReportsByAssignmentIdnEmployeeIdAsync(long assignmentId, string employeeId);
        Task<long> AddAssignmentEngReportAsync(AssignmentEngReport assignmentEngReport);
        Task<bool> UpdateAssignmentEngReportAsync(AssignmentEngReport assignmentEngReport);
        Task<bool> DeleteAssignmentEngReportAsync(long assignmentEngReportId);
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
