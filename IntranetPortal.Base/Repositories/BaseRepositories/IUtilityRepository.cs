using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.WksModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BaseRepositories
{
    public interface IUtilityRepository
    {
        //=========================== Database Connectivity Check Action Methods ==============//
        #region Database Connectivity Check Action Method
        Task<bool> CheckDatabaseConnectionAsync();
        #endregion

        //=========================== Auto Number Action Methods =============================//
        #region Auto Number Action Methods
        Task<string> GetAutoNumberAsync(string numberType);
        Task<bool> IncrementAutoNumberAsync(string numberType);
        Task<int> GetNumberCount(AutoNumberType type, int day, int month, int year);
        Task<bool> AddCodeNumberRecord(AutoNumberType type, int day, int month, int year);
        #endregion

        //=========================== Activity History Action Methods ========================//
        #region Activity History Action Methods
        Task<bool> InsertActivityHistoryAsync(ActivityHistory activityHistory);
        #endregion

        //=========================== Entity Activity History Action Methods ==================//
        #region Entity Activity History Action Methods
        Task<bool> InsertTaskItemActivityHistoryAsync(TaskItemActivityHistory taskItemActivityHistory);
        Task<bool> InsertTaskListActivityHistoryAsync(TaskListActivityHistory taskListActivityHistory);
        Task<List<TaskItemActivityHistory>> GetTaskItemActivityHistoryByTaskItemIdAsync(long taskItemId);
        #endregion

        //=========================== System Applications Action Methods =====================//
        #region System Applications Action Methods
        Task<List<SystemApplication>> GetApplicationsAsync();
        #endregion

        //============================= Message Action Methods ===============================//
        #region Message Action Methods
        Task<List<Message>> GetMessagesByReceipientIdAsync(string recipientId);

        Task<Message> GetMessageByMessageDetailIdAsync(int messageDetailId);

        Task<Message> GetMessageByMessageIdAsync(string messageId);

        Task<bool> AddMessageAsync(Message message);

        Task<bool> AddMessageDetailAsync(MessageDetail messageDetail);

        Task<bool> UpdateMessageReadStatusAsync(int messageDetailId);

        //========= Delete Messages Permanently ==========//
        Task<bool> DeleteMessageDetailByMessageDetailIdAsync(int messageDetailId);
        Task<bool> DeleteMessageByMessageIdAsync(string messageId);
        Task<bool> DeleteReadMessageDetailByRecipientIdAsync(string recipientId);

        #endregion

        //============================= Industry Types Action Methods =======================//
        #region Industry Types Action Methods
        Task<List<IndustryType>> GetIndustryTypesAsync();
        #endregion
    }
}
