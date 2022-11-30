using IntranetPortal.Base.Models.AssetManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AssetManagerRepositories
{
    public interface IAssetRepository
    {
        Task<Asset> GetByIdAsync(string assetId);
        Task<Asset> GetByNameAsync(string assetName);
        Task<IList<Asset>> SearchByNameAsync(string assetName);
        Task<IList<Asset>> GetByAssetTypeIdAsync(int assetTypeId);

        Task<IList<Asset>> GetByCategoryIdAsync(int assetCategoryId);

        Task<IList<Asset>> GetAllAsync();

        Task<bool> AddAsync(Asset asset);

        Task<bool> EditAsync(Asset asset);

        Task<bool> DeleteAsync(string assetId, string deletedBy);
        Task<bool> DeletePermanentlyAsync(string assetId);

        Task<bool> UpdateUsageStatusAsync(string assetId, string newStatus, string modifiedBy);

        Task<bool> UpdateAssetConditionAsync(string assetId, string assetCondition, string modifiedBy);

        Task<bool> UpdateBaseLocationAsync(string assetId, int baseLocationId, string currentLocation, string modifiedBy);

        Task<bool> UpdateCurrentLocationAsync(string assetId, string currentLocation, string modifiedBy);
    }
}
