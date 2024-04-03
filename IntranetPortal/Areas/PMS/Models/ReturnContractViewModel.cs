using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ReturnContractViewModel:BaseViewModel
    {
        public int ReviewMessageID { get; set; }

        [Required]
        public int ReviewHeaderID { get; set; }

        [Required]
        public string FromEmployeeID { get; set; }

        public string FromEmployeeName { get; set; }
        public string FromEmployeeSex { get; set; }

        public string MessageBody { get; set; }
        public DateTime? MessageTime { get; set; }
        public bool MessageIsCancelled { get; set; }
        public DateTime? TimeCancelled { get; set; }
        public string SourcePage { get; set; }
        public string LoggedInEmployeeID { get; set; }
        public int ReviewSubmissionID { get; set; }

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
                ReviewMessageId = ReviewMessageID,
                TimeCancelled = TimeCancelled,
            };
        }
    }
}
