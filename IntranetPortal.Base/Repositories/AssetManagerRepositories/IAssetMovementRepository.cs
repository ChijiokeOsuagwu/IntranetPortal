using IntranetPortal.Base.Models.AssetManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AssetManagerRepositories
{
    public interface IAssetMovementRepository
    {
        Task<IList<AssetMovement>> GetAllAsync();
        Task<AssetMovement> GetByIdAsync(int assetMovementId);
        Task<IList<AssetMovement>> GetByAssetIdAsync(string assetId);
        Task<IList<AssetMovement>> GetByAssetNameAsync(string assetName);
        Task<IList<AssetMovement>> GetByAssetTypeIdAsync(int assetTypeId);
        Task<bool> AddAssetMovementAsync(AssetMovement assetMovement);
        Task<bool> EditAsync(AssetMovement assetMovemente);
        Task<bool> DeleteAsync(int assetMovementId);
    }
}
