using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
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
        Task<bool> DeleteAsync(long businessContactId);
        Task<bool> DeleteByBusinessIdAsync(string businessId);
        Task<BusinessContact> GetByIdAsync(long businessContactId);
        Task<List<BusinessContact>> GetAllAsync();
        Task<List<BusinessContact>> GetByBusinessIdAsync(string businessId);
    }
}
