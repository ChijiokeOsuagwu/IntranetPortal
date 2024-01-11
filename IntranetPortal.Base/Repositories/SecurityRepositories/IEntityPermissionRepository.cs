using IntranetPortal.Base.Models.AssetManagerModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.SecurityRepositories
{
    public interface IEntityPermissionRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAssetPermissionAsync(AssetPermission assetPermission);
        Task<bool> DeleteAssetPermissionAsync(int assetPermissionId);
        Task<IList<AssetPermission>> GetAssetPermissionsByUserIdAsync(string userId);
    }
}