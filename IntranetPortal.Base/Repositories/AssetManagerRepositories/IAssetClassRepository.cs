using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Models.SecurityModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.AssetManagerRepositories
{
    public interface IAssetClassRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(AssetClass assetClass);
        Task<bool> DeleteAsync(int assetClassId);
        Task<bool> EditAsync(AssetClass assetClass);
        Task<IList<AssetClass>> GetAllAsync();
        Task<IList<AssetClass>> GetAllAsync(IEntityPermission entityPermission);
        Task<IList<AssetClass>> GetByCategoryIdAsync(int assetCategoryId);
        Task<IList<AssetClass>> GetByCategoryIdAsync(int assetCategoryId, IEntityPermission entityPermission);
        Task<AssetClass> GetByIdAsync(int assetClassId);
        Task<IList<AssetClass>> GetByNameAsync(string className);
        Task<IList<AssetClass>> GetByNameAsync(string className, IEntityPermission entityPermission);
    }
}