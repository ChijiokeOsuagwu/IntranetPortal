using IntranetPortal.Base.Models.EmployeeRecordModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.ErmRepository
{
    public interface IEmployeesRepository
    {
        #region Employee Action Methods
        Task<bool> AddEmployeeAsync(Employee employee);

        Task<bool> DeleteEmployeeAsync(string Id, string deletedBy, string deletedTime);

        Task<bool> EditEmployeeAsync(Employee employee);

        Task<Employee> GetEmployeeByIdAsync(string Id);

        Task<Employee> GetEmployeeByNameAsync(string employeeName);

        Task<IList<Employee>> GetEmployeesAsync();

        Task<IList<Employee>> GetEmployeesByNameAsync(string employeeName);

        Task<IList<Employee>> GetEmployeesByLocationAsync(int locationId);

        Task<IList<Employee>> GetEmployeesByLocationAsync(int locationId, int deptId);

        Task<IList<Employee>> GetEmployeesByLocationAsync(int locationId, int deptId, int unitId);

        Task<IList<Employee>> GetEmployeesWithoutUserAccountsByNameAsync(string employeeName);

        Task<IList<Employee>> GetAllEmployeesWithoutUserAccountsAsync();

        #endregion

        #region Employee Reporting Lines Action Methods
        Task<bool> AddEmployeeReportLineAsync(EmployeeReportLine employeeReportLine);
        Task<bool> DeleteEmployeeReportLineAsync(int employeeReportId);
        Task<bool> EditEmployeeReportLineAsync(EmployeeReportLine employeeReportLine);
        Task<EmployeeReportLine> GetEmployeeReportLineByIdAsync(int employeeReportLineId);
        Task<IList<EmployeeReportLine>> GetEmployeeReportLinesByEmployeeIdAsync(string employeeId);
        Task<IList<EmployeeReportLine>> GetActiveEmployeeReportLinesByEmployeeIdAsync(string employeeId);
        Task<IList<EmployeeReportLine>> GetEmployeeReportLinesByReportsToEmployeeIdAsync(string reportsToEmployeeId);
        Task<IList<EmployeeReportLine>> GetActiveEmployeeReportLinesByReportsToEmployeeIdAsync(string reportsToEmployeeId);
        #endregion

    }
}
