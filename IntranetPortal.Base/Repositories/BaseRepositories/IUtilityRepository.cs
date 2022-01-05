using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BaseRepositories
{
    public interface IUtilityRepository
    {
        Task<string> GetAutoNumberAsync(string numberType);
        Task<bool> IncrementAutoNumberAsync(string numberType);
    }
}
