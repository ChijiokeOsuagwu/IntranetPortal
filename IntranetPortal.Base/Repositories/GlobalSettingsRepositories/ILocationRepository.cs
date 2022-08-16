using IntranetPortal.Base.Models.GlobalSettingsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.GlobalSettingsRepositories
{
    public interface ILocationRepository
    {
        Task<bool> AddLocationAsync(Location location);
        Task<bool> DeleteLocationAsync(int Id);
        Task<bool> EditLocationAsync(Location location);
        Task<Location> GetLocationByIdAsync(int Id);
        Task<Location> GetLocationByNameAsync(string locationName);
        Task<IList<Location>> GetLocationsAsync();
        Task<IList<Location>> GetOnlyStationsAsync();
        Task<IList<Location>> GetOnlyBureausAsync();
        Task<IList<State>> GetStatesAsync();
        Task<IList<State>> SearchStatesByNameAsync(string name);
        Task<State> GetStateByNameAsync(string stateName);
        Task<IList<Country>> GetCountriesAsync();
    }
}
