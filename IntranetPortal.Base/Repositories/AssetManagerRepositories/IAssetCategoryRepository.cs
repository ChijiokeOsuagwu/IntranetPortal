using IntranetPortal.Base.Models.AssetManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AssetManagerRepositories
{
    public interface IAssetCategoryRepository
    {
        Task<AssetCategory> GetByIdAsync(int categoryId);

        Task<IList<AssetCategory>> GetByNameAsync(string categoryName);

        Task<IList<AssetCategory>> GetAllAsync();

        Task<bool> AddAsync(AssetCategory assetCategory);

        Task<bool> EditAsync(AssetCategory assetCategory);

        Task<bool> DeleteAsync(int assetCategoryId);
    }
}
