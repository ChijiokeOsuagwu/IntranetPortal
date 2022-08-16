using IntranetPortal.Base.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BusinessManagerRepositories
{
    public interface IBusinessContactRepository
    {
        Task<bool> AddAsync(BusinessContact businessContact);
        Task<bool> EditAsync(BusinessContact businessContact);
        Task<bool> DeleteAsync(int businessContactId);
        Task<bool> DeleteByBusinessIdAsync(string businessId);
        Task<BusinessContact> GetByIdAsync(int businessContactId);
        Task<IList<BusinessContact>> GetAllAsync();
        Task<IList<BusinessContact>> GetByBusinessIdAsync(string businessId);
    }
}
