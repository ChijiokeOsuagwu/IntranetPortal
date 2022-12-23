using IntranetPortal.Base.Models.EmployeeRecordModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IErmService
    {
        #region Employee Service Method Interfaces
        Task<bool> CreateEmployeeAsync(Employee employee);

        Task<bool> DeleteEmployeeAsync(string employeeId, string deletedBy, string deletedTime);

        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<bool> UpdateEmployeeImagePathAsync(string employeeId, string imagePath, string updatedBy);
        Task<Employee> GetEmployeeByIdAsync(string EmployeeID);

        Task<Employee> GetEmployeeByNameAsync(string employeeName);

        Task<bool> EmployeeExistsAsync(string EmployeeName);

        Task<List<Employee>> SearchEmployeesByNameAsync(string employeeName);

        Task<List<Employee>> GetAllEmployeesAsync();

        Task<List<Employee>> GetEmployeesByCompanyAsync(string CompanyID);

        Task<List<Employee>> GetEmployeesByLocationAsync(int LocationID, int DepartmentID, int UnitID);

        Task<List<Employee>> GetEmployeesByLocationAsync(int LocationID, int DepartmentID);

        Task<List<Employee>> GetEmployeesByLocationAsync(int LocationID);
        Task<List<Employee>> GetEmployeesByLocationAndUnitAsync(int LocationID, int UnitID);
        Task<List<Employee>> GetEmployeesByDepartmentIDAsync(int DepartmentID);
        Task<List<Employee>> GetEmployeesByUnitIDAsync(int UnitID);
        Task<List<Employee>> GetEmployeesByCompanyAndDepartmentAsync(string CompanyID, int DepartmentID);
        Task<List<Employee>> GetEmployeesByCompanyAndUnitAsync(string CompanyID, int UnitID);
        Task<List<Employee>> GetEmployeesByCompanyAndUnitAsync(string CompanyID, int LocationID, int UnitID);
        Task<List<Employee>> GetEmployeesByCompanyAndLocationAsync(string CompanyID, int LocationID);
        Task<List<Employee>> GetEmployeesByCompanyAndLocationAsync(string CompanyID, int LocationID, int DepartmentID);
        Task<List<Employee>> GetEmployeesByBirthDayAsync(int? BirthMonth, int? BirthDay);




        Task<List<Employee>> GetAllNonUserEmployeesAsync();

        Task<List<Employee>> GetNonUserEmployeesByNameAsync(string employeeName);

        #endregion

        #region EmployeeReportLine Action Methods
        Task<List<EmployeeReportLine>> GetActiveEmployeeReportLinesByEmployeeIdAsync(string employeeId);
        Task<List<EmployeeReportLine>> GetEmployeeReportLinesByEmployeeIdAsync(string employeeId);
        Task<EmployeeReportLine> GetEmployeeReportLineByIdAsync(int employeeReportLineId);
        Task<bool> CreateEmployeeReportLineAsync(EmployeeReportLine employeeReportLine);
        Task<bool> UpdateEmployeeReportLineAsync(EmployeeReportLine employeeReportLine);
        Task<bool> DeleteEmployeeReportLineAsync(int employeeReportLineId);
        #endregion
    }
}
