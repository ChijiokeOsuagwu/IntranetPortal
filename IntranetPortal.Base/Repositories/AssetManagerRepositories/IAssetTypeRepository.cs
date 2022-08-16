using IntranetPortal.Base.Models.AssetManagerModels;
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

        Task<IList<AssetType>> GetByCategoryIdAsync(int assetCategoryId);

        Task<IList<AssetType>> GetAllAsync();

        Task<bool> AddAsync(AssetType assetType);

        Task<bool> EditAsync(AssetType assetType);

        Task<bool> DeleteAsync(int assetTypeId);
    }
}
