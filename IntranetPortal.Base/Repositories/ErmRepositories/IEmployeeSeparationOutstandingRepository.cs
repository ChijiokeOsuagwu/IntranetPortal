using IntranetPortal.Base.Models.EmployeeRecordModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.ErmRepositories
{
    public interface IEmployeeSeparationOutstandingRepository
    {
        IConfiguration _config { get; }

        #region Employee Separation Outstanding Action Methods 

        Task<bool> AddAsync(EmployeeSeparationOutstanding e);
        Task<bool> DeleteAsync(int id);
        Task<bool> EditAsync(EmployeeSeparationOutstanding e);
        Task<bool> DeleteAsync(int employeeSeparationId, string itemDescription);
        Task<List<EmployeeSeparationOutstanding>> GetByEmployeeIdAsync(string employeeId);
        Task<List<EmployeeSeparationOutstanding>> GetBySeparationIdAsync(int employeeSeparationId);
        Task<EmployeeSeparationOutstanding> GetByIdAsync(int id);
        #endregion

        #region Employee Separation Payments Action Methods
        Task<List<EmployeeSeparationPayments>> GetPaymentsByEmployeeIdAsync(string employeeId);
        Task<List<EmployeeSeparationPayments>> GetPaymentsBySeparationIdAsync(int employeeSeparationId);
        Task<List<EmployeeSeparationPayments>> GetPaymentsBySeparationOutstandingIdAsync(int employeeSeparationOutstandingId);
        Task<EmployeeSeparationPayments> GetPaymentByIdAsync(int separationPaymentId);
        Task<bool> AddPaymentAsync(EmployeeSeparationPayments p);
        Task<bool> EditPaymentAsync(EmployeeSeparationPayments p);
        Task<bool> DeletePaymentAsync(int id);
        #endregion

        #region Separation Outstanding Items Action Methods
        Task<bool> AddSeparationOutstandingItemAsync(SeparationOutstandingItem item);
        Task<bool> EditSeparationOutstandingItemAsync(SeparationOutstandingItem item);
        Task<bool> DeleteSeparationOutstandingItemAsync(int id);
        Task<SeparationOutstandingItem> GetSeparationOutstandingItemByIdAsync(int id);
        Task<List<SeparationOutstandingItem>> GetSeparationOutstandingItemsAsync();

        #endregion
    }
}