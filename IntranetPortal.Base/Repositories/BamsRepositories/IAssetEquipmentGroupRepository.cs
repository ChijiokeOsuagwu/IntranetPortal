using IntranetPortal.Base.Models.BamsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BamsRepositories
{
    public interface IAssetEquipmentGroupRepository
    {
        Task<AssetEquipmentGroup> GetByIdAsync(int assetEquipmentGroupId);
        Task<IList<AssetEquipmentGroup>> GetByEquipmentGroupIdAsync(int equipmentGroupId);
        Task<IList<AssetEquipmentGroup>> GetByEquipmentIdAndEquipmentGroupIdAsync(int equipmentGroupId, string equipmentId);
        Task<bool> AddAsync(AssetEquipmentGroup assetEquipmentGroup);
        Task<bool> DeleteAsync(int assetEquipmentGroupId);
    }
}
