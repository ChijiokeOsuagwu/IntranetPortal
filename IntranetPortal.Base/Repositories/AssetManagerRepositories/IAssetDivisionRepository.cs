using IntranetPortal.Base.Models.AssetManagerModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AssetManagerRepositories
{
    public interface IAssetDivisionRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(AssetDivision assetDivision);
        Task<bool> DeleteAsync(int assetDivisionId);
        Task<bool> EditAsync(AssetDivision assetDivision);
        Task<IList<AssetDivision>> GetAllAsync(string userId);
        Task<IList<AssetDivision>> GetAllAsync();
        Task<AssetDivision> GetByIdAsync(int divisionId);
        Task<IList<AssetDivision>> GetByNameAsync(string divisionName);
    }
}