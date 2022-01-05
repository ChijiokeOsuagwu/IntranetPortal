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
        Task<List<Employee>> GetAllEmployeesAsync();
        Task<List<Employee>> GetEmployeesByNameAsync(string employeeName);
        Task<Employee> GetEmployeesByIdAsync(string EmployeeID);
        Task<bool> EmployeeExistsAsync(string EmployeeID);
        Task<bool> CreateEmployeeBasicInfoAsync(EmployeeBasicInfo employeeBasicInfo);
        Task<bool> UpdateEmployeeBasicInfoAsync(EmployeeBasicInfo employeeBasicInfo);
        Task<bool> UpdateEmployeeNextOfKinInfoAsync(EmployeeNextOfKinInfo employeeNextOfKinInfo);
        Task<bool> UpdateEmployeeHistoryInfoAsync(EmployeeHistoryInfo employeeHistoryInfo);
    }
}
