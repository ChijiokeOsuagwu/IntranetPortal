using IntranetPortal.Base.Models.EmployeeRecordModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.ErmRepositories
{
    public interface IEmployeesRepository
    {
        #region Employee Action Methods
        Task<bool> AddEmployeeAsync(Employee employee);
        Task<bool> UpdateEmployeeSeparationAsync(string empId, string recordedBy, string recordedTime);
        Task<bool> DeleteEmployeeAsync(string Id, string deletedBy, string deletedTime);

        Task<bool> EditEmployeeAsync(Employee employee);

        Task<Employee> GetEmployeeByIdAsync(string Id);

        Task<Employee> GetEmployeeByNameAsync(string employeeName);

        Task<IList<Employee>> GetEmployeesAsync();

        Task<IList<Employee>> GetEmployeesByNameAsync(string employeeName);

        Task<IList<Employee>> GetOtherEmployeesByNameAsync(string employeeId, string otherEmployeeName);

        Task<IList<Employee>> GetEmployeesByLocationAsync(int locationId);

        Task<IList<Employee>> GetEmployeesByLocationAsync(int locationId, int deptId);

        Task<IList<Employee>> GetEmployeesByLocationAsync(int locationId, int deptId, int unitId);

        Task<IList<Employee>> GetEmployeesByCompanyCodeAsync(string companyCode);

        Task<IList<Employee>> GetEmployeesByCompanyCodeAsync(string companyCode, int locationId);

        Task<IList<Employee>> GetEmployeesByCompanyCodeAsync(string companyCode, int locationId, int departmentId);

        Task<IList<Employee>> GetEmployeesByCompanyCodeAndUnitAsync(string companyCode, int locationId, int unitId);

        Task<IList<Employee>> GetEmployeesByCompanyCodeAndUnitAsync(string companyCode, int unitId);
        Task<IList<Employee>> GetEmployeesByCompanyCodeAndDeptAsync(string companyCode, int deptId);

        Task<IList<Employee>> GetEmployeesByLocationAndUnitAsync(int locationId, int unitId);

        Task<IList<Employee>> GetEmployeesByUnitAsync(int unitId);

        Task<IList<Employee>> GetEmployeesByDeptAsync(int deptId);

        Task<IList<Employee>> GetEmployeesByBirthMonthAsync(int birthMonth);

        Task<IList<Employee>> GetEmployeesByBirthMonthAndBirthDayAsync(int birthMonth, int birthDay);

        Task<IList<Employee>> GetEmployeesWithoutUserAccountsByNameAsync(string employeeName);

        Task<int> GetEmployeesCountByStartUpDateAsync(int startUpYear, int startUpMonth, int startUpDay);

        Task<IList<Employee>> GetAllEmployeesWithoutUserAccountsAsync();

        #endregion

        #region Employee Reporting Lines Action Methods
        Task<bool> AddEmployeeReportLineAsync(EmployeeReportLine employeeReportLine);
        Task<bool> DeleteEmployeeReportLineAsync(int employeeReportId);
        Task<bool> EditEmployeeReportLineAsync(EmployeeReportLine employeeReportLine);
        Task<EmployeeReportLine> GetEmployeeReportLineByIdAsync(int employeeReportLineId);
        Task<IList<EmployeeReportLine>> GetEmployeeReportLinesByEmployeeIdAsync(string employeeId);
        Task<IList<EmployeeReportLine>> GetActiveEmployeeReportLinesByEmployeeIdAsync(string employeeId);
        Task<IList<EmployeeReportLine>> GetEmployeeReportsByReportsToEmployeeIdAsync(string reportsToEmployeeId);
        Task<IList<EmployeeReportLine>> GetActiveEmployeeReportsByReportsToEmployeeIdAsync(string reportsToEmployeeId);
        #endregion

    }
}
