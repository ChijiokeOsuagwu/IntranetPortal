using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.ErmRepository;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class ErmService : IErmService
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IUtilityRepository _utilityRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUnitRepository _unitRepository;
        public ErmService(IEmployeesRepository employeesRepository, IPersonRepository personRepository,
                          IUtilityRepository utilityRepository, ILocationRepository locationRepository,
                          IUnitRepository unitRepository)
        {
            _employeesRepository = employeesRepository;
            _personRepository = personRepository;
            _utilityRepository = utilityRepository;
            _locationRepository = locationRepository;
            _unitRepository = unitRepository;
        }

        #region Employee Service Methods
        public async Task<bool> CreateEmployeeAsync(Employee employee, bool personExists = false)
        {
            if (employee == null) { throw new ArgumentNullException(nameof(employee), "The required parameter [employee] is missing."); }

            try
            {
                bool PersonIsAdded = false;
                var entity = await _employeesRepository.GetEmployeeByNameAsync(employee.FullName);
                if (entity != null && !string.IsNullOrWhiteSpace(entity.EmployeeID))
                {
                    throw new Exception("Sorry, this employee already has a record in the system.");
                }
                if (personExists) { PersonIsAdded = true; }
                else
                {
                    Person person = employee.ToPerson();
                    PersonIsAdded = await _personRepository.AddPersonAsync(person);
                }

                if (PersonIsAdded)
                {
                    int day = DateTime.Now.Day;
                    int month = DateTime.Now.Month;
                    int year = DateTime.Now.Year;
                    if (employee.StartUpDate.HasValue)
                    {
                        day = employee.StartUpDate.Value.Day;
                        month = employee.StartUpDate.Value.Month;
                        year = employee.StartUpDate.Value.Year;
                    }
                    
                    int recordCount = await _utilityRepository.GetNumberCount(AutoNumberType.EmployeeNumber, day, month, year);
                    string yy = year.ToString().Substring(2, 2);
                    string mm = month.ToString().PadLeft(2, '0');
                    string dd = day.ToString().PadLeft(2, '0');
                    string nn = (recordCount + 1).ToString().PadLeft(2, '0');

                    employee.EmployeeNo1 = $"{employee.CompanyID}{yy}{mm}{dd}{nn}";

                    State state = await _locationRepository.GetStateByNameAsync(employee.StateOfOrigin);
                    if (state != null) { employee.GeoPoliticalRegion = state.Region; }

                    Unit unit = await _unitRepository.GetUnitByIdAsync(employee.UnitID.Value);
                    if (unit != null) { employee.DepartmentID = unit.DepartmentID; }

                    bool EmployeeIsAdded = await _employeesRepository.AddEmployeeAsync(employee);
                    if (EmployeeIsAdded & !string.IsNullOrWhiteSpace(nn))
                    {
                        await _utilityRepository.AddCodeNumberRecord(AutoNumberType.EmployeeNumber, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                    }
                    return EmployeeIsAdded;
                }
                else
                {
                    throw new Exception("Prerequisite Operation Failure. Person Info could not be added.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            if (employee == null) { throw new ArgumentNullException(nameof(employee), "The required parameter [employee] is missing."); }

            try
            {
                Person person = employee.ToPerson();
                bool PersonIsUpdated = await _personRepository.EditPersonAsync(person);
                if (PersonIsUpdated)
                {
                    State state = await _locationRepository.GetStateByNameAsync(employee.StateOfOrigin);
                    if (state != null) { employee.GeoPoliticalRegion = state.Region; }

                    Unit unit = await _unitRepository.GetUnitByIdAsync(employee.UnitID.Value);
                    if (unit != null) { employee.DepartmentID = unit.DepartmentID; }

                    bool EmployeeIsUpdated = await _employeesRepository.EditEmployeeAsync(employee);
                    return EmployeeIsUpdated;
                }
                else
                {
                    throw new Exception("Prerequisite Operation Failure. Person Info could not be added.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateEmployeeImagePathAsync(string employeeId, string imagePath, string updatedBy)
        {

            if (string.IsNullOrWhiteSpace(employeeId)) { throw new ArgumentNullException(nameof(employeeId)); }
            if (string.IsNullOrWhiteSpace(imagePath)) { throw new ArgumentNullException(nameof(imagePath)); }

            try
            {
               return await _personRepository.EditPersonImagePathAsync(employeeId,imagePath,updatedBy);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteEmployeeAsync(string employeeId, string deletedBy, string deletedTime)
        {
            bool personIsDeleted = false;
            bool employeeIsDeleted = false;
            if (string.IsNullOrEmpty(employeeId)) { throw new ArgumentNullException(nameof(employeeId)); }
            if (string.IsNullOrEmpty(deletedBy)) { throw new ArgumentNullException(nameof(deletedBy)); }
            if (string.IsNullOrEmpty(deletedTime)) { throw new ArgumentNullException(nameof(deletedTime)); }

            try
            {
                employeeIsDeleted = await _employeesRepository.DeleteEmployeeAsync(employeeId, deletedBy, deletedTime);
                if (employeeIsDeleted)
                {
                    personIsDeleted = await _personRepository.DeletePersonAsync(employeeId, deletedBy, deletedTime);
                    return employeeIsDeleted;
                }
                else
                {
                    throw new Exception("Prerequisite Operation Failure. An error was encountered while attempting to delete employee.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> EmployeeExistsAsync(string EmployeeName)
        {
            Employee employee = new Employee();
            try
            {
                var entity = await _employeesRepository.GetEmployeeByNameAsync(EmployeeName);
                employee = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employee != null;
        }

        public async Task<Employee> GetEmployeeByIdAsync(string EmployeeID)
        {
            Employee employee = new Employee();
            try
            {
                var entity = await _employeesRepository.GetEmployeeByIdAsync(EmployeeID);
                employee = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employee;
        }

        public async Task<Employee> GetEmployeeByNameAsync(string employeeName)
        {
            Employee employee = new Employee();
            try
            {
                var entity = await _employeesRepository.GetEmployeeByNameAsync(employeeName);
                employee = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employee;
        }

        public async Task<List<Employee>> SearchEmployeesByNameAsync(string employeeName)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByNameAsync(employeeName);
                if (entities != null) { employees = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesAsync();
                if (entities != null) { employees = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetEmployeesByCompanyAsync(string CompanyID)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByCompanyCodeAsync(CompanyID);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }


        public async Task<List<Employee>> GetEmployeesByLocationAsync(int LocationID, int DepartmentID, int UnitID)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByLocationAsync(LocationID,DepartmentID,UnitID);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetEmployeesByLocationAsync(int LocationID, int DepartmentID)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByLocationAsync(LocationID, DepartmentID);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetEmployeesByLocationAsync(int LocationID)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByLocationAsync(LocationID);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetEmployeesByLocationAndUnitAsync(int LocationID, int UnitID)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByLocationAndUnitAsync(LocationID, UnitID);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetEmployeesByUnitIDAsync(int UnitID)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByUnitAsync(UnitID);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetEmployeesByDepartmentIDAsync(int DepartmentID)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByDeptAsync(DepartmentID);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }



        public async Task<List<Employee>> GetEmployeesByCompanyAndLocationAsync(string CompanyID, int LocationID, int DepartmentID)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByCompanyCodeAsync(CompanyID, LocationID, DepartmentID);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetEmployeesByCompanyAndLocationAsync(string CompanyID, int LocationID)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByCompanyCodeAsync(CompanyID, LocationID);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetEmployeesByCompanyAndUnitAsync(string CompanyID, int LocationID, int UnitID)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByCompanyCodeAndUnitAsync(CompanyID,LocationID, UnitID);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetEmployeesByCompanyAndUnitAsync(string CompanyID, int UnitID)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByCompanyCodeAndUnitAsync(CompanyID, UnitID);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetEmployeesByCompanyAndDepartmentAsync(string CompanyID, int DepartmentID)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesByCompanyCodeAndDeptAsync(CompanyID, DepartmentID);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetEmployeesByBirthDayAsync(int? BirthMonth, int? BirthDay)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                if(BirthMonth != null && BirthMonth > 0)
                {
                    if(BirthDay != null && BirthDay.Value > 0)
                    {
                        var entities = await _employeesRepository.GetEmployeesByBirthMonthAndBirthDayAsync(BirthMonth.Value, BirthDay.Value);
                        employees = entities.ToList();
                    }
                    else
                    {
                        var entities = await _employeesRepository.GetEmployeesByBirthMonthAsync(BirthMonth.Value);
                        employees = entities.ToList();
                    }
                }
                else
                {
                    BirthMonth = DateTime.Now.Month;
                    if (BirthDay != null && BirthDay.Value > 0)
                    {
                        var entities = await _employeesRepository.GetEmployeesByBirthMonthAndBirthDayAsync(BirthMonth.Value, BirthDay.Value);
                        employees = entities.ToList();
                    }
                    else
                    {
                        BirthDay = DateTime.Now.Day;
                        var entities = await _employeesRepository.GetEmployeesByBirthMonthAsync(BirthMonth.Value);
                        employees = entities.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }



        public async Task<List<Employee>> GetAllNonUserEmployeesAsync()
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetAllEmployeesWithoutUserAccountsAsync();
                if (entities != null) { employees = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<List<Employee>> GetNonUserEmployeesByNameAsync(string employeeName)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetEmployeesWithoutUserAccountsByNameAsync(employeeName);
                if (entities != null) { employees = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        #endregion

        //========================== EmployeeReportLine Service Methods ==============================================//
        #region EmployeeReportLine Action Methods
        public async Task<bool> CreateEmployeeReportLineAsync(EmployeeReportLine employeeReportLine)
        {
            if (employeeReportLine == null) { throw new ArgumentNullException(nameof(employeeReportLine), "The required parameter [EmployeeReportLine] is missing."); }
            try
            {
                return await _employeesRepository.AddEmployeeReportLineAsync(employeeReportLine);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> UpdateEmployeeReportLineAsync(EmployeeReportLine employeeReportLine)
        {
            if (employeeReportLine == null) { throw new ArgumentNullException(nameof(employeeReportLine), "The required parameter [EmployeeReportLine] is missing."); }
            try
            {
                return await _employeesRepository.EditEmployeeReportLineAsync(employeeReportLine);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> DeleteEmployeeReportLineAsync(int employeeReportLineId)
        {
            if (employeeReportLineId < 1) { throw new ArgumentNullException(nameof(employeeReportLineId), "The required parameter [EmployeeReportLineID] is missing."); }
            try
            {
                return await _employeesRepository.DeleteEmployeeReportLineAsync(employeeReportLineId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<EmployeeReportLine>> GetActiveEmployeeReportLinesByEmployeeIdAsync(string employeeId)
        {
            List<EmployeeReportLine> employeeReportLines = new List<EmployeeReportLine>();
            try
            {
                var entities = await _employeesRepository.GetActiveEmployeeReportLinesByEmployeeIdAsync(employeeId);
                employeeReportLines = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employeeReportLines;
        }
        public async Task<List<EmployeeReportLine>> GetEmployeeReportLinesByEmployeeIdAsync(string employeeId)
        {
            List<EmployeeReportLine> employeeReportLines = new List<EmployeeReportLine>();
            try
            {
                var entities = await _employeesRepository.GetEmployeeReportLinesByEmployeeIdAsync(employeeId);
                employeeReportLines = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employeeReportLines;
        }
        public async Task<EmployeeReportLine> GetEmployeeReportLineByIdAsync(int employeeReportLineId)
        {
            EmployeeReportLine employeeReportLine = new EmployeeReportLine();
            try
            {
                var entity = await _employeesRepository.GetEmployeeReportLineByIdAsync(employeeReportLineId);
                employeeReportLine = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employeeReportLine;
        }


        public async Task<List<EmployeeReportLine>> GetActiveEmployeeReportsByEmployeeIdAsync(string reportsToEmployeeId)
        {
            List<EmployeeReportLine> employeeReportLines = new List<EmployeeReportLine>();
            try
            {
                var entities = await _employeesRepository.GetActiveEmployeeReportsByReportsToEmployeeIdAsync(reportsToEmployeeId);
                employeeReportLines = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employeeReportLines;
        }
        public async Task<List<EmployeeReportLine>> GetEmployeeReportsByReportsToEmployeeIdAsync(string reportsToEmployeeId)
        {
            List<EmployeeReportLine> employeeReportLines = new List<EmployeeReportLine>();
            try
            {
                var entities = await _employeesRepository.GetEmployeeReportsByReportsToEmployeeIdAsync(reportsToEmployeeId);
                employeeReportLines = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employeeReportLines;
        }

        #endregion
    }
}
