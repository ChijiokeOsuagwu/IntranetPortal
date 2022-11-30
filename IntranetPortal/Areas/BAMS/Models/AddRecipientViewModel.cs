using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.BAMS.Models
{
    public class AddRecipientViewModel : BaseViewModel
    {
        public long? MessageDetailID { get; set; }
        [Required]
        public string MessageID { get; set; }
        [Required]
        public int AssignmentEventID { get; set; }

        [Required]
        [Display(Name="Send To:")]
        public string RecipientName { get; set; }
        public string RecipientID { get; set; }
        public string ActionUrl { get; set; }
        [Required]
        public int? MessageType { get; set; }

        public MessageDetail ConvertToMessage()
        {
            return new MessageDetail
            {
                IsRead = false,
                DeletedTime = string.Empty,
                IsDeleted = false,
                MessageID = MessageID,
                ReadTime = string.Empty,
                RecipientID = RecipientID,
                RecipientName = RecipientName,
                MessageDetailID = MessageDetailID ?? 0,
            };
        }
    }
}
