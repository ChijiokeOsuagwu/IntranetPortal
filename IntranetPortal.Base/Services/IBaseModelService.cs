using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.WksModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IBaseModelService
    {
        //========================================= Utility Service Methods ============================================//
        #region Utility Service Methods
        Task<bool> DbConnectionIsOpenAsync();
        #endregion
        //========================================= Person Action Methods ===========================================//
        #region Person Action Methods
        Task<bool> CreatePersonAsync(Person person);

        Task<bool> UpdatePersonAsync(Person person);

        Task<bool> DeletePersonAsync(string personId, string deletedBy, string deletedTime);

        Task<bool> PersonExistsAsync(string personId);

        Task<Person> GetPersonAsync(string personId);

        Task<Person> GetPersonbyNameAsync(string personName);

        Task<List<Person>> SearchPersonsByName(string personName);

        Task<List<Person>> SearchNonEmployeePersonsByName(string personName);

        #endregion

        //========================================= AutoNumber Action Methods ===========================================//
        #region AutoNumber Action Methods 
        Task<string> GenerateAutoNumberAsync(string NumberType);

        Task<bool> IncrementAutoNumberAsync(string NumberType);

        Task<string> GenerateCodeNumberAsync(AutoNumberType type, string typeCode);

        Task<bool> RegisterCodeNumberAsync(AutoNumberType type);
        #endregion

        //========================================= Message Service Methods ==============================================// 
        #region Message Service Methods
        Task<bool> SendMessageAsync(Message message, List<string> receipientIds);
        Task<bool> SendMessageAsync(Message message);
        Task<bool> AddMessageRecipientAsync(MessageDetail messageDetail);
        Task<bool> UpdateReadStatusAsync(int messageDetailId);
        Task<Message> ReadMessageAsync(int messageDetailId);
        Task<List<Message>> GetAllMessages(string recipientId);
        Task<List<Message>> GetUnreadMessages(string recipientId);
        Task<List<Message>> GetReadMessages(string recipientId);

        Task<int> GetUnreadMessagesCount(string recipientId);

        Task<int> GetReadMessagesCount(string recipientId);

        Task<int> GetTotalMessagesCount(string recipientId);

        Task<bool> DeleteMessageByMessageDetailIDAsync(int messageDetailId);

        Task<bool> DeleteReadMessagesByRecipientIdAsync(string recipientId);

        Task<bool> DeleteUnReadMessagesByRecipientIdAsync(string recipientId);
        #endregion

        //========================================== System Application Action Methods ===================================//
        #region System Applications Action Methods 
        Task<List<SystemApplication>> GetSystemApplicationsAsync();
        #endregion

        //================================== Industry Type Service Methods ===============================================//
        #region Industry Types Service Methods
        Task<List<IndustryType>> GetIndustryTypesAsync();
        #endregion

        //================================= Entity Activity History Service Methods =======================================//
        #region Entity Activity History Service Methods
        Task<bool> AddEntityActivityHistoryAsync(EntityActivityHistory entityActivityHistory, EntityType entityType);
        Task<List<TaskItemActivityHistory>> GetTaskItemActivityHistory(long taskItemId);
        #endregion

    }
}
