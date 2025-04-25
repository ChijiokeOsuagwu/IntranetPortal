using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class DeclineTaskApprovalViewModel:BaseViewModel
    {
        [Required]
        public long TaskFolderID { get; set; }
        public string TaskFolderName { get; set; }
        [Required]
        public long TaskItemID { get; set; }
        public long FolderSubmissionID { get; set; }
        public string TaskOwnerID { get; set; }
        [Required]
        [Display(Name ="Reason")]
        public string NoteContent { get; set; }
        [Required]
        public string FromEmployeeName { get; set; }
        public string FromEmployeeID { get; set; }
    }
}
