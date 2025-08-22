using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
using IntranetPortal.Base.Repositories.BusinessManagerRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class BusinessManagerService : IBusinessManagerService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IBusinessContactRepository _businessContactRepository;

        public BusinessManagerService(IBusinessRepository businessRepository, IBusinessContactRepository businessContactRepository)
        {
            _businessRepository = businessRepository;
            _businessContactRepository = businessContactRepository;
        }

        //============================ Business CRUD Action Methods =============================//
        #region Business CRUD Action Methods
        public async Task<bool> CreateBusinessAsync(Business business)
        {
            if (business == null) { throw new ArgumentNullException(nameof(business), "Required parameter [Business] is missing. The request cannot be processed."); }
            bool IsSuccessful = await _businessRepository.AddAsync(business);

            return IsSuccessful;
        }

        public async Task<bool> DeleteBusinessAsync(string businessId)
        {
            if (string.IsNullOrWhiteSpace(businessId)) { throw new ArgumentNullException(nameof(businessId), "Required parameter [BusinessID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _businessRepository.DeleteAsync(businessId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateBusinessAsync(Business business)
        {
            if (business == null) { throw new ArgumentNullException(nameof(business), "Required parameter [Business] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _businessRepository.EditAsync(business);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        #endregion

        //======= Business Contacts Action Methods =====// 
        #region Business Contacts Action Methods
        public async Task<bool> CreateBusinessContactAsync(BusinessContact businessContact)
        {
            if (businessContact == null) { throw new ArgumentNullException(nameof(businessContact), "Required parameter [Business Contact] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
                IsSuccessful = await _businessContactRepository.AddAsync(businessContact);
            return IsSuccessful;
        }

        public async Task<bool> UpdateBusinessContactAsync(BusinessContact businessContact)
        {
            if (businessContact == null) { throw new ArgumentNullException(nameof(businessContact), "Required parameter [Business Contact] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
                IsSuccessful = await _businessContactRepository.EditAsync(businessContact);
            return IsSuccessful;
        }
        public async Task<bool> DeleteBusinessContactAsync(long businessContactId)
        {
            if (businessContactId < 1) { throw new ArgumentNullException(nameof(businessContactId), "Required parameter [BusinessContactID] cannot be null."); }
            bool IsSuccessful = false;
                IsSuccessful = await _businessContactRepository.DeleteAsync(businessContactId);
            return IsSuccessful;
        }
        public async Task<bool> DeleteBusinessContactsAsync(string businessId)
        {
            if (string.IsNullOrWhiteSpace(businessId)) { throw new ArgumentNullException(nameof(businessId), "Required parameter [BusinessID] cannot be null."); }
            bool IsSuccessful = false;
                IsSuccessful = await _businessContactRepository.DeleteByBusinessIdAsync(businessId);
            return IsSuccessful;
        }
        public async Task<List<BusinessContact>> GetBusinessContactsAsync()
        {
            List<BusinessContact> contacts = new List<BusinessContact>();
                var entities = await _businessContactRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { contacts = entities; }
            return contacts;
        }
        public async Task<List<BusinessContact>> GetBusinessContactsByBusinessIdAsync(string businessId)
        {
            List<BusinessContact> contacts = new List<BusinessContact>();
                var entities = await _businessContactRepository.GetByBusinessIdAsync(businessId);
                if (entities != null && entities.Count > 0) { contacts = entities; }
            return contacts;
        }
        public async Task<BusinessContact> GetBusinessContactByIdAsync(long businessContactId)
        {
            BusinessContact contact = new BusinessContact();
                 var entity = await _businessContactRepository.GetByIdAsync(businessContactId);
                if (entity != null) { contact = entity; }
             return contact;
        }
        #endregion

        //======= Customers Action Methods ========//
        #region Customers Action Methods
        public async Task<List<Business>> GetCustomersAsync()
        {
            List<Business> customers = new List<Business>();
                var entities = await _businessRepository.GetAllCustomersAsync();
                if (entities != null && entities.Count > 0) { customers = entities; }
            return customers;
        }

        public async Task<Business> GetCustomerByIdAsync(string customerId)
        {
            Business customer = new Business();
                var entity = await _businessRepository.GetByIdAsync(customerId);
                if (entity != null && !string.IsNullOrWhiteSpace(entity.BusinessName)) { customer = entity; }
             return customer;
        }

        public async Task<Business> GetCustomerByNameAsync(string customerName)
        {
            Business customer = new Business();
               var entity = await _businessRepository.GetCustomerByNameAsync(customerName);
              if (entity != null && !string.IsNullOrWhiteSpace(entity.BusinessName)) { customer = entity; }
            return customer;
        }

        public async Task<List<Business>> SearchCustomersByNameAsync(string customerName)
        {
            List<Business> customers = new List<Business>();
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                var entities = await _businessRepository.SearchCustomersByNameAsync(customerName);
                if (entities != null && entities.Count > 0) { customers = entities; }
            }
            return customers;
        }

        #endregion

        //======= Suppliers Action Methods =======//
        #region Suppliers Action Methods
        public async Task<List<Business>> GetSuppliersAsync()
        {
            List<Business> suppliers = new List<Business>();
                var entities = await _businessRepository.GetAllSuppliersAsync();
                if (entities != null && entities.Count > 0) { suppliers = entities; }
            return suppliers;
        }

        public async Task<Business> GetSupplierByIdAsync(string supplierId)
        {
            Business supplier = new Business();
                var entity = await _businessRepository.GetByIdAsync(supplierId);
                if (entity != null && !string.IsNullOrWhiteSpace(entity.BusinessName)) { supplier = entity; }
            return supplier;
        }

        public async Task<Business> GetSupplierByNameAsync(string supplierName)
        {
            Business supplier = new Business();
                var entity = await _businessRepository.GetSupplierByNameAsync(supplierName);
                if (entity != null && !string.IsNullOrWhiteSpace(entity.BusinessName)) { supplier = entity; }
            return supplier;
        }

        public async Task<List<Business>> SearchSuppliersByNameAsync(string supplierName)
        {
            List<Business> suppliers = new List<Business>();
                var entities = await _businessRepository.SearchSuppliersByNameAsync(supplierName);
                if (entities != null && entities.Count > 0) { suppliers = entities; }
            return suppliers;
        }

        #endregion

        public async Task<string> GetNewCodeNumber()
        {
            char FirstCharacter = 'B';
            List<string> _existingNumbers = new List<string>();
            string yy = DateTime.Now.Year.ToString().Substring(2, 2);
            //string mm = createdDate.Month.ToString().PadLeft(2, '0');
            //string dd = day.ToString().PadLeft(2, '0');

            _existingNumbers = await _businessRepository.GetCodeNumbersByCreatedDateAsync(DateTime.Now);
            if (_existingNumbers == null || _existingNumbers.Count < 1)
            {
                return $"{FirstCharacter}{yy}0001";
            }

            string _newAssignmentNumber = string.Empty;
            int _nextCount = 1;
            bool _isExisting = true;
            do
            {
                string _nextDigitString = _nextCount.ToString().PadLeft(4, '0');
                _newAssignmentNumber = $"{FirstCharacter}{yy}{_nextDigitString}";
                _isExisting = _existingNumbers.Contains(_newAssignmentNumber);
                _nextCount++;
            }
            while (_isExisting);
            return _newAssignmentNumber;
        }
    }
}
