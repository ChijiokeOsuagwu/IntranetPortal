using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.EmployeeRecordRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class EmployeeRecordService : IEmployeeRecordService
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IUtilityRepository _utilityRepository;
        public EmployeeRecordService(IEmployeeRepository employeeRepository, IPersonRepository personRepository,
                                        IUtilityRepository utilityRepository)
        {
            _employeeRepository = employeeRepository;
            _personRepository = personRepository;
            _utilityRepository = utilityRepository;
        }

        //========================== Employee Service Methods ==============================================//
        #region Employee Action Methods
        public async Task<bool> CreateEmployeeAsync(Employee employee)
        {
            bool PersonIsAdded = false;
            bool EmployeeIsAdded = false;
            if (employee == null) { throw new ArgumentNullException(nameof(employee), "The required parameter [employee] is missing."); }
            Person person = employee.ToPerson();
            try
            {
                PersonIsAdded = await _personRepository.AddPersonAsync(person);
                if (PersonIsAdded)
                {
                    EmployeeIsAdded = await _employeeRepository.AddEmployeeAsync(employee);
                    if (EmployeeIsAdded)
                    {
                       bool IsSuccessful = await _utilityRepository.IncrementAutoNumberAsync("empno");
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

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeeRepository.GetEmployeesAsync();
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees.ToList();
        }

        public async Task<List<Employee>> GetEmployeesByNameAsync(string employeeName)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _employeeRepository.GetEmployeesByNameAsync(employeeName);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        public async Task<Employee> GetEmployeesByIdAsync(string EmployeeID)
        {
            Employee employee = new Employee();
            try
            {
                var entity = await _employeeRepository.GetEmployeeByIdAsync(EmployeeID);
                employee = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employee;
        }

        public async Task<bool> EmployeeExistsAsync(string EmployeeID)
        {
            Employee employee = new Employee();
            try
            {
                var entity = await _employeeRepository.GetEmployeeByIdAsync(EmployeeID);
                employee = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employee != null;
        }



        #endregion

        //=========================== Employee Basic Info Service Methods ===========================================//
        #region Employee Basic Info Service Methods
        public async Task<bool> CreateEmployeeBasicInfoAsync(EmployeeBasicInfo employeeBasicInfo)
        {
            bool IsAdded = false;
            if (employeeBasicInfo == null) { throw new ArgumentNullException(nameof(employeeBasicInfo), "The required parameter [Employee Basic Info] is missing."); }
           try
            {
                    IsAdded = await _employeeRepository.AddEmployeeBasicInfoAsync(employeeBasicInfo);
                    return IsAdded;
             }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateEmployeeBasicInfoAsync(EmployeeBasicInfo employeeBasicInfo)
        {
            bool IsUpdated = false;
            if (employeeBasicInfo == null) { throw new ArgumentNullException(nameof(employeeBasicInfo), "The required parameter [Employee Basic Info] is missing."); }
            try
            {
                IsUpdated = await _employeeRepository.EditEmployeeBasicInfoAsync(employeeBasicInfo);
                return IsUpdated;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateEmployeeNextOfKinInfoAsync(EmployeeNextOfKinInfo employeeNextOfKinInfo)
        {
            bool IsUpdated = false;
            if (employeeNextOfKinInfo == null) { throw new ArgumentNullException(nameof(employeeNextOfKinInfo), "The required parameter [Employee Next Of Kin Info] is missing."); }
            try
            {
                IsUpdated = await _employeeRepository.EditEmployeeNextOfKinInfoAsync(employeeNextOfKinInfo);
                return IsUpdated;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateEmployeeHistoryInfoAsync(EmployeeHistoryInfo employeeHistoryInfo)
        {
            bool IsUpdated = false;
            if (employeeHistoryInfo == null) { throw new ArgumentNullException(nameof(employeeHistoryInfo), "The required parameter [Employee History Info] is missing."); }
            try
            {
                IsUpdated = await _employeeRepository.EditEmployeeHistoryInfoAsync(employeeHistoryInfo);
                return IsUpdated;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
