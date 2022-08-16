using IntranetPortal.Base.Models.EmployeeRecordModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IEmployeeRecordService
    {
        Task<bool> CreateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(string employeeId);
        Task<List<Employee>> GetAllEmployeesAsync();
        Task<List<Employee>> GetEmployeesByNameAsync(string employeeName);
        Task<List<string>> GetEmployeeNamesByNameAsync(string employeeName);
        Task<Employee> GetEmployeesByIdAsync(string EmployeeID);
        Task<List<Employee>> GetNonUserEmployeesByNameAsync(string employeeName);
        Task<List<Employee>> GetNonUserEmployeesAsync();
        Task<bool> EmployeeExistsAsync(string EmployeeID);
        Task<bool> CreateEmployeeBasicInfoAsync(EmployeeBasicInfo employeeBasicInfo);
        Task<bool> UpdateEmployeeBasicInfoAsync(EmployeeBasicInfo employeeBasicInfo);
        Task<bool> UpdateEmployeeNextOfKinInfoAsync(EmployeeNextOfKinInfo employeeNextOfKinInfo);
        Task<bool> UpdateEmployeeHistoryInfoAsync(EmployeeHistoryInfo employeeHistoryInfo);
    }
}
