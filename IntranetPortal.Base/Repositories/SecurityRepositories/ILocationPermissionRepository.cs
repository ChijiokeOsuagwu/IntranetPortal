using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.SecurityModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.SecurityRepositories
{
    public interface ILocationPermissionRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(LocationPermission locationPermission);
        Task<bool> DeleteAsync(int locationPermissionId);
        Task<LocationPermission> GetByIdAsync(int locationPermissionId);
        Task<IList<LocationPermission>> GetByUserIdAsync(string userId);
        Task<IList<Location>> GetUnGrantedLocationsByUserIdAsync(string userId);
    }
}