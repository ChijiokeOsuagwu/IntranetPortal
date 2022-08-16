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

        public async Task<bool> DeletePersonAsync(string personId)
        {
            bool PersonIsDeleted = false;
            if (string.IsNullOrEmpty(personId)) { throw new ArgumentNullException(nameof(personId), "Required parameter [personId] is missing."); }
            try
            {
                PersonIsDeleted = await _personRepository.DeletePersonAsync(personId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return PersonIsDeleted;
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

        public async Task<Person> GetPersonbyNameAsync(string personName)
        {
            Person person = new Person();
            if (string.IsNullOrEmpty(personName)) { throw new ArgumentNullException(nameof(personName), "Required parameter [personName] is missing."); }
            try
            {
                person = await _personRepository.GetPersonByNameAsync(personName);
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
            if (person != null && !string.IsNullOrEmpty(person.PersonID)) { return true; } else { return false; }
        }

        #endregion

        //=================================== Auto Number Service Methods ============================================================//
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

        //=================================== Messages Service Methods ===============================================================//
        #region Messages Service Methods
        public async Task<bool> SendMessageAsync(Message message, List<string> receipientIds = null)
        {
            bool MessageIsSent = false;
            if (message == null) { throw new ArgumentNullException(nameof(message), "Required parameter [message] is missing."); }

            try
            {
                string MessageId = Guid.NewGuid().ToString();
                message.MessageID = MessageId;
                var firstAdded = await _utilityRepository.AddMessageAsync(message);
                if (firstAdded)
                {
                    if (receipientIds == null || receipientIds.Count < 1)
                    {
                        MessageDetail detail = new MessageDetail
                        {
                            MessageID = MessageId,
                            IsRead = false,
                            IsDeleted = false,
                            RecipientID = message.RecipientID,
                        };
                        MessageIsSent = await _utilityRepository.AddMessageDetailAsync(detail);
                    }
                    else
                    {
                        foreach (var Id in receipientIds)
                        {
                            MessageDetail detail = new MessageDetail
                            {
                                MessageID = MessageId,
                                IsRead = false,
                                IsDeleted = false,
                                RecipientID = Id,
                            };
                            MessageIsSent = await _utilityRepository.AddMessageDetailAsync(detail);
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return MessageIsSent;
        }

        public async Task<Message> ReadMessageAsync(int messageDetailId)
        {
            Message message = null;
            if (messageDetailId < 1) { throw new ArgumentNullException(nameof(messageDetailId), "Required parameter [MessageDetailID] is missing."); }
            try
            {
                bool ReadStatusUpdated = await _utilityRepository.UpdateMessageReadStatusAsync(messageDetailId);
                if (ReadStatusUpdated)
                {
                    message = await _utilityRepository.GetMessageByMessageDetailIdAsync(messageDetailId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return message;
        }

        public async Task<List<Message>> GetUnreadMessages(string recipientId)
        {
            var entities = await _utilityRepository.GetMessagesByReceipientIdAsync(recipientId);
            return entities.FindAll(x => x.IsRead == false);
        }

        public async Task<List<Message>> GetReadMessages(string recipientId)
        {
            var entities = await _utilityRepository.GetMessagesByReceipientIdAsync(recipientId);
            return entities.FindAll(x => x.IsRead == true);
        }

        public async Task<int> GetUnreadMessagesCount(string recipientId)
        {
            var entities = await _utilityRepository.GetMessagesByReceipientIdAsync(recipientId);
            return entities.FindAll(x => x.IsRead == false).Count;
        }

        public async Task<int> GetReadMessagesCount(string recipientId)
        {
            var entities = await _utilityRepository.GetMessagesByReceipientIdAsync(recipientId);
            return entities.FindAll(x => x.IsRead == true).Count;
        }

        public async Task<int> GetTotalMessagesCount(string recipientId)
        {
            var entities = await _utilityRepository.GetMessagesByReceipientIdAsync(recipientId);
            return entities.Count;
        }

        #endregion

        //================================== System Application Service Methods ======================================================//
        #region System Application Service Methods
        public async Task<List<SystemApplication>> GetSystemApplicationsAsync()
        {
            List<SystemApplication> apps = new List<SystemApplication>();
            try
            {
                apps = await _utilityRepository.GetApplicationsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return apps;
        }
        #endregion

        //================================== Industry Type Service Methods ======================================================//
        #region Industry Types Service Methods
        public async Task<List<IndustryType>> GetIndustryTypesAsync()
        {
            List<IndustryType> industryTypes = new List<IndustryType>();
            try
            {
                industryTypes = await _utilityRepository.GetIndustryTypesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return industryTypes;
        }
        #endregion
    }
}
