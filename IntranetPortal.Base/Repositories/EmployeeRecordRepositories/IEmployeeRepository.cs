using IntranetPortal.Base.Models.EmployeeRecordModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.EmployeeRecordRepositories
{
    public interface IEmployeeRepository
    {
        Task<bool> AddEmployeeAsync(Employee employee);

        Task<bool> DeleteEmployeeAsync(string Id);

        Task<bool> EditEmployeeAsync(Employee employee);

        Task<Employee> GetEmployeeByIdAsync(string Id);

        Task<IList<Employee>> GetEmployeesAsync();

        Task<IList<Employee>> GetEmployeesByNameAsync(string employeeName);

        Task<bool> AddEmployeeBasicInfoAsync(EmployeeBasicInfo employeeBasicInfo);

        Task<bool> EditEmployeeBasicInfoAsync(EmployeeBasicInfo employeeBasicInfo);

        Task<bool> EditEmployeeNextOfKinInfoAsync(EmployeeNextOfKinInfo employeeNextOfKinInfo);
        Task<bool> EditEmployeeHistoryInfoAsync(EmployeeHistoryInfo employeeHistoryInfo);
    }
}
