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
        Task<AssetType> GetByIdAsync(int assetTypeId);
        Task<IList<AssetType>> GetByNameAsync(string typeName);
        Task<IList<AssetType>> GetByNameAsync(string typeName, IEntityPermission entityPermission);
        Task<IList<AssetType>> GetByCategoryIdAsync(int assetCategoryId);
        Task<IList<AssetType>> GetByCategoryIdAsync(int assetCategoryId, IEntityPermission entityPermission);
        Task<IList<AssetType>> GetByClassIdAsync(int assetClassId);
        Task<IList<AssetType>> GetByClassIdAsync(int assetClassId, IEntityPermission entityPermission);
        Task<IList<AssetType>> GetAllAsync();
        Task<IList<AssetType>> GetAllAsync(IEntityPermission entityPermission);
        Task<bool> AddAsync(AssetType assetType);

        Task<bool> EditAsync(AssetType assetType);

        Task<bool> DeleteAsync(int assetTypeId);
    }
}
