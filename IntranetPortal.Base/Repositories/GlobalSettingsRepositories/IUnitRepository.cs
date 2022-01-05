using IntranetPortal.Base.Models.GlobalSettingsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.GlobalSettingsRepositories
{
    public interface IUnitRepository
    {
        Task<bool> AddUnitAsync(Unit unit);
        Task<bool> DeleteUnitAsync(int Id);
        Task<bool> EditUnitAsync(Unit unit);
        Task<Unit> GetUnitByIdAsync(int Id);
        Task<IList<Unit>> GetUnitsAsync();
    }
}
