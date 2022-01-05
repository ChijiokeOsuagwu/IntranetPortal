using IntranetPortal.Base.Models.GlobalSettingsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IGlobalSettingsService
    {
        //========================================= Location Service Methods ================================//
        #region Location Service Methods 
        Task<bool> CreateLocationAsync(Location location);
        Task<bool> DeleteLocationAsync(int locationId);
        Task<bool> UpdateLocationAsync(Location location);
        Task<Location> GetLocationByIdAsync(int locationId);
        Task<IList<Location>> GetAllLocationsAsync();
        Task<IList<Location>> GetStationsAsync();
        Task<IList<Location>> GetBureausAsync();
        Task<IList<State>> GetStatesAsync();
        Task<State> GetStateAsync(string stateName);
        Task<IList<Country>> GetCountriesAsync();
        #endregion

        //============================= Department Service Methods ===========================================//
        #region Department Service Methods
        Task<bool> CreateDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(int departmentId);
        Task<bool> UpdateDepartmentAsync(Department department);
        Task<IList<Department>> GetDepartmentsAsync();
        Task<Department> GetDepartmentAsync(int Id);

        #endregion

        //============================= Unit Service Methods ================================================//
        #region Unit Service Methods
        Task<bool> CreateUnitAsync(Unit unit);
        Task<bool> DeleteUnitAsync(int unitId);
        Task<bool> UpdateUnitAsync(Unit unit);
        Task<IList<Unit>> GetUnitsAsync();
        Task<Unit> GetUnitAsync(int Id);
        #endregion

        //============================= Company Service Methods ==============================================//
        #region Company Service Methods
        Task<IList<Company>> GetCompaniesAsync();
        Task<Company> GetCompanyAsync(string companyCode);

        #endregion
    }
}
