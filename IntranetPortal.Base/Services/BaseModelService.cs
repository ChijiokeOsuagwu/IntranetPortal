using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.EmployeeRecordRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class BaseModelService : IBaseModelService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUtilityRepository _utilityRepository;
        public BaseModelService(IPersonRepository personRepository, IUtilityRepository utilityRepository)
        {
            _personRepository = personRepository;
            _utilityRepository = utilityRepository;
        }

        //======================================== Person Service Methods ===========================================================//
        #region Person Service Methods
        public async Task<bool> CreatePersonAsync(Person person)
        {
            bool PersonIsAdded = false;
            if (person == null) { throw new ArgumentNullException(nameof(person), "Required parameter [person] is missing."); }
            try
            {
                PersonIsAdded = await _personRepository.AddPersonAsync(person);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return PersonIsAdded;
        }

        public async Task<bool> UpdatePersonAsync(Person person)
        {
            bool PersonIsUpdated = false;
            if (person == null) { throw new ArgumentNullException(nameof(person), "Required parameter [person] is missing."); }
            try
            {
                PersonIsUpdated = await _personRepository.EditPersonAsync(person);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return PersonIsUpdated;
        }


        public async Task<Person> GetPersonAsync(string personId)
        {
            Person person = new Person();
            if (string.IsNullOrEmpty(personId)) { throw new ArgumentNullException(nameof(personId), "Required parameter [personId] is missing."); }
            try
            {
                person = await _personRepository.GetPersonByIdAsync(personId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return person;
        }

        public async Task<bool> PersonExistsAsync(string personId)
        {
            Person person = new Person();
            if (string.IsNullOrEmpty(personId)) { throw new ArgumentNullException(nameof(personId), "Required parameter [personId] is missing."); }
            try
            {
                person = await _personRepository.GetPersonByIdAsync(personId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            if(person != null && !string.IsNullOrEmpty(person.PersonID)) { return true; } else { return false; }
        }


        #endregion

        //=================================== Auto Number Service Methods =============================================================//
        #region Auto Number Service Methods
        public async Task<string> GenerateAutoNumberAsync(string NumberType)
        {
            string autoNumber = string.Empty;
            if (string.IsNullOrEmpty(NumberType)) { throw new ArgumentNullException(nameof(NumberType), "The required parameter [Number Type] is missing."); }
            try
            {
                var result = await _utilityRepository.GetAutoNumberAsync(NumberType);
                autoNumber = result.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return autoNumber;
        }

        public async Task<bool> IncrementAutoNumberAsync(string NumberType)
        {
            bool autoNumberUpdated = false;
            if (string.IsNullOrEmpty(NumberType)) { throw new ArgumentNullException(nameof(NumberType), "The required parameter [Number Type] is missing."); }
            try
            {
                autoNumberUpdated = await _utilityRepository.IncrementAutoNumberAsync(NumberType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return autoNumberUpdated;
        }

        #endregion
    }
}
