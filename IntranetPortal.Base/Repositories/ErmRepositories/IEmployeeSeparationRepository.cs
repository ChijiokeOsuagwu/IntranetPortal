using IntranetPortal.Base.Models.EmployeeRecordModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.ErmRepositories
{
    public interface IEmployeeSeparationRepository
    {
        IConfiguration _config { get; }

        #region Employee Separation Action Interfaces
        Task<bool> AddAsync(EmployeeSeparation e);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAsync(string employeeId, DateTime createdDate);
        Task<bool> EditAsync(EmployeeSeparation e);
        Task<List<EmployeeSeparation>> GetByEmployeeIdAsync(string employeeId);
        Task<EmployeeSeparation> GetByIdAsync(int id);
        Task<List<EmployeeSeparation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetBalanceOwedBySeparationIdAsync(int employeeSeparationId);
        Task<decimal> GetBalanceIndebtedBySeparationIdAsync(int employeeSeparationId);

        #endregion

        #region Employee Separation Types Action Interfaces
        Task<bool> AddSeparationTypeAsync(EmployeeSeparationType e);
        Task<bool> EditSeparationTypeAsync(EmployeeSeparationType e);
        Task<bool> DeleteSeparationTypeAsync(int id);
        Task<EmployeeSeparationType> GetSeparationTypeByIdAsync(int id);
        Task<List<EmployeeSeparationType>> GetSeparationTypesbyNameAsync(string name);
        Task<List<EmployeeSeparationType>> GetAllSeparationTypesAsync();

        #endregion

        #region Employee Separation Reasons Action Interfaces
        Task<bool> AddSeparationReasonAsync(EmployeeSeparationReason e);
        Task<bool> EditSeparationReasonAsync(EmployeeSeparationReason e);
        Task<bool> DeleteSeparationReasonAsync(int id);
        Task<EmployeeSeparationReason> GetSeparationReasonByIdAsync(int id);
        Task<List<EmployeeSeparationReason>> GetSeparationReasonsbyNameAsync(string name);
        Task<List<EmployeeSeparationReason>> GetAllSeparationReasonsAsync();

        #endregion
    }
}