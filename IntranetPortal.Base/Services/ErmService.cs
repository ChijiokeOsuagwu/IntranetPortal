using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.ErmRepositories;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using IntranetPortal.Base.Repositories.SecurityRepositories;
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
        private readonly IEmployeeSeparationRepository _employeeSeparationRepository;
        private readonly IEmployeeSeparationOutstandingRepository _separationOutstandingRepository;
        private readonly IUserRepository _userRepository;

        public ErmService(IEmployeesRepository employeesRepository, IPersonRepository personRepository,
                          IUtilityRepository utilityRepository, ILocationRepository locationRepository,
                          IUnitRepository unitRepository, IEmployeeSeparationRepository employeeSeparationRepository,
                          IEmployeeSeparationOutstandingRepository separationOutstandingRespository,
                          IUserRepository userRepository)
        {
            _employeesRepository = employeesRepository;
            _personRepository = personRepository;
            _utilityRepository = utilityRepository;
            _locationRepository = locationRepository;
            _unitRepository = unitRepository;
            _userRepository = userRepository;
            _employeeSeparationRepository = employeeSeparationRepository;
            _separationOutstandingRepository = separationOutstandingRespository;
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

                    // int recordCount = await _utilityRepository.GetNumberCount(AutoNumberType.EmployeeNumber, day, month, year);
                    int recordCount = await _employeesRepository.GetEmployeesCountByStartUpDateAsync(year, month, day);
                    string yy = year.ToString().Substring(2, 2);
                    string mm = month.ToString().PadLeft(2, '0');
                    string dd = day.ToString().PadLeft(2, '0');
                    string nn = (recordCount + 1).ToString().PadLeft(2, '0');

                    employee.EmployeeNo1 = $"{employee.CompanyID}{yy}{mm}{dd}{nn}";

                    if (!string.IsNullOrWhiteSpace(employee.StateOfOrigin))
                    {
                        State state = await _locationRepository.GetStateByNameAsync(employee.StateOfOrigin);
                        if (state != null) { employee.GeoPoliticalRegion = state.Region; }
                    }

                    Unit unit = await _unitRepository.GetUnitByIdAsync(employee.UnitID.Value);
                    if (unit != null) { employee.DepartmentID = unit.DepartmentID; }

                    bool EmployeeIsAdded = await _employeesRepository.AddEmployeeAsync(employee);
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
                    if (!string.IsNullOrWhiteSpace(employee.StateOfOrigin))
                    {
                        State state = await _locationRepository.GetStateByNameAsync(employee.StateOfOrigin);
                        if (state != null) { employee.GeoPoliticalRegion = state.Region; }
                    }

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
                return await _personRepository.EditPersonImagePathAsync(employeeId, imagePath, updatedBy);
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

        public async Task<List<Employee>> SearchOtherEmployeesByNameAsync(string employeeId, string otherEmployeeName)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeesRepository.GetOtherEmployeesByNameAsync(employeeId, otherEmployeeName);
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
                var entities = await _employeesRepository.GetEmployeesByLocationAsync(LocationID, DepartmentID, UnitID);
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
                var entities = await _employeesRepository.GetEmployeesByCompanyCodeAndUnitAsync(CompanyID, LocationID, UnitID);
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
                if (BirthMonth != null && BirthMonth > 0)
                {
                    if (BirthDay != null && BirthDay.Value > 0)
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

        #region Employee Separation Service Methods
        public async Task<bool> AddEmployeeSeparationAsync(EmployeeSeparation e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e), "The required parameter [employee] is missing."); }

            bool EmployeeSeparationRecordIsAdded = false;
            bool EmployeeRecordIsUpdated = false;
            bool UserRecordIsUpdated = false;

            EmployeeSeparationRecordIsAdded = await _employeeSeparationRepository.AddAsync(e);
            if (EmployeeSeparationRecordIsAdded)
            {
                string recordedDate = $"{e.RecordCreatedDate.Value.ToLongDateString() } {e.RecordCreatedDate.Value.ToLongTimeString()} WAT";
                EmployeeRecordIsUpdated = await _employeesRepository.UpdateEmployeeSeparationAsync(e.EmployeeId, e.RecordCreatedBy, recordedDate);
                if (EmployeeRecordIsUpdated)
                {
                    UserRecordIsUpdated = await _userRepository.UpdateUserActivationAsync(e.EmployeeId, e.RecordCreatedBy, true);
                    if (UserRecordIsUpdated) { return true; }
                    else
                    {
                        throw new Exception("Operation was successful but the employee's user account was not deactivated. Please deactivate user account manually.");
                    }
                }
                else
                {
                    await _employeeSeparationRepository.DeleteAsync(e.EmployeeId, e.RecordCreatedDate.Value);
                    return false;
                }
            }
            else
            {
                throw new Exception("Prerequisite Operation Failure. New Employee Exit record could not be added.");
            }
        }

        public async Task<bool> EditEmployeeSeparationAsync(EmployeeSeparation e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e), "The required parameter [employee] is missing."); }

            try
            {
                bool RecordIsAdded = false;
                bool UpdateIsSuccessful = false;
                RecordIsAdded = await _employeeSeparationRepository.EditAsync(e);
                if (RecordIsAdded)
                {
                    string recordedDate = $"{e.RecordCreatedDate.Value.ToLongDateString() } {e.RecordCreatedDate.Value.ToLongTimeString()} WAT";
                    UpdateIsSuccessful = await _employeesRepository.UpdateEmployeeSeparationAsync(e.EmployeeId, e.RecordCreatedBy, recordedDate);
                    if (UpdateIsSuccessful) { return true; }
                    else
                    {
                        await _employeeSeparationRepository.DeleteAsync(e.EmployeeId, e.RecordCreatedDate.Value);
                        return false;
                    }
                }
                else
                {
                    throw new Exception("Prerequisite Operation Failure. Employee Exit record could not be updated.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteEmployeeSeparationAsync(int EmployeeSeparationId)
        {
            bool RecordIsDeleted = false;
            if (EmployeeSeparationId < 1) { throw new ArgumentNullException(nameof(EmployeeSeparationId), "The required parameter [Employee Exit ID] is missing."); }
            string EmployeeID = string.Empty;
            try
            {
                var entity = await _employeeSeparationRepository.GetByIdAsync(EmployeeSeparationId);
                if (entity != null)
                {
                    EmployeeID = entity.EmployeeId;
                }

                RecordIsDeleted = await _employeeSeparationRepository.DeleteAsync(EmployeeSeparationId);
                if (RecordIsDeleted)
                {
                    string recordedDate = string.Empty;
                    string RecordCreatedBy = string.Empty;
                    await _employeesRepository.UpdateEmployeeSeparationAsync(EmployeeID, RecordCreatedBy, recordedDate);
                }
                else
                {
                    throw new Exception("Prerequisite Operation Failure. Employee Exit record could not be updated.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return RecordIsDeleted;
        }

        public async Task<EmployeeSeparation> GetEmployeeSeparationAsync(int employeeSeparationId)
        {
            EmployeeSeparation e = new EmployeeSeparation();
            try
            {
                var entity = await _employeeSeparationRepository.GetByIdAsync(employeeSeparationId);
                e = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return e;
        }

        public async Task<List<EmployeeSeparation>> GetEmployeeSeparationsAsync(string employeeId)
        {
            List<EmployeeSeparation> separationList = new List<EmployeeSeparation>();
            try
            {
                var entities = await _employeeSeparationRepository.GetByEmployeeIdAsync(employeeId);
                separationList = entities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return separationList;
        }

        public async Task<List<EmployeeSeparation>> GetEmployeeSeparationsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();
            if (startDate == null) { StartDate = DateTime.Today.AddYears(-1); }
            else { StartDate = startDate.Value; }
            if (endDate == null) { EndDate = DateTime.Today; }
            else { EndDate = endDate.Value; }
            List<EmployeeSeparation> separationList = new List<EmployeeSeparation>();
            try
            {
                var entities = await _employeeSeparationRepository.GetByDateRangeAsync(StartDate, EndDate);
                separationList = entities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return separationList;
        }

        #endregion

        #region Employee Separation Outstanding Service Methods
        public async Task<EmployeeSeparationOutstanding> GetSeparationOutstandingAsync(int SeparationOutstandingId)
        {
            EmployeeSeparationOutstanding e = new EmployeeSeparationOutstanding();
            try
            {
                var entity = await _separationOutstandingRepository.GetByIdAsync(SeparationOutstandingId);
                e = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return e;
        }

        public async Task<List<EmployeeSeparationOutstanding>> GetSeparationOutstandingsAsync(string employeeId)
        {
            List<EmployeeSeparationOutstanding> separationOutstandingList = new List<EmployeeSeparationOutstanding>();
            try
            {
                var entities = await _separationOutstandingRepository.GetByEmployeeIdAsync(employeeId);
                separationOutstandingList = entities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return separationOutstandingList;
        }

        public async Task<List<EmployeeSeparationOutstanding>> GetSeparationOutstandingsAsync(int employeeSeparationId)
        {
            List<EmployeeSeparationOutstanding> separationOutstandingList = new List<EmployeeSeparationOutstanding>();
            try
            {
                var entities = await _separationOutstandingRepository.GetBySeparationIdAsync(employeeSeparationId);
                separationOutstandingList = entities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return separationOutstandingList;
        }

        public async Task<bool> AddEmployeeSeparationOutstandingAsync(EmployeeSeparationOutstanding e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e), "Required parameter [Employee Separation Outstanding] is missing. The request cannot be processed."); }
            return await _separationOutstandingRepository.AddAsync(e);
        }

        public async Task<bool> DeleteEmployeeSeparationOutstandingAsync(int id)
        {
            if (id < 1) { throw new ArgumentNullException(nameof(id), "Required parameter [Employee Separation Outstanding ID] is missing. The request cannot be processed."); }
            return await _separationOutstandingRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateEmployeeSeparationOutstandingAsync(EmployeeSeparationOutstanding e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e), "Required parameter [Employee Separation Outstanding] is missing. The request cannot be processed."); }
            return await _separationOutstandingRepository.EditAsync(e);
        }

        #endregion

        #region Employee Separation Payments Service Methods

        //============= Read Servie Methods ===================//
        public async Task<EmployeeSeparationPayments> GetSeparationPaymentAsync(int SeparationPaymentId)
        {
            EmployeeSeparationPayments e = new EmployeeSeparationPayments();
            try
            {
                var entity = await _separationOutstandingRepository.GetPaymentByIdAsync(SeparationPaymentId);
                e = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return e;
        }

        public async Task<List<EmployeeSeparationPayments>> GetSeparationPaymentsAsync(string employeeId)
        {
            List<EmployeeSeparationPayments> separationPaymentList = new List<EmployeeSeparationPayments>();
            try
            {
                var entities = await _separationOutstandingRepository.GetPaymentsByEmployeeIdAsync(employeeId);
                separationPaymentList = entities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return separationPaymentList;
        }

        public async Task<List<EmployeeSeparationPayments>> GetSeparationPaymentsAsync(int employeeSeparationId)
        {
            List<EmployeeSeparationPayments> separationPaymentsList = new List<EmployeeSeparationPayments>();
            try
            {
                var entities = await _separationOutstandingRepository.GetPaymentsBySeparationIdAsync(employeeSeparationId);
                separationPaymentsList = entities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return separationPaymentsList;
        }

        public async Task<List<EmployeeSeparationPayments>> GetSeparationPaymentsBySeparationOutstandingIdAsync(int employeeSeparationOutstandingId)
        {
            List<EmployeeSeparationPayments> separationPaymentsList = new List<EmployeeSeparationPayments>();
            try
            {
                var entities = await _separationOutstandingRepository.GetPaymentsBySeparationOutstandingIdAsync(employeeSeparationOutstandingId);
                separationPaymentsList = entities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return separationPaymentsList;
        }

        //========== Write Service Methods ================//
        public async Task<bool> AddEmployeeSeparationPaymentAsync(EmployeeSeparationPayments p)
        {
            if (p == null) { throw new ArgumentNullException(nameof(p), "Required parameter [Employee Separation Payment] is missing. The request cannot be processed."); }
            bool IsAdded = false;
            if (p.OutstandingId > 0)
            {
                EmployeeSeparationOutstanding o = new EmployeeSeparationOutstanding();
                var entity = await _separationOutstandingRepository.GetByIdAsync(p.OutstandingId);
                if (entity != null)
                {
                    o = entity;
                    o.AmountPaid = o.AmountPaid + p.PaymentAmount;
                    o.AmountBalance = o.AmountBalance - p.PaymentAmount;
                    if (await _separationOutstandingRepository.EditAsync(o))
                    {
                        IsAdded = await _separationOutstandingRepository.AddPaymentAsync(p);
                    }
                }
            }
            return IsAdded;
        }

        public async Task<bool> DeleteEmployeeSeparationPaymentAsync(int id)
        {
            if (id < 1) { throw new ArgumentNullException(nameof(id), "Required parameter [Employee Separation Payment ID] is missing. The request cannot be processed."); }
            bool IsDeleted = false;

            EmployeeSeparationPayments s = new EmployeeSeparationPayments();
            var s_entity = await _separationOutstandingRepository.GetPaymentByIdAsync(id);
            if (s_entity != null)
            {
                s = s_entity;
                if (s.OutstandingId > 0)
                {
                    EmployeeSeparationOutstanding o = new EmployeeSeparationOutstanding();
                    var entity = await _separationOutstandingRepository.GetByIdAsync(s.OutstandingId);
                    if (entity != null)
                    {
                        o = entity;
                        o.AmountPaid = o.AmountPaid - s.PaymentAmount;
                        o.AmountBalance = o.AmountBalance + s.PaymentAmount;
                        if (await _separationOutstandingRepository.EditAsync(o))
                        {
                            IsDeleted = await _separationOutstandingRepository.DeletePaymentAsync(id);
                        }
                    }
                }
            }
            return IsDeleted;
        }

        public async Task<bool> UpdateEmployeeSeparationPaymentAsync(EmployeeSeparationPayments p)
        {
            if (p == null) { throw new ArgumentNullException(nameof(p), "Required parameter [Employee Separation Payment] is missing. The request cannot be processed."); }
            bool IsUpdated = false;

            EmployeeSeparationPayments s = new EmployeeSeparationPayments();
            var s_entity = await _separationOutstandingRepository.GetPaymentByIdAsync(p.Id);
            if (s_entity != null)
            {
                s = s_entity;
                if (p.OutstandingId > 0)
                {
                    EmployeeSeparationOutstanding o = new EmployeeSeparationOutstanding();
                    var entity = await _separationOutstandingRepository.GetByIdAsync(p.OutstandingId);
                    if (entity != null)
                    {
                        o = entity;
                        o.AmountPaid = (o.AmountPaid - s.PaymentAmount) + p.PaymentAmount;
                        o.AmountBalance = (o.AmountBalance + s.PaymentAmount) - p.PaymentAmount;
                        if (await _separationOutstandingRepository.EditAsync(o))
                        {
                            IsUpdated = await _separationOutstandingRepository.EditPaymentAsync(p);
                        }
                    }
                }
            }
            return IsUpdated;
        }

        #endregion

        #region Employee Separation Outstanding Items Service Methods
        public async Task<SeparationOutstandingItem> GetSeparationOutstandingItemAsync(int SeparationOutstandingItemId)
        {
            SeparationOutstandingItem e = new SeparationOutstandingItem();
            var entity = await _separationOutstandingRepository.GetSeparationOutstandingItemByIdAsync(SeparationOutstandingItemId);
            if (entity != null) { e = entity; }
            return e;
        }

        public async Task<List<SeparationOutstandingItem>> GetSeparationOutstandingItemsAsync()
        {
            List<SeparationOutstandingItem> itemList = new List<SeparationOutstandingItem>();
            var entities = await _separationOutstandingRepository.GetSeparationOutstandingItemsAsync();
            if (entities != null) { itemList = entities; }
            return itemList;
        }

        #endregion

        #region Employee Separation Types Service Methods

        public async Task<bool> CreateEmployeeSeparationTypeAsync(EmployeeSeparationType t)
        {
            if (t == null) { throw new ArgumentNullException(nameof(t), "Required parameter [Employee Separation Type] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                var entities = await _employeeSeparationRepository.GetSeparationTypesbyNameAsync(t.Description);
                if (entities != null && entities.Count > 0)
                {
                    throw new Exception("Duplicate Entry! There is an existing record in the system with the same description.");
                }
                IsSuccessful = await _employeeSeparationRepository.AddSeparationTypeAsync(t);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteEmployeeSeparationTypeAsync(int id)
        {
            if (id < 1) { throw new ArgumentNullException(nameof(id), "Required parameter [EmployeeSeparationTypeID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _employeeSeparationRepository.DeleteSeparationTypeAsync(id);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateEmployeeSeparationTypeAsync(EmployeeSeparationType t)
        {
            if (t == null) { throw new ArgumentNullException(nameof(t), "Required parameter [EmployeeSeparationType] is missing."); }
            bool IsSuccessful = false;
            try
            {
                var entities = await _employeeSeparationRepository.GetSeparationTypesbyNameAsync(t.Description);
                if (entities != null && entities.Count > 0)
                {
                    var entity = entities.FirstOrDefault();
                    foreach (var item in entities)
                    {
                        if (item.Id != t.Id)
                        {
                            throw new Exception("Duplicate Entry! There is an existing record in the system with the same description.");
                        }
                    }
                    IsSuccessful = await _employeeSeparationRepository.EditSeparationTypeAsync(t);
                }
                else
                {
                    IsSuccessful = await _employeeSeparationRepository.EditSeparationTypeAsync(t);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<EmployeeSeparationType>> GetEmployeeSeparationTypesAsync()
        {
            List<EmployeeSeparationType> employeeSeparationTypes = new List<EmployeeSeparationType>();
            try
            {
                var entities = await _employeeSeparationRepository.GetAllSeparationTypesAsync();
                if (entities != null && entities.Count > 0) { employeeSeparationTypes = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employeeSeparationTypes;
        }

        public async Task<EmployeeSeparationType> GetEmployeeSeparationTypeByIdAsync(int id)
        {
            EmployeeSeparationType t = new EmployeeSeparationType();
            if (id < 1) { throw new ArgumentNullException(nameof(id), "The required parameter [EmployeeSeparationTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _employeeSeparationRepository.GetSeparationTypeByIdAsync(id);
                if (entity != null) { t = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return t;
        }

        #endregion

        #region Employee Separation Reason Service Methods
        public async Task<bool> CreateEmployeeSeparationReasonAsync(EmployeeSeparationReason r)
        {
            if (r == null) { throw new ArgumentNullException(nameof(r), "Required parameter [Employee Separation Reason] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                var entities = await _employeeSeparationRepository.GetSeparationReasonsbyNameAsync(r.Description);
                if (entities != null && entities.Count > 0)
                {
                    throw new Exception("Duplicate Entry! There is an existing record in the system with the same description.");
                }
                IsSuccessful = await _employeeSeparationRepository.AddSeparationReasonAsync(r);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteEmployeeSeparationReasonAsync(int id)
        {
            if (id < 1) { throw new ArgumentNullException(nameof(id), "Required parameter [EmployeeSeparationReasonID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _employeeSeparationRepository.DeleteSeparationReasonAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateEmployeeSeparationReasonAsync(EmployeeSeparationReason t)
        {
            if (t == null) { throw new ArgumentNullException(nameof(t), "Required parameter [EmployeeSeparationReason] is missing."); }
            bool IsSuccessful = false;
            try
            {
                var entities = await _employeeSeparationRepository.GetSeparationReasonsbyNameAsync(t.Description);
                if (entities != null && entities.Count > 0)
                {
                    foreach (var item in entities)
                    {
                        if (item.Id != t.Id)
                        {
                            throw new Exception("Duplicate Entry! There is an existing record in the system with the same description.");
                        }
                    }

                    IsSuccessful = await _employeeSeparationRepository.EditSeparationReasonAsync(t);
                }
                else
                {
                    IsSuccessful = await _employeeSeparationRepository.EditSeparationReasonAsync(t);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<EmployeeSeparationReason>> GetEmployeeSeparationReasonsAsync()
        {
            List<EmployeeSeparationReason> employeeSeparationReasons = new List<EmployeeSeparationReason>();
            try
            {
                var entities = await _employeeSeparationRepository.GetAllSeparationReasonsAsync();
                if (entities != null && entities.Count > 0) { employeeSeparationReasons = entities.ToList(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employeeSeparationReasons;
        }

        public async Task<EmployeeSeparationReason> GetEmployeeSeparationReasonByIdAsync(int id)
        {
            EmployeeSeparationReason t = new EmployeeSeparationReason();
            if (id < 1) { throw new ArgumentNullException(nameof(id), "The required parameter [EmployeeSeparationTypeID] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _employeeSeparationRepository.GetSeparationReasonByIdAsync(id);
                if (entity != null) { t = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return t;
        }

        #endregion
    }
}
