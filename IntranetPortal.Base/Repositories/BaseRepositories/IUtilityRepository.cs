using IntranetPortal.Base.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BaseRepositories
{
    public interface IUtilityRepository
    {
        //=========================== Auto Number Action Methods =============================//
        #region Auto Number Action Methods
        Task<string> GetAutoNumberAsync(string numberType);
        Task<bool> IncrementAutoNumberAsync(string numberType);
        #endregion

        //=========================== Activity History Action Methods ========================//
        #region Activity History Action Methods
        Task<bool> InsertActivityHistoryAsync(ActivityHistory activityHistory);
        #endregion

        //=========================== System Applications Action Methods =====================//
        #region System Applications Action Methods
        Task<List<SystemApplication>> GetApplicationsAsync();
        #endregion

        //============================= Message Action Methods ===============================//
        #region Message Action Methods
        Task<List<Message>> GetMessagesByReceipientIdAsync(string recipientId);

        Task<Message> GetMessageByMessageDetailIdAsync(int messageDetailId);

        Task<bool> AddMessageAsync(Message message);

        Task<bool> AddMessageDetailAsync(MessageDetail messageDetail);

        Task<bool> UpdateMessageReadStatusAsync(int messageDetailId);

        Task<bool> UpdateMessageDeleteStatusAsync(int messageDetailId);

        #endregion

        //============================= Industry Types Action Methods =======================//
        #region Industry Types Action Methods
        Task<List<IndustryType>> GetIndustryTypesAsync();
        #endregion
    }
}
