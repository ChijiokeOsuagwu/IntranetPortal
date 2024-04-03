using IntranetPortal.Base.Models.AssetManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AssetManagerRepositories
{
    public interface IAssetRepository
    {
        #region Asset Master Read Action Methods
        Task<Asset> GetByIdAsync(string assetId);
        Task<Asset> GetByNameAsync(string assetName);
        Task<IList<Asset>> SearchByNameAsync(string assetName, string userId);
        Task<IList<Asset>> GetByAssetTypeIdAsync(int assetTypeId, string userId);
        Task<IList<Asset>> GetByClassIdAsync(int assetClassId, string userId);
        Task<IList<Asset>> GetByCategoryIdAsync(int assetCategoryId, string userId);
        Task<IList<Asset>> GetByDivisionIdAsync(int assetDivisionId, string userId);
        Task<IList<Asset>> GetByAssetGroupIdAsync(int assetGroupId, string userId);
        Task<IList<Asset>> GetByAssetConditionAsync(int assetCondition, string userId);
        Task<IList<Asset>> GetAllAsync(string userId);

        Task<IList<Asset>> GetByBinLocationIdnAssetTypeIdAsync(int binLocationId, int assetTypeId, string userId);
        Task<IList<Asset>> GetByBinLocationIdnAssetTypeIdnAssetConditionAsync(int binLocationId, int assetTypeId, int assetCondition, string userId);
        Task<IList<Asset>> GetByBinLocationIdnAssetGroupIdAsync(int binLocationId, int assetGroupId, string userId);
        Task<IList<Asset>> GetByBinLocationIdnAssetGroupIdnAssetConditionAsync(int binLocationId, int assetGroupId, int assetCondition, string userId);
        Task<IList<Asset>> GetByBinLocationIdAsync(int binLocationId, string userId);
        Task<IList<Asset>> GetByBinLocationIdnAssetConditionAsync(int binLocationId, int assetCondition, string userId);


        Task<IList<Asset>> GetByBaseLocationIdnAssetTypeIdAsync(int baseLocationId, int assetTypeId, string userId);
        Task<IList<Asset>> GetByBaseLocationIdnAssetTypeIdnAssetConditionAsync(int baseLocationId, int assetTypeId, int assetCondition, string userId);
        Task<IList<Asset>> GetByBaseLocationIdnAssetGroupIdAsync(int baseLocationId, int assetGroupId, string userId);
        Task<IList<Asset>> GetByBaseLocationIdnAssetGroupIdnAssetConditionAsync(int baseLocationId, int assetGroupId, int assetCondition, string userId);
        Task<IList<Asset>> GetByBaseLocationIdAsync(int baseLocationId, string userId);
        Task<IList<Asset>> GetByBaseLocationIdnAssetConditionAsync(int baseLocationId, int assetCondition, string userId);

        #endregion

        #region Asset Master Write Action Methods
        Task<bool> AddAsync(Asset asset);

        Task<bool> EditAsync(Asset asset);

        Task<bool> DeleteAsync(string assetId, string deletedBy);
        Task<bool> DeletePermanentlyAsync(string assetId);

        Task<bool> UpdateUsageStatusAsync(string assetId, string newStatus, string modifiedBy);

        Task<bool> UpdateAssetConditionAsync(string assetId, string assetCondition, string modifiedBy);

        Task<bool> UpdateBaseLocationAsync(string assetId, int baseLocationId, string currentLocation, int? binLocationId, string modifiedBy);

        Task<bool> UpdateCurrentLocationAsync(string assetId, string currentLocation, string modifiedBy);
        #endregion
    }
}
