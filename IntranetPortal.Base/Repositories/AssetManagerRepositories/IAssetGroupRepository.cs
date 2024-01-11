using IntranetPortal.Base.Models.AssetManagerModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AssetManagerRepositories
{
    public interface IAssetGroupRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(AssetGroup assetGroup);
        Task<bool> DeleteAsync(int assetGroupId);
        Task<bool> EditAsync(AssetGroup assetGroup);
        Task<IList<AssetGroup>> GetAllAsync();
        Task<IList<AssetGroup>> GetByCategoryIdAsync(int assetCategoryId);
        Task<IList<AssetGroup>> GetByClassIdAsync(int assetClassId);
        Task<AssetGroup> GetByIdAsync(int assetGroupId);
        Task<IList<AssetGroup>> GetByNameAsync(string groupName);
    }
}