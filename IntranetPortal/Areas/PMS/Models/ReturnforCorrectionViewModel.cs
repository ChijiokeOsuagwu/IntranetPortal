using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ReturnforCorrectionViewModel:BaseViewModel
    {
        public int CurrentStageID { get; set; }
        [Required]
        public int ReturnToStageID { get; set; }
        [Required]
        public int ReviewHeaderID { get; set; }
        [Required]
        public int ReviewSessionID { get; set; }
        public string AppraiseeID { get; set; }
        public string AppraiseeName { get; set; }
        public string FromEmployeeID { get; set; }
        public string FromEmployeeName { get; set; }
        public string FromEmployeeSex { get; set; }
        [Required]
        public string ReturnInstruction { get; set; }

        public int ReviewMessageID { get; set; }
        public string MessageBody { get; set; }
        public DateTime? MessageTime { get; set; }
        public bool MessageIsCancelled { get; set; }
        public DateTime? TimeCancelled { get; set; }

        public ReviewMessage ConvertToReviewMessage()
        {
            return new ReviewMessage
            {
                FromEmployeeId = FromEmployeeID,
                FromEmployeeName = FromEmployeeName,
                FromEmployeeSex = FromEmployeeSex,
                MessageBody = MessageBody,
                MessageIsCancelled = MessageIsCancelled,
                MessageTime = MessageTime,
                ReviewHeaderId = ReviewHeaderID,
                ReviewSessionId = ReviewSessionID,
                ReviewMessageId = ReviewMessageID,
                TimeCancelled = TimeCancelled,
            };
        }
    }
}
