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
        Task<bool> CreateEmployeeAsync(Employee employee, bool personExists = false);

        Task<bool> DeleteEmployeeAsync(string employeeId, string deletedBy, string deletedTime);

        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<bool> UpdateEmployeeImagePathAsync(string employeeId, string imagePath, string updatedBy);
        Task<Employee> GetEmployeeByIdAsync(string EmployeeID);

        Task<Employee> GetEmployeeByNameAsync(string employeeName);

        Task<bool> EmployeeExistsAsync(string EmployeeName);

        Task<List<Employee>> SearchEmployeesByNameAsync(string employeeName);

        Task<List<Employee>> SearchOtherEmployeesByNameAsync(string employeeId, string otherEmployeeName);


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


        Task<List<EmployeeReportLine>> GetEmployeeReportsByReportsToEmployeeIdAsync(string reportsToEmployeeId);
        Task<List<EmployeeReportLine>> GetActiveEmployeeReportsByEmployeeIdAsync(string reportsToEmployeeId);


        Task<bool> CreateEmployeeReportLineAsync(EmployeeReportLine employeeReportLine);
        Task<bool> UpdateEmployeeReportLineAsync(EmployeeReportLine employeeReportLine);
        Task<bool> DeleteEmployeeReportLineAsync(int employeeReportLineId);
        #endregion

        #region Employee Separation Action Methods
        Task<bool> AddEmployeeSeparationAsync(EmployeeSeparation e);
        Task<bool> EditEmployeeSeparationAsync(EmployeeSeparation e);
        Task<bool> DeleteEmployeeSeparationAsync(int EmployeeSeparationId);
        Task<EmployeeSeparation> GetEmployeeSeparationAsync(int employeeSeparationId);
        Task<List<EmployeeSeparation>> GetEmployeeSeparationsAsync(string employeeId);
        Task<List<EmployeeSeparation>> GetEmployeeSeparationsAsync(DateTime? startDate = null, DateTime? endDate = null);
        #endregion
        
        #region Employee Separation Outstanding Action Methods
        Task<EmployeeSeparationOutstanding> GetSeparationOutstandingAsync(int SeparationOutstandingId);
        Task<List<EmployeeSeparationOutstanding>> GetSeparationOutstandingsAsync(string employeeId);
        Task<List<EmployeeSeparationOutstanding>> GetSeparationOutstandingsAsync(int employeeSeparationId);
        Task<bool> AddEmployeeSeparationOutstandingAsync(EmployeeSeparationOutstanding e);
        Task<bool> DeleteEmployeeSeparationOutstandingAsync(int id);
        Task<bool> UpdateEmployeeSeparationOutstandingAsync(EmployeeSeparationOutstanding e);

        #endregion

        #region Employee Separation Payments Action Methods
        Task<EmployeeSeparationPayments> GetSeparationPaymentAsync(int SeparationPaymentId);
        Task<List<EmployeeSeparationPayments>> GetSeparationPaymentsAsync(string employeeId);
        Task<List<EmployeeSeparationPayments>> GetSeparationPaymentsAsync(int employeeSeparationId);
        Task<List<EmployeeSeparationPayments>> GetSeparationPaymentsBySeparationOutstandingIdAsync(int employeeSeparationOutstandingId);
        Task<bool> AddEmployeeSeparationPaymentAsync(EmployeeSeparationPayments p);
        Task<bool> DeleteEmployeeSeparationPaymentAsync(int id);
        Task<bool> UpdateEmployeeSeparationPaymentAsync(EmployeeSeparationPayments p);
        #endregion

        #region Employee Separation Outstanding Items Service Interfaces
        Task<SeparationOutstandingItem> GetSeparationOutstandingItemAsync(int SeparationOutstandingItemId);
        Task<List<SeparationOutstandingItem>> GetSeparationOutstandingItemsAsync();
        #endregion

        #region Employee Separation Types Service Methods
        Task<bool> CreateEmployeeSeparationTypeAsync(EmployeeSeparationType t);
        Task<bool> DeleteEmployeeSeparationTypeAsync(int id);
        Task<bool> UpdateEmployeeSeparationTypeAsync(EmployeeSeparationType t);
        Task<IList<EmployeeSeparationType>> GetEmployeeSeparationTypesAsync();
        Task<EmployeeSeparationType> GetEmployeeSeparationTypeByIdAsync(int id);
        #endregion

        #region Employee Separation Reasons Service Methods
        Task<bool> CreateEmployeeSeparationReasonAsync(EmployeeSeparationReason r);
        Task<bool> DeleteEmployeeSeparationReasonAsync(int id);
        Task<bool> UpdateEmployeeSeparationReasonAsync(EmployeeSeparationReason t);
        Task<IList<EmployeeSeparationReason>> GetEmployeeSeparationReasonsAsync();
        Task<EmployeeSeparationReason> GetEmployeeSeparationReasonByIdAsync(int id);
        #endregion
    }
}
