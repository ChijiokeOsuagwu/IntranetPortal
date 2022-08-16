using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IBusinessManagerService
    {
        #region Business CRUD Action Methods
        Task<bool> CreateBusinessAsync(Business business);
        Task<bool> DeleteBusinessAsync(string businessId);
        Task<bool> UpdateBusinessAsync(Business business);
        #endregion

        #region Business Contacts Action Methods
        Task<bool> CreateBusinessContactAsync(BusinessContact businessContact);
        Task<bool> UpdateBusinessContactAsync(BusinessContact businessContact);
        Task<bool> DeleteBusinessContactAsync(int businessContactId);
        Task<bool> DeleteBusinessContactsAsync(string businessId);
        Task<IList<BusinessContact>> GetBusinessContactsAsync();
        Task<IList<BusinessContact>> GetBusinessContactsByBusinessIdAsync(string businessId);
        Task<BusinessContact> GetBusinessContactByIdAsync(int businessContactId);
        #endregion

        #region Customers Action Methods
        Task<IList<Business>> GetCustomersAsync();
        Task<Business> GetCustomerByIdAsync(string customerId);
        Task<Business> GetCustomerByNameAsync(string customerName);
        Task<IList<Business>> SearchCustomersByNameAsync(string customerName);
        #endregion

        #region Suppliers Action Methods
        Task<IList<Business>> GetSuppliersAsync();
        Task<Business> GetSupplierByIdAsync(string supplierId);
        Task<Business> GetSupplierByNameAsync(string supplierName);
        Task<IList<Business>> SearchSuppliersByNameAsync(string supplierName);
        #endregion
    }
}
