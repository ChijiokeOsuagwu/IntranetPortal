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
        Task<bool> UpdateEmployeeSeparationAsync(string empId, string recordedBy, DateTime? exitDate, bool isExitted = true);
        Task<bool> DeleteEmployeeAsync(string Id, string deletedBy, string deletedTime);

        Task<bool> EditEmployeeAsync(Employee employee);

        Task<Employee> GetEmployeeByIdAsync(string Id);

        Task<Employee> GetEmployeeByNameAsync(string employeeName);

        Task<IList<Employee>> GetEmployeesAsync(DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByNameAsync(string employeeName, DateTime? terminalDate = null);

        Task<IList<Employee>> GetOtherEmployeesByNameAsync(string employeeId, string otherEmployeeName, DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByLocationAsync(int locationId, DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByLocationAsync(int locationId, int deptId, DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByLocationAsync(int locationId, int deptId, int unitId, DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByCompanyCodeAsync(string companyCode, DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByCompanyCodeAsync(string companyCode, int locationId, DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByCompanyCodeAsync(string companyCode, int locationId, int departmentId, DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByCompanyCodeAndUnitAsync(string companyCode, int locationId, int unitId, DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByCompanyCodeAndUnitAsync(string companyCode, int unitId, DateTime? terminalDate = null);
        Task<IList<Employee>> GetEmployeesByCompanyCodeAndDeptAsync(string companyCode, int deptId, DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByLocationAndUnitAsync(int locationId, int unitId, DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByUnitAsync(int unitId, DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByDeptAsync(int deptId, DateTime? terminalDate = null);

        Task<IList<Employee>> GetEmployeesByBirthMonthAsync(int birthMonth);

        Task<IList<Employee>> GetEmployeesByBirthMonthAndBirthDayAsync(int birthMonth, int birthDay);

        Task<IList<Employee>> GetEmployeesWithoutUserAccountsByNameAsync(string employeeName, DateTime? TerminalDate = null);

        Task<long> GetEmployeesCountByStartUpDateAsync(int startUpYear, int startUpMonth, int startUpDay);

        Task<IList<Employee>> GetAllEmployeesWithoutUserAccountsAsync(DateTime? TerminalDate = null);

        Task<IList<Employee>> GetEmployeesByLeaveProfileIdAsync(int leaveProfileId);
        Task<IList<EmployeeRoll>> GetEmployeeRollsByLeaveProfileIdAsync(int leaveProfileId);

        #endregion

        #region Employee Count Action Methods
        Task<long> GetEmployeesCountAsync(DateTime? terminalDate = null);
        Task<long> GetEmployeesCountByLocationIdAsync(int locationId, DateTime? terminalDate = null);
        Task<long> GetEmployeesCountByLocationIdnDepartmentIdAsync(int locationId, int deptId, DateTime? terminalDate = null);
        Task<long> GetEmployeesCountByLocationIdnDepartmentIdnUnitIdAsync(int locationId, int deptId, int unitId, DateTime? terminalDate = null);
        Task<long> GetEmployeesCountByLocationIdnUnitIdAsync(int locationId, int unitId, DateTime? terminalDate = null);
        Task<long> GetEmployeesCountByDepartmentIdAsync(int deptId, DateTime? terminalDate = null);
        Task<long> GetEmployeesCountByDepartmentIdnUnitIdAsync(int deptId, int unitId, DateTime? terminalDate = null);
        Task<long> GetEmployeesCountByUnitIdAsync(int unitId, DateTime? terminalDate = null);
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
