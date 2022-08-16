using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.GlobalSettingsRepositories
{
    public interface ITeamRepository
    {
        //=============== Team Action Methods ==================================//
        #region Team Action Methods
        Task<Team> GetTeamByIdAsync(string teamId);

        Task<IList<Team>> GetTeamByNameAsync(string teamName);

        Task<IList<Team>> GetTeamsAsync();

        Task<bool> AddTeamAsync(Team team);

        Task<bool> EditTeamAsync(Team team);

        Task<bool> DeleteTeamAsync(string teamId);
        #endregion

        //============ Team Members Action Methods ================================//
        #region Team Members Action Methods
        Task<IEnumerable<TeamMember>> GetTeamMembersByTeamIdAsync(string teamId);

        Task<TeamMember> GetTeamMemberByIdAsync(int teamMemberId);

        Task<IEnumerable<TeamMember>> GetTeamMembersByMemberNameAsync(string teamId, string memberName);

        Task<IEnumerable<Employee>> GetNonTeamMembersAsync(string teamId);

        Task<bool> AddTeamMemberAsync(TeamMember teamMember);

        Task<bool> EditTeamMemberAsync(TeamMember teamMember);

        Task<bool> DeleteTeamMemberAsync(int teamMemberId);
        #endregion
    }
}
