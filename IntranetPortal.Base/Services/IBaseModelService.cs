using IntranetPortal.Base.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IBaseModelService
    {
        //========================================= Person Action Methods ===========================================//
        #region Person Action Methods
        Task<bool> CreatePersonAsync(Person person);

        Task<bool> UpdatePersonAsync(Person person);

        Task<bool> DeletePersonAsync(string personId);

        Task<bool> PersonExistsAsync(string personId);

        Task<Person> GetPersonAsync(string personId);

        Task<Person> GetPersonbyNameAsync(string personName);
        #endregion

        //========================================= AutoNumber Action Methods ===========================================//
        #region AutoNumber Action Methods 
        Task<string> GenerateAutoNumberAsync(string NumberType);

        Task<bool> IncrementAutoNumberAsync(string NumberType);
        #endregion

        //========================================= Message Service Methods ==============================================// 
        #region Message Service Methods
        Task<bool> SendMessageAsync(Message message, List<string> receipientIds = null);

        Task<Message> ReadMessageAsync(int messageDetailId);

        Task<List<Message>> GetUnreadMessages(string recipientId);

        Task<List<Message>> GetReadMessages(string recipientId);

        Task<int> GetUnreadMessagesCount(string recipientId);

        Task<int> GetReadMessagesCount(string recipientId);

        Task<int> GetTotalMessagesCount(string recipientId);

        #endregion
        //========================================== System Application Action Methods ===================================//
        #region System Applications Action Methods 
        Task<List<SystemApplication>> GetSystemApplicationsAsync();
        #endregion

        //================================== Industry Type Service Methods ======================================================//
        #region Industry Types Service Methods
        Task<List<IndustryType>> GetIndustryTypesAsync();
        #endregion

    }
}
