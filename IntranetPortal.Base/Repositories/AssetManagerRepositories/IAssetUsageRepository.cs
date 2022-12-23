using IntranetPortal.Base.Models.AssetManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AssetManagerRepositories
{
    public interface IAssetUsageRepository
    {
        Task<IList<AssetUsage>> GetAllCurrentAsync();
        Task<IList<AssetUsage>> GetCurrentByAssetIdAsync(string assetId);
        Task<IList<AssetUsage>> GetCurrentByAssetNameAsync(string assetName);
        Task<IList<AssetUsage>> GetCurrentByAssetTypeIdAsync(int assetTypeId);

        Task<IList<AssetUsage>> GetAllAsync();
        Task<AssetUsage> GetByIdAsync(int assetUsageId);
        Task<IList<AssetUsage>> GetByAssetIdAsync(string assetId);
        Task<IList<AssetUsage>> GetByAssetIdAndYearAsync(string assetId, int usageYear);
        Task<IList<AssetUsage>> GetByAssetIdAndYearAndMonthAsync(string assetId, int usageYear, int usageMonth);
        Task<IList<AssetUsage>> GetCheckedOutByAssetIdAsync(string assetId);
        Task<IList<AssetUsage>> GetByAssetNameAsync(string assetName);
        Task<IList<AssetUsage>> GetByAssetTypeIdAsync(int assetTypeId);

        Task<bool> AddCheckOutAsync(AssetUsage assetUsage);
        Task<bool> EditAsync(AssetUsage assetUsage);
        Task<bool> DeleteAsync(int assetUsageId);
    }
}
