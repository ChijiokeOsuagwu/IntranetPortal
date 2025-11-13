using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IAssignmentService
    {
        #region Assignment Service Interfaces
        #region Assignment Read Action Interfaces
        Task<Assignment> GetAssignmentAsync(long AssignmentId);
        Task<List<Assignment>> GetAssignmentsAsync(string ClientId, DateTime? EventStartDate = null, DateTime? EventEndDate = null);
        #endregion

        #region Assignment Write Action Interfaces
        Task<bool> CreateNewAssignmentAsync(Assignment assignment);
        Task<bool> EditAssignmentAsync(Assignment assignment);
        Task<bool> DeleteAssignmentAsync(long assignmentId);
        #endregion

        #endregion

        #region Assignment Crew Service Interfaces
        Task<AssignmentCrewMember> GetAssignmentCrewMemberAsync(long AssignmentCrewId);
        Task<AssignmentCrewMember> GetAssignmentCrewMemberAsync(long AssignmentId, string EmployeeId);
        Task<List<AssignmentCrewMember>> GetAssignmentCrewMembersAsync(long AssignmentId);
        Task<List<Employee>> GetAssignmentEmployeesAsync(long AssignmentId);

        Task<long> AddAssignmentCrewMemberAsync(AssignmentCrewMember crewMember);
        Task<bool> UpdateAssignmentCrewMemberAsync(AssignmentCrewMember crewMember);
        Task<bool> RemoveAssignmentCrewMemberAsync(AssignmentCrewMember crewMember);
        Task<bool> UpdateAssignmentCrewLeadAsync(long AssignmentCrewId, bool IsCrewLead, string updatedBy);
        #endregion

        #region Assignment Crew Report Service Actions
        Task<AssignmentCrewReport> GetAssignmentCrewReportAsync(long AssignmentCrewReportId);
        Task<List<AssignmentCrewReport>> GetAssignmentCrewReportAsync(long AssignmentId, string EmployeeId);
        Task<List<AssignmentCrewReport>> GetAssignmentCrewReportsAsync(long AssignmentId);
        Task<long> AddAssignmentCrewReportAsync(AssignmentCrewReport crewMemberReport);
        Task<bool> UpdateAssignmentCrewReportAsync(AssignmentCrewReport crewMemberReport);
        Task<bool> DeleteAssignmentCrewReportAsync(AssignmentCrewReport crewMemberReport);

        #endregion

        #region Assignment Editing Report Service Actions
        Task<AssignmentEngReport> GetAssignmentEngReportAsync(long AssignmentEngReportId);
        Task<List<AssignmentEngReport>> GetAssignmentEngReportAsync(long AssignmentId, string EmployeeId);
        Task<List<AssignmentEngReport>> GetAssignmentEngReportsAsync(long AssignmentId);
        Task<long> AddAssignmentEngReportAsync(AssignmentEngReport EngReport);
        Task<bool> UpdateAssignmentEngReportAsync(AssignmentEngReport EngReport);
        Task<bool> DeleteAssignmentEngReportAsync(AssignmentEngReport EngReport);

        #endregion

        #region Assignment Equipment Service Interfaces
        Task<AssignmentEquipment> GetAssignmentEquipmentAsync(long AssignmentEquipmentId);
        Task<List<AssignmentEquipment>> GetAssignmentEquipmentsAsync(long AssignmentId);
        Task<long> AddAssignmentEquipmentAsync(AssignmentEquipment equipment);
        Task<bool> UpdateAssignmentEquipmentAsync(AssignmentEquipment equipment);
        Task<bool> RemoveAssignmentEquipmentAsync(AssignmentEquipment equipment);
        #endregion

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

        Task<bool> AddAssignmentNoteAsync(AssignmentNote note);
        #endregion

        #endregion

        #region Assignment Utility Service Interfaces
        Task<string> GetNewAssignmentNumberAsync();
        #endregion
    }
}