using IntranetPortal.Base.Models.BamsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BamsRepositories
{
    public interface IEquipmentGroupsRepository
    {
        Task<EquipmentGroup> GetByIdAsync(int Id);
        Task<IList<EquipmentGroup>> GetAllAsync();
        Task<bool> AddAsync(EquipmentGroup equipmentGroup);
        Task<bool> EditAsync(EquipmentGroup equipmentGroup);
        Task<bool> DeleteAsync(int equipmentGroupId);
    }
}
