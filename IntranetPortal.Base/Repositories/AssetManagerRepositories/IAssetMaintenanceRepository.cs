using IntranetPortal.Base.Models.AssetManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AssetManagerRepositories
{
    public interface IAssetMaintenanceRepository
    {
        Task<IList<AssetMaintenance>> GetAllCurrentAsync();
        Task<IList<AssetMaintenance>> GetCurrentByAssetIdAsync(string assetId);
        Task<IList<AssetMaintenance>> GetCurrentByAssetNameAsync(string assetName);
        Task<IList<AssetMaintenance>> GetCurrentByAssetTypeIdAsync(int assetTypeId);
        Task<IList<AssetMaintenance>> GetAllAsync();
        Task<AssetMaintenance> GetByIdAsync(int assetMaintenanceId);
        Task<IList<AssetMaintenance>> GetByAssetIdAsync(string assetId);
        Task<IList<AssetMaintenance>> GetByAssetNameAsync(string assetName);
        Task<IList<AssetMaintenance>> GetByAssetTypeIdAsync(int assetTypeId);
        Task<bool> AddAssetMaintenanceAsync(AssetMaintenance assetMaintenance);
        Task<bool> EditAsync(AssetMaintenance assetMaintenance);
        Task<bool> DeleteAsync(int assetMaintenanceId);
    }
}
