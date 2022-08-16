using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BusinessManagerRepositories
{
    public interface IBusinessRepository
    {
        Task<Business> GetByIdAsync(string businessId);
        Task<Business> GetCustomerByNameAsync(string businessName);
        Task<IList<Business>> SearchCustomersByNameAsync(string businessName);
        Task<IList<Business>> GetAllCustomersAsync();
        Task<Business> GetSupplierByNameAsync(string businessName);
        Task<IList<Business>> SearchSuppliersByNameAsync(string businessName);
        Task<IList<Business>> GetAllSuppliersAsync();
        Task<bool> AddAsync(Business business);
        Task<bool> EditAsync(Business business);
        Task<bool> DeleteAsync(string businessId);

    }
}
