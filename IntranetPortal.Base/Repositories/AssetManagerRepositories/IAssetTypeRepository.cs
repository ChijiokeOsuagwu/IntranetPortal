using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Models.SecurityModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AssetManagerRepositories
{
    public interface IAssetTypeRepository
    {
        #region Asset Type Read Action Methods
        Task<AssetType> GetByIdAsync(int assetTypeId);
        Task<IList<AssetType>> GetByNameAsync(string typeName);
        Task<IList<AssetType>> GetByCategoryIdAsync(int assetCategoryId);
        Task<IList<AssetType>> GetByClassIdAsync(int assetClassId);
        Task<IList<AssetType>> GetByGroupIdAsync(int assetGroupId);
        Task<IList<AssetType>> GetAllAsync();
        #endregion

        #region Asset Type Write Action Methods
        Task<bool> AddAsync(AssetType assetType);

        Task<bool> EditAsync(AssetType assetType);

        Task<bool> DeleteAsync(int assetTypeId);
        #endregion
    }
}
