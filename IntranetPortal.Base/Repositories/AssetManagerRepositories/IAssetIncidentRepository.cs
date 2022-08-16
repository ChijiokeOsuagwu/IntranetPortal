using IntranetPortal.Base.Models.AssetManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AssetManagerRepositories
{
    public interface IAssetIncidentRepository
    {
        Task<IList<AssetIncident>> GetAllCurrentAsync();
        Task<IList<AssetIncident>> GetCurrentByAssetIdAsync(string assetId);
        Task<IList<AssetIncident>> GetCurrentByAssetNameAsync(string assetName);
        Task<IList<AssetIncident>> GetCurrentByAssetTypeIdAsync(int assetTypeId);
        Task<IList<AssetIncident>> GetAllAsync();
        Task<AssetIncident> GetByIdAsync(int assetIncidentId);
        Task<IList<AssetIncident>> GetByAssetIdAsync(string assetId);
        Task<IList<AssetIncident>> GetByAssetNameAsync(string assetName);
        Task<IList<AssetIncident>> GetByAssetTypeIdAsync(int assetTypeId);
        Task<bool> AddAssetIncidentAsync(AssetIncident assetIncident);
        Task<bool> EditAsync(AssetIncident assetIncident);
        Task<bool> DeleteAsync(int assetIncidentId);
    }
}
