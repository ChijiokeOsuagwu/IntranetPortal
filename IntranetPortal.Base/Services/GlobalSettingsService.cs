using IntranetPortal.Base.Models.EmployeeRecordModels;
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
        private readonly ITeamRepository _teamRepository;
        private readonly IProgramRepository _programRepository;

        public GlobalSettingsService(ILocationRepository locationRepository, IDepartmentRepository departmentRepository,
                                        IUnitRepository unitRepository, ICompanyRepository companyRepository,
                                        ITeamRepository teamRepository, IProgramRepository programRepository)
        {
            _locationRepository = locationRepository;
            _departmentRepository = departmentRepository;
            _unitRepository = unitRepository;
            _companyRepository = companyRepository;
            _teamRepository = teamRepository;
            _programRepository = programRepository;
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

        public async Task<Location> GetLocationByNameAsync(string locationName)
        {
            if (string.IsNullOrEmpty(locationName)) { throw new ArgumentNullException(nameof(locationName), "Required parameter [locationId] is missing."); }
            return await _locationRepository.GetLocationByNameAsync(locationName);
        }

        public async Task<IList<Location>> GetAllLocationsAsync(string userId)
        {
            List<Location> locations = new List<Location>();
                var entities = await _locationRepository.GetLocationsByUserIdAsync(userId);
                if (entities != null)
                {
                    foreach (var item in entities)
                    {
                        item.LocationDescription = $"{item.LocationName} ({item.LocationType})";
                        locations.Add(item);
                    }
                }
            return locations;
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

        public async Task<IList<State>> SearchStatesAsync(string stateName)
        {
            List<State> states = new List<State>();
            try
            {
                var entities = await _locationRepository.SearchStatesByNameAsync(stateName);
                states = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return states;
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

        //================================= Team Action Methods ==================================//
        #region Team Action Methods

        public async Task<bool> CreateTeamAsync(Team team)
        {
            if (team == null) { throw new ArgumentNullException(nameof(team), "Required parameter [team] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _teamRepository.AddTeamAsync(team);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteTeamAsync(string teamId)
        {
            if (string.IsNullOrWhiteSpace(teamId)) { throw new ArgumentNullException(nameof(teamId), "Required parameter [teamId] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _teamRepository.DeleteTeamAsync(teamId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateTeamAsync(Team team)
        {
            if (team == null) { throw new ArgumentNullException(nameof(team), "Required parameter [team] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _teamRepository.EditTeamAsync(team);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IList<Team>> GetTeamsAsync()
        {
            List<Team> teams = new List<Team>();
            try
            {
                var entities = await _teamRepository.GetTeamsAsync().ConfigureAwait(false);
                teams = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return teams;
        }

        public async Task<Team> GetTeamByIdAsync(string teamId)
        {
            Team team = new Team();
            if (string.IsNullOrWhiteSpace(teamId)) { throw new ArgumentNullException(nameof(teamId), "The required parameter [teamId] is missing. The request cannot be processed."); }
            try
            {
                var entity = await _teamRepository.GetTeamByIdAsync(teamId).ConfigureAwait(false);
                team = entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return team;
        }

        public async Task<IList<Team>> SearchTeamsByNameAsync(string teamName)
        {
            List<Team> teams = new List<Team>();
            try
            {
                var entities = await _teamRepository.GetTeamByNameAsync(teamName).ConfigureAwait(false);
                teams = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return teams;
        }

        #endregion

        //================================ Team Members Action Methods ==========================//
        #region Team Members Action Methods

        public async Task<bool> CreateTeamMemberAsync(TeamMember teamMember)
        {
            if (teamMember == null) { throw new ArgumentNullException(nameof(teamMember), "Required parameter [Team Member] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _teamRepository.AddTeamMemberAsync(teamMember);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteTeamMemberAsync(int teamMemberId)
        {
            if (teamMemberId < 1) { throw new ArgumentNullException(nameof(teamMemberId), "Required parameter [TeamMemberID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _teamRepository.DeleteTeamMemberAsync(teamMemberId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateTeamMemberAsync(TeamMember teamMember)
        {
            if (teamMember == null) { throw new ArgumentNullException(nameof(teamMember), "Required parameter [TeamMember] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _teamRepository.EditTeamMemberAsync(teamMember);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<IEnumerable<TeamMember>> GetTeamMembersByTeamIdAsync(string teamId)
        {
            List<TeamMember> teamMembers = new List<TeamMember>();
            try
            {
                var entities = await _teamRepository.GetTeamMembersByTeamIdAsync(teamId);
                teamMembers = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return teamMembers;
        }

        public async Task<TeamMember> GetTeamMemberByIdAsync(int teamMemberId)
        {
            TeamMember teamMember = new TeamMember();
            try
            {
                teamMember = await _teamRepository.GetTeamMemberByIdAsync(teamMemberId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return teamMember;
        }

        public async Task<IEnumerable<TeamMember>> GetTeamMembersByMemberNameAsync(string teamId, string memberName)
        {
            List<TeamMember> teamMembers = new List<TeamMember>();
            try
            {
                var entities = await _teamRepository.GetTeamMembersByMemberNameAsync(teamId, memberName);
                teamMembers = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return teamMembers;
        }

        public async Task<IEnumerable<Employee>> GetNonTeamMembersByTeamIdAsync(string teamId)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                var entities = await _teamRepository.GetNonTeamMembersAsync(teamId);
                employees = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employees;
        }

        #endregion

        #region Programme Action Methods

        //======== Programme Write Service Methods =================//
        public async Task<bool> CreateProgramAsync(Programme program)
        {
            if (program == null) { throw new ArgumentNullException(nameof(program), "Required parameter [Program] is missing. The request cannot be processed."); }
            var entity = await _programRepository.GetByTitleAsync(program.Title);
            if (entity != null && entity.Count > 0)
            {
                throw new Exception("Double Entry Error! A program with the same title already exists in the system.");
            }
            return await _programRepository.AddProgramAsync(program);
        }

        public async Task<bool> DeleteProgramAsync(int programId)
        {
            if (programId < 1) { throw new ArgumentNullException(nameof(programId), "Required parameter [Program ID] is missing."); }
            return await _programRepository.DeleteProgramAsync(programId);
        }

        public async Task<bool> UpdateProgramAsync(Programme program)
        {
            if (program == null) { throw new ArgumentNullException(nameof(program), "Required parameter [Program] is missing. The request cannot be processed."); }
            var entity = await _programRepository.GetByTitleAsync(program.Title);
            if (entity != null && entity.Count > 0 && entity[0].Id != program.Id)
            {
                throw new Exception("Double Entry Error! Another program exists in the system with the same title.");
            }
            return await _programRepository.EditProgramAsync(program);
        }

        //============== Programme Read Service Methods ==================//
        public async Task<List<Programme>> GetProgramsAsync()
        {
            var entities = await _programRepository.GetAllAsync();
            return entities.ToList();
        }

        public async Task<Programme> GetProgramAsync(int Id)
        {
            if (Id < 1) { throw new ArgumentNullException(nameof(Id), "The required parameter [Program ID] is missing. The request cannot be processed."); }
            return await _programRepository.GetByIdAsync(Id);
        }

        public async Task<Programme> GetProgramAsync(string programTitle)
        {
            Programme program = new Programme();
            if (string.IsNullOrWhiteSpace(programTitle)) { throw new ArgumentNullException(nameof(programTitle), "The required parameter [Program Title] is missing. The request cannot be processed."); }
            var entities = await _programRepository.GetByTitleAsync(programTitle);
            if(entities != null && entities.Count > 0)
            {
                program = entities.FirstOrDefault();
            }
            return program;
        }

        public async Task<List<Programme>> SearchProgramsAsync(string programType = null, string programBelt = null, string programTitle = null)
        {
            List<Programme> programs = new List<Programme>();
            if (!string.IsNullOrWhiteSpace(programTitle))
            {
                var entities = await _programRepository.SearchByTitleAsync(programTitle);
                programs = entities.ToList();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(programType) && !string.IsNullOrWhiteSpace(programBelt))
                {
                    var entities = await _programRepository.GetByProgramTypeAndProgramBeltAsync(programType, programBelt);
                    programs = entities.ToList();
                }
                else if(!string.IsNullOrWhiteSpace(programType) && string.IsNullOrWhiteSpace(programBelt))
                {
                    var entities = await _programRepository.GetByProgramTypeAsync(programType);
                    programs = entities.ToList();
                }
                else if (string.IsNullOrWhiteSpace(programType) && !string.IsNullOrWhiteSpace(programBelt))
                {
                    var entities = await _programRepository.GetByProgramBeltAsync(programBelt);
                    programs = entities.ToList();
                }
                else
                {
                        var entities = await _programRepository.GetAllAsync();
                        programs = entities.ToList();
                }
            }
            return programs;
        }

        #endregion

        #region Programme Belt Service Methods
        public async Task<List<ProgrammeBelt>> GetProgrammeBeltsAsync()
        {
            var entities = await _programRepository.GetAllProgrammeBeltsAsync();
            return entities.ToList();
        }

        #endregion

        #region Program Platform Service Methods
        public async Task<List<ProgramPlatform>> GetProgramPlatformsAsync()
        {
            var entities = await _programRepository.GetAllProgramPlatformsAsync();
            return entities.ToList();
        }
        #endregion
    }
}
