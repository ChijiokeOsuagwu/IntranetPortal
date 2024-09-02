using IntranetPortal.Base.Models.EmployeeRecordModels;
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
        Task<Location> GetLocationByNameAsync(string locationName);
        Task<IList<Location>> GetAllLocationsAsync(string userId);
        Task<IList<Location>> GetAllLocationsAsync();
        Task<IList<Location>> GetStationsAsync();
        Task<IList<Location>> GetBureausAsync();
        Task<IList<State>> GetStatesAsync();
        Task<IList<State>> SearchStatesAsync(string stateName);
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

        //============================= Team Service Methods ==================================================//
        #region Teams Service Methods
        Task<bool> CreateTeamAsync(Team team);

        Task<bool> DeleteTeamAsync(string teamId);

        Task<bool> UpdateTeamAsync(Team team);

        Task<IList<Team>> GetTeamsAsync();

        Task<Team> GetTeamByIdAsync(string teamId);

        Task<IList<Team>> SearchTeamsByNameAsync(string teamName);

        #endregion

        //============================ Team Members Service Methods =========================================//
        #region Team Members Service Methods

        Task<bool> CreateTeamMemberAsync(TeamMember teamMember);

        Task<bool> DeleteTeamMemberAsync(int teamMemberId);

        Task<bool> UpdateTeamMemberAsync(TeamMember teamMember);

        Task<IEnumerable<TeamMember>> GetTeamMembersByTeamIdAsync(string teamId);

        Task<TeamMember> GetTeamMemberByIdAsync(int teamMemberId);

        Task<IEnumerable<TeamMember>> GetTeamMembersByMemberNameAsync(string teamId, string memberName);

        Task<IEnumerable<Employee>> GetNonTeamMembersByTeamIdAsync(string teamId);

        #endregion

        #region Program Service Methods
        
        //===== Program Write Service Methods ===========//
        Task<bool> CreateProgramAsync(Programme program);
        Task<bool> DeleteProgramAsync(int programId);
        Task<bool> UpdateProgramAsync(Programme program);

        //===== Program Read Service Methods ============//
        Task<List<Programme>> GetProgramsAsync();
        Task<Programme> GetProgramAsync(int Id);
        Task<Programme> GetProgramAsync(string programTitle);
        Task<List<Programme>> SearchProgramsAsync(string programType = null, string programBelt=null, string programTitle=null);

        //======== Programme Belt Service Methods ==============//
        Task<List<ProgrammeBelt>> GetProgrammeBeltsAsync();

        //======== Program Platform Service Methods =======================//
        Task<List<ProgramPlatform>> GetProgramPlatformsAsync();

        #endregion

        #region Currency Service Interfaces
        Task<IList<Currency>> GetCurrenciesAsync();
        Task<Currency> GetCurrencyByCodeAsync(string currencyCode);
        #endregion
    }
}
