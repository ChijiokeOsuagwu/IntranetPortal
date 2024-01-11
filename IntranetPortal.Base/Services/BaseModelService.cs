using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Base.Repositories.BaseRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
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

        //======================================== Utility Service Methods ==========================================================//
        #region Utility Service Methods
        public async Task<bool> DbConnectionIsOpenAsync()
        {
            return await _utilityRepository.CheckDatabaseConnectionAsync();
        }
        #endregion

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

        public async Task<bool> DeletePersonAsync(string personId, string deletedBy, string deletedTime)
        {
            bool PersonIsDeleted = false;
            if (string.IsNullOrEmpty(personId)) { throw new ArgumentNullException(nameof(personId), "Required parameter [personId] is missing."); }
            try
            {
                PersonIsDeleted = await _personRepository.DeletePersonAsync(personId, deletedBy, deletedTime);
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

        public async Task<List<Person>> SearchPersonsByName(string personName)
        {
            List<Person> personList = new List<Person>();
            var entities = await _personRepository.SearchPersonsByNameAsync(personName);
            if(entities != null && entities.Count > 0)
            {
                personList = entities.ToList();
            }
            return personList;
        }

        public async Task<List<Person>> SearchNonEmployeePersonsByName(string personName)
        {
            List<Person> personList = new List<Person>();
            var entities = await _personRepository.SearchNonEmployeePersonsByNameAsync(personName);
            if (entities != null && entities.Count > 0)
            {
                personList = entities.ToList();
            }
            return personList;
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

        public async Task<string> GenerateCodeNumberAsync(AutoNumberType type, string typeCode)
        {
            try
            {
                string yy = (DateTime.Now.Year).ToString().Substring(2, 2);
                string mm = (DateTime.Now.Month).ToString().PadLeft(2, '0');
                string dd = (DateTime.Now.Day).ToString().PadLeft(2, '0');
                int recordCount = await _utilityRepository.GetNumberCount(type, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                string nn = recordCount.ToString().PadLeft(2, '0');
                return $"{typeCode}{yy}{mm}{dd}{nn}";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> RegisterCodeNumberAsync(AutoNumberType type)
        {
            try
            {
                return await _utilityRepository.AddCodeNumberRecord(type, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #endregion

        //=================================== Messages Service Methods ===============================================================//
        #region Messages Service Methods
        public async Task<bool> SendMessageAsync(Message message, List<string> receipientIds)
        {
            bool MessageIsSent = false;
            if (message == null) { throw new ArgumentNullException(nameof(message), "Required parameter [message] is missing."); }
            if (receipientIds.Count < 1) { throw new ArgumentNullException(nameof(receipientIds)); }
            try
            {
                if (string.IsNullOrWhiteSpace(message.MessageID)) { message.MessageID = Guid.NewGuid().ToString(); }
                string messageId = message.MessageID;
                var existing_msg = await _utilityRepository.GetMessageByMessageIdAsync(messageId);
                if (existing_msg == null || string.IsNullOrWhiteSpace(existing_msg.Subject))
                {
                    await _utilityRepository.AddMessageAsync(message);
                }

                foreach (var Id in receipientIds)
                {
                    MessageDetail detail = new MessageDetail
                    {
                        MessageID = messageId,
                        IsRead = false,
                        IsDeleted = false,
                        RecipientID = Id,
                    };
                    MessageIsSent = await _utilityRepository.AddMessageDetailAsync(detail);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return MessageIsSent;
        }

        public async Task<bool> SendMessageAsync(Message message)
        {
            bool MessageIsSent = false;
            if (message == null) { throw new ArgumentNullException(nameof(message), "Required parameter [message] is missing."); }
            if (string.IsNullOrWhiteSpace(message.MessageID)) { throw new ArgumentNullException(nameof(message.MessageID)); }
            try
            {
                var existing_msg = await _utilityRepository.GetMessageByMessageIdAsync(message.MessageID);
                if (existing_msg == null || string.IsNullOrWhiteSpace(existing_msg.Subject))
                {
                    MessageIsSent = await _utilityRepository.AddMessageAsync(message);
                }
                else
                {
                    MessageIsSent = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return MessageIsSent;
        }

        public async Task<bool> AddMessageRecipientAsync(MessageDetail messageDetail)
        {
            bool MessageIsSent = false;
            if (messageDetail == null) { throw new ArgumentNullException(nameof(messageDetail)); }

            try
            {
                MessageIsSent = await _utilityRepository.AddMessageDetailAsync(messageDetail);

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

        public async Task<bool> UpdateReadStatusAsync(int messageDetailId)
        {
           if (messageDetailId < 1) { throw new ArgumentNullException(nameof(messageDetailId), "Required parameter [MessageDetailID] is missing."); }
            try
            {
                return await _utilityRepository.UpdateMessageReadStatusAsync(messageDetailId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<Message>> GetAllMessages(string recipientId)
        {
            return await _utilityRepository.GetMessagesByReceipientIdAsync(recipientId);
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

        public async Task<bool> DeleteMessageByMessageDetailIDAsync(int messageDetailId)
        {
            bool IsDeleted = false;
            if (messageDetailId < 1) { throw new ArgumentNullException(nameof(messageDetailId)); }
            try
            {
                IsDeleted = await _utilityRepository.UpdateMessageDeleteStatusByMessageDetailIdAsync(messageDetailId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsDeleted;
        }

        public async Task<bool> DeleteReadMessagesByRecipientIdAsync(string recipientId)
        {
            bool IsDeleted = false;
            if (string.IsNullOrWhiteSpace(recipientId)) { throw new ArgumentNullException(nameof(recipientId)); }
            try
            {
                IsDeleted = await _utilityRepository.UpdateMessageDeleteStatusByRecipientIdAsync(recipientId, true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsDeleted;
        }

        public async Task<bool> DeleteUnReadMessagesByRecipientIdAsync(string recipientId)
        {
            bool IsDeleted = false;
            if (string.IsNullOrWhiteSpace(recipientId)) { throw new ArgumentNullException(nameof(recipientId)); }
            try
            {
                IsDeleted = await _utilityRepository.UpdateMessageDeleteStatusByRecipientIdAsync(recipientId, false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsDeleted;
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

        //================================= Entity Activity History Service Methods =============================================//
        #region Entity Activity History Service Methods
        public async Task<bool> AddEntityActivityHistoryAsync(EntityActivityHistory entityActivityHistory, EntityType entityType)
        {
            bool IsAdded = false;
            switch (entityType)
            {
                case EntityType.TaskItem:
                    TaskItemActivityHistory taskItemHistory = entityActivityHistory.ConvertToTaskItemActivityHistory();
                    IsAdded = await _utilityRepository.InsertTaskItemActivityHistoryAsync(taskItemHistory);
                    break;
                case EntityType.TaskList:
                    TaskListActivityHistory taskListHistory = entityActivityHistory.ConvertToTaskListActivityHistory();
                    IsAdded = await _utilityRepository.InsertTaskListActivityHistoryAsync(taskListHistory);
                    break;
                default:
                    break;
            }
            return IsAdded;
        }
        
        public async Task<List<TaskItemActivityHistory>> GetTaskItemActivityHistory(long taskItemId)
        {
            List<TaskItemActivityHistory> history = new List<TaskItemActivityHistory>();
            try
            {
                history = await _utilityRepository.GetTaskItemActivityHistoryByTaskItemIdAsync(taskItemId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return history;
        }

        #endregion
    }
}
