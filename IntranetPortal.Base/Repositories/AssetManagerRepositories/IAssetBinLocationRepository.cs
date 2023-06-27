using IntranetPortal.Base.Models.AssetManagerModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.AssetManagerRepositories
{
    public interface IAssetBinLocationRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(AssetBinLocation assetBinLocation);
        Task<bool> DeleteAsync(int assetBinLocationId);
        Task<bool> EditAsync(AssetBinLocation assetBinLocation);
        Task<IList<AssetBinLocation>> GetAllAsync();
        Task<AssetBinLocation> GetByIdAsync(int assetBinLocationId);
        Task<IList<AssetBinLocation>> GetByLocationIdAsync(int locationId);
        Task<IList<AssetBinLocation>> GetByNameAsync(string assetBinLocationName);
        Task<IList<AssetBinLocation>> SearchByNameAsync(string assetBinLocationName);
    }
}
