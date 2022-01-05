using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class GlobalSettingsService : IGlobalSettingsService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ICompanyRepository _companyRepository;
        public GlobalSettingsService(ILocationRepository locationRepository, IDepartmentRepository departmentRepository,
                                        IUnitRepository unitRepository, ICompanyRepository companyRepository)
        {
            _locationRepository = locationRepository;
            _departmentRepository = departmentRepository;
            _unitRepository = unitRepository;
            _companyRepository = companyRepository;
        }

        //================================= Location Action Methods ====================================//
        #region Location Action Methods
        public async Task<bool> CreateLocationAsync(Location location)
        {
            if (location == null) { throw new ArgumentNullException(nameof(location), "Required parameter [location] is missing."); }
            return await _locationRepository.AddLocationAsync(location);
        }

        public async Task<bool> DeleteLocationAsync(int locationId)
        {
            if (locationId < 1) { throw new ArgumentNullException(nameof(locationId), "Required parameter [locationId] is missing."); }
            return await _locationRepository.DeleteLocationAsync(locationId);
        }

        public async Task<bool> UpdateLocationAsync(Location location)
        {
            if (location == null) { throw new ArgumentNullException(nameof(location), "Required parameter [location] is missing."); }
            return await _locationRepository.EditLocationAsync(location);
        }

        public async Task<Location> GetLocationByIdAsync(int locationId)
        {
            if (locationId < 1) { throw new ArgumentNullException(nameof(locationId), "Required parameter [locationId] is missing."); }
            return await _locationRepository.GetLocationByIdAsync(locationId);
        }

        public async Task<IList<Location>> GetAllLocationsAsync()
        {
            List<Location> locations = new List<Location>();
            try
            {
                var entities = await _locationRepository.GetLocationsAsync();
                if (entities != null)
                {
                    foreach (var item in entities)
                    {
                        item.LocationDescription = $"{item.LocationName} ({item.LocationType})";
                        locations.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return locations;
        }

        public async Task<IList<Location>> GetStationsAsync()
        {
            return await _locationRepository.GetOnlyStationsAsync();
        }

        public async Task<IList<Location>> GetBureausAsync()
        {
            return await _locationRepository.GetOnlyBureausAsync();
        }

        #endregion

        //================================= States Action Methods ======================================//
        #region States Action Methods
        public async Task<IList<State>> GetStatesAsync()
        {
            List<State> states = new List<State>();
            try
            {
                var entities = await _locationRepository.GetStatesAsync().ConfigureAwait(false);
                states = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return states;
        }

        public async Task<State> GetStateAsync(string stateName)
        {
            State state = new State();
            try
            {
                var entity = await _locationRepository.GetStateByNameAsync(stateName).ConfigureAwait(false);
                state = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return state;
        }


        public async Task<IList<Country>> GetCountriesAsync()
        {
            return await _locationRepository.GetCountriesAsync();
        }
        #endregion

        //================================= Department Action Methods ==================================//
        #region Department Action Methods

        public async Task<bool> CreateDepartmentAsync(Department department)
        {
            if (department == null) { throw new ArgumentNullException(nameof(department), "Required parameter [department] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _departmentRepository.AddDepartmentAsync(department);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteDepartmentAsync(int departmentId)
        {
            if (departmentId < 1) { throw new ArgumentNullException(nameof(departmentId), "Required parameter [locationId] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _departmentRepository.DeleteDepartmentAsync(departmentId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateDepartmentAsync(Department department)
        {
            if (department == null) { throw new ArgumentNullException(nameof(department), "Required parameter [department] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _departmentRepository.EditDepartmentAsync(department);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<Department>> GetDepartmentsAsync()
        {
            List<Department> depts = new List<Department>();
            try
            {
                var entities = await _departmentRepository.GetDepartmentsAsync().ConfigureAwait(false);
                depts = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return depts;
        }

        public async Task<Department> GetDepartmentAsync(int Id)
        {
            Department dept = new Department();
            if (Id < 1) { throw new ArgumentNullException(nameof(Id), "The required parameter [DepartmentId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _departmentRepository.GetDepartmentByIdAsync(Id).ConfigureAwait(false);
                dept = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dept;
        }
        #endregion

        //================================= Unit Service Methods =======================================//
        #region Unit Service Methods
        public async Task<bool> CreateUnitAsync(Unit unit)
        {
            if (unit == null) { throw new ArgumentNullException(nameof(unit), "Required parameter [unit] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _unitRepository.AddUnitAsync(unit);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteUnitAsync(int unitId)
        {
            if (unitId < 1) { throw new ArgumentNullException(nameof(unitId), "Required parameter [unitId] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _unitRepository.DeleteUnitAsync(unitId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateUnitAsync(Unit unit)
        {
            if (unit == null) { throw new ArgumentNullException(nameof(unit), "Required parameter [unit] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _unitRepository.EditUnitAsync(unit);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<Unit>> GetUnitsAsync()
        {
            List<Unit> depts = new List<Unit>();
            try
            {
                var entities = await _unitRepository.GetUnitsAsync().ConfigureAwait(false);
                depts = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return depts;
        }

        public async Task<Unit> GetUnitAsync(int Id)
        {
            Unit unit = new Unit();
            if (Id < 1) { throw new ArgumentNullException(nameof(Id), "The required parameter [UnitId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _unitRepository.GetUnitByIdAsync(Id).ConfigureAwait(false);
                unit = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return unit;
        }
        #endregion

        //================================= Company Action Methods =====================================//
        #region Company Action Methods

        public async Task<IList<Company>> GetCompaniesAsync()
        {
            List<Company> companies = new List<Company>();
            try
            {
                var entities = await _companyRepository.GetCompaniesAsync().ConfigureAwait(false);
                companies = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return companies;
        }

        public async Task<Company> GetCompanyAsync(string companyCode)
        {
            Company company = new Company();
            if (string.IsNullOrEmpty(companyCode)) { throw new ArgumentNullException(nameof(companyCode), "The required parameter [Company Code] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _companyRepository.GetCompanyByCodeAsync(companyCode).ConfigureAwait(false);
                company = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return company;
        }
        #endregion

    }
}
