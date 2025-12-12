using IntranetPortal.Base.Models.GlobalSettingsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.GlobalSettingsRepositories
{
    public interface ILocationRepository
    {
        #region Location Action Methods
        //Location Write Action Methods
        Task<bool> AddLocationAsync(Location location);
        Task<bool> DeleteLocationAsync(int Id);
        Task<bool> EditLocationAsync(Location location);

        //Location Read Action Methods
        Task<Location> GetLocationByIdAsync(int Id);
        Task<Location> GetLocationByNameAsync(string locationName);
        Task<IList<Location>> GetLocationsByUserIdAsync(string userId);
        Task<IList<Location>> GetLocationsAsync();
        Task<List<Location>> GetLocationsByNameAsync(string locationName);
        #endregion

        #region Location Group Action Methods
        Task<bool> AddLocationGroupAsync(LocationGroup locationGroup);
        Task<bool> DeleteLocationGroupAsync(int locationGroupId);
        Task<bool> EditLocationGroupAsync(LocationGroup locationGroup);
        Task<LocationGroup> GetLocationGroupByIdAsync(int locationGroupId);
        Task<List<LocationGroup>> GetLocationGroupsByNameAsync(string locationGroupName);
        Task<List<LocationGroup>> GetLocationGroupsAsync();
        #endregion

        #region Location Group Members Action Methods
        Task<bool> AddLocationGroupMemberAsync(LocationGroupMember locationGroupMember);
        Task<bool> DeleteLocationGroupMemberAsync(int locationGroupMemberId);
        Task<bool> EditLocationGroupMemberAsync(LocationGroupMember locationGroupMember);
        Task<List<Location>> GetLocationsByLocationGroupIdAsync(int locationGroupId);
        Task<List<LocationGroupMember>> GetLocationGroupMembersByLocationGroupIdAsync(int locationGroupId);
        Task<List<LocationGroupMember>> GetLocationGroupMembersByLocationIdnLocationGroupIdAsync(int locationId, int locationGroupId);
        #endregion

        #region States, Stations, Bureaus and Countries Action Methods
        Task<IList<Location>> GetOnlyStationsAsync();
        Task<IList<Location>> GetOnlyBureausAsync();
        Task<IList<State>> GetStatesAsync();
        Task<IList<State>> SearchStatesByNameAsync(string name);
        Task<State> GetStateByNameAsync(string stateName);
        Task<IList<Country>> GetCountriesAsync();
#endregion
    }
}
