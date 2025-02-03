using IntranetPortal.Base.Models.LmsModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.LmsRepositories
{
    public interface IEmployeeLeaveRepository
    {
        #region Employee Leave Action Methods
        Task<long> AddAsync(EmployeeLeave e);
        Task<bool> DeleteAsync(long id);
        Task<bool> EditAsync(EmployeeLeave e);
        Task<bool> UpdateStatusAsync(long leaveId, string newStatus);
        Task<bool> UpdateApprovalStatusAsync(long leaveId, string newStatus, string approvalType);

        // Employee Leave By Employee ID
        Task<EmployeeLeave> GetByIdAsync(long id);
        Task<List<EmployeeLeave>> GetByEmployeeIdAsync(string employeeId, bool isPlan);
        Task<List<EmployeeLeave>> GetByEmployeeIdnYearAsync(string employeeId, int year, bool getPlan);
        Task<List<EmployeeLeave>> GetByEmployeeIdnYearnMonthAsync(string employeeId, int year, int month, bool isPlan);
        Task<List<EmployeeLeave>> GetByEmployeeIdnYearnMonthnStatusAsync(string employeeId, int year, int month, string leaveStatus, bool isPlan);
        Task<List<EmployeeLeave>> GetByEmployeeIdnYearnStatusAsync(string employeeId, int year, string leaveStatus, bool isPlan);
        Task<List<EmployeeLeave>> GetByEmployeeIdnStatusAsync(string employeeId, string leaveStatus, bool isPlan);

        //Employee Leave By Employee Name
        Task<List<EmployeeLeave>> GetByEmployeeNamenYearAsync(string employeeName, int year, bool isPlan);
        Task<List<EmployeeLeave>> GetByEmployeeNamenYearnStatusAsync(string employeeName, int year, string leaveStatus, bool isPlan);
        Task<List<EmployeeLeave>> GetByEmployeeNamenYearnMonthnStatusAsync(string employeeName, int year, int month, string leaveStatus, bool isPlan);
        Task<List<EmployeeLeave>> GetByEmployeeNamenStatusAsync(string employeeName, string leaveStatus, bool isPlan);
        Task<List<EmployeeLeave>> GetByEmployeeNamenYearnMonthAsync(string employeeName, int year, int month, bool isPlan);
        Task<List<EmployeeLeave>> GetByEmployeeNameAsync(string employeeName, bool isPlan);

        // Employee Leaves By Team Lead ID
        Task<List<EmployeeLeave>> GetByReportingLineIdAsync(string teamLeadId, bool isPlan);
        Task<List<EmployeeLeave>> GetByReportingLineIdnYearAsync(string teamLeadId, int year, bool isPlan);
        Task<List<EmployeeLeave>> GetByReportingLineIdnYearnStatusAsync(string teamLeadId, int year, string leaveStatus, bool isPlan);
        Task<List<EmployeeLeave>> GetByReportingLineIdnYearnMonthAsync(string teamLeadId, int year, int month, bool isPlan);
        Task<List<EmployeeLeave>> GetByReportingLineIdnYearnMonthnStatusAsync(string teamLeadId, int year, int month, string leaveStatus, bool isPlan);
        Task<List<EmployeeLeave>> GetByReportingLineIdnStatusAsync(string teamLeadId, string leaveStatus, bool isPlan);


        //=== Employee Leaves by Location, Department, Unit etc.
        Task<List<EmployeeLeave>> GetAllAsync(bool isPlan);
        Task<List<EmployeeLeave>> GetByYearAsync(int year, bool isPlan);
        Task<List<EmployeeLeave>> GetByYearnStatusAsync(int year, string leaveStatus, bool isPlan);
        Task<List<EmployeeLeave>> GetByYearnMonthAsync(int year, int month, bool isPlan);
        Task<List<EmployeeLeave>> GetByYearnMonthnStatusAsync(int year, int month, string leaveStatus, bool isPlan);
        Task<List<EmployeeLeave>> GetByStatusAsync(string leaveStatus, bool isPlan);

        #endregion

        #region Leave Submission Action Methods
        //======== Write Action Methods =============//
        Task<bool> AddSubmissionAsync(LeaveSubmission e);
        Task<bool> DeleteSubmissionAsync(long id);
        Task<bool> EditSubmissionActionStatusAsync(long submissionId, DateTime? timeActioned);

        //======== Read Action Methods ==============//
        Task<List<LeaveSubmission>> GetSubmissionsByToEmployeeIdAsync(string toEmployeeId);
        Task<List<LeaveSubmission>> GetSubmissionsByYearSubmittedAsync(int yearSubmitted);
        Task<List<LeaveSubmission>> GetSubmissionsByYearnMonthSubmittedAsync(int yearSubmitted, int monthSubmitted);
        Task<List<LeaveSubmission>> GetSubmissionsByToEmployeeIdnYearSubmittedAsync(string toEmployeeId, int yearSubmitted);
        Task<List<LeaveSubmission>> GetSubmissionsByToEmployeeIdnYearnMonthSubmittedAsync(string toEmployeeId, int yearSubmitted, int monthSubmitted);
        #endregion

        #region Leave Approval Action Interfaces
        Task<long> AddApprovalAsync(LeaveApproval e);
        Task<bool> DeleteApprovalAsync(long approvalId);
        Task<LeaveApproval> GetApprovalByIdAsync(long approvalId);
        Task<List<LeaveApproval>> GetApprovalsByLeaveIdAsync(long leaveId);

        #endregion
        #region Leave Notes Action Interfaces
        Task<bool> AddNoteAsync(LeaveNote e);
        Task<List<LeaveNote>> GetNotesByLeaveIdAsync(long leaveId);
        #endregion

        #region Leave Activity Log Action Methods
        Task<bool> AddLogAsync(LeaveActivityLog e);
        Task<List<LeaveActivityLog>> GetLogByLeaveIdAsync(long leaveId);
        #endregion

        #region Leave Documents Action Methods
        Task<bool> AddDocumentAsync(LeaveDocument e);
        Task<bool> DeleteDocumentAsync(long id);
        Task<LeaveDocument> GetDocumentByIdAsync(long id);
        Task<List<LeaveDocument>> GetDocumentsByLeaveIdAsync(long leaveId);
        #endregion
    }
}